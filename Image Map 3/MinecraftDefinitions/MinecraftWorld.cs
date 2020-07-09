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
        public abstract IEnumerable<Map> MapsFromSettings(MapCreationSettings settings, IProgress<MapCreationProgress> progress);
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
}
