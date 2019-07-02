namespace Image_Map
{
    partial class TheForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TheForm));
            this.JavaWorldButton = new System.Windows.Forms.Button();
            this.SelectWorldLabel = new System.Windows.Forms.Label();
            this.BedrockWorldButton = new System.Windows.Forms.Button();
            this.ExistingTab = new System.Windows.Forms.TabPage();
            this.ExistingZone = new System.Windows.Forms.FlowLayoutPanel();
            this.ImportTab = new System.Windows.Forms.TabPage();
            this.ImportControls = new System.Windows.Forms.Panel();
            this.AddChestCheck = new System.Windows.Forms.CheckBox();
            this.SendButton = new System.Windows.Forms.Button();
            this.OpenButton = new System.Windows.Forms.Button();
            this.ImportZone = new System.Windows.Forms.FlowLayoutPanel();
            this.MapView = new System.Windows.Forms.TabControl();
            this.MapViewZone = new System.Windows.Forms.Panel();
            this.ImportContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ImportContextSend = new System.Windows.Forms.ToolStripMenuItem();
            this.ImportContextChangeID = new System.Windows.Forms.ToolStripMenuItem();
            this.ImportContextDiscard = new System.Windows.Forms.ToolStripMenuItem();
            this.ImportContextSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.ExistingContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ExistingContextAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.ExistingContextChangeID = new System.Windows.Forms.ToolStripMenuItem();
            this.ExistingContextExport = new System.Windows.Forms.ToolStripMenuItem();
            this.ExistingContextDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.ExistingContextSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.ExistingControls = new System.Windows.Forms.Panel();
            this.ExistingTab.SuspendLayout();
            this.ImportTab.SuspendLayout();
            this.ImportControls.SuspendLayout();
            this.MapView.SuspendLayout();
            this.MapViewZone.SuspendLayout();
            this.ImportContextMenu.SuspendLayout();
            this.ExistingContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // JavaWorldButton
            // 
            this.JavaWorldButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.JavaWorldButton.Location = new System.Drawing.Point(11, 10);
            this.JavaWorldButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.JavaWorldButton.Name = "JavaWorldButton";
            this.JavaWorldButton.Size = new System.Drawing.Size(158, 92);
            this.JavaWorldButton.TabIndex = 0;
            this.JavaWorldButton.Text = "Java World";
            this.JavaWorldButton.UseVisualStyleBackColor = true;
            this.JavaWorldButton.Click += new System.EventHandler(this.JavaWorldButton_Click);
            // 
            // SelectWorldLabel
            // 
            this.SelectWorldLabel.AutoSize = true;
            this.SelectWorldLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.SelectWorldLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.SelectWorldLabel.Location = new System.Drawing.Point(305, 36);
            this.SelectWorldLabel.Name = "SelectWorldLabel";
            this.SelectWorldLabel.Size = new System.Drawing.Size(458, 156);
            this.SelectWorldLabel.TabIndex = 17;
            this.SelectWorldLabel.Text = "← Click Here!\r\n\r\nMaps will show up in this\r\narea once you select a world.";
            // 
            // BedrockWorldButton
            // 
            this.BedrockWorldButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.BedrockWorldButton.Location = new System.Drawing.Point(11, 114);
            this.BedrockWorldButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.BedrockWorldButton.Name = "BedrockWorldButton";
            this.BedrockWorldButton.Size = new System.Drawing.Size(158, 92);
            this.BedrockWorldButton.TabIndex = 21;
            this.BedrockWorldButton.Text = "Bedrock World";
            this.BedrockWorldButton.UseVisualStyleBackColor = true;
            this.BedrockWorldButton.Click += new System.EventHandler(this.BedrockWorldButton_Click);
            // 
            // ExistingTab
            // 
            this.ExistingTab.Controls.Add(this.ExistingZone);
            this.ExistingTab.Controls.Add(this.ExistingControls);
            this.ExistingTab.Location = new System.Drawing.Point(4, 38);
            this.ExistingTab.Name = "ExistingTab";
            this.ExistingTab.Padding = new System.Windows.Forms.Padding(3);
            this.ExistingTab.Size = new System.Drawing.Size(785, 410);
            this.ExistingTab.TabIndex = 0;
            this.ExistingTab.Text = "Existing Maps";
            this.ExistingTab.UseVisualStyleBackColor = true;
            // 
            // ExistingZone
            // 
            this.ExistingZone.AutoScroll = true;
            this.ExistingZone.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ExistingZone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ExistingZone.Location = new System.Drawing.Point(3, 3);
            this.ExistingZone.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ExistingZone.Name = "ExistingZone";
            this.ExistingZone.Size = new System.Drawing.Size(779, 343);
            this.ExistingZone.TabIndex = 2;
            // 
            // ImportTab
            // 
            this.ImportTab.Controls.Add(this.ImportZone);
            this.ImportTab.Controls.Add(this.ImportControls);
            this.ImportTab.Location = new System.Drawing.Point(4, 38);
            this.ImportTab.Name = "ImportTab";
            this.ImportTab.Padding = new System.Windows.Forms.Padding(3);
            this.ImportTab.Size = new System.Drawing.Size(785, 410);
            this.ImportTab.TabIndex = 1;
            this.ImportTab.Text = "Import Maps";
            this.ImportTab.UseVisualStyleBackColor = true;
            // 
            // ImportControls
            // 
            this.ImportControls.Controls.Add(this.AddChestCheck);
            this.ImportControls.Controls.Add(this.SendButton);
            this.ImportControls.Controls.Add(this.OpenButton);
            this.ImportControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ImportControls.Location = new System.Drawing.Point(3, 346);
            this.ImportControls.Name = "ImportControls";
            this.ImportControls.Size = new System.Drawing.Size(779, 61);
            this.ImportControls.TabIndex = 4;
            // 
            // AddChestCheck
            // 
            this.AddChestCheck.AutoSize = true;
            this.AddChestCheck.Checked = true;
            this.AddChestCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AddChestCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.AddChestCheck.Location = new System.Drawing.Point(365, 17);
            this.AddChestCheck.Name = "AddChestCheck";
            this.AddChestCheck.Size = new System.Drawing.Size(269, 29);
            this.AddChestCheck.TabIndex = 5;
            this.AddChestCheck.Text = "Add new maps to inventory";
            this.AddChestCheck.UseVisualStyleBackColor = true;
            // 
            // SendButton
            // 
            this.SendButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.SendButton.Location = new System.Drawing.Point(138, 3);
            this.SendButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SendButton.Name = "SendButton";
            this.SendButton.Size = new System.Drawing.Size(212, 53);
            this.SendButton.TabIndex = 18;
            this.SendButton.Text = "Send All to World";
            this.SendButton.UseVisualStyleBackColor = true;
            this.SendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // OpenButton
            // 
            this.OpenButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.OpenButton.Location = new System.Drawing.Point(3, 3);
            this.OpenButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(129, 53);
            this.OpenButton.TabIndex = 17;
            this.OpenButton.Text = "Open";
            this.OpenButton.UseVisualStyleBackColor = true;
            this.OpenButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // ImportZone
            // 
            this.ImportZone.AllowDrop = true;
            this.ImportZone.AutoScroll = true;
            this.ImportZone.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ImportZone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ImportZone.Location = new System.Drawing.Point(3, 3);
            this.ImportZone.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ImportZone.Name = "ImportZone";
            this.ImportZone.Size = new System.Drawing.Size(779, 343);
            this.ImportZone.TabIndex = 3;
            this.ImportZone.DragDrop += new System.Windows.Forms.DragEventHandler(this.ImportZone_DragDrop);
            this.ImportZone.DragEnter += new System.Windows.Forms.DragEventHandler(this.ImportZone_DragEnter);
            // 
            // MapView
            // 
            this.MapView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MapView.Controls.Add(this.ImportTab);
            this.MapView.Controls.Add(this.ExistingTab);
            this.MapView.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.MapView.Location = new System.Drawing.Point(0, 0);
            this.MapView.Name = "MapView";
            this.MapView.SelectedIndex = 0;
            this.MapView.Size = new System.Drawing.Size(793, 452);
            this.MapView.TabIndex = 16;
            // 
            // MapViewZone
            // 
            this.MapViewZone.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MapViewZone.Controls.Add(this.MapView);
            this.MapViewZone.Location = new System.Drawing.Point(175, 12);
            this.MapViewZone.Name = "MapViewZone";
            this.MapViewZone.Size = new System.Drawing.Size(793, 452);
            this.MapViewZone.TabIndex = 5;
            this.MapViewZone.Visible = false;
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
            this.ImportContextMenu.Size = new System.Drawing.Size(172, 100);
            this.ImportContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.ImportContextMenu_Opening);
            // 
            // ImportContextSend
            // 
            this.ImportContextSend.Name = "ImportContextSend";
            this.ImportContextSend.Size = new System.Drawing.Size(171, 24);
            this.ImportContextSend.Text = "Send to world";
            this.ImportContextSend.Click += new System.EventHandler(this.ImportContextSend_Click);
            // 
            // ImportContextChangeID
            // 
            this.ImportContextChangeID.Name = "ImportContextChangeID";
            this.ImportContextChangeID.Size = new System.Drawing.Size(171, 24);
            this.ImportContextChangeID.Text = "Change ID";
            this.ImportContextChangeID.Click += new System.EventHandler(this.ImportContextChangeID_Click);
            // 
            // ImportContextDiscard
            // 
            this.ImportContextDiscard.Name = "ImportContextDiscard";
            this.ImportContextDiscard.Size = new System.Drawing.Size(171, 24);
            this.ImportContextDiscard.Text = "Discard";
            this.ImportContextDiscard.Click += new System.EventHandler(this.ImportContextDiscard_Click);
            // 
            // ImportContextSelectAll
            // 
            this.ImportContextSelectAll.Name = "ImportContextSelectAll";
            this.ImportContextSelectAll.Size = new System.Drawing.Size(171, 24);
            this.ImportContextSelectAll.Text = "Select all";
            this.ImportContextSelectAll.Click += new System.EventHandler(this.ImportContextSelectAll_Click);
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
            this.ExistingContextMenu.Size = new System.Drawing.Size(190, 124);
            this.ExistingContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.ExistingContextMenu_Opening);
            // 
            // ExistingContextAdd
            // 
            this.ExistingContextAdd.Name = "ExistingContextAdd";
            this.ExistingContextAdd.Size = new System.Drawing.Size(189, 24);
            this.ExistingContextAdd.Text = "Add to inventory";
            this.ExistingContextAdd.Click += new System.EventHandler(this.ExistingContextAdd_Click);
            // 
            // ExistingContextChangeID
            // 
            this.ExistingContextChangeID.Name = "ExistingContextChangeID";
            this.ExistingContextChangeID.Size = new System.Drawing.Size(189, 24);
            this.ExistingContextChangeID.Text = "Change ID";
            this.ExistingContextChangeID.Click += new System.EventHandler(this.ExistingContextChangeID_Click);
            // 
            // ExistingContextExport
            // 
            this.ExistingContextExport.Name = "ExistingContextExport";
            this.ExistingContextExport.Size = new System.Drawing.Size(189, 24);
            this.ExistingContextExport.Text = "Export image";
            this.ExistingContextExport.Click += new System.EventHandler(this.ExistingContextExport_Click);
            // 
            // ExistingContextDelete
            // 
            this.ExistingContextDelete.Name = "ExistingContextDelete";
            this.ExistingContextDelete.Size = new System.Drawing.Size(189, 24);
            this.ExistingContextDelete.Text = "Delete";
            this.ExistingContextDelete.Click += new System.EventHandler(this.ExistingContextDelete_Click);
            // 
            // ExistingContextSelectAll
            // 
            this.ExistingContextSelectAll.Name = "ExistingContextSelectAll";
            this.ExistingContextSelectAll.Size = new System.Drawing.Size(189, 24);
            this.ExistingContextSelectAll.Text = "Select all";
            this.ExistingContextSelectAll.Click += new System.EventHandler(this.ExistingContextSelectAll_Click);
            // 
            // ExistingControls
            // 
            this.ExistingControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ExistingControls.Location = new System.Drawing.Point(3, 346);
            this.ExistingControls.Name = "ExistingControls";
            this.ExistingControls.Size = new System.Drawing.Size(779, 61);
            this.ExistingControls.TabIndex = 5;
            // 
            // TheForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(968, 464);
            this.Controls.Add(this.BedrockWorldButton);
            this.Controls.Add(this.JavaWorldButton);
            this.Controls.Add(this.MapViewZone);
            this.Controls.Add(this.SelectWorldLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(400, 250);
            this.Name = "TheForm";
            this.Text = "Image Map";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TheForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TheForm_FormClosed);
            this.Load += new System.EventHandler(this.TheForm_Load);
            this.ExistingTab.ResumeLayout(false);
            this.ImportTab.ResumeLayout(false);
            this.ImportControls.ResumeLayout(false);
            this.ImportControls.PerformLayout();
            this.MapView.ResumeLayout(false);
            this.MapViewZone.ResumeLayout(false);
            this.ImportContextMenu.ResumeLayout(false);
            this.ExistingContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button JavaWorldButton;
        public System.Windows.Forms.Label SelectWorldLabel;
        public System.Windows.Forms.Button BedrockWorldButton;
        public System.Windows.Forms.TabPage ExistingTab;
        public System.Windows.Forms.FlowLayoutPanel ExistingZone;
        public System.Windows.Forms.TabPage ImportTab;
        public System.Windows.Forms.Panel ImportControls;
        public System.Windows.Forms.Button SendButton;
        public System.Windows.Forms.Button OpenButton;
        public System.Windows.Forms.FlowLayoutPanel ImportZone;
        public System.Windows.Forms.TabControl MapView;
        private System.Windows.Forms.CheckBox AddChestCheck;
        public System.Windows.Forms.Panel MapViewZone;
        public System.Windows.Forms.ContextMenuStrip ImportContextMenu;
        private System.Windows.Forms.ToolStripMenuItem ImportContextSend;
        private System.Windows.Forms.ToolStripMenuItem ImportContextChangeID;
        private System.Windows.Forms.ToolStripMenuItem ImportContextDiscard;
        private System.Windows.Forms.ToolStripMenuItem ImportContextSelectAll;
        public System.Windows.Forms.ContextMenuStrip ExistingContextMenu;
        private System.Windows.Forms.ToolStripMenuItem ExistingContextAdd;
        private System.Windows.Forms.ToolStripMenuItem ExistingContextChangeID;
        private System.Windows.Forms.ToolStripMenuItem ExistingContextExport;
        private System.Windows.Forms.ToolStripMenuItem ExistingContextDelete;
        private System.Windows.Forms.ToolStripMenuItem ExistingContextSelectAll;
        public System.Windows.Forms.Panel ExistingControls;
    }
}

