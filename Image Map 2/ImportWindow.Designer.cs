namespace ImageMap
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
            this.PreviewPanel = new System.Windows.Forms.Panel();
            this.ControlsPanel = new System.Windows.Forms.Panel();
            this.DitherCheck = new System.Windows.Forms.CheckBox();
            this.RotateButton = new System.Windows.Forms.Button();
            this.ApplyAllCheck = new System.Windows.Forms.CheckBox();
            this.CancellationButton = new System.Windows.Forms.Button();
            this.ConfirmButton = new System.Windows.Forms.Button();
            this.HeightInput = new System.Windows.Forms.NumericUpDown();
            this.WidthInput = new System.Windows.Forms.NumericUpDown();
            this.InterpolationModeBox = new System.Windows.Forms.ComboBox();
            this.CurrentIndexLabel = new System.Windows.Forms.Label();
            this.PreviewBox = new ImageMap.InterpPictureBox();
            this.PreviewPanel.SuspendLayout();
            this.ControlsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HeightInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WidthInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewBox)).BeginInit();
            this.SuspendLayout();
            // 
            // PreviewPanel
            // 
            this.PreviewPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.PreviewPanel.Controls.Add(this.PreviewBox);
            this.PreviewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PreviewPanel.Location = new System.Drawing.Point(0, 0);
            this.PreviewPanel.Name = "PreviewPanel";
            this.PreviewPanel.Size = new System.Drawing.Size(666, 327);
            this.PreviewPanel.TabIndex = 10;
            // 
            // ControlsPanel
            // 
            this.ControlsPanel.Controls.Add(this.DitherCheck);
            this.ControlsPanel.Controls.Add(this.RotateButton);
            this.ControlsPanel.Controls.Add(this.ApplyAllCheck);
            this.ControlsPanel.Controls.Add(this.CancellationButton);
            this.ControlsPanel.Controls.Add(this.ConfirmButton);
            this.ControlsPanel.Controls.Add(this.HeightInput);
            this.ControlsPanel.Controls.Add(this.WidthInput);
            this.ControlsPanel.Controls.Add(this.InterpolationModeBox);
            this.ControlsPanel.Controls.Add(this.CurrentIndexLabel);
            this.ControlsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ControlsPanel.Location = new System.Drawing.Point(0, 327);
            this.ControlsPanel.Name = "ControlsPanel";
            this.ControlsPanel.Size = new System.Drawing.Size(666, 207);
            this.ControlsPanel.TabIndex = 11;
            // 
            // DitherCheck
            // 
            this.DitherCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DitherCheck.AutoSize = true;
            this.DitherCheck.Checked = true;
            this.DitherCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DitherCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.DitherCheck.Location = new System.Drawing.Point(274, 64);
            this.DitherCheck.Name = "DitherCheck";
            this.DitherCheck.Size = new System.Drawing.Size(85, 29);
            this.DitherCheck.TabIndex = 18;
            this.DitherCheck.Text = "Dither";
            this.DitherCheck.UseVisualStyleBackColor = true;
            // 
            // RotateButton
            // 
            this.RotateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RotateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.RotateButton.Location = new System.Drawing.Point(264, 12);
            this.RotateButton.Name = "RotateButton";
            this.RotateButton.Size = new System.Drawing.Size(49, 39);
            this.RotateButton.TabIndex = 17;
            this.RotateButton.Text = "↻";
            this.RotateButton.UseVisualStyleBackColor = true;
            this.RotateButton.Click += new System.EventHandler(this.RotateButton_Click);
            // 
            // ApplyAllCheck
            // 
            this.ApplyAllCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ApplyAllCheck.AutoSize = true;
            this.ApplyAllCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ApplyAllCheck.Location = new System.Drawing.Point(390, 135);
            this.ApplyAllCheck.Name = "ApplyAllCheck";
            this.ApplyAllCheck.Size = new System.Drawing.Size(132, 29);
            this.ApplyAllCheck.TabIndex = 16;
            this.ApplyAllCheck.Text = "Apply to All";
            this.ApplyAllCheck.UseVisualStyleBackColor = true;
            // 
            // CancellationButton
            // 
            this.CancellationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CancellationButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.CancellationButton.Location = new System.Drawing.Point(226, 111);
            this.CancellationButton.Name = "CancellationButton";
            this.CancellationButton.Size = new System.Drawing.Size(158, 59);
            this.CancellationButton.TabIndex = 15;
            this.CancellationButton.Text = "Cancel";
            this.CancellationButton.UseVisualStyleBackColor = true;
            this.CancellationButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // ConfirmButton
            // 
            this.ConfirmButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ConfirmButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.ConfirmButton.Location = new System.Drawing.Point(12, 111);
            this.ConfirmButton.Name = "ConfirmButton";
            this.ConfirmButton.Size = new System.Drawing.Size(208, 59);
            this.ConfirmButton.TabIndex = 14;
            this.ConfirmButton.Text = "Confirm";
            this.ConfirmButton.UseVisualStyleBackColor = true;
            this.ConfirmButton.Click += new System.EventHandler(this.ConfirmButton_Click);
            // 
            // HeightInput
            // 
            this.HeightInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.HeightInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.HeightInput.Location = new System.Drawing.Point(138, 57);
            this.HeightInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.HeightInput.Name = "HeightInput";
            this.HeightInput.Size = new System.Drawing.Size(120, 38);
            this.HeightInput.TabIndex = 13;
            this.HeightInput.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.HeightInput.ValueChanged += new System.EventHandler(this.DimensionsInput_ValueChanged);
            // 
            // WidthInput
            // 
            this.WidthInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.WidthInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.WidthInput.Location = new System.Drawing.Point(12, 57);
            this.WidthInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.WidthInput.Name = "WidthInput";
            this.WidthInput.Size = new System.Drawing.Size(120, 38);
            this.WidthInput.TabIndex = 12;
            this.WidthInput.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.WidthInput.ValueChanged += new System.EventHandler(this.DimensionsInput_ValueChanged);
            // 
            // InterpolationModeBox
            // 
            this.InterpolationModeBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.InterpolationModeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.InterpolationModeBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.InterpolationModeBox.FormattingEnabled = true;
            this.InterpolationModeBox.Items.AddRange(new object[] {
            "Automatic",
            "Pixel Art",
            "Normal Image"});
            this.InterpolationModeBox.Location = new System.Drawing.Point(12, 12);
            this.InterpolationModeBox.Name = "InterpolationModeBox";
            this.InterpolationModeBox.Size = new System.Drawing.Size(246, 39);
            this.InterpolationModeBox.TabIndex = 11;
            this.InterpolationModeBox.SelectedIndexChanged += new System.EventHandler(this.InterpolationModeBox_SelectedIndexChanged);
            // 
            // CurrentIndexLabel
            // 
            this.CurrentIndexLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CurrentIndexLabel.AutoSize = true;
            this.CurrentIndexLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.CurrentIndexLabel.Location = new System.Drawing.Point(6, 173);
            this.CurrentIndexLabel.Name = "CurrentIndexLabel";
            this.CurrentIndexLabel.Size = new System.Drawing.Size(66, 31);
            this.CurrentIndexLabel.TabIndex = 10;
            this.CurrentIndexLabel.Text = "0 / 0";
            // 
            // PreviewBox
            // 
            this.PreviewBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.PreviewBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PreviewBox.Interp = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.PreviewBox.Location = new System.Drawing.Point(228, 12);
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
            this.ClientSize = new System.Drawing.Size(666, 534);
            this.Controls.Add(this.PreviewPanel);
            this.Controls.Add(this.ControlsPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ImportWindow";
            this.Text = "Import";
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.ImportWindow_Layout);
            this.PreviewPanel.ResumeLayout(false);
            this.ControlsPanel.ResumeLayout(false);
            this.ControlsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HeightInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WidthInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        public InterpPictureBox PreviewBox;
        private System.Windows.Forms.Panel PreviewPanel;
        private System.Windows.Forms.Panel ControlsPanel;
        public System.Windows.Forms.CheckBox DitherCheck;
        private System.Windows.Forms.Button RotateButton;
        public System.Windows.Forms.CheckBox ApplyAllCheck;
        private System.Windows.Forms.Button CancellationButton;
        private System.Windows.Forms.Button ConfirmButton;
        private System.Windows.Forms.NumericUpDown HeightInput;
        private System.Windows.Forms.NumericUpDown WidthInput;
        public System.Windows.Forms.ComboBox InterpolationModeBox;
        private System.Windows.Forms.Label CurrentIndexLabel;
    }
}