using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageMap
{
    public interface IMapSource
    {
        IReadOnlyDictionary<long, Map> GetMaps();
        IEnumerable<long> GetTakenIDs();
        void ChangeMapID(long from, long to);
        void AddMaps(IReadOnlyDictionary<long, Map> maps);
        void RemoveMaps(IEnumerable<long> ids);
    }

    public class ImportMaps : IMapSource
    {
        public event EventHandler MapsChanged;
        private readonly SortedDictionary<long, Map> ImportingMaps = new SortedDictionary<long, Map>();
        // simulate a set
        private readonly ConcurrentDictionary<PendingMapsWithID, PendingMapsWithID> ProcessingMaps = new ConcurrentDictionary<PendingMapsWithID, PendingMapsWithID>();

        public IReadOnlyDictionary<long, Map> GetMaps()
        {
            return ImportingMaps;
        }

        public IEnumerable<PendingMapsWithID> GetPending() => ProcessingMaps.Keys;

        public IEnumerable<long> GetTakenIDs()
        {
            return ImportingMaps.Keys.Concat(ProcessingMaps.Keys.SelectMany(x => x.IDs));
        }

        public void ChangeMapID(long from, long to)
        {
            if (ImportingMaps.TryGetValue(from, out var map))
            {
                // if we're changing the ID of an existing map to replace another existing map
                ImportingMaps.Remove(from);
                ImportingMaps[to] = map;
            }
            else
            {   
                // if we're changing the ID of a pending map to replace an existing map
                if (ImportingMaps.ContainsKey(to))
                    ImportingMaps.Remove(to);
            }
            foreach (var pending in ProcessingMaps.Keys)
            {
                // if we're changing the ID of a map to replace a pending map
                if (pending.IDs.Contains(to))
                    pending.DiscardResult(to);
                pending.ChangeMapID(from, to);
            }
            MapsChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool HasAnyMaps()
        {
            return ImportingMaps.Any() || ProcessingMaps.Any();
        }

        public void AddPending(PendingMapsWithID pending)
        {
            pending.Finished += Pending_Finished;
            ProcessingMaps.TryAdd(pending, pending);
            MapsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveMaps(IEnumerable<long> ids)
        {
            foreach (var id in ids)
            {
                ImportingMaps.Remove(id);
            }
            foreach (var pending in ProcessingMaps.Keys)
            {
                foreach (var id in ids)
                {
                    pending.DiscardResult(id);
                }
            }
            MapsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Pending_Finished(object sender, EventArgs e)
        {
            var pending = (PendingMapsWithID)sender;
            ProcessingMaps.TryRemove(pending, out _);
            foreach (var item in pending.ResultMaps)
            {
                ImportingMaps[item.Key] = item.Value;
            }
            MapsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void AddMaps(IReadOnlyDictionary<long, Map> maps)
        {
            foreach (var item in maps)
            {
                ImportingMaps[item.Key] = item.Value;
            }
            MapsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
