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
        public bool IsSelected { get; private set; }
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

        public MapIDControl(long id, Map map) : this(id)
        {
            SetMap(map);
        }

        private void SetSize(int width, int height)
        {
            this.Width = width + 6;
            this.Height = height + IDLabel.Height + 6;
        }

        public void SetMap(Map map)
        {
            if (Box != null)
            {
                if (Box.Map == map)
                    return;
                Box.MouseDown -= Box_MouseDown;
                Controls.Remove(Box);
            }
            if (map != null)
            {
                Box = new MapPreviewBox(map);
                Box.MouseDown += Box_MouseDown;
                Controls.Add(Box);
                SetSize(Box.Width, Box.Height);
                Box.Left = this.Width / 2 - Box.Width / 2;
                Box.Top = 3;
            }
        }

        public void SetID(long id)
        {
            ID = id;
            IDLabel.Text = Util.MapName(id);
        }

        public void SetConflict(bool conflict)
        {
            Conflicted = conflict;
            UpdateColor();
        }

        public void ToggleSelected()
        {
            SetSelected(!IsSelected);
        }

        public void SetSelected(bool selected)
        {
            if (selected != IsSelected)
            {
                IsSelected = selected;
                SelectedChanged?.Invoke(this, IsSelected);
                UpdateColor();
            }
        }

        private void UpdateColor()
        {
            if (IsSelected)
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
