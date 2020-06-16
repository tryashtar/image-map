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
            this.MapTabs = new System.Windows.Forms.TabControl();
            this.ImportTab = new System.Windows.Forms.TabPage();
            this.ImportZone = new System.Windows.Forms.FlowLayoutPanel();
            this.ClickOpenLabel = new System.Windows.Forms.Label();
            this.ImportControls = new System.Windows.Forms.Panel();
            this.AddChestCheck = new System.Windows.Forms.CheckBox();
            this.SendButton = new System.Windows.Forms.Button();
            this.OpenButton = new System.Windows.Forms.Button();
            this.ExistingTab = new System.Windows.Forms.TabPage();
            this.ExistingZone = new System.Windows.Forms.FlowLayoutPanel();
            this.ExistingControls = new System.Windows.Forms.Panel();
            this.MapTabs.SuspendLayout();
            this.ImportTab.SuspendLayout();
            this.ImportZone.SuspendLayout();
            this.ImportControls.SuspendLayout();
            this.ExistingTab.SuspendLayout();
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
            // 
            // ExistingTab
            // 
            this.ExistingTab.Controls.Add(this.ExistingZone);
            this.ExistingTab.Controls.Add(this.ExistingControls);
            this.ExistingTab.Location = new System.Drawing.Point(4, 33);
            this.ExistingTab.Margin = new System.Windows.Forms.Padding(2);
            this.ExistingTab.Name = "ExistingTab";
            this.ExistingTab.Padding = new System.Windows.Forms.Padding(2);
            this.ExistingTab.Size = new System.Drawing.Size(587, 330);
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
            this.ExistingZone.Size = new System.Drawing.Size(583, 276);
            this.ExistingZone.TabIndex = 2;
            // 
            // ExistingControls
            // 
            this.ExistingControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ExistingControls.Location = new System.Drawing.Point(2, 278);
            this.ExistingControls.Margin = new System.Windows.Forms.Padding(2);
            this.ExistingControls.Name = "ExistingControls";
            this.ExistingControls.Size = new System.Drawing.Size(583, 50);
            this.ExistingControls.TabIndex = 5;
            this.ExistingControls.Visible = false;
            // 
            // WorldView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TabControl MapTabs;
        public System.Windows.Forms.TabPage ImportTab;
        public System.Windows.Forms.FlowLayoutPanel ImportZone;
        public System.Windows.Forms.Label ClickOpenLabel;
        public System.Windows.Forms.Panel ImportControls;
        private System.Windows.Forms.CheckBox AddChestCheck;
        public System.Windows.Forms.Button SendButton;
        public System.Windows.Forms.Button OpenButton;
        public System.Windows.Forms.TabPage ExistingTab;
        public System.Windows.Forms.FlowLayoutPanel ExistingZone;
        public System.Windows.Forms.Panel ExistingControls;
    }
}
