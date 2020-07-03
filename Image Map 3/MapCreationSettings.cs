using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMap
{
    public class MapCreationSettings : IDisposable
    {
        public readonly Image Original;
        public readonly int SplitW;
        public readonly int SplitH;
        public readonly InterpolationMode InterpMode;
        public readonly bool Dither;
        public readonly bool Stretch;
        public readonly IColorAlgorithm Algorithm;

        public int NumberOfMaps => SplitW * SplitH;

        public MapCreationSettings(Image original, int splitW, int splitH, InterpolationMode interpMode, bool dither, bool stretch, IColorAlgorithm algorithm)
        {
            Original = original;
            SplitW = splitW;
            SplitH = splitH;
            InterpMode = interpMode;
            Dither = dither;
            Stretch = stretch;
            Algorithm = algorithm;
        }

        public void Dispose()
        {
            Original?.Dispose();
        }
    }

    public class PendingMapsWithID
    {
        public event EventHandler Finished;
        public int MapCount => Settings.NumberOfMaps;
        private readonly List<long> UsedIDs;
        private readonly HashSet<int> DiscardIndexes = new HashSet<int>();
        private bool AlreadyFinished = false;
        public IEnumerable<long> IDs => UsedIDs.Where((v, i) => !DiscardIndexes.Contains(i));
        public readonly MapCreationSettings Settings;
        public Dictionary<long, Map> ResultMaps { get; private set; }
        private readonly object IDModificationLock = new object();

        public PendingMapsWithID(long first_id, MapCreationSettings settings, EditionProperties edition)
        {
            UsedIDs = Util.CreateRange(first_id, settings.NumberOfMaps).ToList();
            Settings = settings;
            var task = new Task<IEnumerable<Map>>(() => edition.MapFromSettings(settings));
            task.Start();
            task.ContinueWith((t) =>
            {
                lock (IDModificationLock)
                {
                    var ids = UsedIDs;
                    var maps = task.Result.ToList();
                    foreach (int index in DiscardIndexes.OrderByDescending(x => x))
                    {
                        ids.RemoveAt(index);
                        maps.RemoveAt(index);
                    }
                    ResultMaps = ids.Zip(maps, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
                }
                Finish();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Finish()
        {
            if (!AlreadyFinished)
            {
                AlreadyFinished = true;
                Finished?.Invoke(this, EventArgs.Empty);
            }
        }

        public void ChangeMapID(long from, long to)
        {
            lock (IDModificationLock)
            {
                int index = UsedIDs.IndexOf(from);
                if (index != -1)
                {
                    AddDiscard(UsedIDs.IndexOf(to));
                    UsedIDs[index] = to;
                }
            }
        }

        public void DiscardResult(long id)
        {
            lock (IDModificationLock)
            {
                AddDiscard(UsedIDs.IndexOf(id));
            }
        }

        private void AddDiscard(int index)
        {
            if (index == -1)
                return;
            DiscardIndexes.Add(index);
            // if everything is discarded, there's nothing left to do
            // ideally we'd also cancel the background task here
            if (!IDs.Any())
            {
                ResultMaps = new Dictionary<long, Map>();
                Finish();
            }
        }
    }
}
