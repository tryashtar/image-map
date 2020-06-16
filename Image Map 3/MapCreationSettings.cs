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
        public readonly long FirstID;
        public long LastID => FirstID + MapCount - 1;
        public IEnumerable<long> IDs => Util.CreateRange(FirstID, MapCount);
        public readonly MapCreationSettings Settings;
        public Dictionary<long, Map> ResultMaps { get; private set; }

        public PendingMapsWithID(long first_id, MapCreationSettings settings, EditionProperties edition)
        {
            FirstID = first_id;
            Settings = settings;
            var task = new Task<IEnumerable<Map>>(() => edition.MapFromSettings(settings));
            task.Start();
            task.ContinueWith((t) =>
            {
                long id = FirstID;
                ResultMaps = task.Result.ToDictionary(x => id++);
                Finished?.Invoke(this, EventArgs.Empty);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
