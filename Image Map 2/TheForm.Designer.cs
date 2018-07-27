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
            this.OpenButton = new System.Windows.Forms.Button();
            this.ExportButton = new System.Windows.Forms.Button();
            this.PictureZone = new System.Windows.Forms.FlowLayoutPanel();
            this.BedrockCheck = new System.Windows.Forms.CheckBox();
            this.FirstIDLabel = new System.Windows.Forms.Label();
            this.MapIDNum = new System.Windows.Forms.NumericUpDown();
            this.LastIDLabel = new System.Windows.Forms.Label();
            this.ToLabel = new System.Windows.Forms.Label();
            this.AutoIDCheck = new System.Windows.Forms.CheckBox();
            this.ManualIDPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.MapIDNum)).BeginInit();
            this.ManualIDPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // OpenButton
            // 
            this.OpenButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.OpenButton.Location = new System.Drawing.Point(11, 10);
            this.OpenButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(158, 58);
            this.OpenButton.TabIndex = 0;
            this.OpenButton.Text = "Open";
            this.OpenButton.UseVisualStyleBackColor = true;
            this.OpenButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // ExportButton
            // 
            this.ExportButton.Enabled = false;
            this.ExportButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.ExportButton.Location = new System.Drawing.Point(11, 73);
            this.ExportButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(158, 64);
            this.ExportButton.TabIndex = 1;
            this.ExportButton.Text = "Export";
            this.ExportButton.UseVisualStyleBackColor = true;
            this.ExportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // PictureZone
            // 
            this.PictureZone.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PictureZone.AutoScroll = true;
            this.PictureZone.BackColor = System.Drawing.SystemColors.ControlLight;
            this.PictureZone.Location = new System.Drawing.Point(175, 10);
            this.PictureZone.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.PictureZone.Name = "PictureZone";
            this.PictureZone.Size = new System.Drawing.Size(788, 444);
            this.PictureZone.TabIndex = 2;
            // 
            // BedrockCheck
            // 
            this.BedrockCheck.AutoSize = true;
            this.BedrockCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.BedrockCheck.Location = new System.Drawing.Point(12, 142);
            this.BedrockCheck.Name = "BedrockCheck";
            this.BedrockCheck.Size = new System.Drawing.Size(106, 54);
            this.BedrockCheck.TabIndex = 9;
            this.BedrockCheck.Text = "Bedrock\r\nEdition";
            this.BedrockCheck.UseVisualStyleBackColor = true;
            this.BedrockCheck.CheckedChanged += new System.EventHandler(this.BedrockCheck_CheckedChanged);
            // 
            // FirstIDLabel
            // 
            this.FirstIDLabel.AutoSize = true;
            this.FirstIDLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.FirstIDLabel.Location = new System.Drawing.Point(3, 2);
            this.FirstIDLabel.Name = "FirstIDLabel";
            this.FirstIDLabel.Size = new System.Drawing.Size(61, 25);
            this.FirstIDLabel.TabIndex = 10;
            this.FirstIDLabel.Text = "map_";
            // 
            // MapIDNum
            // 
            this.MapIDNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.MapIDNum.Location = new System.Drawing.Point(70, 3);
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
            this.LastIDLabel.Location = new System.Drawing.Point(3, 62);
            this.LastIDLabel.Name = "LastIDLabel";
            this.LastIDLabel.Size = new System.Drawing.Size(61, 25);
            this.LastIDLabel.TabIndex = 12;
            this.LastIDLabel.Text = "map_";
            // 
            // ToLabel
            // 
            this.ToLabel.AutoSize = true;
            this.ToLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ToLabel.Location = new System.Drawing.Point(14, 35);
            this.ToLabel.Name = "ToLabel";
            this.ToLabel.Size = new System.Drawing.Size(23, 20);
            this.ToLabel.TabIndex = 13;
            this.ToLabel.Text = "to";
            // 
            // AutoIDCheck
            // 
            this.AutoIDCheck.AutoSize = true;
            this.AutoIDCheck.Checked = true;
            this.AutoIDCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AutoIDCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.AutoIDCheck.Location = new System.Drawing.Point(11, 202);
            this.AutoIDCheck.Name = "AutoIDCheck";
            this.AutoIDCheck.Size = new System.Drawing.Size(123, 54);
            this.AutoIDCheck.TabIndex = 14;
            this.AutoIDCheck.Text = "Save to\r\nFree Slots";
            this.AutoIDCheck.UseVisualStyleBackColor = true;
            this.AutoIDCheck.CheckedChanged += new System.EventHandler(this.AutoIDCheck_CheckedChanged);
            // 
            // ManualIDPanel
            // 
            this.ManualIDPanel.Controls.Add(this.FirstIDLabel);
            this.ManualIDPanel.Controls.Add(this.MapIDNum);
            this.ManualIDPanel.Controls.Add(this.ToLabel);
            this.ManualIDPanel.Controls.Add(this.LastIDLabel);
            this.ManualIDPanel.Location = new System.Drawing.Point(1, 262);
            this.ManualIDPanel.Name = "ManualIDPanel";
            this.ManualIDPanel.Size = new System.Drawing.Size(168, 100);
            this.ManualIDPanel.TabIndex = 15;
            this.ManualIDPanel.Visible = false;
            // 
            // TheForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(975, 464);
            this.Controls.Add(this.ManualIDPanel);
            this.Controls.Add(this.AutoIDCheck);
            this.Controls.Add(this.BedrockCheck);
            this.Controls.Add(this.PictureZone);
            this.Controls.Add(this.ExportButton);
            this.Controls.Add(this.OpenButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(400, 250);
            this.Name = "TheForm";
            this.Text = "Image Map";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TheForm_FormClosed);
            this.Load += new System.EventHandler(this.TheForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MapIDNum)).EndInit();
            this.ManualIDPanel.ResumeLayout(false);
            this.ManualIDPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OpenButton;
        private System.Windows.Forms.Button ExportButton;
        private System.Windows.Forms.FlowLayoutPanel PictureZone;
        private System.Windows.Forms.CheckBox BedrockCheck;
        private System.Windows.Forms.Label FirstIDLabel;
        private System.Windows.Forms.NumericUpDown MapIDNum;
        private System.Windows.Forms.Label LastIDLabel;
        private System.Windows.Forms.Label ToLabel;
        private System.Windows.Forms.CheckBox AutoIDCheck;
        private System.Windows.Forms.Panel ManualIDPanel;
    }
}

