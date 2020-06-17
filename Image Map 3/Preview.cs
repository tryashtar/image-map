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
    public abstract class HalfPreview
    {
        public event EventHandler ControlsChanged;
        public abstract IReadOnlyDictionary<long, Map> GetMaps();
        public virtual bool HasAnyMaps() => GetMaps().Any();
        public virtual IEnumerable<long> GetTakenIDs() => GetMaps().Keys;
        public abstract ContextMenuStrip GetContextMenu();
        protected readonly List<MapIDControl> Controls = new List<MapIDControl>();
        public IReadOnlyCollection<MapIDControl> MapIDControls => Controls.AsReadOnly();

        protected readonly ContainerControl ParentControl;
        public HalfPreview(ContainerControl parent)
        {
            ParentControl = parent;
        }

        public void SelectAll()
        {
            foreach (var box in Controls)
            {
                box.SetSelected(true);
            }
        }
        public void DeselectAll()
        {
            foreach (var box in Controls)
            {
                box.SetSelected(true);
            }
        }

        protected void SignalControlsChanged()
        {
            ControlsChanged?.Invoke(this, EventArgs.Empty);
        }

        protected MapIDControl CreateMapIdControl(long id)
        {
            var box = new MapIDControl(id);
            box.MouseDown += Box_MouseDown;
            return box;
        }

        protected void Box_MouseDown(object sender, MouseEventArgs e)
        {
            var box = (MapIDControl)sender;
            var context = GetContextMenu();
            if (e.Button == MouseButtons.Right)
            {
                if (!box.Selected)
                {
                    DeselectAll();
                    box.SetSelected(true);
                }
                context.Show(box, new Point(e.X, e.Y));
            }
            //else
            //    ClickSelect(box, where);
        }
    }

    public class ImportPreview : HalfPreview
    {
        private readonly Dictionary<long, Map> ImportingMaps = new Dictionary<long, Map>();
        // simulate a threadsafe set
        private readonly ConcurrentDictionary<PendingMapsWithID, PendingMapsWithID> ProcessingMaps = new ConcurrentDictionary<PendingMapsWithID, PendingMapsWithID>();
        public override IReadOnlyDictionary<long, Map> GetMaps()
        {
            return ImportingMaps;
        }
        public override bool HasAnyMaps()
        {
            return base.HasAnyMaps() || ProcessingMaps.Any();
        }
        public override IEnumerable<long> GetTakenIDs()
        {
            return base.GetTakenIDs().Concat(ProcessingMaps.SelectMany(x => x.Key.IDs));
        }

        public void AddPending(PendingMapsWithID pending)
        {
            CreateEmptyIDControls(pending.IDs);
            pending.Finished += Pending_Finished;
            ProcessingMaps.TryAdd(pending, pending);
        }

        private void CreateEmptyIDControls(IEnumerable<long> ids)
        {
            var boxes = new List<MapIDControl>();
            int i = -1;
            foreach (var id in ids)
            {
                i++;
                var box = CreateMapIdControl(id);
                boxes[i] = box;
            }
            Controls.AddRange(boxes.ToArray());
        }

        private void Pending_Finished(object sender, EventArgs e)
        {
            var pending = (PendingMapsWithID)sender;
            ProcessingMaps.TryRemove(pending, out _);
            foreach (var item in pending.ResultMaps)
            {
                ImportingMaps[item.Key] = item.Value;
            }
            FillEmptyControls(pending.IDs);
        }

        private void FillEmptyControls(IEnumerable<long> ids)
        {
            var maps = GetMaps();
            foreach (var box in MapIDControls)
            {
                if (ids.Contains(box.ID) && maps.TryGetValue(box.ID, out var map))
                    box.SetBox(new MapPreviewBox(map));
            }
        }
    }

    public class WorldPreview : HalfPreview
    {
        protected readonly MinecraftWorld World;

        public WorldPreview(ContainerControl parent, MinecraftWorld world) : base(parent)
        {
            World = world;
        }

        public override IReadOnlyDictionary<long, Map> GetMaps()
        {
            return World.WorldMaps;
        }
    }
}
