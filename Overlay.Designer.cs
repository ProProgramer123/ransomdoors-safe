namespace rans0m
{
    partial class Overlay
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pc_ransom = new PictureBox();
            pc_attack = new PictureBox();
            txt_download = new Label();
            pc_stopsign = new PictureBox();
            pb_download = new ProgressBar();
            ((System.ComponentModel.ISupportInitialize)pc_ransom).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pc_attack).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pc_stopsign).BeginInit();
            SuspendLayout();
            // 
            // pc_ransom
            // 
            pc_ransom.BackColor = Color.Transparent;
            pc_ransom.Image = Properties.Resources.ransom_idle;
            pc_ransom.Location = new Point(35, 22);
            pc_ransom.Name = "pc_ransom";
            pc_ransom.Size = new Size(192, 191);
            pc_ransom.SizeMode = PictureBoxSizeMode.StretchImage;
            pc_ransom.TabIndex = 2;
            pc_ransom.TabStop = false;
            // 
            // pc_attack
            // 
            pc_attack.BackColor = Color.Transparent;
            pc_attack.Image = Properties.Resources.ransom_attack;
            pc_attack.Location = new Point(273, -543);
            pc_attack.Name = "pc_attack";
            pc_attack.Size = new Size(900, 900);
            pc_attack.SizeMode = PictureBoxSizeMode.Zoom;
            pc_attack.TabIndex = 3;
            pc_attack.TabStop = false;
            // 
            // txt_download
            // 
            txt_download.Anchor = AnchorStyles.None;
            txt_download.AutoSize = true;
            txt_download.BackColor = Color.Transparent;
            txt_download.Font = new Font("Consolas", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txt_download.ForeColor = SystemColors.ButtonHighlight;
            txt_download.Location = new Point(206, 501);
            txt_download.Name = "txt_download";
            txt_download.Size = new Size(388, 56);
            txt_download.TabIndex = 5;
            txt_download.Text = "DOWNLOADING...";
            txt_download.UseMnemonic = false;
            // 
            // pc_stopsign
            // 
            pc_stopsign.BackColor = Color.Transparent;
            pc_stopsign.Dock = DockStyle.Fill;
            pc_stopsign.Image = Properties.Resources.stop_sign;
            pc_stopsign.Location = new Point(0, 0);
            pc_stopsign.Name = "pc_stopsign";
            pc_stopsign.Size = new Size(1940, 1100);
            pc_stopsign.SizeMode = PictureBoxSizeMode.CenterImage;
            pc_stopsign.TabIndex = 1;
            pc_stopsign.TabStop = false;
            // 
            // pb_download
            // 
            pb_download.Location = new Point(173, 580);
            pb_download.Name = "pb_download";
            pb_download.RightToLeft = RightToLeft.No;
            pb_download.Size = new Size(473, 42);
            pb_download.Step = 0;
            pb_download.TabIndex = 8;
            // 
            // Overlay
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(0, 0, 192);
            CausesValidation = false;
            ClientSize = new Size(1940, 1100);
            ControlBox = false;
            Controls.Add(pb_download);
            Controls.Add(txt_download);
            Controls.Add(pc_attack);
            Controls.Add(pc_ransom);
            Controls.Add(pc_stopsign);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            Name = "Overlay";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.Manual;
            Text = "Rans0m";
            TopMost = true;
            TransparencyKey = Color.FromArgb(0, 0, 192);
            Load += Overlay_Load;
            ((System.ComponentModel.ISupportInitialize)pc_ransom).EndInit();
            ((System.ComponentModel.ISupportInitialize)pc_attack).EndInit();
            ((System.ComponentModel.ISupportInitialize)pc_stopsign).EndInit();
            FormClosing += Overlay_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private PictureBox pc_ransom;
        private PictureBox pc_attack;
        private Label txt_download;
        private PictureBox pc_stopsign;
        private ProgressBar pb_download;
    }
}
