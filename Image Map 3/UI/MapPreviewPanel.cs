using Colourful;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageMap
{
    public class MapPreviewPanel : FlowLayoutPanel
    {
        private readonly SortedDictionary<long, MapIDControl> MapControls = new SortedDictionary<long, MapIDControl>();
        private ContextMenuStrip RightClickBoxMenu;
        public IReadOnlyDictionary<long, Map> AllMaps => MapControls.ToDictionary(x => x.Key, x => x.Value.Map);
        public IReadOnlyDictionary<long, Map> ReadyMaps => MapControls.Where(x => x.Value.Map != null).ToDictionary(x => x.Key, x => x.Value.Map);
        public IReadOnlyDictionary<long, Map> AllSelectedMaps => MapControls.Where(x => x.Value.IsSelected).ToDictionary(x => x.Key, x => x.Value.Map);
        public IReadOnlyDictionary<long, Map> ReadySelectedMaps => MapControls.Where(x => x.Value.IsSelected && x.Value.Map != null).ToDictionary(x => x.Key, x => x.Value.Map);

        public void SetContextMenu(ContextMenuStrip menu)
        {
            RightClickBoxMenu = menu;
        }

        public bool AllAreSelected => MapControls.Any() && MapControls.Values.All(x => x.IsSelected);

        public void SetMapsImport(ImportMaps import)
        {
            var maps = import.GetMaps().Copy();
            var pendings = import.GetPending();
            foreach (var pending in pendings)
            {
                foreach (var id in pending.IDs)
                {
                    maps[id] = null;
                }
            }
            SetMaps(maps);
        }

        public void SetConflicts(IEnumerable<long> conflicted_ids)
        {
            foreach (var item in MapControls)
            {
                item.Value.SetConflict(conflicted_ids.Contains(item.Key));
            }
        }

        public void SetProgress(long id, decimal percentage)
        {
            var box = MapControls[id];
            box.SetProgress(percentage);
        }

        public void SetMaps(IReadOnlyDictionary<long, Map> new_maps)
        {
            var add = new_maps.Keys.Except(MapControls.Keys).Select(x => NewMapIDControl(x, new_maps[x])).ToList();
            var remove = MapControls.Keys.Except(new_maps.Keys).ToList();
            var update = new_maps.Keys.Intersect(MapControls.Keys);

            foreach (var box in add)
            {
                MapControls[box.ID] = box;
            }
            foreach (var id in remove)
            {
                MapControls.Remove(id);
            }
            foreach (var id in update)
            {
                MapControls[id].SetMap(new_maps[id]);
            }

            Util.SetControls(this, MapControls.Values);
        }

        public MapIDControl NewMapIDControl(long id, Map map)
        {
            MapIDControl box = new MapIDControl(id, map);
            box.MouseDown += Box_MouseDown;
            return box;
        }

        private void Box_MouseDown(object sender, MouseEventArgs e)
        {
            var box = (MapIDControl)sender;
            if (e.Button == MouseButtons.Right)
            {
                if (!box.IsSelected)
                {
                    DeselectAll();
                    box.SetSelected(true);
                }
                if (RightClickBoxMenu != null)
                    RightClickBoxMenu.Show(box, new Point(e.X, e.Y));
            }
            else
                ClickSelect(box);
        }

        private MapIDControl LastSelected;
        private void ClickSelect(MapIDControl box)
        {
            var controls = Controls.OfType<MapIDControl>().ToList();
            var current = LastSelected;
            box.ToggleSelected();
            if (Control.ModifierKeys == Keys.Shift && current != null)
            {
                bool state = current.IsSelected;
                int first = controls.IndexOf(current);
                int last = controls.IndexOf(box);
                for (int i = Math.Min(first, last); i < Math.Max(first, last); i++)
                {
                    controls[i].SetSelected(state);
                }
            }
            LastSelected = box;
        }

        public void SelectAll()
        {
            foreach (var control in MapControls.Values)
            {
                control.SetSelected(true);
            }
        }

        public void DeselectAll()
        {
            foreach (var control in MapControls.Values)
            {
                control.SetSelected(false);
            }
        }
    }
}
