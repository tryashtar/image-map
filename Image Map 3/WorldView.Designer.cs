namespace ImageMap
{
    partial class WorldView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.MapTabs = new System.Windows.Forms.TabControl();
            this.ImportTab = new System.Windows.Forms.TabPage();
            this.ImportZone = new MapPreviewPanel();
            this.ClickOpenLabel = new System.Windows.Forms.Label();
            this.ImportControls = new System.Windows.Forms.Panel();
            this.AddChestCheck = new System.Windows.Forms.CheckBox();
            this.SendButton = new System.Windows.Forms.Button();
            this.OpenButton = new System.Windows.Forms.Button();
            this.ExistingTab = new System.Windows.Forms.TabPage();
            this.ExistingZone = new MapPreviewPanel();
            this.ExistingControls = new System.Windows.Forms.Panel();
            this.ExistingContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ExistingContextAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.ExistingContextChangeID = new System.Windows.Forms.ToolStripMenuItem();
            this.ExistingContextExport = new System.Windows.Forms.ToolStripMenuItem();
            this.ExistingContextDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.ExistingContextSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.ImportContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ImportContextSend = new System.Windows.Forms.ToolStripMenuItem();
            this.ImportContextChangeID = new System.Windows.Forms.ToolStripMenuItem();
            this.ImportContextDiscard = new System.Windows.Forms.ToolStripMenuItem();
            this.ImportContextSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.Shortcuts = new System.Windows.Forms.MenuStrip();
            this.PasteShortcut = new System.Windows.Forms.ToolStripMenuItem();
            this.SelectAllShortcut = new System.Windows.Forms.ToolStripMenuItem();
            this.DeselectAllShortcut = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteShortcut = new System.Windows.Forms.ToolStripMenuItem();
            this.MapTabs.SuspendLayout();
            this.ImportTab.SuspendLayout();
            this.ImportZone.SuspendLayout();
            this.ImportControls.SuspendLayout();
            this.ExistingTab.SuspendLayout();
            this.ExistingContextMenu.SuspendLayout();
            this.ImportContextMenu.SuspendLayout();
            this.Shortcuts.SuspendLayout();
            this.SuspendLayout();
            // 
            // MapTabs
            // 
            this.MapTabs.Controls.Add(this.ImportTab);
            this.MapTabs.Controls.Add(this.ExistingTab);
            this.MapTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MapTabs.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.MapTabs.Location = new System.Drawing.Point(0, 0);
            this.MapTabs.Margin = new System.Windows.Forms.Padding(2);
            this.MapTabs.Name = "MapTabs";
            this.MapTabs.SelectedIndex = 0;
            this.MapTabs.Size = new System.Drawing.Size(602, 356);
            this.MapTabs.TabIndex = 17;
            // 
            // ImportTab
            // 
            this.ImportTab.Controls.Add(this.ImportZone);
            this.ImportTab.Controls.Add(this.ImportControls);
            this.ImportTab.Location = new System.Drawing.Point(4, 33);
            this.ImportTab.Margin = new System.Windows.Forms.Padding(2);
            this.ImportTab.Name = "ImportTab";
            this.ImportTab.Padding = new System.Windows.Forms.Padding(2);
            this.ImportTab.Size = new System.Drawing.Size(594, 319);
            this.ImportTab.TabIndex = 1;
            this.ImportTab.Text = "Import Maps";
            this.ImportTab.UseVisualStyleBackColor = true;
            // 
            // ImportZone
            // 
            this.ImportZone.AllowDrop = true;
            this.ImportZone.AutoScroll = true;
            this.ImportZone.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ImportZone.Controls.Add(this.ClickOpenLabel);
            this.ImportZone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ImportZone.Location = new System.Drawing.Point(2, 2);
            this.ImportZone.Margin = new System.Windows.Forms.Padding(2);
            this.ImportZone.Name = "ImportZone";
            this.ImportZone.Size = new System.Drawing.Size(590, 265);
            this.ImportZone.TabIndex = 3;
            this.ImportZone.DragDrop += new System.Windows.Forms.DragEventHandler(this.ImportZone_DragDrop);
            this.ImportZone.DragEnter += new System.Windows.Forms.DragEventHandler(this.ImportZone_DragEnter);
            // 
            // ClickOpenLabel
            // 
            this.ClickOpenLabel.AutoSize = true;
            this.ClickOpenLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.ClickOpenLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.ClickOpenLabel.Location = new System.Drawing.Point(2, 0);
            this.ClickOpenLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ClickOpenLabel.Name = "ClickOpenLabel";
            this.ClickOpenLabel.Size = new System.Drawing.Size(505, 155);
            this.ClickOpenLabel.TabIndex = 22;
            this.ClickOpenLabel.Text = "Click \"Open\" to import some images and convert them to maps!\r\n↓\r\n\r\nOr, just drag " +
    "image files right here!";
            // 
            // ImportControls
            // 
            this.ImportControls.Controls.Add(this.AddChestCheck);
            this.ImportControls.Controls.Add(this.SendButton);
            this.ImportControls.Controls.Add(this.OpenButton);
            this.ImportControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ImportControls.Location = new System.Drawing.Point(2, 267);
            this.ImportControls.Margin = new System.Windows.Forms.Padding(2);
            this.ImportControls.Name = "ImportControls";
            this.ImportControls.Size = new System.Drawing.Size(590, 50);
            this.ImportControls.TabIndex = 4;
            // 
            // AddChestCheck
            // 
            this.AddChestCheck.AutoSize = true;
            this.AddChestCheck.Checked = true;
            this.AddChestCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AddChestCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.AddChestCheck.Location = new System.Drawing.Point(295, 14);
            this.AddChestCheck.Margin = new System.Windows.Forms.Padding(2);
            this.AddChestCheck.Name = "AddChestCheck";
            this.AddChestCheck.Size = new System.Drawing.Size(218, 24);
            this.AddChestCheck.TabIndex = 5;
            this.AddChestCheck.Text = "Add new maps to inventory";
            this.AddChestCheck.UseVisualStyleBackColor = true;
            // 
            // SendButton
            // 
            this.SendButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.SendButton.Location = new System.Drawing.Point(104, 2);
            this.SendButton.Margin = new System.Windows.Forms.Padding(2);
            this.SendButton.Name = "SendButton";
            this.SendButton.Size = new System.Drawing.Size(187, 43);
            this.SendButton.TabIndex = 18;
            this.SendButton.Text = "Send All to World";
            this.SendButton.UseVisualStyleBackColor = true;
            this.SendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // OpenButton
            // 
            this.OpenButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.OpenButton.Location = new System.Drawing.Point(2, 2);
            this.OpenButton.Margin = new System.Windows.Forms.Padding(2);
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(97, 43);
            this.OpenButton.TabIndex = 17;
            this.OpenButton.Text = "Open";
            this.OpenButton.UseVisualStyleBackColor = true;
            this.OpenButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // ExistingTab
            // 
            this.ExistingTab.Controls.Add(this.ExistingZone);
            this.ExistingTab.Controls.Add(this.ExistingControls);
            this.ExistingTab.Location = new System.Drawing.Point(4, 33);
            this.ExistingTab.Margin = new System.Windows.Forms.Padding(2);
            this.ExistingTab.Name = "ExistingTab";
            this.ExistingTab.Padding = new System.Windows.Forms.Padding(2);
            this.ExistingTab.Size = new System.Drawing.Size(594, 319);
            this.ExistingTab.TabIndex = 0;
            this.ExistingTab.Text = "Existing Maps";
            this.ExistingTab.UseVisualStyleBackColor = true;
            // 
            // ExistingZone
            // 
            this.ExistingZone.AutoScroll = true;
            this.ExistingZone.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ExistingZone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ExistingZone.Location = new System.Drawing.Point(2, 2);
            this.ExistingZone.Margin = new System.Windows.Forms.Padding(2);
            this.ExistingZone.Name = "ExistingZone";
            this.ExistingZone.Size = new System.Drawing.Size(590, 265);
            this.ExistingZone.TabIndex = 2;
            // 
            // ExistingControls
            // 
            this.ExistingControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ExistingControls.Location = new System.Drawing.Point(2, 267);
            this.ExistingControls.Margin = new System.Windows.Forms.Padding(2);
            this.ExistingControls.Name = "ExistingControls";
            this.ExistingControls.Size = new System.Drawing.Size(590, 50);
            this.ExistingControls.TabIndex = 5;
            this.ExistingControls.Visible = false;
            // 
            // ExistingContextMenu
            // 
            this.ExistingContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ExistingContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ExistingContextAdd,
            this.ExistingContextChangeID,
            this.ExistingContextExport,
            this.ExistingContextDelete,
            this.ExistingContextSelectAll});
            this.ExistingContextMenu.Name = "ImportContextMenu";
            this.ExistingContextMenu.Size = new System.Drawing.Size(164, 114);
            this.ExistingContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.ExistingContextMenu_Opening);
            // 
            // ExistingContextAdd
            // 
            this.ExistingContextAdd.Name = "ExistingContextAdd";
            this.ExistingContextAdd.Size = new System.Drawing.Size(163, 22);
            this.ExistingContextAdd.Text = "Add to inventory";
            // 
            // ExistingContextChangeID
            // 
            this.ExistingContextChangeID.Name = "ExistingContextChangeID";
            this.ExistingContextChangeID.Size = new System.Drawing.Size(163, 22);
            this.ExistingContextChangeID.Text = "Change ID";
            this.ExistingContextChangeID.Click += new System.EventHandler(this.ExistingContextChangeID_Click);
            // 
            // ExistingContextExport
            // 
            this.ExistingContextExport.Name = "ExistingContextExport";
            this.ExistingContextExport.Size = new System.Drawing.Size(163, 22);
            this.ExistingContextExport.Text = "Export image";
            this.ExistingContextExport.Click += new System.EventHandler(this.ContextExport_Click);
            // 
            // ExistingContextDelete
            // 
            this.ExistingContextDelete.Name = "ExistingContextDelete";
            this.ExistingContextDelete.Size = new System.Drawing.Size(163, 22);
            this.ExistingContextDelete.Text = "Delete";
            this.ExistingContextDelete.Click += new System.EventHandler(this.ExistingContextDelete_Click);
            // 
            // ExistingContextSelectAll
            // 
            this.ExistingContextSelectAll.Name = "ExistingContextSelectAll";
            this.ExistingContextSelectAll.Size = new System.Drawing.Size(163, 22);
            this.ExistingContextSelectAll.Text = "Select all";
            this.ExistingContextSelectAll.Click += new System.EventHandler(this.ContextSelectAll_Click);
            // 
            // ImportContextMenu
            // 
            this.ImportContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ImportContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ImportContextSend,
            this.ImportContextChangeID,
            this.ImportContextDiscard,
            this.ImportContextSelectAll});
            this.ImportContextMenu.Name = "ImportContextMenu";
            this.ImportContextMenu.Size = new System.Drawing.Size(181, 114);
            this.ImportContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.ImportContextMenu_Opening);
            // 
            // ImportContextSend
            // 
            this.ImportContextSend.Name = "ImportContextSend";
            this.ImportContextSend.Size = new System.Drawing.Size(180, 22);
            this.ImportContextSend.Text = "Send to world";
            this.ImportContextSend.Click += new System.EventHandler(this.ImportContextSend_Click);
            // 
            // ImportContextChangeID
            // 
            this.ImportContextChangeID.Name = "ImportContextChangeID";
            this.ImportContextChangeID.Size = new System.Drawing.Size(180, 22);
            this.ImportContextChangeID.Text = "Change ID";
            this.ImportContextChangeID.Click += new System.EventHandler(this.ImportContextChangeID_Click);
            // 
            // ImportContextDiscard
            // 
            this.ImportContextDiscard.Name = "ImportContextDiscard";
            this.ImportContextDiscard.Size = new System.Drawing.Size(180, 22);
            this.ImportContextDiscard.Text = "Discard";
            this.ImportContextDiscard.Click += new System.EventHandler(this.ImportContextDiscard_Click);
            // 
            // ImportContextSelectAll
            // 
            this.ImportContextSelectAll.Name = "ImportContextSelectAll";
            this.ImportContextSelectAll.Size = new System.Drawing.Size(180, 22);
            this.ImportContextSelectAll.Text = "Select all";
            this.ImportContextSelectAll.Click += new System.EventHandler(this.ContextSelectAll_Click);
            // 
            // Shortcuts
            // 
            this.Shortcuts.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PasteShortcut,
            this.SelectAllShortcut,
            this.DeselectAllShortcut,
            this.DeleteShortcut});
            this.Shortcuts.Location = new System.Drawing.Point(0, 0);
            this.Shortcuts.Name = "Shortcuts";
            this.Shortcuts.Size = new System.Drawing.Size(602, 24);
            this.Shortcuts.TabIndex = 18;
            this.Shortcuts.Text = "Shortcuts";
            this.Shortcuts.Visible = false;
            // 
            // PasteShortcut
            // 
            this.PasteShortcut.Name = "PasteShortcut";
            this.PasteShortcut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.PasteShortcut.Size = new System.Drawing.Size(47, 20);
            this.PasteShortcut.Text = "Paste";
            this.PasteShortcut.Click += new System.EventHandler(this.PasteShortcut_Click);
            // 
            // SelectAllShortcut
            // 
            this.SelectAllShortcut.Name = "SelectAllShortcut";
            this.SelectAllShortcut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.SelectAllShortcut.Size = new System.Drawing.Size(67, 20);
            this.SelectAllShortcut.Text = "Select All";
            this.SelectAllShortcut.Click += new System.EventHandler(this.SelectAllShortcut_Click);
            // 
            // DeselectAllShortcut
            // 
            this.DeselectAllShortcut.Name = "DeselectAllShortcut";
            this.DeselectAllShortcut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.DeselectAllShortcut.Size = new System.Drawing.Size(80, 20);
            this.DeselectAllShortcut.Text = "Deselect All";
            this.DeselectAllShortcut.Click += new System.EventHandler(this.DeselectAllShortcut_Click);
            // 
            // DeleteShortcut
            // 
            this.DeleteShortcut.Name = "DeleteShortcut";
            this.DeleteShortcut.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.DeleteShortcut.Size = new System.Drawing.Size(52, 20);
            this.DeleteShortcut.Text = "Delete";
            this.DeleteShortcut.Click += new System.EventHandler(this.DeleteShortcut_Click);
            // 
            // WorldView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Shortcuts);
            this.Controls.Add(this.MapTabs);
            this.Name = "WorldView";
            this.Size = new System.Drawing.Size(602, 356);
            this.MapTabs.ResumeLayout(false);
            this.ImportTab.ResumeLayout(false);
            this.ImportZone.ResumeLayout(false);
            this.ImportZone.PerformLayout();
            this.ImportControls.ResumeLayout(false);
            this.ImportControls.PerformLayout();
            this.ExistingTab.ResumeLayout(false);
            this.ExistingContextMenu.ResumeLayout(false);
            this.ImportContextMenu.ResumeLayout(false);
            this.Shortcuts.ResumeLayout(false);
            this.Shortcuts.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TabControl MapTabs;
        public System.Windows.Forms.TabPage ImportTab;
        public MapPreviewPanel ImportZone;
        public System.Windows.Forms.Label ClickOpenLabel;
        public System.Windows.Forms.Panel ImportControls;
        private System.Windows.Forms.CheckBox AddChestCheck;
        public System.Windows.Forms.Button SendButton;
        public System.Windows.Forms.Button OpenButton;
        public System.Windows.Forms.TabPage ExistingTab;
        public MapPreviewPanel ExistingZone;
        public System.Windows.Forms.Panel ExistingControls;
        public System.Windows.Forms.ContextMenuStrip ExistingContextMenu;
        private System.Windows.Forms.ToolStripMenuItem ExistingContextAdd;
        private System.Windows.Forms.ToolStripMenuItem ExistingContextChangeID;
        private System.Windows.Forms.ToolStripMenuItem ExistingContextExport;
        private System.Windows.Forms.ToolStripMenuItem ExistingContextDelete;
        private System.Windows.Forms.ToolStripMenuItem ExistingContextSelectAll;
        public System.Windows.Forms.ContextMenuStrip ImportContextMenu;
        private System.Windows.Forms.ToolStripMenuItem ImportContextSend;
        private System.Windows.Forms.ToolStripMenuItem ImportContextChangeID;
        private System.Windows.Forms.ToolStripMenuItem ImportContextDiscard;
        private System.Windows.Forms.ToolStripMenuItem ImportContextSelectAll;
        private System.Windows.Forms.MenuStrip Shortcuts;
        private System.Windows.Forms.ToolStripMenuItem PasteShortcut;
        private System.Windows.Forms.ToolStripMenuItem SelectAllShortcut;
        private System.Windows.Forms.ToolStripMenuItem DeselectAllShortcut;
        private System.Windows.Forms.ToolStripMenuItem DeleteShortcut;
    }
}
