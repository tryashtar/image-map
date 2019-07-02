using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Image_Map
{
    public partial class ReplaceOptionDialog : Form
    {
        public MapReplaceOption SelectedOption { get; private set; }

        public ReplaceOptionDialog(int count)
        {
            InitializeComponent();
            DescriptionLabel.Text = $"{count} map(s) you selected will end up with an ID that's already taken. You can skip these maps, overwrite the old ones, or auto-pick new IDs for the old ones.";
        }

        private void AutoButton_Click(object sender, EventArgs e)
        {
            SelectedOption = MapReplaceOption.ChangeExisting;
            this.Close();
        }

        private void OverwriteButton_Click(object sender, EventArgs e)
        {
            SelectedOption = MapReplaceOption.ReplaceExisting;
            this.Close();
        }

        private void SkipButton_Click(object sender, EventArgs e)
        {
            SelectedOption = MapReplaceOption.Skip;
            this.Close();
        }
    }
}
