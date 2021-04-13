using fNbt;
using LevelDBWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImageMap
{
    public class BedrockWorld : MinecraftWorld, IDisposable
    {
        public IBedrockVersion Version { get; private set; }
        private LevelDB BedrockDB;
        private NbtFile LevelDat;
        private readonly List<long> UnloadedIDs;
        public override Edition Edition => Edition.Bedrock;

        public BedrockWorld(string folder) : base(folder)
        {
            var levelname = Path.Combine(Folder, "levelname.txt");
            if (File.Exists(levelname))
                Name = File.ReadLines(levelname).First();
            UnloadedIDs = LoadAllMapIDs().OrderBy(x => x).ToList();
        }

        public override IEnumerable<Map> MapsFromSettings(MapCreationSettings settings, IProgress<MapCreationProgress> progress)
        {
            // bedrock maps are fast enough that reporting progress is not needed
            return BedrockMap.FromSettings(settings);
        }

        private void OpenDB()
        {
            BedrockDB = new LevelDB(Path.Combine(Folder, "db"));
            LevelDat = LoadNbtFromFile(Path.Combine(Folder, "level.dat"));
            Version = DetermineVersionFromLevelDat(LevelDat.RootTag);
        }

        private static IBedrockVersion DetermineVersionFromLevelDat(NbtCompound leveldat)
        {
            var versiontag = leveldat["lastOpenedWithVersion"];
            if (versiontag is NbtList list)
            {
                var minor = list[1];
                if (minor is NbtInt num)
                {
                    if (num.Value >= 11)
                        return Bedrock1p11Version.Instance;
                    if (num.Value >= 7)
                        return Bedrock1p7Version.Instance;
                    if (num.Value >= 2)
                        return Bedrock1p2Version.Instance;
                }
            }
            throw new InvalidOperationException("Couldn't determine world version");
        }

        private NbtFile LoadNbtFromFile(string filepath)
        {
            return LoadNbtFromBytes(File.ReadAllBytes(filepath), 8);
        }

        private NbtFile LoadNbtFromDB(string key)
        {
            byte[] data = BedrockDB.Get(key);
            if (data == null)
                throw new FileNotFoundException($"Key {key} not found in leveldb");
            return LoadNbtFromBytes(data);
        }

        private NbtFile LoadNbtFromBytes(byte[] data, int skip = 0)
        {
            var file = new NbtFile();
            file.BigEndian = false;
            file.LoadFromBuffer(data, skip, data.Length - skip, NbtCompression.None);
            return file;
        }

        private byte[] WriteNbtToBytes(NbtCompound root)
        {
            NbtFile file = new NbtFile(root);
            file.BigEndian = false;
            return file.SaveToBuffer(NbtCompression.None);
        }

        private void WriteNbtToDB(string key, NbtFile file)
        {
            file.BigEndian = false;
            var bytes = file.SaveToBuffer(NbtCompression.None);
            BedrockDB.Put(key, bytes);
        }

        private void CloseDB()
        {
            BedrockDB?.Close();
        }

        public override void AddMaps(IReadOnlyDictionary<long, Map> maps)
        {
            var batch = new WriteBatch();
            foreach (var map in maps)
            {
                var mapfile = Version.CreateMapCompound(map.Key, map.Value.Colors);
                mapfile.Name = "image map";
                var bytes = WriteNbtToBytes(mapfile);
                batch.Put(Util.MapName(map.Key), bytes);
                Maps[map.Key] = map.Value;
            }
            OpenDB();
            BedrockDB.Write(batch);
            CloseDB();
            SignalMapsChanged();
        }

        public override void RemoveMaps(IEnumerable<long> mapids)
        {
            OpenDB();
            foreach (var id in mapids)
            {
                BedrockDB.Delete(Util.MapName(id));
                Maps.Remove(id);
            }
            CloseDB();
            SignalMapsChanged();
        }

        public override bool AddChestsLocalPlayer(IEnumerable<long> mapids) => AddChestsExact(mapids, "~local_player");
        public override bool AddChests(IEnumerable<long> mapids, string playerid) => AddChestsExact(mapids, UuidToKey(playerid));
        private bool AddChestsExact(IEnumerable<long> mapids, string exact_playerid)
        {
            if (!mapids.Any())
                return true;
            OpenDB();
            // acquire the file this player is stored in, and the tag that represents said player
            var player = LoadNbtFromDB(exact_playerid);
            var invtag = (NbtList)player.RootTag["Inventory"];
            if (invtag == null)
                return false;
            var success = PutChestsInInventory(invtag, mapids);
            WriteNbtToDB(exact_playerid, player);
            CloseDB();

            return success;
        }

        private static bool UuidString(string input, out string uuid)
        {
            var match = Regex.Match(input, @"^player_(server_)?([0-f]{8}-[0-f]{4}-[0-f]{4}-[0-f]{4}-[0-f]{12})$");
            if (match.Success)
            {
                uuid = match.Groups[2].Value;
                return true;
            }
            else
            {
                uuid = null;
                return false;
            }
        }

        private static string UuidToKey(string uuid)
        {
            return $"player_server_{uuid}";
        }

        public override void Dispose()
        {
            CloseDB();
            BedrockDB?.Dispose();
            base.Dispose();
        }

        public override IEnumerable<long> GetTakenIDs()
        {
            return Maps.Keys.Concat(UnloadedIDs);
        }

        private IEnumerable<long> LoadAllMapIDs()
        {
            var ids = new List<long>();
            OpenDB();
            // thank you A Cynodont for help with this section
            const string MapKeyword = "map";
            var iterator = BedrockDB.CreateIterator();
            iterator.Seek(MapKeyword);
            while (iterator.IsValid())
            {
                var name = iterator.StringKey();
                if (name.StartsWith(MapKeyword))
                {
                    if (Util.MapString(name, out long number))
                        ids.Add(number);
                }
                else
                    break;
                iterator.Next();
            }
            iterator.Dispose();

            foreach (var item in BedrockDB)
            {
                var key = item.Key;
                var value = item.Value;
                var str_key = Encoding.UTF8.GetString(key);
                if ((key.Length == 9 || key.Length == 13) && key[key.Length - 1] == 49)
                {
                    if (value.Length > 0)
                    {
                        var files = new List<NbtCompound>();
                        int index = 0;
                        do
                        {
                            var file = LoadNbt(value, index);
                            files.Add(file.Item2.RootTag);
                            index += (int)file.Item1;
                        } while (index < value.Length);
                    }
                }
                else if ((key.Length == 9 || key.Length == 13) && key[key.Length - 1] == 44)
                {
                    if (value.Length != 1)
                        throw new Exception();
                }
                else if ((key.Length == 9 || key.Length == 13) && key[key.Length - 1] == 45)
                {
                }
                else if ((key.Length == 10 || key.Length == 14) && key[key.Length - 2] == 47)
                {
                }
                else if ((key.Length == 9 || key.Length == 13) && key[key.Length - 1] == 54)
                {
                }
                else if ((key.Length == 9 || key.Length == 13) && key[key.Length - 1] == 50)
                {
                    if (value.Length > 0)
                    {
                        var file = LoadNbt(value);
                    }
                }
                else if ((key.Length == 9 || key.Length == 13) && key[key.Length - 1] == 51)
                {
                    var file = LoadNbt(value);
                }
                else if ((key.Length == 9 || key.Length == 13) && key[key.Length - 1] == 58)
                {
                    var file = LoadNbt(value);
                }
                else if ((key.Length == 9 || key.Length == 13) && key[key.Length - 1] == 56)
                {
                }
                else if ((key.Length == 9 || key.Length == 13) && key[key.Length - 1] == 53)
                {
                }
                else if ((key.Length == 9 || key.Length == 13) && key[key.Length - 1] == 57)
                {
                }
                else if ((key.Length == 9 || key.Length == 13) && key[key.Length - 1] == 59)
                {
                }
                else if ((key.Length == 9 || key.Length == 13) && key[key.Length - 1] == 118)
                {
                }
                else if (!str_key.Any(x => Char.IsControl(x)) && !key.Any(x => x == 255))
                {
                    var file = LoadNbt(value);
                }
                else
                    throw new Exception();
            }

            CloseDB();
            return ids;
        }

        private Tuple<long, NbtFile> LoadNbt(byte[] data, int skip = 0)
        {
            var file = new NbtFile();
            file.BigEndian = false;
            long l = file.LoadFromBuffer(data, skip, data.Length - skip, NbtCompression.None);
            return Tuple.Create(l, file);
        }

        private void LoadMaps(IEnumerable<long> ids)
        {
            OpenDB();
            foreach (var id in ids)
            {
                var key = Util.MapName(id);
                var map = LoadNbtFromDB(key);
                var colors = map.RootTag["colors"];
                if (colors != null)
                {
                    var bytes = colors.ByteArrayValue;
                    // skip completely blank maps (bedrock likes generating pointless parents)
                    if (!bytes.All(x => x == 0))
                        Maps.Add(id, new BedrockMap(bytes));
                }
                UnloadedIDs.Remove(id);
            }
            CloseDB();
            SignalMapsChanged();
        }

        public override void LoadAllMaps() => LoadMaps(UnloadedIDs.ToList());
        public override void LoadMapsFront(int take) => LoadMaps(UnloadedIDs.Take(take).ToList());
        public override void LoadMapsBack(int take) => LoadMaps(UnloadedIDs.Skip(Math.Max(0, UnloadedIDs.Count - take)).ToList());

        public override IEnumerable<string> GetPlayerIDs()
        {
            OpenDB();
            var names = new List<string>();
            const string PlayerKeyword = "player";
            var iterator = BedrockDB.CreateIterator();
            iterator.Seek(PlayerKeyword);
            while (iterator.IsValid())
            {
                var name = iterator.StringKey();
                if (!name.StartsWith(PlayerKeyword))
                    break;
                var value = iterator.Value();
                if (UuidString(name, out string uuid))
                {
                    var player = LoadNbtFromBytes(value);
                    if (player.RootTag["Inventory"] != null)
                        names.Add(uuid);
                }
                iterator.Next();
            }
            iterator.Dispose();
            CloseDB();
            return names;
        }

        protected override IEnumerable<byte> GetFreeSlots(NbtList invtag)
        {
            List<byte> emptyslots = new List<byte>(35);
            for (byte i = 0; i < 35; i++)
            {
                emptyslots.Add(i);
            }
            foreach (NbtCompound slot in invtag)
            {
                if (slot["Count"].ByteValue > 0)
                    emptyslots.Remove(slot["Slot"].ByteValue);
            }
            return emptyslots;
        }

        protected override NbtCompound CreateChest(IEnumerable<long> mapids)
        {
            NbtList chestcontents = new NbtList("Items");
            byte slot = 0;
            foreach (var mapid in mapids)
            {
                chestcontents.Add(Version.CreateMapItem(slot, mapid));
                slot++;
            }
            var chest = new NbtCompound
            {
                new NbtString("Name", "minecraft:chest"), // 1.6+ support
                new NbtShort("id", 54), // 1.5 support
                new NbtByte("Count", 1),
                new NbtCompound("tag") { chestcontents }
            };
            return chest;
        }
    }
}
