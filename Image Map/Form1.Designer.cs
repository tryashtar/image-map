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
            this.InterpolationCombo = new System.Windows.Forms.ComboBox();
            this.InterpolationLabel = new System.Windows.Forms.Label();
            this.ImportProcessor = new System.ComponentModel.BackgroundWorker();
            this.ImportBar = new System.Windows.Forms.ProgressBar();
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
            this.PictureZone.Location = new System.Drawing.Point(196, 12);
            this.PictureZone.Name = "PictureZone";
            this.PictureZone.Size = new System.Drawing.Size(591, 383);
            this.PictureZone.TabIndex = 2;
            this.PictureZone.Resize += new System.EventHandler(this.PictureZone_Resize);
            // 
            // InterpolationCombo
            // 
            this.InterpolationCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.InterpolationCombo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.InterpolationCombo.FormattingEnabled = true;
            this.InterpolationCombo.Items.AddRange(new object[] {
            "Pixelly",
            "Bicubic"});
            this.InterpolationCombo.Location = new System.Drawing.Point(12, 223);
            this.InterpolationCombo.Name = "InterpolationCombo";
            this.InterpolationCombo.Size = new System.Drawing.Size(178, 33);
            this.InterpolationCombo.TabIndex = 5;
            // 
            // InterpolationLabel
            // 
            this.InterpolationLabel.AutoSize = true;
            this.InterpolationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.InterpolationLabel.Location = new System.Drawing.Point(12, 195);
            this.InterpolationLabel.Name = "InterpolationLabel";
            this.InterpolationLabel.Size = new System.Drawing.Size(138, 25);
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
            this.ImportBar.Location = new System.Drawing.Point(12, 280);
            this.ImportBar.Name = "ImportBar";
            this.ImportBar.Size = new System.Drawing.Size(178, 19);
            this.ImportBar.TabIndex = 7;
            this.ImportBar.Visible = false;
            // 
            // TheForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 408);
            this.Controls.Add(this.ImportBar);
            this.Controls.Add(this.InterpolationLabel);
            this.Controls.Add(this.InterpolationCombo);
            this.Controls.Add(this.PictureZone);
            this.Controls.Add(this.ExportButton);
            this.Controls.Add(this.OpenButton);
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
        private System.Windows.Forms.Panel PictureZone;
        private System.Windows.Forms.ComboBox InterpolationCombo;
        private System.Windows.Forms.Label InterpolationLabel;
        private System.ComponentModel.BackgroundWorker ImportProcessor;
        private System.Windows.Forms.ProgressBar ImportBar;
    }
}

