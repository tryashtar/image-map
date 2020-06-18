using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace ImageMap
{
    public partial class WorldView : UserControl
    {
        private ImportPreview ImportSide;
        private WorldPreview WorldSide;
        private HalfPreview ActivePreview
        {
            get
            {
                if (MapTabs.SelectedTab == ImportTab)
                    return ImportSide;
                else
                    return WorldSide;
            }
        }

        private MinecraftWorld World;
        private EditionProperties ActiveEdition => EditionProperties.FromEdition(World.Edition);

        public WorldView()
        {
            InitializeComponent();
        }

        public void SetWorld(MinecraftWorld world)
        {
            World = world;

            if (ImportSide != null)
                ImportSide.ControlsChanged -= ImportSide_ControlsChanged;
            ImportSide = new ImportPreview();
            ImportSide.ContextMenu = ImportContextMenu;
            ImportSide.ControlsChanged += ImportSide_ControlsChanged;
            ImportSide_ControlsChanged(this, EventArgs.Empty);

            if (WorldSide != null)
                WorldSide.ControlsChanged -= WorldSide_ControlsChanged;
            WorldSide = new WorldPreview(world);
            WorldSide.ContextMenu = ExistingContextMenu;
            WorldSide.ControlsChanged += WorldSide_ControlsChanged;
            WorldSide_ControlsChanged(this, EventArgs.Empty);
        }

        private void ImportSide_ControlsChanged(object sender, EventArgs e)
        {
            Util.SetControls(ImportZone, ImportSide.MapIDControls);
            ClickOpenLabel.Visible = !ImportSide.MapIDControls.Any();
            DetermineTransferConflicts();
        }

        private void WorldSide_ControlsChanged(object sender, EventArgs e)
        {
            Util.SetControls(ExistingZone, WorldSide.MapIDControls);
            DetermineTransferConflicts();
        }

        public bool HasUnsavedChanges()
        {
            return ImportSide?.HasAnyMaps() ?? false;
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

        private ImportWindow PrepareImportWindow()
        {
            var window = ActiveEdition.CreateImportWindow();
            window.InterpolationModeBox.SelectedIndex = Properties.Settings.Default.InterpIndex;
            window.ColorAlgorithmBox.SelectedIndex = Properties.Settings.Default.AlgorithmIndex;
            window.DitherChecked = Properties.Settings.Default.Dither;
            window.StretchChecked = Properties.Settings.Default.Stretch;
            window.ImageReady += Window_ImageReady;
            return window;
        }

        private void Window_ImageReady(object sender, MapCreationSettings settings)
        {
            var pending = new PendingMapsWithID(GetSafeID(), settings, ActiveEdition);
            ImportSide.AddPending(pending);
        }

        private void CloseImportWindow(ImportWindow window)
        {
            Properties.Settings.Default.InterpIndex = window.InterpolationModeBox.SelectedIndex;
            Properties.Settings.Default.AlgorithmIndex = window.ColorAlgorithmBox.SelectedIndex;
            Properties.Settings.Default.Dither = window.DitherChecked;
            Properties.Settings.Default.Stretch = window.StretchChecked;
        }

        private long GetSafeID()
        {
            var taken = ImportSide.GetTakenIDs().Concat(WorldSide.GetTakenIDs()).ToList();
            taken.Add(-1);
            return taken.Max() + 1;
        }

        private void InputChangeMapIDs(IEnumerable<MapIDControl> boxes)
        {
            var input = new IDInputDialog(boxes.First().ID);
            input.ShowDialog(this);
            if (input.Confirmed)
            {
                long id;
                if (input.WantsAuto)
                    id = GetSafeID();
                else
                    id = input.SelectedID;

                var current_maps = ActivePreview.GetMaps();
                var used_ids = current_maps.Keys.Except(boxes.Select(x => x.ID));
                var conflicted_maps = Util.CreateRange(id, boxes.Count()).Where(x => used_ids.Contains(x));
                var replace_option = MapReplaceOption.ReplaceExisting;
                if (conflicted_maps.Any())
                {
                    var picker = new ReplaceOptionDialog(conflicted_maps.Count());
                    picker.ShowDialog(this);
                    if (picker.Confirmed)
                        replace_option = picker.SelectedOption;
                    else
                        return;
                }
                foreach (var box in boxes.ToList())
                {
                    if (replace_option == MapReplaceOption.ChangeExisting && conflicted_maps.Contains(id))
                        ActivePreview.ChangeMapID(id, GetSafeID());
                    else if (replace_option == MapReplaceOption.Skip && conflicted_maps.Contains(id))
                    { /* do nothing */}
                    else
                        ActivePreview.ChangeMapID(box.ID, id);
                    id++;
                }
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            SendMapsCheckConflicts(ImportSide.MapIDControls, AddChestCheck.Checked, null);
        }

        private void ImportContextSend_Click(object sender, EventArgs e)
        {
            SendMapsCheckConflicts(ImportSide.SelectedControls, AddChestCheck.Checked, null);
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
            if (ImportSide.MapIDControls.All(x => x.IsSelected))
                ImportContextSelectAll.Text = "Deselect all";
            else
                ImportContextSelectAll.Text = "Select all";
        }

        private void ExistingContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ExistingContextAdd.DropDownItems.Clear();
            ExistingContextAdd.DropDownItems.Add(new ToolStripMenuItem("Local player", null, ExistingContextChestLocal_Click));
            ExistingContextAdd.DropDownItems.AddRange(World.GetPlayerIDs().Select(x => new ToolStripMenuItem(x, null, ExistingContextChestUUID_Click)).ToArray());
            if (WorldSide.MapIDControls.All(x => x.IsSelected))
                ExistingContextSelectAll.Text = "Deselect all";
            else
                ExistingContextSelectAll.Text = "Select all";
        }

        private void ImportContextChangeID_Click(object sender, EventArgs e)
        {
            InputChangeMapIDs(ImportSide.SelectedControls);
        }

        private void ExistingContextChangeID_Click(object sender, EventArgs e)
        {
            InputChangeMapIDs(WorldSide.SelectedControls);
        }

        private void ContextExport_Click(object sender, EventArgs e)
        {
            var selected = ActivePreview.SelectedControls;
            bool onlyone = Util.IsSingleSequence(selected);
            var export_dialog = new SaveFileDialog()
            {
                Title = $"Export {Util.Pluralize(selected.Count(), "map")} as PNG",
                Filter = "Image Files|*.png|All Files|*.*"
            };
            if (onlyone)
                export_dialog.FileName = selected.First().GetMapName() + ".png";
            else
                export_dialog.FileName = "";
            export_dialog.InitialDirectory = Properties.Settings.Default.LastImgExportPath;
            if (export_dialog.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.LastImgExportPath = Path.GetDirectoryName(export_dialog.FileName);
                if (onlyone)
                    SaveMap(selected.First(), export_dialog.FileName);
                else
                    SaveMaps(selected, Path.ChangeExtension(export_dialog.FileName, ""));
            }
        }

        private void ImportContextDiscard_Click(object sender, EventArgs e)
        {
            var selected = ImportSide.SelectedControls;
            ImportSide.RemoveMaps(selected.Select(x => x.ID));
        }

        private void ExistingContextDelete_Click(object sender, EventArgs e)
        {
            var selected = WorldSide.SelectedControls;
            if (selected.Any() && MessageBox.Show("Deleting these maps will remove all copies from the world permanently.\n\nWould you like to delete these maps?", $"Delete { Util.Pluralize(selected.Count(), "map")}?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                World.RemoveMaps(selected.Select(x => x.ID));
        }

        private void AddChests(IEnumerable<MapIDControl> maps, bool local, string uuid)
        {
            try
            {
                var ids = maps.Select(x => x.ID);
                bool success = true;
                if (local)
                    success = World.AddChestsLocalPlayer(ids);
                else if (uuid != null)
                    success = World.AddChests(ids, uuid);
                if (!success)
                    MessageBox.Show("There wasn't enough space to fit the chests in your inventory. One or more were not added.", "Chest alert!");
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"Could not find any player in that world with name {uuid}\n\n{Util.ExceptionMessage(ex)}", "Player not found");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unknown error happened.\n\n{Util.ExceptionMessage(ex)}", "Error!");
            }
        }

        private void ExistingContextChestUUID_Click(object sender, EventArgs e)
        {
            var uuid = ((ToolStripMenuItem)sender).Text;
            AddChests(WorldSide.SelectedControls, false, uuid);
        }

        private void ExistingContextChestLocal_Click(object sender, EventArgs e)
        {
            AddChests(WorldSide.SelectedControls, true, null);
        }

        private void ContextSelectAll_Click(object sender, EventArgs e)
        {
            if (ActivePreview.MapIDControls.All(x => x.IsSelected))
                ActivePreview.DeselectAll();
            else
                ActivePreview.SelectAll();
        }

        private void SendMapsCheckConflicts(IEnumerable<MapIDControl> maps, bool local, string uuid)
        {
            var world = WorldSide.GetMaps();
            var conflicts = maps.Where(x => world.ContainsKey(x.ID));
            if (conflicts.Any())
            {
                var option = new ReplaceOptionDialog(conflicts.Count());
                option.ShowDialog(this);
                if (option.Confirmed)
                    SendMapsToWorld(maps, option.SelectedOption, local, uuid);
            }
            else
                SendMapsToWorld(maps, MapReplaceOption.ReplaceExisting, local, uuid);
        }

        private void SendMapsToWorld(IEnumerable<MapIDControl> maps, MapReplaceOption option, bool local, string uuid)
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
            if (option == MapReplaceOption.ChangeExisting)
            {
                foreach (var conflict in conflictids)
                {
                    World.ChangeMapID(conflict, GetSafeID());
                }
            }
            ImportSide.RemoveMaps(writemaps.Select(x => x.ID));
            World.AddMaps(writemaps.ToDictionary(x => x.ID, x => x.Map));
            AddChests(writemaps, local, uuid);
        }

        private void DetermineTransferConflicts()
        {
            foreach (var box in ImportSide.MapIDControls)
            {
                var maps = WorldSide.GetMaps();
                box.SetConflict(maps.ContainsKey(box.ID));
            }
        }

        private void SaveMaps(IEnumerable<MapIDControl> maps, string folder)
        {
            Directory.CreateDirectory(folder);
            foreach (var box in maps)
            {
                box.Map.Image.Save(Path.Combine(folder, box.GetMapName() + ".png"));
            }
        }

        private void SaveMap(MapIDControl map, string file)
        {
            map.Map.Image.Save(file);
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
            ActivePreview.SelectAll();
        }

        private void DeselectAllShortcut_Click(object sender, EventArgs e)
        {
            ActivePreview.DeselectAll();
        }

        private void DeleteShortcut_Click(object sender, EventArgs e)
        {
            if (ActivePreview == ImportSide)
                ImportContextDiscard_Click(this, EventArgs.Empty);
            else if (ActivePreview == WorldSide)
                ExistingContextDelete_Click(this, EventArgs.Empty);
        }
    }
}
