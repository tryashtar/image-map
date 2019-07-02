namespace Image_Map
{
    partial class ReplaceOptionDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReplaceOptionDialog));
            this.DescriptionLabel = new System.Windows.Forms.Label();
            this.SkipButton = new System.Windows.Forms.Button();
            this.OverwriteButton = new System.Windows.Forms.Button();
            this.AutoButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // DescriptionLabel
            // 
            this.DescriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DescriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.DescriptionLabel.Location = new System.Drawing.Point(12, 9);
            this.DescriptionLabel.Name = "DescriptionLabel";
            this.DescriptionLabel.Size = new System.Drawing.Size(378, 117);
            this.DescriptionLabel.TabIndex = 0;
            this.DescriptionLabel.Text = "Due to a conflict...";
            // 
            // SkipButton
            // 
            this.SkipButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.SkipButton.Location = new System.Drawing.Point(16, 141);
            this.SkipButton.Name = "SkipButton";
            this.SkipButton.Size = new System.Drawing.Size(119, 44);
            this.SkipButton.TabIndex = 1;
            this.SkipButton.Text = "Skip";
            this.SkipButton.UseVisualStyleBackColor = true;
            this.SkipButton.Click += new System.EventHandler(this.SkipButton_Click);
            // 
            // OverwriteButton
            // 
            this.OverwriteButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.OverwriteButton.Location = new System.Drawing.Point(141, 141);
            this.OverwriteButton.Name = "OverwriteButton";
            this.OverwriteButton.Size = new System.Drawing.Size(119, 44);
            this.OverwriteButton.TabIndex = 2;
            this.OverwriteButton.Text = "Overwrite";
            this.OverwriteButton.UseVisualStyleBackColor = true;
            this.OverwriteButton.Click += new System.EventHandler(this.OverwriteButton_Click);
            // 
            // AutoButton
            // 
            this.AutoButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.AutoButton.Location = new System.Drawing.Point(266, 141);
            this.AutoButton.Name = "AutoButton";
            this.AutoButton.Size = new System.Drawing.Size(119, 44);
            this.AutoButton.TabIndex = 3;
            this.AutoButton.Text = "Auto";
            this.AutoButton.UseVisualStyleBackColor = true;
            this.AutoButton.Click += new System.EventHandler(this.AutoButton_Click);
            // 
            // ReplaceOptionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 197);
            this.Controls.Add(this.AutoButton);
            this.Controls.Add(this.OverwriteButton);
            this.Controls.Add(this.SkipButton);
            this.Controls.Add(this.DescriptionLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ReplaceOptionDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Map ID conflicts!";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label DescriptionLabel;
        private System.Windows.Forms.Button SkipButton;
        private System.Windows.Forms.Button OverwriteButton;
        private System.Windows.Forms.Button AutoButton;
    }
}