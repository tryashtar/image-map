using fNbt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMap
{
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

        public override IEnumerable<Map> MapsFromSettings(MapCreationSettings settings, IProgress<MapCreationProgress> progress)
        {
            return JavaMap.FromSettings(settings, Version, progress);
        }

        private void ReloadLevelDat()
        {
            LevelDat = new NbtFile(Path.Combine(Folder, "level.dat"));
            Name = Util.GetNbt<NbtString>(LevelDat, "Data", "LevelName")?.StringValue ?? Path.GetFileName(Folder);
            Version = DetermineVersionFromLevelDat(Util.GetNbt<NbtCompound>(LevelDat, "Data"));
        }

        private static IJavaVersion DetermineVersionFromLevelDat(NbtCompound leveldat)
        {
            var dataversion = leveldat["DataVersion"];
            if (dataversion is NbtInt intversion)
            {
                if (intversion.Value >= 2711)
                    return Java1p17Version.Instance;
                if (intversion.Value >= 2709)
                    return Java1p17SnapshotVersion.Instance;
                if (intversion.Value >= 2562) // 1.16 pre-6
                    return Java1p16Version.Instance;
                if (intversion.Value >= 1128) // 17w17a
                    return Java1p12Version.Instance;
            }
            if (Util.GetNbt<NbtString>(leveldat, "GameRules", "doEntityDrops") != null) // 1.8.1 pre-1
                return Java1p8Version.Instance;
            if (Util.GetNbt<NbtFloat>(leveldat, "Player", "HealF") != null) // 1.6.4, not great (ideally 13w42a, with another check for 13w43a)
                return Java1p7Version.Instance;
            if (leveldat["MapFeatures"] != null)
                return JavaB1p8Version.Instance;
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
                int existing = Util.GetNbt<NbtInt>(idcounts, "data", "map")?.IntValue ?? 0;
                Util.SetNbt(idcounts, new NbtInt("map", Math.Max(existing, id)), "data");
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
            var invtag = Util.GetNbt<NbtList>(LevelDat, "Data", "Player", "Inventory");
            if (invtag == null)
                return false;
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
            if (invtag == null)
                return false;
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
                var colors = Util.GetNbt<NbtByteArray>(nbtfile, "data", "colors");
                if (colors != null)
                    Maps.Add(id, new JavaMap(colors.ByteArrayValue, Version));
                UnloadedIDs.Remove(id);
            }
            SignalMapsChanged();
        }

        public override void LoadAllMaps() => LoadMaps(UnloadedIDs.ToList());
        public override void LoadMapsFront(int take) => LoadMaps(UnloadedIDs.Take(take).ToList());
        public override void LoadMapsBack(int take) => LoadMaps(UnloadedIDs.Skip(Math.Max(0, UnloadedIDs.Count - take)).ToList());
    }
}
