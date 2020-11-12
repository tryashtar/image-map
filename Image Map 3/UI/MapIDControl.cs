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
        private ProgressBar Progress;
        public event EventHandler<bool> SelectedChanged;

        // awaiting to receive a preview box
        public MapIDControl(long id, Map map)
        {
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.None;
            SetSelected(false);
            SetID(id);
            SetConflict(false);
            SetSize(128, 128);
            Progress = new ProgressBar();
            Progress.Minimum = 0;
            Progress.Maximum = 100;
            Controls.Add(Progress);
            Progress.Left = this.Left + 5;
            Progress.Width = this.Width - 10;
            Progress.Height = 15;
            Progress.Top = this.Bottom - Progress.Height - IDLabel.Height - 5;
            Progress.Visible = true;
            SetMap(map);
        }

        private void SetSize(int width, int height)
        {
            this.Width = width + 6;
            this.Height = height + IDLabel.Height + 6;
        }

        public void SetMap(Map map)
        {
            if (Map == map)
                return;
            // remove old box
            if (Box != null)
            {
                Box.MouseDown -= Box_MouseDown;
                Controls.Remove(Box);
                Box = null;
            }
            if (map == null)
                Progress.Visible = true;
            else
            {
                Box = new MapPreviewBox(map);
                Box.MouseDown += Box_MouseDown;
                Controls.Add(Box);
                SetSize(Box.Width, Box.Height);
                Box.Left = this.Width / 2 - Box.Width / 2;
                Box.Top = 3;
                Progress.Visible = false;
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

        public void SetProgress(decimal percent)
        {
            Progress.Value = (int)(percent * 100);
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
