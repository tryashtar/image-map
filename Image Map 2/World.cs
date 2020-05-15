using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using LevelDBWrapper;

namespace ImageMap
{
    public abstract class MinecraftWorld : IDisposable
    {
        protected Dictionary<long, Map> Maps;
        protected const string LOCAL_IDENTIFIER = "~local";
        public IReadOnlyDictionary<long, Map> WorldMaps => Maps;
        public string Folder { get; protected set; }
        public string Name { get; protected set; }
        public MinecraftWorld(string folder)
        {
            Folder = folder;
        }
        // user needs to call this
        public void Initialize()
        {
            Maps = LoadMaps();
        }
        public abstract void AddMaps(Dictionary<long, Map> maps);
        public abstract void RemoveMaps(IEnumerable<long> mapids);
        public bool AddChestsLocalPlayer(IEnumerable<long> mapids)
        {
            return AddChests(mapids, LOCAL_IDENTIFIER);
        }

        // returns whether there was enough room to fit the chests
        public abstract bool AddChests(IEnumerable<long> mapids, string playerid);
        public abstract IEnumerable<string> GetPlayerIDs();
        protected abstract Dictionary<long, Map> LoadMaps();
        // returns slot IDs not occupied with an item
        protected abstract IEnumerable<byte> GetFreeSlots(NbtList invtag);
        // mapids count must not exceed 27
        protected abstract NbtCompound CreateChest(IEnumerable<long> mapids);
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

        protected static bool MapString(string input, out long mapid)
        {
            var match = Regex.Match(input, @"^map_(-?\d+)$");
            if (match.Success)
            {
                mapid = Int64.Parse(match.Groups[1].Value);
                return true;
            }
            else
            {
                mapid = 0;
                return false;
            }
        }

        protected static bool UuidString(string input, out string uuid)
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

        protected static string UuidToKey(string uuid)
        {
            return $"player_server_{uuid}";
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
        private readonly bool HasLocalPlayer;

        public JavaWorld(string folder) : base(folder)
        {
            LevelDat = new NbtFile(Path.Combine(folder, "level.dat"));
            Name = LevelDat.RootTag["Data"]["LevelName"].StringValue;
            HasLocalPlayer = (LevelDat.RootTag["Data"]["Player"] != null);
        }

        public override void AddMaps(Dictionary<long, Map> maps)
        {
            foreach (var map in maps)
            {
                NbtCompound mapfile = new NbtCompound("map")
                {
                    new NbtCompound("data")
                    {
                        new NbtByte("scale", 0),
                        new NbtByte("dimension", 0),
                        new NbtShort("height", Map.MAP_HEIGHT),
                        new NbtShort("width", Map.MAP_WIDTH),
                        new NbtByte("trackingPosition", 0),
                        new NbtByte("unlimitedTracking", 0),
                        new NbtInt("xCenter", Int32.MaxValue),
                        new NbtInt("zCenter", Int32.MaxValue),
                        new NbtByte("locked", 1),
                        new NbtByteArray("colors", map.Value.Colors)
                    }
                };
                new NbtFile(mapfile).SaveToFile(MapFileLocation(map.Key), NbtCompression.GZip);
            }
        }

        public override void RemoveMaps(IEnumerable<long> mapids)
        {
            foreach (var id in mapids)
            {
                File.Delete(MapFileLocation(id));
            }
        }

        public override IEnumerable<string> GetPlayerIDs()
        {
            foreach (var file in Directory.EnumerateFiles(Path.Combine(Folder, "playerdata"), "*.dat"))
            {
                yield return Path.GetFileNameWithoutExtension(file);
            }
        }

        public override bool AddChests(IEnumerable<long> mapids, string playerid)
        {
            // acquire the file this player is stored in, and the tag that represents said player
            NbtCompound playertag;
            NbtFile activefile;
            if (playerid == LOCAL_IDENTIFIER)
            {
                if (!HasLocalPlayer)
                    throw new FileNotFoundException("Requested local player but there is none for this world");
                activefile = LevelDat;
                playertag = (NbtCompound)activefile.RootTag["Data"]["Player"];
            }
            else
            {
                activefile = new NbtFile(PlayerFileLocation(playerid));
                playertag = activefile.RootTag;
            }
            var invtag = (NbtList)playertag["Inventory"];

            var success = PutChestsInInventory(invtag, mapids);

            activefile.SaveToFile(activefile.FileName, NbtCompression.GZip);
            return success;
        }

        protected override Dictionary<long, Map> LoadMaps()
        {
            var maps = new Dictionary<long, Map>();
            foreach (string file in Directory.GetFiles(Path.Combine(Folder, "data"), "*.dat"))
            {
                string name = Path.GetFileNameWithoutExtension(file);
                if (MapString(name, out long number))
                {
                    NbtFile nbtfile = new NbtFile(file);
                    maps.Add(number, new JavaMap(nbtfile.RootTag["data"]["colors"].ByteArrayValue));
                }
            }
            return maps;
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
            return Path.Combine(Folder, "data", $"map_{mapid}.dat");
        }

        private string PlayerFileLocation(string playerid)
        {
            return Path.Combine(Folder, "playerdata", $"{playerid}.dat");
        }
    }

    public class BedrockWorld : MinecraftWorld, IDisposable
    {
        private LevelDB BedrockDB;

        public BedrockWorld(string folder) : base(folder)
        {
            Name = File.ReadAllText(Path.Combine(Folder, "levelname.txt"));
        }

        private void OpenDB()
        {
            BedrockDB = new LevelDB(Path.Combine(Folder, "db"));
        }

        private void CloseDB()
        {
            BedrockDB?.Close();
        }

        public override void AddMaps(Dictionary<long, Map> maps)
        {
            var batch = new WriteBatch();
            foreach (var map in maps)
            {
                NbtCompound mapfile = new NbtCompound("map")
                {
                    new NbtLong("mapId", map.Key),
                    new NbtLong("parentMapId", -1),
                    new NbtList("decorations", NbtTagType.Compound),
                    new NbtByte("fullyExplored", 1),
                    new NbtByte("scale", 4),
                    new NbtByte("dimension", 0),
                    new NbtShort("height", Map.MAP_HEIGHT),
                    new NbtShort("width", Map.MAP_WIDTH),
                    new NbtByte("unlimitedTracking", 0),
                    new NbtInt("xCenter", Int32.MaxValue),
                    new NbtInt("zCenter", Int32.MaxValue),
                    new NbtByte("mapLocked", 1),
                    new NbtByteArray("colors", map.Value.Colors)
                };
                NbtFile file = new NbtFile(mapfile);
                file.BigEndian = false;
                byte[] bytes = file.SaveToBuffer(NbtCompression.None);
                batch.Put($"map_{map.Key}", bytes);
            }
            OpenDB();
            BedrockDB.Write(batch);
            CloseDB();
        }

        public override void RemoveMaps(IEnumerable<long> mapids)
        {
            OpenDB();
            foreach (var id in mapids)
            {
                BedrockDB.Delete($"map_{id}");
            }
            CloseDB();
        }

        public override bool AddChests(IEnumerable<long> mapids, string playerid)
        {
            OpenDB();
            // acquire the file this player is stored in, and the tag that represents said player
            string file_identifier;
            if (playerid == LOCAL_IDENTIFIER)
                file_identifier = "~local_player";
            else
                file_identifier = UuidToKey(playerid);
            byte[] playerdata = BedrockDB.Get(file_identifier);
            if (playerdata == null)
                throw new FileNotFoundException($"Player with UUID {playerid} not found");
            var file = new NbtFile();
            file.BigEndian = false;
            file.LoadFromBuffer(playerdata, 0, playerdata.Length, NbtCompression.None);
            var invtag = (NbtList)file.RootTag["Inventory"];

            var success = PutChestsInInventory(invtag, mapids);

            byte[] bytes = file.SaveToBuffer(NbtCompression.None);
            BedrockDB.Put(file_identifier, bytes);
            CloseDB();

            return success;
        }

        public override void Dispose()
        {
            CloseDB();
            base.Dispose();
        }

        protected override Dictionary<long, Map> LoadMaps()
        {
            var maps = new Dictionary<long, Map>();
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
                    if (MapString(name, out long number))
                    {
                        NbtFile nbtfile = new NbtFile();
                        nbtfile.BigEndian = false;
                        byte[] data = iterator.Value();
                        nbtfile.LoadFromBuffer(data, 0, data.Length, NbtCompression.AutoDetect);
                        var colors = nbtfile.RootTag["colors"].ByteArrayValue;
                        // skip completely blank maps (bedrock likes generating pointless parents)
                        if (!colors.All(x => x == 0))
                            maps.Add(number, new BedrockMap(colors));
                    }
                }
                else
                    break;
                iterator.Next();
            }
            iterator.Dispose();
            CloseDB();
            return maps;
        }

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
                    NbtFile nbtfile = new NbtFile();
                    nbtfile.BigEndian = false;
                    nbtfile.LoadFromBuffer(value, 0, value.Length, NbtCompression.AutoDetect);
                    if (nbtfile.RootTag["Inventory"] != null)
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
                chestcontents.Add(new NbtCompound
                {
                    new NbtString("Name", "minecraft:map"), // 1.6+ support
                    new NbtShort("id", 358), // 1.5 support
                    new NbtByte("Count", 1),
                    new NbtByte("Slot", slot),
                    new NbtCompound("tag") { new NbtLong("map_uuid", mapid)
                    }
                });
                slot++;
            }
            var chest = new NbtCompound()
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
