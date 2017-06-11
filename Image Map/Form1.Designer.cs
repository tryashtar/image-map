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
            this.OpenButton = new System.Windows.Forms.Button();
            this.ExportButton = new System.Windows.Forms.Button();
            this.PictureZone = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // OpenButton
            // 
            this.OpenButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.OpenButton.Location = new System.Drawing.Point(8, 8);
            this.OpenButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(119, 47);
            this.OpenButton.TabIndex = 0;
            this.OpenButton.Text = "Open";
            this.OpenButton.UseVisualStyleBackColor = true;
            this.OpenButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // ExportButton
            // 
            this.ExportButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.ExportButton.Location = new System.Drawing.Point(8, 58);
            this.ExportButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(119, 52);
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
            this.PictureZone.Location = new System.Drawing.Point(131, 8);
            this.PictureZone.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PictureZone.Name = "PictureZone";
            this.PictureZone.Size = new System.Drawing.Size(394, 249);
            this.PictureZone.TabIndex = 2;
            this.PictureZone.Resize += new System.EventHandler(this.PictureZone_Resize);
            // 
            // TheForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(529, 265);
            this.Controls.Add(this.PictureZone);
            this.Controls.Add(this.ExportButton);
            this.Controls.Add(this.OpenButton);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "TheForm";
            this.Text = "Image Map";
            this.Load += new System.EventHandler(this.TheForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button OpenButton;
        private System.Windows.Forms.Button ExportButton;
        private System.Windows.Forms.Panel PictureZone;
    }
}

