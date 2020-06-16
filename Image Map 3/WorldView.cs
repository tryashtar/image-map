using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections.Concurrent;

namespace ImageMap
{
    public partial class WorldView : UserControl
    {
        private readonly Dictionary<long, Map> ImportingMaps = new Dictionary<long, Map>();
        // simulate a threadsafe set
        private readonly ConcurrentDictionary<PendingMapsWithID, PendingMapsWithID> ProcessingMaps = new ConcurrentDictionary<PendingMapsWithID, PendingMapsWithID>();
        private MinecraftWorld World;
        private EditionProperties ActiveEdition => EditionProperties.FromEdition(World.Edition);

        public WorldView()
        {
            InitializeComponent();
        }

        public void SetWorld(MinecraftWorld world)
        {
            if (World != null)
                World.MapsChanged -= World_MapsChanged;
            World = world;
            World.MapsChanged += World_MapsChanged;
            ExistingZone.Controls.Clear();
            ImportZone.Controls.Clear();
        }

        public void Import(IEnumerable<string> paths)
        {
            var window = PrepareImportWindow();
            window.StartImports(ParentForm, paths);
            CloseImportWindow(window);
        }

        public void Import(Image image)
        {
            var window = PrepareImportWindow();
            window.StartImports(ParentForm, image);
            CloseImportWindow(window);
        }

        public bool HasUnsavedChanges()
        {
            return ImportingMaps.Any() || ProcessingMaps.Any();
        }

        private ImportWindow PrepareImportWindow()
        {
            var window = ActiveEdition.CreateImportWindow();
            window.InterpolationModeBox.SelectedIndex = Properties.Settings.Default.InterpIndex;
            window.ApplyAllCheck.Checked = Properties.Settings.Default.ApplyAllCheck;
            window.DitherChecked = Properties.Settings.Default.Dither;
            window.StretchChecked = Properties.Settings.Default.Stretch;
            window.ImageReady += Window_ImageReady;
            return window;
        }

        private void Window_ImageReady(object sender, MapCreationSettings settings)
        {
            var pending = new PendingMapsWithID(GetSafeID(), settings, ActiveEdition);
            CreateEmptyIDControls(pending);
            pending.Finished += Pending_Finished;
            ProcessingMaps.TryAdd(pending, pending);
        }

        private void Pending_Finished(object sender, EventArgs e)
        {
            var pending = (PendingMapsWithID)sender;
            ProcessingMaps.TryRemove(pending, out _);
            foreach (var item in pending.ResultMaps)
            {
                ImportingMaps[item.Key] = item.Value;
            }
            UpdateImportView();
        }

        private void UpdateImportView()
        {
            foreach (var control in ImportZone.Controls)
            {
                if (control is MapIDControl box)
                {
                    if (!box.HasBox && ImportingMaps.TryGetValue(box.ID, out var map))
                        box.SetBox(new MapPreviewBox(map));
                }
            }
        }

        private void CloseImportWindow(ImportWindow window)
        {
            Properties.Settings.Default.InterpIndex = window.InterpolationModeBox.SelectedIndex;
            Properties.Settings.Default.ApplyAllCheck = window.ApplyAllCheck.Checked;
            Properties.Settings.Default.Dither = window.DitherChecked;
            Properties.Settings.Default.Stretch = window.StretchChecked;
        }

        private IEnumerable<MapIDControl> CreateEmptyIDControls(PendingMapsWithID maps)
        {
            var boxes = new MapIDControl[maps.MapCount];
            int i = -1;
            foreach (var id in maps.IDs)
            {
                i++;
                var box = new MapIDControl(id);
                box.MouseDown += Box_MouseDown;
                boxes[i] = box;
            }
            ImportZone.Controls.AddRange(boxes);
            return boxes;
        }


        private void RemoveFromZone(IEnumerable<MapIDControl> boxes, MapStatus place) => throw new NotImplementedException();
        private void SelectAll(MapStatus place) => throw new NotImplementedException();
        private void DeselectAll(MapStatus place) => throw new NotImplementedException();

        private long GetSafeID()
        {
            var taken = ImportingMaps.Concat(World.WorldMaps).Select(x => x.Key).Concat(ProcessingMaps.SelectMany(x => x.Value.IDs)).ToList();
            taken.Add(-1);
            return taken.Max() + 1;
        }

        private void ChangeMapIDs(IEnumerable<MapIDControl> boxes, MapStatus area)
        {
            //var input = new IDInputDialog(boxes.First().ID);
            //input.ShowDialog(this);
            //if (input.Confirmed)
            //{
            //    long firstid;
            //    if (input.WantsAuto)
            //        firstid = Controller.GetSafeID();
            //    else
            //        firstid = input.SelectedID;
            //    int count = Controller.ChangeMapIDs(boxes, firstid, area, MapReplaceOption.Info);
            //    if (count > 0)
            //    {
            //        var picker = new ReplaceOptionDialog(count);
            //        picker.ShowDialog(this);
            //        Controller.ChangeMapIDs(boxes, firstid, area, picker.SelectedOption);
            //    }
            //    else
            //        Controller.ChangeMapIDs(boxes, firstid, area, MapReplaceOption.Skip);
            //}
        }

        private void World_MapsChanged(object sender, EventArgs e)
        {
            ExistingZone.Controls.Clear();
            ExistingZone.Controls.AddRange(World.WorldMaps.Select(x => new MapIDControl(x.Key, new MapPreviewBox(x.Value))).ToArray());
        }

        private void Box_MouseDown(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            //SendMapsWithMessage(Controller.GetAllMaps(MapStatus.Importing), AddChestCheck.Checked ? ViewController.LOCAL_IDENTIFIER : ViewController.NOBODY_IDENTIFIER);
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            var open_dialog = new OpenFileDialog()
            {
                Title = "Import image files to turn into maps",
                Multiselect = true,
                Filter = Util.GenerateFilter("Image Files", Util.ImageExtensions)
            };
            open_dialog.InitialDirectory = Properties.Settings.Default.LastOpenPath;
            if (Util.ShowCompatibleOpenDialog(open_dialog) == DialogResult.OK)
            {
                Properties.Settings.Default.LastOpenPath = Path.GetDirectoryName(open_dialog.FileName);
                Import(open_dialog.FileNames);
            }
        }

        private void ImportZone_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.Html))
                e.Effect = DragDropEffects.Copy;
        }

        private void ImportZone_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            Import(files);
        }

        private void ImportContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //if (Controller.GetAllMaps(MapStatus.Importing).All(x => x.Selected))
            //    ImportContextSelectAll.Text = "Deselect all";
            //else
            //    ImportContextSelectAll.Text = "Select all";
            //ImportContextSend.Enabled = !IsProcessingMaps;
            //ImportContextDiscard.Enabled = !IsProcessingMaps;
            //ImportContextChangeID.Enabled = !IsProcessingMaps;
        }

        private void ExistingContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //ExistingContextAdd.DropDownItems.Clear();
            //ExistingContextAdd.DropDownItems.AddRange(Controller.GetPlayerDestinations().Select(x => new ToolStripMenuItem(x, null, ExistingContextPlayerName_Click)).ToArray());
            //if (Controller.GetAllMaps(MapStatus.Existing).All(x => x.Selected))
            //    ExistingContextSelectAll.Text = "Deselect all";
            //else
            //    ExistingContextSelectAll.Text = "Select all";
        }

        private void ImportContextSend_Click(object sender, EventArgs e)
        {
            //SendMapsWithMessage(Controller.GetSelectedMaps(MapStatus.Importing), AddChestCheck.Checked ? ViewController.LOCAL_IDENTIFIER : ViewController.NOBODY_IDENTIFIER);
        }

        private void ImportContextChangeID_Click(object sender, EventArgs e)
        {
            //ChangeMapIDs(Controller.GetSelectedMaps(MapStatus.Importing).ToArray(), MapStatus.Importing);
        }

        private void ImportContextDiscard_Click(object sender, EventArgs e)
        {
            //var selected = Controller.GetSelectedMaps(MapStatus.Importing).ToArray();
            //foreach (var box in selected)
            //{
            //    Controller.RemoveFromZone(box, MapStatus.Importing);
            //    box.Map.Dispose();
            //}
        }

        private void ImportContextSelectAll_Click(object sender, EventArgs e)
        {
            //if (Controller.GetAllMaps(MapStatus.Importing).All(x => x.Selected))
            //    Controller.DeselectAll(MapStatus.Importing);
            //else
            //    Controller.SelectAll(MapStatus.Importing);
        }

        private void ExistingContextAdd_Click(object sender, EventArgs e)
        { }

        private void ExistingContextChangeID_Click(object sender, EventArgs e)
        {
            //ChangeMapIDs(Controller.GetSelectedMaps(MapStatus.Existing).ToArray(), MapStatus.Existing);
        }

        private void ExistingContextExport_Click(object sender, EventArgs e)
        {
            //var selected = Controller.GetSelectedMaps(MapStatus.Existing);
            //// super epic way to check if there is exactly one item
            //bool onlyone = selected.Take(2).Count() == 1;
            //var export_dialog = new SaveFileDialog()
            //{
            //    Title = "Export this map as a PNG",
            //    Filter = "Image Files|*.png|All Files|*.*"
            //};
            //if (onlyone)
            //    export_dialog.FileName = selected.First().GetMapName() + ".png";
            //else
            //    export_dialog.FileName = "";
            //export_dialog.InitialDirectory = LastImgExportPath;
            //if (export_dialog.ShowDialog() == DialogResult.OK)
            //{
            //    LastImgExportPath = Path.GetDirectoryName(export_dialog.FileName);
            //    if (onlyone)
            //        Controller.SaveMap(selected.First(), export_dialog.FileName);
            //    else
            //        Controller.SaveMaps(selected, Path.ChangeExtension(export_dialog.FileName, ""));
            //}
        }

        private void ExistingContextDelete_Click(object sender, EventArgs e)
        {
            //var selected = Controller.GetSelectedMaps(MapStatus.Existing);
            //if (selected.Any() && MessageBox.Show("Deleting these maps will remove all copies from the world permanently.\n\nWould you like to delete these maps?", $"Delete {selected.Count()} maps?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //    Controller.DeleteMapsFromWorld(selected);
        }

        private void ExistingContextPlayerName_Click(object sender, EventArgs e)
        {
            //var playername = ((ToolStripMenuItem)sender).Text;
            //try
            //{
            //    bool success = Controller.AddChests(Controller.GetSelectedMaps(MapStatus.Existing).Select(x => x.ID), playername);
            //    if (!success)
            //        MessageBox.Show("There wasn't enough space to fit the chests in your inventory. One or more were not added.", "Chest alert!");
            //}
            //catch (FileNotFoundException ex)
            //{
            //    MessageBox.Show($"Could not find any player in that world with name {playername}\n\nFull error: {ex.Message}", "Player not found");
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Unknown error happened: {ex.Message}", "Error!");
            //}
        }

        private void ExistingContextSelectAll_Click(object sender, EventArgs e)
        {
            //if (Controller.GetAllMaps(MapStatus.Existing).All(x => x.Selected))
            //    Controller.DeselectAll(MapStatus.Existing);
            //else
            //    Controller.SelectAll(MapStatus.Existing);
        }

        private void SendMapsWithMessage(IEnumerable<MapIDControl> maps, string destination)
        {
            //int conflicts = Controller.SendMapsToWorld(maps, MapReplaceOption.Info, destination);
            //if (conflicts > 0)
            //{
            //    var option = new ReplaceOptionDialog(conflicts);
            //    option.ShowDialog(this);
            //    Controller.SendMapsToWorld(maps, option.SelectedOption, destination);
            //}
            //else
            //    Controller.SendMapsToWorld(maps, MapReplaceOption.ReplaceExisting, destination);
        }

        private void PasteShortcut_Click(object sender, EventArgs e)
        {
            if (MapTabs.SelectedTab == ImportTab)
            {
                if (Clipboard.ContainsFileDropList())
                {
                    var files = Clipboard.GetFileDropList();
                    string[] array = new string[files.Count];
                    files.CopyTo(array, 0);
                    Import(array);
                }
                else if (Clipboard.ContainsImage())
                {
                    var image = Clipboard.GetImage();
                    Import(image);
                }
            }
        }

        private void SelectAllShortcut_Click(object sender, EventArgs e)
        {
            if (MapTabs.SelectedTab == ImportTab)
                SelectAll(MapStatus.Importing);
            else if (MapTabs.SelectedTab == ExistingTab)
                SelectAll(MapStatus.Existing);
        }

        private void DeselectAllShortcut_Click(object sender, EventArgs e)
        {
            if (MapTabs.SelectedTab == ImportTab)
                DeselectAll(MapStatus.Importing);
            else if (MapTabs.SelectedTab == ExistingTab)
                DeselectAll(MapStatus.Existing);
        }

        private void DeleteShortcut_Click(object sender, EventArgs e)
        {
            if (MapTabs.SelectedTab == ImportTab)
                ImportContextDiscard_Click(this, EventArgs.Empty);
            else if (MapTabs.SelectedTab == ExistingTab)
                ExistingContextDelete_Click(this, EventArgs.Empty);
        }
    }
}
