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

    public enum MapStatus
    {
        Importing,
        Existing
    }

    public class ViewController
    {
        private const string LOCAL_IDENTIFIER = "(singleplayer)";
        private const string NOBODY_IDENTIFIER = "(nobody)";
        MinecraftWorld SelectedWorld;
        TheForm UI;
        List<MapIDControl> ImportingMapPreviews;
        List<MapIDControl> ExistingMapPreviews;

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
            UI.PlayerSelector.Items.Clear();
            UI.PlayerSelector.Items.Add(LOCAL_IDENTIFIER);
            UI.PlayerSelector.Items.Add(NOBODY_IDENTIFIER);
            UI.PlayerSelector.Items.AddRange(SelectedWorld.GetPlayerIDs().ToArray());
            UI.PlayerSelector.SelectedIndex = Properties.Settings.Default.GiveChest ? 0 : 1;
            NewWorldOpened();
            return ActionResult.Success;
        }

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
            var taken = ImportingMapPreviews.Concat(ExistingMapPreviews).Select(x => x.ID).ToList();
            taken.Add(-1);
            long id = taken.Max();
            var tasks = new List<Task>();
            UI.OpenButton.Enabled = false;
            UI.SendButton.Enabled = false;
            foreach (var premap in import.OutputImages)
            {
                id++;
                MapIDControl box = new MapIDControl(id);
                ImportingMapPreviews.Add(box);
                UI.ImportZone.Controls.Add(box);
                box.MouseDown += ImportingBox_MouseDown;
                var t = new Task<MapPreviewBox>(() =>
                {
                    return new MapPreviewBox(premap, SelectedWorld is JavaWorld ? Edition.Java : Edition.Bedrock);
                });
                t.Start();
                t.ContinueWith((prev) =>
                {
                    box.SetBox(prev.Result);
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

        public IEnumerable<MapIDControl> GetSelectedMaps()
        {
            return ExistingMapPreviews.Where(x => x.Selected);
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
                ExistingMapPreviews.Remove(box);
                UI.ExistingZone.Controls.Remove(box);
                SelectedWorld.RemoveMap(box.ID);
            }
        }

        public void SaveMap(MapIDControl map, string file)
        {
            map.Map.Image.GetImage().Save(file);
        }

        public bool UnsavedChanges()
        {
            return ImportingMapPreviews.Any();
        }

        public void SendMapsToWorld()
        {
            SendMapsToWorld(NOBODY_IDENTIFIER);
        }

        public void SendMapsToWorld(string playerid)
        {
            SelectedWorld.AddMaps(ImportingMapPreviews.ToDictionary(x => x.ID, x => x.Map));
            var ids = ImportingMapPreviews.Select(x => x.ID);
            AddChests(ids, playerid);
            ExistingMapPreviews.AddRange(ImportingMapPreviews);
            UI.ExistingZone.Controls.AddRange(ImportingMapPreviews.ToArray());
            ImportingMapPreviews.Clear();
            UI.ImportZone.Controls.Clear();
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
            var boxes = area == MapStatus.Importing ? ImportingMapPreviews : ExistingMapPreviews;
            foreach (var box in boxes)
            {
                box.SetSelected(true);
            }
        }

        public void DeselectAll(MapStatus area)
        {
            var boxes = area == MapStatus.Importing ? ImportingMapPreviews : ExistingMapPreviews;
            foreach (var box in boxes)
            {
                box.SetSelected(false);
            }
        }

        // right-click maps to remove them
        private void ImportingBox_MouseDown(object sender, MouseEventArgs e)
        {
            MapIDControl box = sender as MapIDControl;
            if (e.Button == MouseButtons.Right)
            {
                ImportingMapPreviews.Remove(box);
                UI.ImportZone.Controls.Remove(box);
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
                ExistingMapPreviews.Add(mapbox);
                UI.ExistingZone.Controls.Add(mapbox);
            }
            UI.Text = "Image Map – " + SelectedWorld.Name;
        }
    }
}
