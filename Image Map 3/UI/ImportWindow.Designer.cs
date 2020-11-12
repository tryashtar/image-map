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
            this.PreviewBox = new ImageMap.InterpPictureBox();
            this.ControlsPanel = new System.Windows.Forms.Panel();
            this.HeightLabel = new System.Windows.Forms.Label();
            this.WidthLabel = new System.Windows.Forms.Label();
            this.CancelAllButton = new System.Windows.Forms.Button();
            this.ConfirmAllButton = new System.Windows.Forms.Button();
            this.ColorAlgorithmBox = new System.Windows.Forms.ComboBox();
            this.StretchCheck = new System.Windows.Forms.CheckBox();
            this.DitherCheck = new System.Windows.Forms.CheckBox();
            this.RotateButton = new System.Windows.Forms.Button();
            this.CancellationButton = new System.Windows.Forms.Button();
            this.ConfirmButton = new System.Windows.Forms.Button();
            this.HeightInput = new System.Windows.Forms.NumericUpDown();
            this.WidthInput = new System.Windows.Forms.NumericUpDown();
            this.InterpolationModeBox = new System.Windows.Forms.ComboBox();
            this.CurrentIndexLabel = new System.Windows.Forms.Label();
            this.PreviewPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewBox)).BeginInit();
            this.ControlsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HeightInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WidthInput)).BeginInit();
            this.SuspendLayout();
            // 
            // PreviewPanel
            // 
            this.PreviewPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.PreviewPanel.Controls.Add(this.PreviewBox);
            this.PreviewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PreviewPanel.Location = new System.Drawing.Point(0, 0);
            this.PreviewPanel.Margin = new System.Windows.Forms.Padding(2);
            this.PreviewPanel.Name = "PreviewPanel";
            this.PreviewPanel.Size = new System.Drawing.Size(675, 326);
            this.PreviewPanel.TabIndex = 10;
            // 
            // PreviewBox
            // 
            this.PreviewBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.PreviewBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PreviewBox.Interp = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.PreviewBox.Location = new System.Drawing.Point(258, 10);
            this.PreviewBox.Margin = new System.Windows.Forms.Padding(2);
            this.PreviewBox.Name = "PreviewBox";
            this.PreviewBox.Size = new System.Drawing.Size(126, 134);
            this.PreviewBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PreviewBox.TabIndex = 1;
            this.PreviewBox.TabStop = false;
            this.PreviewBox.Paint += new System.Windows.Forms.PaintEventHandler(this.PreviewBox_Paint);
            // 
            // ControlsPanel
            // 
            this.ControlsPanel.Controls.Add(this.HeightLabel);
            this.ControlsPanel.Controls.Add(this.WidthLabel);
            this.ControlsPanel.Controls.Add(this.CancelAllButton);
            this.ControlsPanel.Controls.Add(this.ConfirmAllButton);
            this.ControlsPanel.Controls.Add(this.ColorAlgorithmBox);
            this.ControlsPanel.Controls.Add(this.StretchCheck);
            this.ControlsPanel.Controls.Add(this.DitherCheck);
            this.ControlsPanel.Controls.Add(this.RotateButton);
            this.ControlsPanel.Controls.Add(this.CancellationButton);
            this.ControlsPanel.Controls.Add(this.ConfirmButton);
            this.ControlsPanel.Controls.Add(this.HeightInput);
            this.ControlsPanel.Controls.Add(this.WidthInput);
            this.ControlsPanel.Controls.Add(this.InterpolationModeBox);
            this.ControlsPanel.Controls.Add(this.CurrentIndexLabel);
            this.ControlsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ControlsPanel.Location = new System.Drawing.Point(0, 326);
            this.ControlsPanel.Margin = new System.Windows.Forms.Padding(2);
            this.ControlsPanel.Name = "ControlsPanel";
            this.ControlsPanel.Size = new System.Drawing.Size(675, 199);
            this.ControlsPanel.TabIndex = 11;
            // 
            // HeightLabel
            // 
            this.HeightLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.HeightLabel.AutoSize = true;
            this.HeightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.HeightLabel.Location = new System.Drawing.Point(167, 65);
            this.HeightLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.HeightLabel.Name = "HeightLabel";
            this.HeightLabel.Size = new System.Drawing.Size(70, 24);
            this.HeightLabel.TabIndex = 24;
            this.HeightLabel.Text = "Height:";
            // 
            // WidthLabel
            // 
            this.WidthLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.WidthLabel.AutoSize = true;
            this.WidthLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.WidthLabel.Location = new System.Drawing.Point(7, 65);
            this.WidthLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.WidthLabel.Name = "WidthLabel";
            this.WidthLabel.Size = new System.Drawing.Size(63, 24);
            this.WidthLabel.TabIndex = 23;
            this.WidthLabel.Text = "Width:";
            // 
            // CancelAllButton
            // 
            this.CancelAllButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.CancelAllButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.CancelAllButton.Location = new System.Drawing.Point(507, 112);
            this.CancelAllButton.Margin = new System.Windows.Forms.Padding(2);
            this.CancelAllButton.Name = "CancelAllButton";
            this.CancelAllButton.Size = new System.Drawing.Size(156, 48);
            this.CancelAllButton.TabIndex = 22;
            this.CancelAllButton.Text = "Cancel All";
            this.CancelAllButton.UseVisualStyleBackColor = true;
            this.CancelAllButton.Click += new System.EventHandler(this.CancelAllButton_Click);
            // 
            // ConfirmAllButton
            // 
            this.ConfirmAllButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ConfirmAllButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.ConfirmAllButton.Location = new System.Drawing.Point(347, 112);
            this.ConfirmAllButton.Margin = new System.Windows.Forms.Padding(2);
            this.ConfirmAllButton.Name = "ConfirmAllButton";
            this.ConfirmAllButton.Size = new System.Drawing.Size(156, 48);
            this.ConfirmAllButton.TabIndex = 21;
            this.ConfirmAllButton.Text = "Confirm All";
            this.ConfirmAllButton.UseVisualStyleBackColor = true;
            this.ConfirmAllButton.Click += new System.EventHandler(this.ConfirmAllButton_Click);
            // 
            // ColorAlgorithmBox
            // 
            this.ColorAlgorithmBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ColorAlgorithmBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ColorAlgorithmBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ColorAlgorithmBox.FormattingEnabled = true;
            this.ColorAlgorithmBox.Location = new System.Drawing.Point(371, 18);
            this.ColorAlgorithmBox.Margin = new System.Windows.Forms.Padding(2);
            this.ColorAlgorithmBox.Name = "ColorAlgorithmBox";
            this.ColorAlgorithmBox.Size = new System.Drawing.Size(186, 28);
            this.ColorAlgorithmBox.TabIndex = 20;
            // 
            // StretchCheck
            // 
            this.StretchCheck.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.StretchCheck.AutoSize = true;
            this.StretchCheck.Checked = true;
            this.StretchCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.StretchCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.StretchCheck.Location = new System.Drawing.Point(334, 65);
            this.StretchCheck.Margin = new System.Windows.Forms.Padding(2);
            this.StretchCheck.Name = "StretchCheck";
            this.StretchCheck.Size = new System.Drawing.Size(169, 24);
            this.StretchCheck.TabIndex = 19;
            this.StretchCheck.Text = "Stretch to fill frames";
            this.StretchCheck.UseVisualStyleBackColor = true;
            this.StretchCheck.CheckedChanged += new System.EventHandler(this.StretchCheck_CheckedChanged);
            // 
            // DitherCheck
            // 
            this.DitherCheck.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.DitherCheck.AutoSize = true;
            this.DitherCheck.Checked = true;
            this.DitherCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DitherCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.DitherCheck.Location = new System.Drawing.Point(571, 22);
            this.DitherCheck.Margin = new System.Windows.Forms.Padding(2);
            this.DitherCheck.Name = "DitherCheck";
            this.DitherCheck.Size = new System.Drawing.Size(92, 24);
            this.DitherCheck.TabIndex = 18;
            this.DitherCheck.Text = "Dithering";
            this.DitherCheck.UseVisualStyleBackColor = true;
            // 
            // RotateButton
            // 
            this.RotateButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.RotateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.RotateButton.Location = new System.Drawing.Point(11, 18);
            this.RotateButton.Margin = new System.Windows.Forms.Padding(2);
            this.RotateButton.Name = "RotateButton";
            this.RotateButton.Size = new System.Drawing.Size(143, 30);
            this.RotateButton.TabIndex = 17;
            this.RotateButton.Text = "↻ Rotate Image";
            this.RotateButton.UseVisualStyleBackColor = true;
            this.RotateButton.Click += new System.EventHandler(this.RotateButton_Click);
            // 
            // CancellationButton
            // 
            this.CancellationButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.CancellationButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.CancellationButton.Location = new System.Drawing.Point(171, 112);
            this.CancellationButton.Margin = new System.Windows.Forms.Padding(2);
            this.CancellationButton.Name = "CancellationButton";
            this.CancellationButton.Size = new System.Drawing.Size(156, 48);
            this.CancellationButton.TabIndex = 15;
            this.CancellationButton.Text = "Cancel";
            this.CancellationButton.UseVisualStyleBackColor = true;
            this.CancellationButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // ConfirmButton
            // 
            this.ConfirmButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ConfirmButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.ConfirmButton.Location = new System.Drawing.Point(11, 112);
            this.ConfirmButton.Margin = new System.Windows.Forms.Padding(2);
            this.ConfirmButton.Name = "ConfirmButton";
            this.ConfirmButton.Size = new System.Drawing.Size(156, 48);
            this.ConfirmButton.TabIndex = 14;
            this.ConfirmButton.Text = "Confirm";
            this.ConfirmButton.UseVisualStyleBackColor = true;
            this.ConfirmButton.Click += new System.EventHandler(this.ConfirmButton_Click);
            // 
            // HeightInput
            // 
            this.HeightInput.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.HeightInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.HeightInput.Location = new System.Drawing.Point(241, 62);
            this.HeightInput.Margin = new System.Windows.Forms.Padding(2);
            this.HeightInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.HeightInput.Name = "HeightInput";
            this.HeightInput.Size = new System.Drawing.Size(80, 32);
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
            this.WidthInput.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.WidthInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.WidthInput.Location = new System.Drawing.Point(74, 62);
            this.WidthInput.Margin = new System.Windows.Forms.Padding(2);
            this.WidthInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.WidthInput.Name = "WidthInput";
            this.WidthInput.Size = new System.Drawing.Size(80, 32);
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
            this.InterpolationModeBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.InterpolationModeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.InterpolationModeBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.InterpolationModeBox.FormattingEnabled = true;
            this.InterpolationModeBox.IntegralHeight = false;
            this.InterpolationModeBox.Location = new System.Drawing.Point(171, 19);
            this.InterpolationModeBox.Margin = new System.Windows.Forms.Padding(2);
            this.InterpolationModeBox.Name = "InterpolationModeBox";
            this.InterpolationModeBox.Size = new System.Drawing.Size(186, 28);
            this.InterpolationModeBox.TabIndex = 11;
            this.InterpolationModeBox.SelectedIndexChanged += new System.EventHandler(this.InterpolationModeBox_SelectedIndexChanged);
            // 
            // CurrentIndexLabel
            // 
            this.CurrentIndexLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CurrentIndexLabel.AutoSize = true;
            this.CurrentIndexLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.CurrentIndexLabel.Location = new System.Drawing.Point(3, 166);
            this.CurrentIndexLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.CurrentIndexLabel.Name = "CurrentIndexLabel";
            this.CurrentIndexLabel.Size = new System.Drawing.Size(54, 26);
            this.CurrentIndexLabel.TabIndex = 10;
            this.CurrentIndexLabel.Text = "0 / 0";
            // 
            // ImportWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(675, 525);
            this.Controls.Add(this.PreviewPanel);
            this.Controls.Add(this.ControlsPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ImportWindow";
            this.Text = "Import";
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.ImportWindow_Layout);
            this.PreviewPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PreviewBox)).EndInit();
            this.ControlsPanel.ResumeLayout(false);
            this.ControlsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HeightInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WidthInput)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        public InterpPictureBox PreviewBox;
        private System.Windows.Forms.Panel PreviewPanel;
        private System.Windows.Forms.Panel ControlsPanel;
        public System.Windows.Forms.CheckBox DitherCheck;
        private System.Windows.Forms.Button RotateButton;
        private System.Windows.Forms.Button CancellationButton;
        private System.Windows.Forms.Button ConfirmButton;
        private System.Windows.Forms.NumericUpDown HeightInput;
        private System.Windows.Forms.NumericUpDown WidthInput;
        public System.Windows.Forms.ComboBox InterpolationModeBox;
        private System.Windows.Forms.Label CurrentIndexLabel;
        public System.Windows.Forms.CheckBox StretchCheck;
        public System.Windows.Forms.ComboBox ColorAlgorithmBox;
        private System.Windows.Forms.Button ConfirmAllButton;
        private System.Windows.Forms.Button CancelAllButton;
        private System.Windows.Forms.Label WidthLabel;
        private System.Windows.Forms.Label HeightLabel;
    }
}