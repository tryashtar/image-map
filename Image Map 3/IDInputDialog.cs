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
    public partial class IDInputDialog : Form
    {
        public bool Confirmed { get; private set; } = false;
        public bool WantsAuto { get; private set; } = false;
        public long SelectedID { get; private set; } = 0;
        public IDInputDialog(long current)
        {
            InitializeComponent();
            IDInput.Value = current;
        }

        private void IDInputDialog_Load(object sender, EventArgs e)
        {
            IDInput.Select();
            IDInput.Select(0, IDInput.Text.Length);
        }

        // enter to confirm, escape to cancel
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                ConfirmButton_Click(this, EventArgs.Empty);
                return true;
            }
            else if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            Confirmed = true;
            SelectedID = (long)IDInput.Value;
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AutoButton_Click(object sender, EventArgs e)
        {
            WantsAuto = true;
            Confirmed = true;
            this.Close();
        }
    }
}
