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
            this.PictureZone = new System.Windows.Forms.Panel();
            this.InterpolationLabel = new System.Windows.Forms.Label();
            this.ImportProcessor = new System.ComponentModel.BackgroundWorker();
            this.ImportBar = new System.Windows.Forms.ProgressBar();
            this.ImportLabel = new System.Windows.Forms.Label();
            this.MosaicWidth = new System.Windows.Forms.NumericUpDown();
            this.MosaicLabel = new System.Windows.Forms.Label();
            this.MosaicOutputBox = new System.Windows.Forms.TextBox();
            this.MosaicOutputCopy = new System.Windows.Forms.Button();
            this.InterpolationPanel = new System.Windows.Forms.Panel();
            this.NormalRadio = new System.Windows.Forms.RadioButton();
            this.PixelRadio = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ScaleRadio = new System.Windows.Forms.RadioButton();
            this.SplitRadio = new System.Windows.Forms.RadioButton();
            this.ImportTypeLabel = new System.Windows.Forms.Label();
            this.MosaicPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.MosaicWidth)).BeginInit();
            this.InterpolationPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.MosaicPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // OpenButton
            // 
            this.OpenButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.OpenButton.Location = new System.Drawing.Point(12, 12);
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(178, 72);
            this.OpenButton.TabIndex = 0;
            this.OpenButton.Text = "Open";
            this.OpenButton.UseVisualStyleBackColor = true;
            this.OpenButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // ExportButton
            // 
            this.ExportButton.Enabled = false;
            this.ExportButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.ExportButton.Location = new System.Drawing.Point(12, 91);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(178, 80);
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
            this.PictureZone.Location = new System.Drawing.Point(197, 12);
            this.PictureZone.Name = "PictureZone";
            this.PictureZone.Size = new System.Drawing.Size(893, 555);
            this.PictureZone.TabIndex = 2;
            this.PictureZone.Resize += new System.EventHandler(this.PictureZone_Resize);
            // 
            // InterpolationLabel
            // 
            this.InterpolationLabel.AutoSize = true;
            this.InterpolationLabel.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InterpolationLabel.Location = new System.Drawing.Point(7, 180);
            this.InterpolationLabel.Name = "InterpolationLabel";
            this.InterpolationLabel.Size = new System.Drawing.Size(150, 25);
            this.InterpolationLabel.TabIndex = 6;
            this.InterpolationLabel.Text = "Scaling Mode:";
            // 
            // ImportProcessor
            // 
            this.ImportProcessor.WorkerReportsProgress = true;
            this.ImportProcessor.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ImportProcessor_DoWork);
            this.ImportProcessor.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.ImportProcessor_ProgressChanged);
            this.ImportProcessor.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ImportProcessor_RunWorkerCompleted);
            // 
            // ImportBar
            // 
            this.ImportBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ImportBar.Location = new System.Drawing.Point(12, 547);
            this.ImportBar.Name = "ImportBar";
            this.ImportBar.Size = new System.Drawing.Size(178, 19);
            this.ImportBar.TabIndex = 7;
            this.ImportBar.Visible = false;
            // 
            // ImportLabel
            // 
            this.ImportLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ImportLabel.AutoSize = true;
            this.ImportLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.ImportLabel.Location = new System.Drawing.Point(12, 519);
            this.ImportLabel.Name = "ImportLabel";
            this.ImportLabel.Size = new System.Drawing.Size(111, 25);
            this.ImportLabel.TabIndex = 8;
            this.ImportLabel.Text = "Mapifying...";
            this.ImportLabel.Visible = false;
            // 
            // MosaicWidth
            // 
            this.MosaicWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.MosaicWidth.Location = new System.Drawing.Point(7, 39);
            this.MosaicWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.MosaicWidth.Name = "MosaicWidth";
            this.MosaicWidth.Size = new System.Drawing.Size(90, 35);
            this.MosaicWidth.TabIndex = 9;
            this.MosaicWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // MosaicLabel
            // 
            this.MosaicLabel.AutoSize = true;
            this.MosaicLabel.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MosaicLabel.Location = new System.Drawing.Point(3, 10);
            this.MosaicLabel.Name = "MosaicLabel";
            this.MosaicLabel.Size = new System.Drawing.Size(150, 25);
            this.MosaicLabel.TabIndex = 11;
            this.MosaicLabel.Text = "Mosaic Width:";
            // 
            // MosaicOutputBox
            // 
            this.MosaicOutputBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.MosaicOutputBox.Location = new System.Drawing.Point(7, 80);
            this.MosaicOutputBox.Name = "MosaicOutputBox";
            this.MosaicOutputBox.Size = new System.Drawing.Size(178, 30);
            this.MosaicOutputBox.TabIndex = 12;
            this.MosaicOutputBox.Visible = false;
            // 
            // MosaicOutputCopy
            // 
            this.MosaicOutputCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.MosaicOutputCopy.Location = new System.Drawing.Point(103, 39);
            this.MosaicOutputCopy.Name = "MosaicOutputCopy";
            this.MosaicOutputCopy.Size = new System.Drawing.Size(82, 35);
            this.MosaicOutputCopy.TabIndex = 13;
            this.MosaicOutputCopy.Text = "Copy";
            this.MosaicOutputCopy.UseVisualStyleBackColor = true;
            this.MosaicOutputCopy.Visible = false;
            this.MosaicOutputCopy.Click += new System.EventHandler(this.MosaicOutputCopy_Click);
            // 
            // InterpolationPanel
            // 
            this.InterpolationPanel.Controls.Add(this.NormalRadio);
            this.InterpolationPanel.Controls.Add(this.PixelRadio);
            this.InterpolationPanel.Location = new System.Drawing.Point(17, 208);
            this.InterpolationPanel.Name = "InterpolationPanel";
            this.InterpolationPanel.Size = new System.Drawing.Size(172, 74);
            this.InterpolationPanel.TabIndex = 14;
            // 
            // NormalRadio
            // 
            this.NormalRadio.AutoSize = true;
            this.NormalRadio.Checked = true;
            this.NormalRadio.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.NormalRadio.Location = new System.Drawing.Point(3, -1);
            this.NormalRadio.Name = "NormalRadio";
            this.NormalRadio.Size = new System.Drawing.Size(158, 29);
            this.NormalRadio.TabIndex = 1;
            this.NormalRadio.TabStop = true;
            this.NormalRadio.Text = "Normal Image";
            this.NormalRadio.UseVisualStyleBackColor = true;
            // 
            // PixelRadio
            // 
            this.PixelRadio.AutoSize = true;
            this.PixelRadio.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.PixelRadio.Location = new System.Drawing.Point(3, 29);
            this.PixelRadio.Name = "PixelRadio";
            this.PixelRadio.Size = new System.Drawing.Size(109, 29);
            this.PixelRadio.TabIndex = 0;
            this.PixelRadio.Text = "Pixel Art";
            this.PixelRadio.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ScaleRadio);
            this.panel1.Controls.Add(this.SplitRadio);
            this.panel1.Location = new System.Drawing.Point(17, 323);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(172, 74);
            this.panel1.TabIndex = 16;
            // 
            // ScaleRadio
            // 
            this.ScaleRadio.AutoSize = true;
            this.ScaleRadio.Checked = true;
            this.ScaleRadio.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.ScaleRadio.Location = new System.Drawing.Point(3, -1);
            this.ScaleRadio.Name = "ScaleRadio";
            this.ScaleRadio.Size = new System.Drawing.Size(87, 29);
            this.ScaleRadio.TabIndex = 1;
            this.ScaleRadio.TabStop = true;
            this.ScaleRadio.Text = "Scale";
            this.ScaleRadio.UseVisualStyleBackColor = true;
            // 
            // SplitRadio
            // 
            this.SplitRadio.AutoSize = true;
            this.SplitRadio.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.SplitRadio.Location = new System.Drawing.Point(3, 29);
            this.SplitRadio.Name = "SplitRadio";
            this.SplitRadio.Size = new System.Drawing.Size(75, 29);
            this.SplitRadio.TabIndex = 0;
            this.SplitRadio.Text = "Split";
            this.SplitRadio.UseVisualStyleBackColor = true;
            // 
            // ImportTypeLabel
            // 
            this.ImportTypeLabel.AutoSize = true;
            this.ImportTypeLabel.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ImportTypeLabel.Location = new System.Drawing.Point(7, 295);
            this.ImportTypeLabel.Name = "ImportTypeLabel";
            this.ImportTypeLabel.Size = new System.Drawing.Size(157, 25);
            this.ImportTypeLabel.TabIndex = 15;
            this.ImportTypeLabel.Text = "Import Images:";
            // 
            // MosaicPanel
            // 
            this.MosaicPanel.Controls.Add(this.MosaicLabel);
            this.MosaicPanel.Controls.Add(this.MosaicWidth);
            this.MosaicPanel.Controls.Add(this.MosaicOutputBox);
            this.MosaicPanel.Controls.Add(this.MosaicOutputCopy);
            this.MosaicPanel.Location = new System.Drawing.Point(8, 403);
            this.MosaicPanel.Name = "MosaicPanel";
            this.MosaicPanel.Size = new System.Drawing.Size(190, 117);
            this.MosaicPanel.TabIndex = 15;
            // 
            // TheForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1097, 580);
            this.Controls.Add(this.MosaicPanel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ImportTypeLabel);
            this.Controls.Add(this.InterpolationPanel);
            this.Controls.Add(this.ImportLabel);
            this.Controls.Add(this.ImportBar);
            this.Controls.Add(this.InterpolationLabel);
            this.Controls.Add(this.PictureZone);
            this.Controls.Add(this.ExportButton);
            this.Controls.Add(this.OpenButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TheForm";
            this.Text = "Image Map";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TheForm_FormClosed);
            this.Load += new System.EventHandler(this.TheForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MosaicWidth)).EndInit();
            this.InterpolationPanel.ResumeLayout(false);
            this.InterpolationPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.MosaicPanel.ResumeLayout(false);
            this.MosaicPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OpenButton;
        private System.Windows.Forms.Button ExportButton;
        private System.Windows.Forms.Panel PictureZone;
        private System.Windows.Forms.Label InterpolationLabel;
        private System.ComponentModel.BackgroundWorker ImportProcessor;
        private System.Windows.Forms.ProgressBar ImportBar;
        private System.Windows.Forms.Label ImportLabel;
        private System.Windows.Forms.NumericUpDown MosaicWidth;
        private System.Windows.Forms.Label MosaicLabel;
        private System.Windows.Forms.TextBox MosaicOutputBox;
        private System.Windows.Forms.Button MosaicOutputCopy;
        private System.Windows.Forms.Panel InterpolationPanel;
        private System.Windows.Forms.RadioButton NormalRadio;
        private System.Windows.Forms.RadioButton PixelRadio;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton ScaleRadio;
        private System.Windows.Forms.RadioButton SplitRadio;
        private System.Windows.Forms.Label ImportTypeLabel;
        private System.Windows.Forms.Panel MosaicPanel;
    }
}

