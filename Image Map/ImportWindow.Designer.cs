namespace Image_Map
{
    partial class ImportWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportWindow));
            this.CurrentIndexLabel = new System.Windows.Forms.Label();
            this.InterpolationModeBox = new System.Windows.Forms.ComboBox();
            this.WidthInput = new System.Windows.Forms.NumericUpDown();
            this.HeightInput = new System.Windows.Forms.NumericUpDown();
            this.ConfirmButton = new System.Windows.Forms.Button();
            this.CancellationButton = new System.Windows.Forms.Button();
            this.ApplyAllCheck = new System.Windows.Forms.CheckBox();
            this.RotateButton = new System.Windows.Forms.Button();
            this.PreviewBox = new Image_Map.InterpPictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.WidthInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HeightInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewBox)).BeginInit();
            this.SuspendLayout();
            // 
            // CurrentIndexLabel
            // 
            this.CurrentIndexLabel.AutoSize = true;
            this.CurrentIndexLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.CurrentIndexLabel.Location = new System.Drawing.Point(8, 362);
            this.CurrentIndexLabel.Name = "CurrentIndexLabel";
            this.CurrentIndexLabel.Size = new System.Drawing.Size(66, 31);
            this.CurrentIndexLabel.TabIndex = 0;
            this.CurrentIndexLabel.Text = "0 / 0";
            // 
            // InterpolationModeBox
            // 
            this.InterpolationModeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.InterpolationModeBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.InterpolationModeBox.FormattingEnabled = true;
            this.InterpolationModeBox.Items.AddRange(new object[] {
            "Automatic",
            "Pixel Art",
            "Normal Image"});
            this.InterpolationModeBox.Location = new System.Drawing.Point(14, 201);
            this.InterpolationModeBox.Name = "InterpolationModeBox";
            this.InterpolationModeBox.Size = new System.Drawing.Size(246, 39);
            this.InterpolationModeBox.TabIndex = 2;
            this.InterpolationModeBox.SelectedIndexChanged += new System.EventHandler(this.InterpolationModeBox_SelectedIndexChanged);
            // 
            // WidthInput
            // 
            this.WidthInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.WidthInput.Location = new System.Drawing.Point(14, 246);
            this.WidthInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.WidthInput.Name = "WidthInput";
            this.WidthInput.Size = new System.Drawing.Size(120, 38);
            this.WidthInput.TabIndex = 3;
            this.WidthInput.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.WidthInput.ValueChanged += new System.EventHandler(this.DimensionsInput_ValueChanged);
            // 
            // HeightInput
            // 
            this.HeightInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.HeightInput.Location = new System.Drawing.Point(140, 246);
            this.HeightInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.HeightInput.Name = "HeightInput";
            this.HeightInput.Size = new System.Drawing.Size(120, 38);
            this.HeightInput.TabIndex = 4;
            this.HeightInput.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.HeightInput.ValueChanged += new System.EventHandler(this.DimensionsInput_ValueChanged);
            // 
            // ConfirmButton
            // 
            this.ConfirmButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.ConfirmButton.Location = new System.Drawing.Point(14, 300);
            this.ConfirmButton.Name = "ConfirmButton";
            this.ConfirmButton.Size = new System.Drawing.Size(208, 59);
            this.ConfirmButton.TabIndex = 5;
            this.ConfirmButton.Text = "Confirm";
            this.ConfirmButton.UseVisualStyleBackColor = true;
            this.ConfirmButton.Click += new System.EventHandler(this.ConfirmButton_Click);
            // 
            // CancellationButton
            // 
            this.CancellationButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.CancellationButton.Location = new System.Drawing.Point(228, 300);
            this.CancellationButton.Name = "CancellationButton";
            this.CancellationButton.Size = new System.Drawing.Size(158, 59);
            this.CancellationButton.TabIndex = 6;
            this.CancellationButton.Text = "Cancel";
            this.CancellationButton.UseVisualStyleBackColor = true;
            this.CancellationButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // ApplyAllCheck
            // 
            this.ApplyAllCheck.AutoSize = true;
            this.ApplyAllCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ApplyAllCheck.Location = new System.Drawing.Point(392, 324);
            this.ApplyAllCheck.Name = "ApplyAllCheck";
            this.ApplyAllCheck.Size = new System.Drawing.Size(132, 29);
            this.ApplyAllCheck.TabIndex = 7;
            this.ApplyAllCheck.Text = "Apply to All";
            this.ApplyAllCheck.UseVisualStyleBackColor = true;
            // 
            // RotateButton
            // 
            this.RotateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.RotateButton.Location = new System.Drawing.Point(266, 201);
            this.RotateButton.Name = "RotateButton";
            this.RotateButton.Size = new System.Drawing.Size(49, 39);
            this.RotateButton.TabIndex = 8;
            this.RotateButton.Text = "↻";
            this.RotateButton.UseVisualStyleBackColor = true;
            this.RotateButton.Click += new System.EventHandler(this.RotateButton_Click);
            // 
            // PreviewBox
            // 
            this.PreviewBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.PreviewBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PreviewBox.Interp = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.PreviewBox.Location = new System.Drawing.Point(303, 12);
            this.PreviewBox.Name = "PreviewBox";
            this.PreviewBox.Size = new System.Drawing.Size(167, 164);
            this.PreviewBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PreviewBox.TabIndex = 1;
            this.PreviewBox.TabStop = false;
            this.PreviewBox.Paint += new System.Windows.Forms.PaintEventHandler(this.PreviewBox_Paint);
            // 
            // ImportWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(838, 402);
            this.Controls.Add(this.RotateButton);
            this.Controls.Add(this.ApplyAllCheck);
            this.Controls.Add(this.CancellationButton);
            this.Controls.Add(this.ConfirmButton);
            this.Controls.Add(this.HeightInput);
            this.Controls.Add(this.WidthInput);
            this.Controls.Add(this.InterpolationModeBox);
            this.Controls.Add(this.PreviewBox);
            this.Controls.Add(this.CurrentIndexLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ImportWindow";
            this.Text = "Import";
            ((System.ComponentModel.ISupportInitialize)(this.WidthInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HeightInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label CurrentIndexLabel;
        public InterpPictureBox PreviewBox;
        public System.Windows.Forms.ComboBox InterpolationModeBox;
        private System.Windows.Forms.NumericUpDown WidthInput;
        private System.Windows.Forms.NumericUpDown HeightInput;
        private System.Windows.Forms.Button ConfirmButton;
        private System.Windows.Forms.Button CancellationButton;
        public System.Windows.Forms.CheckBox ApplyAllCheck;
        private System.Windows.Forms.Button RotateButton;
    }
}