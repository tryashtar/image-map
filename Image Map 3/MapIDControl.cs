using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageMap
{
    public partial class MapIDControl : UserControl
    {
        public long ID { get; private set; }
        public Map Map => Box?.Map;
        public bool Selected { get; private set; }
        public bool Conflicted { get; private set; }
        public bool HasBox => Box != null;
        private MapPreviewBox Box;
        public event EventHandler<bool> SelectedChanged;

        // awaiting to receive a preview box
        public MapIDControl(long id)
        {
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.None;
            SetSelected(false);
            SetID(id);
            SetConflict(false);
            SetSize(128, 128);
        }

        public MapIDControl(long id, MapPreviewBox box) : this(id)
        {
            SetBox(box);
        }

        private void SetSize(int width, int height)
        {
            this.Width = width + 6;
            this.Height = height + IDLabel.Height + 6;
        }

        public void SetBox(MapPreviewBox box)
        {
            if (!HasBox)
            {
                Box = box;
                box.MouseDown += Box_MouseDown;
                Controls.Add(box);
                SetSize(box.Width, box.Height);
                box.Left = this.Width / 2 - box.Width / 2;
                box.Top = 3;
            }
        }

        public void SetID(long id)
        {
            ID = id;
            IDLabel.Text = $"map_{id}";
        }

        public void SetConflict(bool conflict)
        {
            Conflicted = conflict;
            UpdateColor();
        }

        public void ToggleSelected()
        {
            SetSelected(!Selected);
        }

        public void SetSelected(bool selected)
        {
            if (selected != Selected)
            {
                Selected = selected;
                SelectedChanged?.Invoke(this, Selected);
                UpdateColor();
            }
        }

        public string GetMapName()
        {
            return "map_" + ID;
        }

        private void UpdateColor()
        {
            if (Selected)
            {
                if (Conflicted)
                    BackColor = Color.FromArgb(255, 173, 213);
                else
                    BackColor = Color.LightBlue;
            }
            else
            {
                if (Conflicted)
                    BackColor = Color.FromArgb(255, 179, 153);
                else
                    BackColor = Color.White;
            }
        }

        private void Box_MouseDown(object sender, MouseEventArgs e)
        {
            OnMouseDown(e);
        }
    }
}
