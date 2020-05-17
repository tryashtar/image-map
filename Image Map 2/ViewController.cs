using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageMap
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
            var oldworld = SelectedWorld;
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
            oldworld?.Dispose();
            // that's right, I did it!
            GC.Collect();
            PlayerDestinations = new List<string>();
            PlayerDestinations.Add(LOCAL_IDENTIFIER);
            foreach (var uuid in SelectedWorld.GetPlayerIDs())
            {
                PlayerDestinations.Add(uuid);
            }
            return ActionResult.Success;
        }

        public IEnumerable<string> GetPlayerDestinations() => PlayerDestinations;

        private Task ImportFromSettings(long id, Edition edition, MapCreationSettings settings)
        {
            var number = settings.NumberOfMaps;
            var boxes = new MapIDControl[number];
            for (int i = 0; i < number; i++)
            {
                var box = new MapIDControl(id + i);
                boxes[i] = box;
                box.MouseDown += Box_MouseDown;
            }
            SendToZone(boxes, MapStatus.Importing);
            Task<IEnumerable<Map>> t;
            if (edition == Edition.Java)
                t = new Task<IEnumerable<Map>>(() =>
                {
                    return JavaMap.FromSettings(settings);
                });
            else
                t = new Task<IEnumerable<Map>>(() =>
                {
                    return BedrockMap.FromSettings(settings);
                });
            t.ContinueWith((prev) =>
            {
                settings.Dispose();
                if (t.IsFaulted)
                {
                    MessageBox.Show($"Failed to create some maps for this reason: {ExceptionMessage(t.Exception)}", "Map load error!");
                    RemoveFromZone(boxes, MapStatus.Importing);
                }
                var results = prev.Result.ToArray();
                for (int i = 0; i < number; i++)
                {
                    boxes[i].SetBox(new MapPreviewBox(results[i]));
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
            return t;
        }

        public void ImportImages(string[] imagepaths)
        {
            bool java = (SelectedWorld is JavaWorld);
            var import = new ImportWindow(java);
            import.InterpolationModeBox.SelectedIndex = Properties.Settings.Default.InterpIndex;
            import.ApplyAllCheck.Checked = Properties.Settings.Default.ApplyAllCheck;
            import.DitherChecked = Properties.Settings.Default.Dither;
            long id = GetSafeID();
            var tasks = new List<Task>();
            import.ImageReady += (s, settings) =>
            {
                var task = ImportFromSettings(id, java ? Edition.Java : Edition.Bedrock, settings);
                tasks.Add(task);
                task.Start();
                id += settings.NumberOfMaps;
            };
            import.StartImports(UI, imagepaths);
            Properties.Settings.Default.InterpIndex = import.InterpolationModeBox.SelectedIndex;
            Properties.Settings.Default.ApplyAllCheck = import.ApplyAllCheck.Checked;
            Properties.Settings.Default.Dither = import.DitherChecked;
            UI.ProcessingMapsStart();
            var done = Task.WhenAll(tasks);
            done.ContinueWith((t) =>
                {
                    UI.ProcessingMapsDone();
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private static string ExceptionMessage(Exception ex)
        {
            if (ex is AggregateException agg)
                return String.Join("\n", agg.InnerExceptions.Select(x => x.Message));
            return ex.Message;
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
            if (write.Any())
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
                box.Map.Image.Save(Path.Combine(folder, box.GetMapName() + ".png"));
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
            map.Map.Image.Save(file);
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


        private void Box_MouseDown(object sender, MouseEventArgs e)
        {
            var box = sender as MapIDControl;
            var where = WhereIsBox(box) ?? MapStatus.Importing;
            var previews = GetPreviews(where);
            var context = GetContextMenu(where);
            if (e.Button == MouseButtons.Right)
            {
                if (!box.Selected)
                {
                    foreach (var other in previews)
                    {
                        other.SetSelected(false);
                    }
                    box.SetSelected(true);
                }
                context.Show(box, new Point(e.X, e.Y));
            }
            else
                ClickSelect(box, where);
        }

        private List<MapIDControl> GetPreviews(MapStatus location)
        {
            if (location == MapStatus.Importing)
                return ImportingMapPreviews;
            if (location == MapStatus.Existing)
                return ExistingMapPreviews;
            throw new ArgumentException();
        }

        private ContextMenuStrip GetContextMenu(MapStatus location)
        {
            if (location == MapStatus.Importing)
                return UI.ImportContextMenu;
            if (location == MapStatus.Existing)
                return UI.ExistingContextMenu;
            throw new ArgumentException();
        }

        private void SendToZone(MapIDControl box, MapStatus location)
        {
            SendToZone(new MapIDControl[] { box }, location);
        }

        public void RemoveFromZone(MapIDControl box, MapStatus location)
        {
            RemoveFromZone(new MapIDControl[] { box }, location);
        }


        public void RemoveFromZone(IEnumerable<MapIDControl> boxes, MapStatus location)
        {
            foreach (var box in boxes)
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
                }
                else if (location == MapStatus.Existing)
                {
                    UI.ExistingZone.Controls.Remove(box);
                    ExistingMapPreviews.Remove(box);
                }
            }
        }

        private void SendToZone(IEnumerable<MapIDControl> boxes, MapStatus location)
        {
            var array = boxes.ToArray();
            if (location == MapStatus.Importing)
            {
                RemoveFromZone(boxes, MapStatus.Existing);
                UI.ImportZone.Controls.AddRange(array);
                ImportingMapPreviews.AddRange(array);
                UI.ClickOpenLabel.Visible = false;
            }
            else if (location == MapStatus.Existing)
            {
                RemoveFromZone(boxes, MapStatus.Importing);
                UI.ExistingZone.Controls.AddRange(array);
                ExistingMapPreviews.AddRange(array);
            }
        }

        private MapStatus? WhereIsBox(MapIDControl box)
        {
            if (ImportingMapPreviews.Contains(box))
                return MapStatus.Importing;
            if (ExistingMapPreviews.Contains(box))
                return MapStatus.Existing;
            return null;
        }

        private void NewWorldOpened()
        {
            ImportingMapPreviews.Clear();
            ExistingMapPreviews.Clear();

            UI.MapViewZone.Visible = true;
            UI.ImportZone.Controls.Clear();
            UI.ImportZone.Controls.Add(UI.ClickOpenLabel);
            UI.ClickOpenLabel.Visible = true;
            UI.ExistingZone.Controls.Clear();
            UI.ProcessingMapsDone();
            foreach (var map in SelectedWorld.WorldMaps.OrderBy(x => x.Key))
            {
                MapIDControl mapbox = new MapIDControl(map.Key, new MapPreviewBox(map.Value));
                mapbox.MouseDown += Box_MouseDown;
                SendToZone(mapbox, MapStatus.Existing);
            }
            UI.Text = "Image Map – " + SelectedWorld.Name;
        }
    }
}
