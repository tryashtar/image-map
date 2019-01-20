namespace Image_Map
{
    partial class BedrockWorldControl
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
            this.WorldIcon = new System.Windows.Forms.PictureBox();
            this.WorldName = new System.Windows.Forms.Label();
            this.FolderName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.WorldIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // WorldIcon
            // 
            this.WorldIcon.Location = new System.Drawing.Point(14, 17);
            this.WorldIcon.Name = "WorldIcon";
            this.WorldIcon.Size = new System.Drawing.Size(121, 99);
            this.WorldIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.WorldIcon.TabIndex = 0;
            this.WorldIcon.TabStop = false;
            this.WorldIcon.Click += new System.EventHandler(this.Control_Click);
            this.WorldIcon.DoubleClick += new System.EventHandler(this.Control_DoubleClick);
            // 
            // WorldName
            // 
            this.WorldName.AutoSize = true;
            this.WorldName.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.WorldName.Location = new System.Drawing.Point(141, 17);
            this.WorldName.Name = "WorldName";
            this.WorldName.Size = new System.Drawing.Size(0, 36);
            this.WorldName.TabIndex = 1;
            this.WorldName.Click += new System.EventHandler(this.Control_Click);
            this.WorldName.DoubleClick += new System.EventHandler(this.Control_DoubleClick);
            // 
            // FolderName
            // 
            this.FolderName.AutoSize = true;
            this.FolderName.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.FolderName.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.FolderName.Location = new System.Drawing.Point(141, 55);
            this.FolderName.Name = "FolderName";
            this.FolderName.Size = new System.Drawing.Size(0, 36);
            this.FolderName.TabIndex = 2;
            this.FolderName.Click += new System.EventHandler(this.Control_Click);
            this.FolderName.DoubleClick += new System.EventHandler(this.Control_DoubleClick);
            // 
            // BedrockWorldControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.FolderName);
            this.Controls.Add(this.WorldName);
            this.Controls.Add(this.WorldIcon);
            this.Name = "BedrockWorldControl";
            this.Size = new System.Drawing.Size(462, 136);
            ((System.ComponentModel.ISupportInitialize)(this.WorldIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox WorldIcon;
        private System.Windows.Forms.Label WorldName;
        private System.Windows.Forms.Label FolderName;
    }
}
