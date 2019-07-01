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

        // awaiting to receive a preview box
        public MapIDControl(long id)
        {
            ID = id;
            Selected = false;
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.None;
            IDLabel.Text = $"map_{id}";
        }

        public MapIDControl(long id, MapPreviewBox box) : this(id)
        {
            SetBox(box);
        }

        public void SetBox(MapPreviewBox box)
        {
            box.MouseDown += Box_MouseDown;
            Controls.Add(box);
            Map = box.Map;
        }

        public void ToggleSelected()
        {
            SetSelected(!Selected);
        }

        public void SetSelected(bool selected)
        {
            Selected = selected;
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
