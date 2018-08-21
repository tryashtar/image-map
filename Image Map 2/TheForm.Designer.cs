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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TheForm));
            this.SelectWorldButton = new System.Windows.Forms.Button();
            this.ExistingZone = new System.Windows.Forms.FlowLayoutPanel();
            this.MapView = new System.Windows.Forms.TabControl();
            this.ImportTab = new System.Windows.Forms.TabPage();
            this.ImportControls = new System.Windows.Forms.Panel();
            this.AddChestCheck = new System.Windows.Forms.CheckBox();
            this.SendButton = new System.Windows.Forms.Button();
            this.OpenButton = new System.Windows.Forms.Button();
            this.ImportZone = new System.Windows.Forms.FlowLayoutPanel();
            this.ExistingTab = new System.Windows.Forms.TabPage();
            this.SelectWorldLabel = new System.Windows.Forms.Label();
            this.WorldNameLabel = new System.Windows.Forms.Label();
            this.ExistingControls = new System.Windows.Forms.Panel();
            this.ExportImageButton = new System.Windows.Forms.Button();
            this.AddInventoryButton = new System.Windows.Forms.Button();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.MapView.SuspendLayout();
            this.ImportTab.SuspendLayout();
            this.ImportControls.SuspendLayout();
            this.ExistingTab.SuspendLayout();
            this.ExistingControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // SelectWorldButton
            // 
            this.SelectWorldButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.SelectWorldButton.Location = new System.Drawing.Point(11, 10);
            this.SelectWorldButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SelectWorldButton.Name = "SelectWorldButton";
            this.SelectWorldButton.Size = new System.Drawing.Size(158, 92);
            this.SelectWorldButton.TabIndex = 0;
            this.SelectWorldButton.Text = "Select World";
            this.SelectWorldButton.UseVisualStyleBackColor = true;
            this.SelectWorldButton.Click += new System.EventHandler(this.SelectWorldButton_Click);
            // 
            // ExistingZone
            // 
            this.ExistingZone.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ExistingZone.AutoScroll = true;
            this.ExistingZone.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ExistingZone.Location = new System.Drawing.Point(6, 5);
            this.ExistingZone.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ExistingZone.Name = "ExistingZone";
            this.ExistingZone.Size = new System.Drawing.Size(778, 333);
            this.ExistingZone.TabIndex = 2;
            // 
            // MapView
            // 
            this.MapView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MapView.Controls.Add(this.ImportTab);
            this.MapView.Controls.Add(this.ExistingTab);
            this.MapView.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.MapView.Location = new System.Drawing.Point(175, 12);
            this.MapView.Name = "MapView";
            this.MapView.SelectedIndex = 0;
            this.MapView.Size = new System.Drawing.Size(800, 452);
            this.MapView.TabIndex = 16;
            this.MapView.Visible = false;
            // 
            // ImportTab
            // 
            this.ImportTab.Controls.Add(this.ImportControls);
            this.ImportTab.Controls.Add(this.ImportZone);
            this.ImportTab.Location = new System.Drawing.Point(4, 38);
            this.ImportTab.Name = "ImportTab";
            this.ImportTab.Padding = new System.Windows.Forms.Padding(3);
            this.ImportTab.Size = new System.Drawing.Size(792, 410);
            this.ImportTab.TabIndex = 1;
            this.ImportTab.Text = "Import Maps";
            this.ImportTab.UseVisualStyleBackColor = true;
            // 
            // ImportControls
            // 
            this.ImportControls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ImportControls.Controls.Add(this.AddChestCheck);
            this.ImportControls.Controls.Add(this.SendButton);
            this.ImportControls.Controls.Add(this.OpenButton);
            this.ImportControls.Location = new System.Drawing.Point(7, 343);
            this.ImportControls.Name = "ImportControls";
            this.ImportControls.Size = new System.Drawing.Size(777, 61);
            this.ImportControls.TabIndex = 4;
            // 
            // AddChestCheck
            // 
            this.AddChestCheck.AutoSize = true;
            this.AddChestCheck.Checked = true;
            this.AddChestCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AddChestCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.AddChestCheck.Location = new System.Drawing.Point(356, 17);
            this.AddChestCheck.Name = "AddChestCheck";
            this.AddChestCheck.Size = new System.Drawing.Size(269, 29);
            this.AddChestCheck.TabIndex = 19;
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
            this.ImportZone.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ImportZone.AutoScroll = true;
            this.ImportZone.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ImportZone.Location = new System.Drawing.Point(7, 5);
            this.ImportZone.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ImportZone.Name = "ImportZone";
            this.ImportZone.Size = new System.Drawing.Size(778, 333);
            this.ImportZone.TabIndex = 3;
            // 
            // ExistingTab
            // 
            this.ExistingTab.Controls.Add(this.ExistingControls);
            this.ExistingTab.Controls.Add(this.ExistingZone);
            this.ExistingTab.Location = new System.Drawing.Point(4, 38);
            this.ExistingTab.Name = "ExistingTab";
            this.ExistingTab.Padding = new System.Windows.Forms.Padding(3);
            this.ExistingTab.Size = new System.Drawing.Size(792, 410);
            this.ExistingTab.TabIndex = 0;
            this.ExistingTab.Text = "Existing Maps";
            this.ExistingTab.UseVisualStyleBackColor = true;
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
            // WorldNameLabel
            // 
            this.WorldNameLabel.AutoSize = true;
            this.WorldNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.WorldNameLabel.Location = new System.Drawing.Point(12, 104);
            this.WorldNameLabel.Name = "WorldNameLabel";
            this.WorldNameLabel.Size = new System.Drawing.Size(0, 17);
            this.WorldNameLabel.TabIndex = 20;
            // 
            // ExistingControls
            // 
            this.ExistingControls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ExistingControls.Controls.Add(this.DeleteButton);
            this.ExistingControls.Controls.Add(this.AddInventoryButton);
            this.ExistingControls.Controls.Add(this.ExportImageButton);
            this.ExistingControls.Location = new System.Drawing.Point(7, 343);
            this.ExistingControls.Name = "ExistingControls";
            this.ExistingControls.Size = new System.Drawing.Size(777, 61);
            this.ExistingControls.TabIndex = 5;
            // 
            // ExportImageButton
            // 
            this.ExportImageButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.ExportImageButton.Location = new System.Drawing.Point(3, 3);
            this.ExportImageButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ExportImageButton.Name = "ExportImageButton";
            this.ExportImageButton.Size = new System.Drawing.Size(190, 53);
            this.ExportImageButton.TabIndex = 17;
            this.ExportImageButton.Text = "Export Image";
            this.ExportImageButton.UseVisualStyleBackColor = true;
            this.ExportImageButton.Click += new System.EventHandler(this.ExportImageButton_Click);
            // 
            // AddInventoryButton
            // 
            this.AddInventoryButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.AddInventoryButton.Location = new System.Drawing.Point(199, 3);
            this.AddInventoryButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.AddInventoryButton.Name = "AddInventoryButton";
            this.AddInventoryButton.Size = new System.Drawing.Size(208, 53);
            this.AddInventoryButton.TabIndex = 18;
            this.AddInventoryButton.Text = "Add to Inventory";
            this.AddInventoryButton.UseVisualStyleBackColor = true;
            this.AddInventoryButton.Click += new System.EventHandler(this.AddInventoryButton_Click);
            // 
            // DeleteButton
            // 
            this.DeleteButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.DeleteButton.Location = new System.Drawing.Point(645, 3);
            this.DeleteButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(119, 53);
            this.DeleteButton.TabIndex = 19;
            this.DeleteButton.Text = "Delete";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // TheForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(975, 464);
            this.Controls.Add(this.WorldNameLabel);
            this.Controls.Add(this.MapView);
            this.Controls.Add(this.SelectWorldButton);
            this.Controls.Add(this.SelectWorldLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(400, 250);
            this.Name = "TheForm";
            this.Text = "Image Map";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TheForm_FormClosed);
            this.Load += new System.EventHandler(this.TheForm_Load);
            this.MapView.ResumeLayout(false);
            this.ImportTab.ResumeLayout(false);
            this.ImportControls.ResumeLayout(false);
            this.ImportControls.PerformLayout();
            this.ExistingTab.ResumeLayout(false);
            this.ExistingControls.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SelectWorldButton;
        private System.Windows.Forms.FlowLayoutPanel ExistingZone;
        private System.Windows.Forms.TabControl MapView;
        private System.Windows.Forms.TabPage ExistingTab;
        private System.Windows.Forms.TabPage ImportTab;
        private System.Windows.Forms.FlowLayoutPanel ImportZone;
        private System.Windows.Forms.Panel ImportControls;
        private System.Windows.Forms.Button SendButton;
        private System.Windows.Forms.Button OpenButton;
        private System.Windows.Forms.Label SelectWorldLabel;
        private System.Windows.Forms.CheckBox AddChestCheck;
        private System.Windows.Forms.Label WorldNameLabel;
        private System.Windows.Forms.Panel ExistingControls;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.Button AddInventoryButton;
        private System.Windows.Forms.Button ExportImageButton;
    }
}

