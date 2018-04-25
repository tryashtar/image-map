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
            this.ImportBar = new System.Windows.Forms.ProgressBar();
            this.ImportLabel = new System.Windows.Forms.Label();
            this.BedrockCheck = new System.Windows.Forms.CheckBox();
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
            // ImportBar
            // 
            this.ImportBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ImportBar.Location = new System.Drawing.Point(11, 438);
            this.ImportBar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ImportBar.Name = "ImportBar";
            this.ImportBar.Size = new System.Drawing.Size(158, 15);
            this.ImportBar.TabIndex = 7;
            this.ImportBar.Visible = false;
            // 
            // ImportLabel
            // 
            this.ImportLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ImportLabel.AutoSize = true;
            this.ImportLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.ImportLabel.Location = new System.Drawing.Point(11, 415);
            this.ImportLabel.Name = "ImportLabel";
            this.ImportLabel.Size = new System.Drawing.Size(92, 20);
            this.ImportLabel.TabIndex = 8;
            this.ImportLabel.Text = "Mapifying...";
            this.ImportLabel.Visible = false;
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
            // TheForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(975, 464);
            this.Controls.Add(this.BedrockCheck);
            this.Controls.Add(this.ImportLabel);
            this.Controls.Add(this.ImportBar);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OpenButton;
        private System.Windows.Forms.Button ExportButton;
        private System.Windows.Forms.FlowLayoutPanel PictureZone;
        private System.Windows.Forms.ProgressBar ImportBar;
        private System.Windows.Forms.Label ImportLabel;
        private System.Windows.Forms.CheckBox BedrockCheck;
    }
}

