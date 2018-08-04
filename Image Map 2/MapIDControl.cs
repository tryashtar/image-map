using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Image_Map
{
    public partial class MapIDControl : UserControl
    {
        public long ID { get; private set; }
        public Map Map { get; private set; }
        public MapIDControl(long id, Map map)
        {
            ID = id;
            Map = map;
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.None;
            MapPreviewBox box = new MapPreviewBox(map);
            box.MouseDown += Box_MouseDown;
            Controls.Add(box);
            IDLabel.Text = $"map_{id}";
        }

        private void Box_MouseDown(object sender, MouseEventArgs e)
        {
            OnMouseDown(e);
        }
    }
}
