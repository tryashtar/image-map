namespace Image_Map
{
    partial class SplitImageForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplitImageForm));
            this.PicturePreview = new System.Windows.Forms.PictureBox();
            this.SplitLabel = new System.Windows.Forms.Label();
            this.WidthInput = new System.Windows.Forms.NumericUpDown();
            this.HeightInput = new System.Windows.Forms.NumericUpDown();
            this.WidthLabel = new System.Windows.Forms.Label();
            this.HeightLabel = new System.Windows.Forms.Label();
            this.ConfirmButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.PicturePreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WidthInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HeightInput)).BeginInit();
            this.SuspendLayout();
            // 
            // PicturePreview
            // 
            this.PicturePreview.Location = new System.Drawing.Point(12, 12);
            this.PicturePreview.Name = "PicturePreview";
            this.PicturePreview.Size = new System.Drawing.Size(250, 250);
            this.PicturePreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PicturePreview.TabIndex = 1;
            this.PicturePreview.TabStop = false;
            // 
            // SplitLabel
            // 
            this.SplitLabel.AutoSize = true;
            this.SplitLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F);
            this.SplitLabel.Location = new System.Drawing.Point(268, 12);
            this.SplitLabel.Name = "SplitLabel";
            this.SplitLabel.Size = new System.Drawing.Size(325, 78);
            this.SplitLabel.TabIndex = 2;
            this.SplitLabel.Text = "Split your image into\r\nmultiple maps";
            // 
            // WidthInput
            // 
            this.WidthInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.WidthInput.Location = new System.Drawing.Point(275, 105);
            this.WidthInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.WidthInput.Name = "WidthInput";
            this.WidthInput.Size = new System.Drawing.Size(120, 53);
            this.WidthInput.TabIndex = 3;
            this.WidthInput.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // HeightInput
            // 
            this.HeightInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.HeightInput.Location = new System.Drawing.Point(275, 164);
            this.HeightInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.HeightInput.Name = "HeightInput";
            this.HeightInput.Size = new System.Drawing.Size(120, 53);
            this.HeightInput.TabIndex = 4;
            this.HeightInput.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // WidthLabel
            // 
            this.WidthLabel.AutoSize = true;
            this.WidthLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.WidthLabel.Location = new System.Drawing.Point(401, 118);
            this.WidthLabel.Name = "WidthLabel";
            this.WidthLabel.Size = new System.Drawing.Size(88, 32);
            this.WidthLabel.TabIndex = 5;
            this.WidthLabel.Text = "Width";
            // 
            // HeightLabel
            // 
            this.HeightLabel.AutoSize = true;
            this.HeightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.HeightLabel.Location = new System.Drawing.Point(401, 177);
            this.HeightLabel.Name = "HeightLabel";
            this.HeightLabel.Size = new System.Drawing.Size(98, 32);
            this.HeightLabel.TabIndex = 6;
            this.HeightLabel.Text = "Height";
            // 
            // ConfirmButton
            // 
            this.ConfirmButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.ConfirmButton.Location = new System.Drawing.Point(275, 227);
            this.ConfirmButton.Name = "ConfirmButton";
            this.ConfirmButton.Size = new System.Drawing.Size(224, 58);
            this.ConfirmButton.TabIndex = 7;
            this.ConfirmButton.Text = "Confirm";
            this.ConfirmButton.UseVisualStyleBackColor = true;
            this.ConfirmButton.Click += new System.EventHandler(this.ConfirmButton_Click);
            // 
            // SplitImageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(643, 293);
            this.Controls.Add(this.ConfirmButton);
            this.Controls.Add(this.HeightLabel);
            this.Controls.Add(this.WidthLabel);
            this.Controls.Add(this.HeightInput);
            this.Controls.Add(this.WidthInput);
            this.Controls.Add(this.SplitLabel);
            this.Controls.Add(this.PicturePreview);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SplitImageForm";
            this.ShowInTaskbar = false;
            this.Text = "Split Image";
            this.Load += new System.EventHandler(this.SplitImageForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PicturePreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WidthInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HeightInput)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.PictureBox PicturePreview;
        private System.Windows.Forms.Label SplitLabel;
        public System.Windows.Forms.NumericUpDown WidthInput;
        public System.Windows.Forms.NumericUpDown HeightInput;
        private System.Windows.Forms.Label WidthLabel;
        private System.Windows.Forms.Label HeightLabel;
        private System.Windows.Forms.Button ConfirmButton;
    }
}