using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageMap
{
    public partial class ReplaceOptionDialog : Form
    {
        public bool Confirmed { get; private set; }
        public MapReplaceOption SelectedOption { get; private set; }

        public ReplaceOptionDialog(int count)
        {
            InitializeComponent();
            DescriptionLabel.Text = $"{Util.Pluralize(count, "map")} you selected will end up with an ID that's already taken. You can skip these maps, overwrite the old ones, or auto-pick new IDs for the old ones.";
        }

        private void AutoButton_Click(object sender, EventArgs e)
        {
            Confirmed = true;
            SelectedOption = MapReplaceOption.ChangeExisting;
            this.Close();
        }

        private void OverwriteButton_Click(object sender, EventArgs e)
        {
            Confirmed = true;
            SelectedOption = MapReplaceOption.ReplaceExisting;
            this.Close();
        }

        private void SkipButton_Click(object sender, EventArgs e)
        {
            Confirmed = true;
            SelectedOption = MapReplaceOption.Skip;
            this.Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Confirmed = false;
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }

    public enum MapReplaceOption
    {
        ChangeExisting,
        ReplaceExisting,
        Skip
    }
}
