using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Image_Map
{
    public enum Edition
    {
        Java,
        Bedrock
    }

    public enum ActionResult
    {
        Success,
        MapsNotImported,
        Failure
    }

    public enum MapReplaceOption
    {
        ChangeExisting,
        ReplaceExisting,
        Skip,
        Info
    }

    public enum MapStatus
    {
        Importing,
        Existing
    }

    public class ViewController
    {
        public const string LOCAL_IDENTIFIER = "(singleplayer)";
        public const string NOBODY_IDENTIFIER = "(nobody)";
        MinecraftWorld SelectedWorld;
        readonly TheForm UI;
        List<MapIDControl> ImportingMapPreviews;
        List<MapIDControl> ExistingMapPreviews;
        private MapIDControl LastImportSelected;
        private MapIDControl LastExistingSelected;
        private List<string> PlayerDestinations;

        public ViewController(TheForm form)
        {
            UI = form;
            ImportingMapPreviews = new List<MapIDControl>();
            ExistingMapPreviews = new List<MapIDControl>();
        }

        public ActionResult OpenWorld(Edition edition, string folder, bool bypass_mapwarning = false)
        {
            if (!bypass_mapwarning && ImportingMapPreviews.Any())
                return ActionResult.MapsNotImported;
            if (SelectedWorld is IDisposable disp)
                disp.Dispose();
            try
            {
                if (edition == Edition.Java)
                    SelectedWorld = new JavaWorld(folder);
                else if (edition == Edition.Bedrock)
                    SelectedWorld = new BedrockWorld(folder);
                else
                    throw new ArgumentException($"Don't know what edition {edition} is");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open this world. Perhaps it's the wrong edition, or is missing some important files.\n\n" + ex.Message, "Failed to open world!");
                return ActionResult.Failure;
            }
            SelectedWorld.Initialize();
            NewWorldOpened();
            PlayerDestinations = new List<string>();
            PlayerDestinations.Add(LOCAL_IDENTIFIER);
            foreach (var uuid in SelectedWorld.GetPlayerIDs())
            {
                PlayerDestinations.Add(uuid);
            }
            return ActionResult.Success;
        }

        public IEnumerable<string> GetPlayerDestinations() => PlayerDestinations;

        public void ImportImages(string[] imagepaths)
        {
            bool java = (SelectedWorld is JavaWorld);
            var import = new ImportWindow(java);
            import.InterpolationModeBox.SelectedIndex = Properties.Settings.Default.InterpIndex;
            import.ApplyAllCheck.Checked = Properties.Settings.Default.ApplyAllCheck;
            import.DitherChecked = Properties.Settings.Default.Dither;
            import.StartImports(UI, imagepaths);
            Properties.Settings.Default.InterpIndex = import.InterpolationModeBox.SelectedIndex;
            Properties.Settings.Default.ApplyAllCheck = import.ApplyAllCheck.Checked;
            Properties.Settings.Default.Dither = import.DitherChecked;
            long id = GetSafeID();
            var tasks = new List<Task>();
            UI.OpenButton.Enabled = false;
            UI.SendButton.Enabled = false;
            foreach (var settings in import.OutputSettings)
            {
                var boxes = new List<MapIDControl>();
                for (int i = 0; i < settings.SplitH * settings.SplitW; i++)
                {
                    var box = new MapIDControl(id);
                    boxes.Add(box);
                    SendToZone(box, MapStatus.Importing);
                    id++;
                }
                var t = new Task<IEnumerable<Map>>(() =>
                {
                    if (java)
                        return JavaMap.FromSettings(settings);
                    else
                        return BedrockMap.FromSettings(settings);
                });
                t.Start();
                t.ContinueWith((prev) =>
                {
                    var results = prev.Result.ToArray();
                    for (int i = 0; i < settings.SplitH * settings.SplitW; i++)
                    {
                        boxes[i].SetBox(new MapPreviewBox(results[i]));
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
                tasks.Add(t);
            }
            var done = Task.WhenAll(tasks);
            done.ContinueWith((t) =>
                {
                    UI.OpenButton.Enabled = true;
                    UI.SendButton.Enabled = true;
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        // returns number of conflicts
        public int ChangeMapIDs(IEnumerable<MapIDControl> boxes, long id, MapStatus area, MapReplaceOption option)
        {
            int conflicts = 0;
            var write = new List<MapIDControl>();
            foreach (var box in boxes.ToArray())
            {
                bool shouldchange = (option != MapReplaceOption.Info);
                var currentholder = GetMapByID(id, area);
                if (currentholder != null && currentholder != box)
                {
                    if (option == MapReplaceOption.ChangeExisting)
                    {
                        currentholder.SetID(GetSafeID());
                        if (area == MapStatus.Existing)
                            write.Add(currentholder);
                    }
                    else if (option == MapReplaceOption.ReplaceExisting)
                        RemoveFromZone(currentholder, area);
                    else if (option == MapReplaceOption.Skip)
                        shouldchange = false;
                    conflicts++;
                }
                if (shouldchange)
                {
                    if (area == MapStatus.Existing)
                    {
                        SelectedWorld.RemoveMaps(new[] { box.ID });
                        write.Add(box);
                    }
                    box.SetID(id);
                }
                id++;
            }
            SendMapsToWorld(write, MapReplaceOption.ReplaceExisting);
            DetermineTransferConflicts();
            return conflicts;
        }

        private void DetermineTransferConflicts()
        {
            foreach (var box in GetAllMaps(MapStatus.Importing))
            {
                var counterpart = GetMapByID(box.ID, MapStatus.Existing);
                box.SetConflict(counterpart != null);
            }
        }

        public long GetSafeID()
        {
            var taken = ImportingMapPreviews.Concat(ExistingMapPreviews).Select(x => x.ID).ToList();
            taken.Add(-1);
            return taken.Max() + 1;
        }

        public MapIDControl GetMapByID(long id, MapStatus area)
        {
            return GetAllMaps(area).FirstOrDefault(x => x.ID == id);
        }

        public IEnumerable<MapIDControl> GetSelectedMaps(MapStatus area)
        {
            return GetAllMaps(area).Where(x => x.Selected);
        }

        public IEnumerable<MapIDControl> GetAllMaps(MapStatus area)
        {
            if (area == MapStatus.Importing)
                return ImportingMapPreviews;
            else
                return ExistingMapPreviews;
        }

        public void SaveMaps(IEnumerable<MapIDControl> maps, string folder)
        {
            Directory.CreateDirectory(folder);
            foreach (var box in maps)
            {
                box.Map.Image.GetImage().Save(Path.Combine(folder, box.GetMapName() + ".png"));
            }
        }

        public void DeleteMapsFromWorld(IEnumerable<MapIDControl> maps)
        {
            var remove = maps.ToArray();
            foreach (var box in remove)
            {
                RemoveFromZone(box, MapStatus.Existing);
            }
            SelectedWorld.RemoveMaps(remove.Select(x => x.ID));
            DetermineTransferConflicts();
        }

        public void SaveMap(MapIDControl map, string file)
        {
            map.Map.Image.GetImage().Save(file);
        }

        public bool UnsavedChanges()
        {
            return ImportingMapPreviews.Any();
        }

        // returns number of conflicts, does nothing if there is at least one
        public int SendMapsToWorld(IEnumerable<MapIDControl> maps, MapReplaceOption option, string playerid = NOBODY_IDENTIFIER)
        {
            var writemaps = maps.ToList();
            var conflictids = new List<long>();
            // check for  conflicts
            foreach (var map in maps)
            {
                if (map.Conflicted)
                {
                    conflictids.Add(map.ID);
                    if (option == MapReplaceOption.Skip)
                        writemaps.Remove(map);
                }
            }
            if (option == MapReplaceOption.Info)
                return conflictids.Count;
            if (option == MapReplaceOption.ChangeExisting)
            {
                foreach (var conflict in conflictids)
                {
                    var currentholder = GetMapByID(conflict, MapStatus.Existing);
                    currentholder.SetID(GetSafeID());
                    writemaps.Add(currentholder);
                }
            }
            SelectedWorld.AddMaps(writemaps.ToDictionary(x => x.ID, x => x.Map));
            var ids = writemaps.Select(x => x.ID);
            AddChests(ids, playerid);
            foreach (var box in writemaps.ToArray())
            {
                var exists = GetMapByID(box.ID, MapStatus.Existing);
                if (exists != null && exists != box)
                    RemoveFromZone(exists, MapStatus.Existing);
                SendToZone(box, MapStatus.Existing);
            }
            DetermineTransferConflicts();
            return conflictids.Count;
        }

        // returns false if there wasn't enough room
        public bool AddChests(IEnumerable<long> mapids, string playerid)
        {
            if (!mapids.Any())
                return true;
            if (playerid == LOCAL_IDENTIFIER)
                return SelectedWorld.AddChestsLocalPlayer(mapids);
            else if (playerid != NOBODY_IDENTIFIER)
                return SelectedWorld.AddChests(mapids, playerid);
            return true;
        }

        public void SelectAll(MapStatus area)
        {
            foreach (var box in GetAllMaps(area))
            {
                box.SetSelected(true);
            }
        }

        public void DeselectAll(MapStatus area)
        {
            foreach (var box in GetAllMaps(area))
            {
                box.SetSelected(false);
            }
        }

        private void ClickSelect(MapIDControl box, MapStatus area)
        {
            MapIDControl current = area == MapStatus.Importing ? LastImportSelected : LastExistingSelected;
            var list = area == MapStatus.Importing ? ImportingMapPreviews : ExistingMapPreviews;

            box.ToggleSelected();
            if (Control.ModifierKeys == Keys.Shift && current != null)
            {
                bool state = current.Selected;
                int first = list.IndexOf(current);
                int last = list.IndexOf(box);
                for (int i = Math.Min(first, last); i < Math.Max(first, last); i++)
                {
                    list[i].SetSelected(state);
                }
            }
            if (area == MapStatus.Importing)
                LastImportSelected = box;
            else
                LastExistingSelected = box;
        }

        private void ImportingBox_MouseDown(object sender, MouseEventArgs e)
        {
            MapIDControl box = sender as MapIDControl;
            if (e.Button == MouseButtons.Right)
            {
                if (!box.Selected)
                {
                    foreach (var other in ImportingMapPreviews)
                    {
                        other.SetSelected(false);
                    }
                    box.SetSelected(true);
                }
                UI.ImportContextMenu.Show(box, new Point(e.X, e.Y));
            }
            else
                ClickSelect(box, MapStatus.Importing);
        }

        private void ExistingBox_MouseDown(object sender, MouseEventArgs e)
        {
            MapIDControl box = sender as MapIDControl;
            if (e.Button == MouseButtons.Right)
            {
                if (!box.Selected)
                {
                    foreach (var other in ExistingMapPreviews)
                    {
                        other.SetSelected(false);
                    }
                    box.SetSelected(true);
                }
                UI.ExistingContextMenu.Show(box, new Point(e.X, e.Y));
            }
            else
                ClickSelect(box, MapStatus.Existing);
        }

        public void RemoveFromZone(MapIDControl box, MapStatus location)
        {
            box.SetSelected(false);
            box.SetConflict(false);
            if (box == LastImportSelected)
                LastImportSelected = null;
            if (box == LastExistingSelected)
                LastExistingSelected = null;
            if (location == MapStatus.Importing)
            {
                UI.ImportZone.Controls.Remove(box);
                ImportingMapPreviews.Remove(box);
                box.MouseDown -= ImportingBox_MouseDown;
            }
            else if (location == MapStatus.Existing)
            {
                UI.ExistingZone.Controls.Remove(box);
                ExistingMapPreviews.Remove(box);
                box.MouseDown -= ExistingBox_MouseDown;
            }
        }

        private void SendToZone(MapIDControl box, MapStatus location)
        {
            if (location == MapStatus.Importing)
            {
                RemoveFromZone(box, MapStatus.Existing);
                UI.ImportZone.Controls.Add(box);
                ImportingMapPreviews.Add(box);
                box.MouseDown += ImportingBox_MouseDown;
            }
            else if (location == MapStatus.Existing)
            {
                RemoveFromZone(box, MapStatus.Importing);
                UI.ExistingZone.Controls.Add(box);
                ExistingMapPreviews.Add(box);
                box.MouseDown += ExistingBox_MouseDown;
            }
        }

        private void NewWorldOpened()
        {
            ImportingMapPreviews.Clear();
            ExistingMapPreviews.Clear();

            UI.MapViewZone.Visible = true;
            UI.ImportZone.Controls.Clear();
            UI.ExistingZone.Controls.Clear();
            foreach (var map in SelectedWorld.WorldMaps.OrderBy(x => x.Key))
            {
                MapIDControl mapbox = new MapIDControl(map.Key, new MapPreviewBox(map.Value));
                SendToZone(mapbox, MapStatus.Existing);
            }
            UI.Text = "Image Map – " + SelectedWorld.Name;
        }
    }
}
