namespace FBO
{
    partial class Main
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.StartButton = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.ScanArea = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Status = new System.Windows.Forms.Label();
            this.CatchLabel = new System.Windows.Forms.Label();
            this.MissLabel = new System.Windows.Forms.Label();
            this.Confidence = new System.Windows.Forms.Label();
            this.FishPerHourLabel = new System.Windows.Forms.Label();
            this.PreviewImage = new System.Windows.Forms.PictureBox();
            this.FpsCount = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewImage)).BeginInit();
            this.SuspendLayout();
            // 
            // StartButton
            // 
            this.StartButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StartButton.Location = new System.Drawing.Point(8, 8);
            this.StartButton.Margin = new System.Windows.Forms.Padding(2);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(284, 23);
            this.StartButton.TabIndex = 0;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButtonClick);
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.DisplayMember = "KEY";
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(8, 35);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(285, 21);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.ValueMember = "VALUE";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // ScanArea
            // 
            this.ScanArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ScanArea.Location = new System.Drawing.Point(8, 246);
            this.ScanArea.Margin = new System.Windows.Forms.Padding(2);
            this.ScanArea.Name = "ScanArea";
            this.ScanArea.Size = new System.Drawing.Size(284, 25);
            this.ScanArea.TabIndex = 2;
            this.ScanArea.Text = "ScanArea";
            this.ScanArea.UseVisualStyleBackColor = true;
            this.ScanArea.Click += new System.EventHandler(this.ScanArea_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(5, 230);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(276, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "CTRL + F7 = START | F7 = END (also SHIFT + F7)";
            // 
            // Status
            // 
            this.Status.AutoSize = true;
            this.Status.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Status.Location = new System.Drawing.Point(9, 58);
            this.Status.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(46, 20);
            this.Status.TabIndex = 4;
            this.Status.Text = "IDLE";
            // 
            // CatchLabel
            // 
            this.CatchLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CatchLabel.AutoSize = true;
            this.CatchLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CatchLabel.Location = new System.Drawing.Point(7, 194);
            this.CatchLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.CatchLabel.Name = "CatchLabel";
            this.CatchLabel.Size = new System.Drawing.Size(35, 37);
            this.CatchLabel.TabIndex = 5;
            this.CatchLabel.Text = "0";
            this.CatchLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MissLabel
            // 
            this.MissLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.MissLabel.AutoSize = true;
            this.MissLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MissLabel.Location = new System.Drawing.Point(258, 194);
            this.MissLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.MissLabel.Name = "MissLabel";
            this.MissLabel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.MissLabel.Size = new System.Drawing.Size(35, 37);
            this.MissLabel.TabIndex = 5;
            this.MissLabel.Text = "0";
            this.MissLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Confidence
            // 
            this.Confidence.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Confidence.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Confidence.Location = new System.Drawing.Point(9, 77);
            this.Confidence.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Confidence.Name = "Confidence";
            this.Confidence.Size = new System.Drawing.Size(148, 19);
            this.Confidence.TabIndex = 4;
            this.Confidence.Text = "0";
            // 
            // FishPerHourLabel
            // 
            this.FishPerHourLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.FishPerHourLabel.AutoSize = true;
            this.FishPerHourLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FishPerHourLabel.Location = new System.Drawing.Point(9, 158);
            this.FishPerHourLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.FishPerHourLabel.Name = "FishPerHourLabel";
            this.FishPerHourLabel.Size = new System.Drawing.Size(58, 29);
            this.FishPerHourLabel.TabIndex = 6;
            this.FishPerHourLabel.Text = "0 / h";
            this.FishPerHourLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PreviewImage
            // 
            this.PreviewImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PreviewImage.BackColor = System.Drawing.Color.Black;
            this.PreviewImage.Location = new System.Drawing.Point(162, 61);
            this.PreviewImage.Name = "PreviewImage";
            this.PreviewImage.Size = new System.Drawing.Size(130, 130);
            this.PreviewImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PreviewImage.TabIndex = 7;
            this.PreviewImage.TabStop = false;
            // 
            // FpsCount
            // 
            this.FpsCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FpsCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FpsCount.Location = new System.Drawing.Point(9, 96);
            this.FpsCount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.FpsCount.Name = "FpsCount";
            this.FpsCount.Size = new System.Drawing.Size(148, 19);
            this.FpsCount.TabIndex = 4;
            this.FpsCount.Text = "0";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 279);
            this.Controls.Add(this.PreviewImage);
            this.Controls.Add(this.FishPerHourLabel);
            this.Controls.Add(this.MissLabel);
            this.Controls.Add(this.CatchLabel);
            this.Controls.Add(this.FpsCount);
            this.Controls.Add(this.Confidence);
            this.Controls.Add(this.Status);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ScanArea);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.StartButton);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Opacity = 0.9D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Anime Player 0.1~alpha";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PreviewImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button ScanArea;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label Status;
        private System.Windows.Forms.Label CatchLabel;
        private System.Windows.Forms.Label MissLabel;
        private System.Windows.Forms.Label Confidence;
        private System.Windows.Forms.Label FishPerHourLabel;
        private System.Windows.Forms.PictureBox PreviewImage;
        private System.Windows.Forms.Label FpsCount;
    }
}

