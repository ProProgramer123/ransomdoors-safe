namespace rans0m
{
    partial class ThankYou
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
            pc_oksign = new PictureBox();
            txt_ty = new Label();
            ((System.ComponentModel.ISupportInitialize)pc_oksign).BeginInit();
            SuspendLayout();
            // 
            // pc_oksign
            // 
            pc_oksign.Image = Properties.Resources.ok_sign;
            pc_oksign.Location = new Point(169, 83);
            pc_oksign.Name = "pc_oksign";
            pc_oksign.Size = new Size(210, 210);
            pc_oksign.SizeMode = PictureBoxSizeMode.StretchImage;
            pc_oksign.TabIndex = 0;
            pc_oksign.TabStop = false;
            // 
            // txt_ty
            // 
            txt_ty.AutoSize = true;
            txt_ty.Font = new Font("Consolas", 48F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            txt_ty.ForeColor = Color.White;
            txt_ty.Location = new Point(79, 5);
            txt_ty.Name = "txt_ty";
            txt_ty.Size = new Size(382, 75);
            txt_ty.TabIndex = 1;
            txt_ty.Text = "THANK YOU!";
            // 
            // ThankYou
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(0, 192, 0);
            ClientSize = new Size(544, 315);
            ControlBox = false;
            Controls.Add(txt_ty);
            Controls.Add(pc_oksign);
            MaximizeBox = false;
            MaximumSize = new Size(560, 354);
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            MinimumSize = new Size(560, 354);
            Name = "ThankYou";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            Text = "RANS0M";
            TopMost = true;
            Load += ThankYou_Load;
            ((System.ComponentModel.ISupportInitialize)pc_oksign).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pc_oksign;
        private Label txt_ty;
    }
}