using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMap
{
    // LFU cache
    public class ColorCache
    {
        private readonly int MaxSize = 10000;
        private readonly int CutSize = 5000;
        private int SemiAccurateCounter = 0;
        private readonly ConcurrentDictionary<Color, Color> Cache = new ConcurrentDictionary<Color, Color>();
        private readonly ConcurrentDictionary<Color, int> TimesUsed = new ConcurrentDictionary<Color, int>();
        public ColorCache()
        { }

        public void Set(Color key, Color value)
        {
            Cache[key] = value;
            IncreaseTimesUsed(key);
            SemiAccurateCounter++;
            PruneIfBig();
        }

        public bool TryGetValue(Color key, out Color value)
        {
            var result = Cache.TryGetValue(key, out var color);
            value = color;
            if (result)
                IncreaseTimesUsed(key);
            return result;
        }

        private void IncreaseTimesUsed(Color key)
        {
            if (TimesUsed.ContainsKey(key))
                TimesUsed[key]++;
            TimesUsed[key] = 1;
        }

        private void PruneIfBig()
        {
            if (SemiAccurateCounter > MaxSize)
                Prune(CutSize);
        }

        public void Prune(int size)
        {
            // thread-safe
            var snapshot = TimesUsed.ToArray();
            var least_used = snapshot.OrderBy(x => x.Value).Take(size).ToList();
            foreach (var item in least_used)
            {
                Cache.TryRemove(item.Key, out _);
                TimesUsed.TryRemove(item.Key, out _);
            }
            SemiAccurateCounter = Cache.Count;
        }
    }
}
