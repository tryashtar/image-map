using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using LevelDBWrapper;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace ImageMap
{
    public abstract class MinecraftWorld : IMapSource, IDisposable
    {
        protected SortedDictionary<long, Map> Maps;
        public IReadOnlyDictionary<long, Map> GetMaps() => Maps;
        public abstract IEnumerable<long> GetTakenIDs();
        public string Folder { get; protected set; }
        public string Name { get; protected set; }
        public event EventHandler MapsChanged;
        public abstract Edition Edition { get; }
        public MinecraftWorld(string folder)
        {
            Folder = folder;
            Maps = new SortedDictionary<long, Map>();
        }
        // user needs to call this
        public abstract IEnumerable<Map> MapsFromSettings(MapCreationSettings settings);
        public abstract void AddMaps(IReadOnlyDictionary<long, Map> maps);
        public abstract void RemoveMaps(IEnumerable<long> mapids);
        // returns whether there was enough room to fit the chests
        public abstract bool AddChestsLocalPlayer(IEnumerable<long> mapids);
        public abstract bool AddChests(IEnumerable<long> mapids, string playerid);
        public void ChangeMapID(long from, long to)
        {
            if (Maps.TryGetValue(from, out var map))
            {
                RemoveMaps(new[] { from });
                AddMaps(new Dictionary<long, Map> { { to, map } });
            }
        }
        public abstract IEnumerable<string> GetPlayerIDs();
        public abstract void LoadAllMaps();
        public abstract void LoadMapsFront(int take);
        public abstract void LoadMapsBack(int take);
        // returns slot IDs not occupied with an item
        protected abstract IEnumerable<byte> GetFreeSlots(NbtList invtag);
        // mapids count must not exceed 27
        protected abstract NbtCompound CreateChest(IEnumerable<long> mapids);
        protected void SignalMapsChanged()
        {
            MapsChanged?.Invoke(this, EventArgs.Empty);
        }
        // returns whether there was enough room to fit the chests
        protected bool PutChestsInInventory(NbtList invtag, IEnumerable<long> mapids)
        {
            // add to chests one by one
            var slots = GetFreeSlots(invtag);
            int total = mapids.Count();
            int current = 0;
            foreach (var slot in slots)
            {
                var chestcontents = mapids.Skip(current).Take(27);
                var chest = CreateChest(chestcontents);
                chest.Add(new NbtByte("Slot", slot));
                // bedrock-specific lines, replace existing item in this slot which should only be air
                var existingitem = invtag.Where(x => x["Slot"].ByteValue == slot).FirstOrDefault();
                if (existingitem != null)
                {
                    invtag.Insert(invtag.IndexOf(existingitem), chest);
                    invtag.Remove(existingitem);
                }
                else
                {
                    invtag.ListType = NbtTagType.Compound;
                    invtag.Add(chest);
                }
                current += 27;
                if (current >= total)
                    return true;
            }
            return false;
        }

        public virtual void Dispose()
        {
            foreach (var map in Maps.Values)
            {
                map.Dispose();
            }
        }
    }

    public class JavaWorld : MinecraftWorld
    {
        private NbtFile LevelDat;
        private readonly List<long> UnloadedIDs;
        public IJavaVersion Version { get; private set; }
        public override Edition Edition => Edition.Java;

        public JavaWorld(string folder) : base(folder)
        {
            ReloadLevelDat();
            UnloadedIDs = LoadAllMapIDs().OrderBy(x => x).ToList();
        }

        public override IEnumerable<Map> MapsFromSettings(MapCreationSettings settings)
        {
            return JavaMap.FromSettings(settings, Version);
        }

        private void ReloadLevelDat()
        {
            LevelDat = new NbtFile(Path.Combine(Folder, "level.dat"));
            Name = LevelDat.RootTag["Data"]["LevelName"].StringValue;
            Version = DetermineVersionFromLevelDat((NbtCompound)LevelDat.RootTag["Data"]);
        }

        private static IJavaVersion DetermineVersionFromLevelDat(NbtCompound leveldat)
        {
            var dataversion = leveldat["DataVersion"];
            if (dataversion is NbtInt intversion)
            {
                if (intversion.Value >= 2562) // 1.16 pre-6
                    return Java1p16Mapping.Instance;
                if (intversion.Value >= 1128) // 17w17a
                    return Java1p12Mapping.Instance;
            }
            var gamerules = leveldat["GameRules"];
            if (gamerules != null && gamerules["doEntityDrops"] != null) // 1.8.1 pre-1
                return Java1p8Mapping.Instance;
            var player = leveldat["Player"];
            if (player != null && player["HealF"] != null) // 1.6.4, not great (ideally 13w42a, with another check for 13w43a)
                return Java1p7Mapping.Instance;
            if (leveldat["MapFeatures"] != null)
                return JavaOldMapping.Instance;
            throw new InvalidOperationException("Couldn't determine world version, or it's from an old version from before maps existed (pre-beta 1.8)");
        }

        public override void AddMaps(IReadOnlyDictionary<long, Map> maps)
        {
            foreach (var map in maps)
            {
                var data = Version.CreateMapCompound(map.Key, map.Value.Colors);
                data.Name = "data";
                var mapfile = new NbtCompound("image map") { data };
                new NbtFile(mapfile).SaveToFile(MapFileLocation(map.Key), NbtCompression.GZip);
                Maps[map.Key] = map.Value;
            }
            if (maps.Any())
            {
                long biggest_id = maps.Keys.Max();
                IncreaseMapIdCount((int)biggest_id);
            }
            SignalMapsChanged();
        }

        private void IncreaseMapIdCount(int id)
        {
            NbtFile idcounts;
            string path = Path.Combine(Folder, "data", "idcounts.dat");
            if (File.Exists(path))
            {
                idcounts = new NbtFile(Path.Combine(Folder, "data", "idcounts.dat"));
                int existing = idcounts.RootTag["data"]["map"].IntValue;
                idcounts.RootTag["data"]["map"] = new NbtInt("map", Math.Max(existing, id));
            }
            else
                idcounts = new NbtFile(new NbtCompound("") { new NbtCompound("data") { new NbtInt("map", id) } });
            idcounts.SaveToFile(path, NbtCompression.GZip);
        }

        public override void RemoveMaps(IEnumerable<long> mapids)
        {
            foreach (var id in mapids)
            {
                File.Delete(MapFileLocation(id));
                Maps.Remove(id);
            }
            SignalMapsChanged();
        }

        public override IEnumerable<string> GetPlayerIDs()
        {
            var folder = Path.Combine(Folder, "playerdata");
            if (!Directory.Exists(folder))
                yield break;
            foreach (var file in Directory.EnumerateFiles(folder, "*.dat"))
            {
                yield return Path.GetFileNameWithoutExtension(file);
            }
        }

        public override bool AddChestsLocalPlayer(IEnumerable<long> mapids)
        {
            if (!mapids.Any())
                return true;
            ReloadLevelDat();
            var playertag = (NbtCompound)LevelDat.RootTag["Data"]["Player"];
            if (playertag == null)
                return false;
            var invtag = (NbtList)playertag["Inventory"];
            var success = PutChestsInInventory(invtag, mapids);
            LevelDat.SaveToFile(LevelDat.FileName, LevelDat.FileCompression);
            return success;
        }

        public override bool AddChests(IEnumerable<long> mapids, string playerid)
        {
            if (!mapids.Any())
                return true;
            var activefile = new NbtFile(PlayerFileLocation(playerid));
            var playertag = activefile.RootTag;
            var invtag = (NbtList)playertag["Inventory"];
            var success = PutChestsInInventory(invtag, mapids);
            activefile.SaveToFile(activefile.FileName, activefile.FileCompression);
            return success;
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
                chestcontents.Add(new NbtCompound
                {
                    new NbtString("id", "minecraft:filled_map"),
                    new NbtByte("Count", 1),
                    new NbtByte("Slot", slot),
                    new NbtShort("Damage", (short)mapid), // 1.12 support
                    new NbtCompound("tag") { new NbtInt("map", (int)mapid) } // 1.13+ support
                });
                slot++;
            }
            NbtCompound chest = new NbtCompound()
            {
                new NbtString("id", "minecraft:chest"),
                new NbtByte("Count", 1),
                new NbtCompound("tag") { new NbtCompound("BlockEntityTag") { chestcontents } }
            };
            return chest;
        }

        private string MapFileLocation(long mapid)
        {
            return Path.Combine(Folder, "data", $"{Util.MapName(mapid)}.dat");
        }

        private string PlayerFileLocation(string playerid)
        {
            return Path.Combine(Folder, "playerdata", $"{playerid}.dat");
        }

        public override IEnumerable<long> GetTakenIDs()
        {
            return Maps.Keys.Concat(UnloadedIDs);
        }

        private IEnumerable<long> LoadAllMapIDs()
        {
            var ids = new List<long>();
            foreach (string file in Directory.GetFiles(Path.Combine(Folder, "data"), "*.dat"))
            {
                string name = Path.GetFileNameWithoutExtension(file);
                if (Util.MapString(name, out long number))
                {
                    ids.Add(number);
                }
            }
            return ids;
        }

        private void LoadMaps(IEnumerable<long> ids)
        {
            foreach (var id in ids)
            {
                var file = Path.Combine(Folder, "data", $"{Util.MapName(id)}.dat");
                var nbtfile = new NbtFile(file);
                Maps.Add(id, new JavaMap(nbtfile.RootTag["data"]["colors"].ByteArrayValue, Version));
                UnloadedIDs.Remove(id);
            }
            SignalMapsChanged();
        }

        public override void LoadAllMaps() => LoadMaps(UnloadedIDs.ToList());
        public override void LoadMapsFront(int take) => LoadMaps(UnloadedIDs.Take(take).ToList());
        public override void LoadMapsBack(int take) => LoadMaps(UnloadedIDs.Skip(Math.Max(0, UnloadedIDs.Count - take)).ToList());
    }

    public class BedrockWorld : MinecraftWorld, IDisposable
    {
        public IBedrockVersion Version { get; private set; }
        private LevelDB BedrockDB;
        private NbtFile LevelDat;
        private readonly List<long> UnloadedIDs;
        public override Edition Edition => Edition.Bedrock;

        public BedrockWorld(string folder) : base(folder)
        {
            Name = File.ReadAllText(Path.Combine(Folder, "levelname.txt"));
            UnloadedIDs = LoadAllMapIDs().OrderBy(x => x).ToList();
        }

        public override IEnumerable<Map> MapsFromSettings(MapCreationSettings settings)
        {
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
            CloseDB();
            return ids;
        }

        private void LoadMaps(IEnumerable<long> ids)
        {
            OpenDB();
            foreach (var id in ids)
            {
                var key = Util.MapName(id);
                var map = LoadNbtFromDB(key);
                var colors = map.RootTag["colors"].ByteArrayValue;
                // skip completely blank maps (bedrock likes generating pointless parents)
                if (!colors.All(x => x == 0))
                    Maps.Add(id, new BedrockMap(colors));
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
                var value = iterator.Value();
                if (UuidString(name, out string uuid))
                {
                    var player = LoadNbtFromBytes(value);
                    if (player.RootTag["Inventory"] != null)
                        names.Add(uuid);
                }
                else
                    break;
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
