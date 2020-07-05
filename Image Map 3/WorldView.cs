using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;

namespace ImageMap
{
    public partial class WorldView : UserControl
    {
        private ImportMaps ImportSide;
        private MinecraftWorld WorldSide;
        private IMapSource ActiveSide
        {
            get
            {
                if (MapTabs.SelectedTab == ImportTab)
                    return ImportSide;
                else
                    return WorldSide;
            }
        }
        private MapPreviewPanel ActiveZone
        {
            get
            {
                if (MapTabs.SelectedTab == ImportTab)
                    return ImportZone;
                else
                    return ExistingZone;
            }
        }

        private EditionProperties ActiveEdition => EditionProperties.FromEdition(WorldSide.Edition);

        public WorldView()
        {
            InitializeComponent();
            ImportZone.SetContextMenu(ImportContextMenu);
            ExistingZone.SetContextMenu(ExistingContextMenu);
        }

        public void SetWorld(MinecraftWorld world)
        {
            WorldSide = world;
            WorldSide.MapsChanged += WorldSide_MapsChanged;
            WorldSide_MapsChanged(this, EventArgs.Empty);
            ImportSide = new ImportMaps();
            ImportSide.MapsChanged += ImportSide_MapsChanged;
            ImportSide_MapsChanged(this, EventArgs.Empty);
        }

        private void ImportSide_MapsChanged(object sender, EventArgs e)
        {
            ImportZone.SetMapsImport(ImportSide);
            ClickOpenLabel.Visible = !ImportSide.HasAnyMaps();
            DetermineTransferConflicts();
        }

        private void WorldSide_MapsChanged(object sender, EventArgs e)
        {
            ExistingZone.SetMaps(WorldSide.GetMaps());
            DetermineTransferConflicts();
        }

        private void DetermineTransferConflicts()
        {
            ImportZone.SetConflicts(WorldSide.GetTakenIDs());
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
            var pending = new PendingMapsWithID(GetSafeID(), settings, WorldSide);
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

        private void InputChangeMapIDs(IEnumerable<long> originals)
        {
            var input = new IDInputDialog(originals.First());
            input.ShowDialog(this);
            if (input.Confirmed)
            {
                long firstid;
                if (input.WantsAuto)
                    firstid = GetSafeID();
                else
                    firstid = input.SelectedID;
                var desired_range = Util.CreateRange(firstid, originals.Count());
                var from_to = originals.OrderBy(x => x).Zip(desired_range.OrderBy(x => x), (x, y) => new KeyValuePair<long, long>(x, y));

                // the map IDs that are currently being used and are not changing (potentially conflicting with the ones we want)
                var taken_ids = ActiveSide.GetTakenIDs().Except(originals);
                var conflicts = taken_ids.Intersect(desired_range);

                var replace_option = MapReplaceOption.ReplaceExisting;
                if (conflicts.Any())
                {
                    var picker = new ReplaceOptionDialog(conflicts.Count());
                    picker.ShowDialog(this);
                    if (picker.Confirmed)
                        replace_option = picker.SelectedOption;
                    else
                        return;
                }

                // go reverse so that we can change 0->1->2->3 without replacing 1 before we can change it to 2
                if (firstid > originals.First())
                    from_to = from_to.Reverse();

                foreach (var change in from_to.ToList())
                {
                    if (replace_option == MapReplaceOption.ChangeExisting && conflicts.Contains(change.Value))
                        ActiveSide.ChangeMapID(change.Value, GetSafeID());
                    else if (replace_option == MapReplaceOption.Skip && conflicts.Contains(change.Value))
                    { /* do nothing */}
                    else
                        ActiveSide.ChangeMapID(change.Key, change.Value);
                }
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            SendMapsCheckConflicts(ImportZone.ReadyMaps.Keys, AddChestCheck.Checked, null);
        }

        private void ImportContextSend_Click(object sender, EventArgs e)
        {
            SendMapsCheckConflicts(ImportZone.ReadySelectedMaps.Keys, AddChestCheck.Checked, null);
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
            if (ImportZone.AllAreSelected)
                ImportContextSelectAll.Text = "Deselect all";
            else
                ImportContextSelectAll.Text = "Select all";
        }

        private void ExistingContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ExistingContextAdd.DropDownItems.Clear();
            ExistingContextAdd.DropDownItems.Add(new ToolStripMenuItem("Local player", null, ExistingContextChestLocal_Click));
            ExistingContextAdd.DropDownItems.AddRange(WorldSide.GetPlayerIDs().Select(x => new ToolStripMenuItem(x, null, ExistingContextChestUUID_Click)).ToArray());
            if (ExistingZone.AllAreSelected)
                ExistingContextSelectAll.Text = "Deselect all";
            else
                ExistingContextSelectAll.Text = "Select all";
        }

        private void ImportContextChangeID_Click(object sender, EventArgs e)
        {
            InputChangeMapIDs(ImportZone.AllSelectedMaps.Keys);
        }

        private void ExistingContextChangeID_Click(object sender, EventArgs e)
        {
            InputChangeMapIDs(ExistingZone.AllSelectedMaps.Keys);
        }

        private void ContextExport_Click(object sender, EventArgs e)
        {
            var selected = ActiveZone.AllSelectedMaps;
            bool onlyone = Util.IsSingleSequence(selected);
            var export_dialog = new SaveFileDialog()
            {
                Title = $"Export {Util.Pluralize(selected.Count(), "map")} as PNG",
                Filter = "Image Files|*.png|All Files|*.*"
            };
            if (onlyone)
                export_dialog.FileName = Util.MapName(selected.Keys.First()) + ".png";
            else
                export_dialog.FileName = "";
            export_dialog.InitialDirectory = Properties.Settings.Default.LastImgExportPath;
            if (export_dialog.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.LastImgExportPath = Path.GetDirectoryName(export_dialog.FileName);
                if (onlyone)
                    SaveMap(selected.Values.First(), export_dialog.FileName);
                else
                    SaveMaps(selected, Path.ChangeExtension(export_dialog.FileName, ""));
            }
        }

        private void ImportContextDiscard_Click(object sender, EventArgs e)
        {
            var selected = ImportZone.AllSelectedMaps;
            ImportSide.RemoveMaps(selected.Keys);
        }

        private void ExistingContextDelete_Click(object sender, EventArgs e)
        {
            var selected = ExistingZone.AllSelectedMaps;
            if (selected.Any() && MessageBox.Show("Deleting these maps will remove all copies from the world permanently.\n\nWould you like to delete these maps?", $"Delete { Util.Pluralize(selected.Count(), "map")}?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                WorldSide.RemoveMaps(selected.Keys);
        }

        private void AddChests(IEnumerable<long> maps, bool local, string uuid)
        {
            try
            {
                bool success = true;
                if (local)
                    success = WorldSide.AddChestsLocalPlayer(maps);
                else if (uuid != null)
                    success = WorldSide.AddChests(maps, uuid);
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
            AddChests(ExistingZone.AllSelectedMaps.Keys, false, uuid);
        }

        private void ExistingContextChestLocal_Click(object sender, EventArgs e)
        {
            AddChests(ExistingZone.AllSelectedMaps.Keys, true, null);
        }

        private void ContextSelectAll_Click(object sender, EventArgs e)
        {
            if (ActiveZone.AllAreSelected)
                ActiveZone.DeselectAll();
            else
                ActiveZone.SelectAll();
        }

        private void SendMapsCheckConflicts(IEnumerable<long> maps, bool local, string uuid)
        {
            var world = WorldSide.GetTakenIDs();
            var conflicts = maps.Where(x => world.Contains(x));
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

        private void SendMapsToWorld(IEnumerable<long> maps, MapReplaceOption option, bool local, string uuid)
        {
            var worldids = WorldSide.GetTakenIDs();
            var writemaps = maps.ToList();
            // check for  conflicts
            foreach (var map in maps)
            {
                bool conflicted = worldids.Contains(map);
                if (conflicted)
                {
                    if (option == MapReplaceOption.ChangeExisting)
                        WorldSide.ChangeMapID(map, GetSafeID());
                    else if (option == MapReplaceOption.Skip)
                        writemaps.Remove(map);
                }
            }
            var import = ImportSide.GetMaps().Copy();
            ImportSide.RemoveMaps(writemaps);
            WorldSide.AddMaps(writemaps.ToDictionary(x => x, x => import[x]));
            AddChests(writemaps, local, uuid);
        }

        private void SaveMaps(IReadOnlyDictionary<long, Map> maps, string folder)
        {
            Directory.CreateDirectory(folder);
            foreach (var map in maps)
            {
                map.Value.Image.Save(Path.Combine(folder, Util.MapName(map.Key) + ".png"));
            }
        }

        private void SaveMap(Map map, string file)
        {
            map.Image.Save(file);
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
            ActiveZone.SelectAll();
        }

        private void DeselectAllShortcut_Click(object sender, EventArgs e)
        {
            ActiveZone.DeselectAll();
        }

        private void DeleteShortcut_Click(object sender, EventArgs e)
        {
            if (ActiveSide == ImportSide)
                ImportContextDiscard_Click(this, EventArgs.Empty);
            else if (ActiveSide == WorldSide)
                ExistingContextDelete_Click(this, EventArgs.Empty);
        }
    }
}
