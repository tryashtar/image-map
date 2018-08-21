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
        public bool Selected { get; private set; }
        public event EventHandler<bool> SelectedChanged;
        public MapIDControl(long id, Map map)
        {
            ID = id;
            Map = map;
            Selected = false;
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.None;
            MapPreviewBox box = new MapPreviewBox(map);
            box.MouseDown += Box_MouseDown;
            Controls.Add(box);
            IDLabel.Text = $"map_{id}";
        }

        public void ToggleSelected()
        {
            Selected = !Selected;
            BackColor = Selected ? Color.LightGreen : Color.White;
            SelectedChanged?.Invoke(this, Selected);
        }

        public string GetMapName()
        {
            return "map_" + ID;
        }

        private void Box_MouseDown(object sender, MouseEventArgs e)
        {
            OnMouseDown(e);
            ToggleSelected();
        }
    }
}
