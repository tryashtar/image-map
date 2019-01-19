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
        MapsNotImported
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
            if (edition == Edition.Java)
                SelectedWorld = new JavaWorld(folder);
            else if (edition == Edition.Bedrock)
                SelectedWorld = new BedrockWorld(folder);
            else
                throw new ArgumentException($"Don't know what edition {edition} is");
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
            var import = new ImportWindow();
            import.InterpolationModeBox.SelectedIndex = Properties.Settings.Default.InterpIndex;
            import.ApplyAllCheck.Checked = Properties.Settings.Default.ApplyAllCheck;
            import.StartImports(UI, imagepaths);
            Properties.Settings.Default.InterpIndex = import.InterpolationModeBox.SelectedIndex;
            Properties.Settings.Default.ApplyAllCheck = import.ApplyAllCheck.Checked;
            var taken = ImportingMapPreviews.Concat(ExistingMapPreviews).Select(x => x.ID).ToList();
            taken.Add(-1);
            long id = taken.Max() + 1;
            foreach (var image in import.OutputImages)
            {
                MapIDControl mapbox = new MapIDControl(id, SelectedWorld is JavaWorld ? (Map)new JavaMap(image) : new BedrockMap(image));
                ImportingMapPreviews.Add(mapbox);
                UI.ImportZone.Controls.Add(mapbox);
                mapbox.MouseDown += ImportingBox_MouseDown;
                id++;
            }
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
                box.Map.Image.Save(Path.Combine(folder, box.GetMapName() + ".png"));
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
            map.Map.Image.Save(file);
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

            UI.MapView.Visible = true;
            UI.ImportZone.Controls.Clear();
            UI.ExistingZone.Controls.Clear();
            foreach (var map in SelectedWorld.WorldMaps.OrderBy(x => x.Key))
            {
                MapIDControl mapbox = new MapIDControl(map.Key, map.Value);
                ExistingMapPreviews.Add(mapbox);
                UI.ExistingZone.Controls.Add(mapbox);
            }
            UI.Text = "Image Map – " + SelectedWorld.Name;
        }
    }
}
