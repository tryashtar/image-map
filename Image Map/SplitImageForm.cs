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
    public partial class SplitImageForm : Form
    {
        public SplitImageForm()
        {
            InitializeComponent();
        }

        private void SplitImageForm_Load(object sender, EventArgs e)
        {

        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
