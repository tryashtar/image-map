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
            this.BedrockCheck = new System.Windows.Forms.CheckBox();
            this.FirstIDLabel = new System.Windows.Forms.Label();
            this.MapIDNum = new System.Windows.Forms.NumericUpDown();
            this.LastIDLabel = new System.Windows.Forms.Label();
            this.ToLabel = new System.Windows.Forms.Label();
            this.MapView = new System.Windows.Forms.TabControl();
            this.ExistingTab = new System.Windows.Forms.TabPage();
            this.ImportTab = new System.Windows.Forms.TabPage();
            this.ImportZone = new System.Windows.Forms.FlowLayoutPanel();
            this.ImportControls = new System.Windows.Forms.Panel();
            this.OpenButton = new System.Windows.Forms.Button();
            this.ImportButton = new System.Windows.Forms.Button();
            this.SelectWorldLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.MapIDNum)).BeginInit();
            this.MapView.SuspendLayout();
            this.ExistingTab.SuspendLayout();
            this.ImportTab.SuspendLayout();
            this.ImportControls.SuspendLayout();
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
            this.SelectWorldButton.Click += new System.EventHandler(this.OpenButton_Click);
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
            this.ExistingZone.Size = new System.Drawing.Size(778, 400);
            this.ExistingZone.TabIndex = 2;
            // 
            // BedrockCheck
            // 
            this.BedrockCheck.AutoSize = true;
            this.BedrockCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.BedrockCheck.Location = new System.Drawing.Point(12, 107);
            this.BedrockCheck.Name = "BedrockCheck";
            this.BedrockCheck.Size = new System.Drawing.Size(106, 54);
            this.BedrockCheck.TabIndex = 9;
            this.BedrockCheck.Text = "Bedrock\r\nEdition";
            this.BedrockCheck.UseVisualStyleBackColor = true;
            // 
            // FirstIDLabel
            // 
            this.FirstIDLabel.AutoSize = true;
            this.FirstIDLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.FirstIDLabel.Location = new System.Drawing.Point(356, 18);
            this.FirstIDLabel.Name = "FirstIDLabel";
            this.FirstIDLabel.Size = new System.Drawing.Size(61, 25);
            this.FirstIDLabel.TabIndex = 10;
            this.FirstIDLabel.Text = "map_";
            // 
            // MapIDNum
            // 
            this.MapIDNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.MapIDNum.Location = new System.Drawing.Point(421, 14);
            this.MapIDNum.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.MapIDNum.Minimum = new decimal(new int[] {
            9999,
            0,
            0,
            -2147483648});
            this.MapIDNum.Name = "MapIDNum";
            this.MapIDNum.Size = new System.Drawing.Size(83, 30);
            this.MapIDNum.TabIndex = 11;
            this.MapIDNum.ValueChanged += new System.EventHandler(this.MapIDNum_ValueChanged);
            // 
            // LastIDLabel
            // 
            this.LastIDLabel.AutoSize = true;
            this.LastIDLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.LastIDLabel.Location = new System.Drawing.Point(532, 19);
            this.LastIDLabel.Name = "LastIDLabel";
            this.LastIDLabel.Size = new System.Drawing.Size(72, 25);
            this.LastIDLabel.TabIndex = 12;
            this.LastIDLabel.Text = "map_0";
            // 
            // ToLabel
            // 
            this.ToLabel.AutoSize = true;
            this.ToLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ToLabel.Location = new System.Drawing.Point(510, 21);
            this.ToLabel.Name = "ToLabel";
            this.ToLabel.Size = new System.Drawing.Size(23, 20);
            this.ToLabel.TabIndex = 13;
            this.ToLabel.Text = "to";
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
            // ExistingTab
            // 
            this.ExistingTab.Controls.Add(this.ExistingZone);
            this.ExistingTab.Location = new System.Drawing.Point(4, 38);
            this.ExistingTab.Name = "ExistingTab";
            this.ExistingTab.Padding = new System.Windows.Forms.Padding(3);
            this.ExistingTab.Size = new System.Drawing.Size(792, 410);
            this.ExistingTab.TabIndex = 0;
            this.ExistingTab.Text = "Existing Maps";
            this.ExistingTab.UseVisualStyleBackColor = true;
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
            // ImportControls
            // 
            this.ImportControls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ImportControls.Controls.Add(this.LastIDLabel);
            this.ImportControls.Controls.Add(this.ToLabel);
            this.ImportControls.Controls.Add(this.FirstIDLabel);
            this.ImportControls.Controls.Add(this.MapIDNum);
            this.ImportControls.Controls.Add(this.ImportButton);
            this.ImportControls.Controls.Add(this.OpenButton);
            this.ImportControls.Location = new System.Drawing.Point(7, 343);
            this.ImportControls.Name = "ImportControls";
            this.ImportControls.Size = new System.Drawing.Size(777, 61);
            this.ImportControls.TabIndex = 4;
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
            // 
            // ImportButton
            // 
            this.ImportButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.ImportButton.Location = new System.Drawing.Point(138, 3);
            this.ImportButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ImportButton.Name = "ImportButton";
            this.ImportButton.Size = new System.Drawing.Size(212, 53);
            this.ImportButton.TabIndex = 18;
            this.ImportButton.Text = "Add All to World";
            this.ImportButton.UseVisualStyleBackColor = true;
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
            // TheForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(975, 464);
            this.Controls.Add(this.MapView);
            this.Controls.Add(this.BedrockCheck);
            this.Controls.Add(this.SelectWorldButton);
            this.Controls.Add(this.SelectWorldLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(400, 250);
            this.Name = "TheForm";
            this.Text = "Image Map";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TheForm_FormClosed);
            this.Load += new System.EventHandler(this.TheForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MapIDNum)).EndInit();
            this.MapView.ResumeLayout(false);
            this.ExistingTab.ResumeLayout(false);
            this.ImportTab.ResumeLayout(false);
            this.ImportControls.ResumeLayout(false);
            this.ImportControls.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SelectWorldButton;
        private System.Windows.Forms.FlowLayoutPanel ExistingZone;
        private System.Windows.Forms.CheckBox BedrockCheck;
        private System.Windows.Forms.Label FirstIDLabel;
        private System.Windows.Forms.NumericUpDown MapIDNum;
        private System.Windows.Forms.Label LastIDLabel;
        private System.Windows.Forms.Label ToLabel;
        private System.Windows.Forms.TabControl MapView;
        private System.Windows.Forms.TabPage ExistingTab;
        private System.Windows.Forms.TabPage ImportTab;
        private System.Windows.Forms.FlowLayoutPanel ImportZone;
        private System.Windows.Forms.Panel ImportControls;
        private System.Windows.Forms.Button ImportButton;
        private System.Windows.Forms.Button OpenButton;
        private System.Windows.Forms.Label SelectWorldLabel;
    }
}

