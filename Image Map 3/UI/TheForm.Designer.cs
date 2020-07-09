namespace ImageMap
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
            this.JavaWorldButton = new System.Windows.Forms.Button();
            this.SelectWorldLabel = new System.Windows.Forms.Label();
            this.BedrockWorldButton = new System.Windows.Forms.Button();
            this.MapViewZone = new System.Windows.Forms.Panel();
            this.WorldView = new ImageMap.WorldView();
            this.MapViewZone.SuspendLayout();
            this.SuspendLayout();
            // 
            // JavaWorldButton
            // 
            this.JavaWorldButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.JavaWorldButton.Location = new System.Drawing.Point(8, 8);
            this.JavaWorldButton.Margin = new System.Windows.Forms.Padding(2);
            this.JavaWorldButton.Name = "JavaWorldButton";
            this.JavaWorldButton.Size = new System.Drawing.Size(118, 75);
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
            this.SelectWorldLabel.Location = new System.Drawing.Point(229, 29);
            this.SelectWorldLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.SelectWorldLabel.Name = "SelectWorldLabel";
            this.SelectWorldLabel.Size = new System.Drawing.Size(461, 186);
            this.SelectWorldLabel.TabIndex = 17;
            this.SelectWorldLabel.Text = "← Click Here!\r\n\r\nMaps will show up in this\r\narea once you select a world.\r\n\r\nOr, " +
    "just drag a world folder right here!";
            // 
            // BedrockWorldButton
            // 
            this.BedrockWorldButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.BedrockWorldButton.Location = new System.Drawing.Point(8, 93);
            this.BedrockWorldButton.Margin = new System.Windows.Forms.Padding(2);
            this.BedrockWorldButton.Name = "BedrockWorldButton";
            this.BedrockWorldButton.Size = new System.Drawing.Size(118, 75);
            this.BedrockWorldButton.TabIndex = 21;
            this.BedrockWorldButton.Text = "Bedrock World";
            this.BedrockWorldButton.UseVisualStyleBackColor = true;
            this.BedrockWorldButton.Click += new System.EventHandler(this.BedrockWorldButton_Click);
            // 
            // MapViewZone
            // 
            this.MapViewZone.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MapViewZone.Controls.Add(this.WorldView);
            this.MapViewZone.Location = new System.Drawing.Point(131, 10);
            this.MapViewZone.Margin = new System.Windows.Forms.Padding(2);
            this.MapViewZone.Name = "MapViewZone";
            this.MapViewZone.Size = new System.Drawing.Size(604, 394);
            this.MapViewZone.TabIndex = 5;
            this.MapViewZone.Visible = false;
            // 
            // WorldView
            // 
            this.WorldView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WorldView.Location = new System.Drawing.Point(0, 0);
            this.WorldView.Name = "WorldView";
            this.WorldView.Size = new System.Drawing.Size(604, 394);
            this.WorldView.TabIndex = 22;
            // 
            // TheForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(735, 404);
            this.Controls.Add(this.BedrockWorldButton);
            this.Controls.Add(this.JavaWorldButton);
            this.Controls.Add(this.MapViewZone);
            this.Controls.Add(this.SelectWorldLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(304, 210);
            this.Name = "TheForm";
            this.Text = "Image Map";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TheForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TheForm_FormClosed);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.TheForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.TheForm_DragEnter);
            this.MapViewZone.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button JavaWorldButton;
        public System.Windows.Forms.Label SelectWorldLabel;
        public System.Windows.Forms.Button BedrockWorldButton;
        public System.Windows.Forms.Panel MapViewZone;
        private WorldView WorldView;
    }
}

