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

namespace ImageMap
{
    public partial class WorldView : UserControl
    {
        private MinecraftWorld World;
        private EditionProperties ActiveEdition => EditionProperties.FromEdition(World.Edition);

        public WorldView()
        {
            InitializeComponent();
        }

        public void SetWorld(MinecraftWorld world)
        {
            World = world;
            ExistingZone.Controls.Clear();
            ImportZone.Controls.Clear();
        }

        public void Import(IEnumerable<string> paths)
        {
            var window = ActiveEdition.CreateImportWindow();
            window.InterpolationModeBox.SelectedIndex = Properties.Settings.Default.InterpIndex;
            window.ApplyAllCheck.Checked = Properties.Settings.Default.ApplyAllCheck;
            window.DitherChecked = Properties.Settings.Default.Dither;
            window.StretchChecked = Properties.Settings.Default.Stretch;
            long id = GetSafeID();

            var tasks = new List<Task>();
            window.ImageReady += (s, settings) =>
            {
                var task = ImportFromSettings(id, java ? Edition.Java : Edition.Bedrock, settings);
                tasks.Add(task);
                task.Start();
                id += settings.NumberOfMaps;
            };
            return Tuple.Create(import, tasks);
        }

        public void Import(Image image)
        {

        }

        private long GetSafeID()
        {
            var taken = ImportingMapPreviews.Concat(ExistingMapPreviews).Select(x => x.ID).ToList();
            taken.Add(-1);
            return taken.Max() + 1;
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
    }
}
