namespace GTYApp.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;  // ไม่ใช้ ?

        private Panel nav;
        private Label lblBrand;
        private Button btnAbout;
        private Button btnContact;
        private PictureBox picUser;
        private Label lblUsername;
        private Button btnFull;
        private ContextMenuStrip menuUser;
        private ToolStripMenuItem miSettings;
        private ToolStripMenuItem miLogout;
        private Panel page;
        private System.Windows.Forms.Timer anim;   // ระบุเต็ม ๆ ว่าเป็น Forms.Timer

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.nav = new System.Windows.Forms.Panel();
            this.lblBrand = new System.Windows.Forms.Label();
            this.btnAbout = new System.Windows.Forms.Button();
            this.btnContact = new System.Windows.Forms.Button();
            this.picUser = new System.Windows.Forms.PictureBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.btnFull = new System.Windows.Forms.Button();
            this.menuUser = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.miLogout = new System.Windows.Forms.ToolStripMenuItem();
            this.page = new System.Windows.Forms.Panel();
            this.anim = new System.Windows.Forms.Timer(this.components);
            this.nav.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUser)).BeginInit();
            this.menuUser.SuspendLayout();
            this.SuspendLayout();
            // 
            // nav
            // 
            this.nav.Controls.Add(this.lblBrand);
            this.nav.Controls.Add(this.btnAbout);
            this.nav.Controls.Add(this.btnContact);
            this.nav.Controls.Add(this.picUser);
            this.nav.Controls.Add(this.lblUsername);
            this.nav.Controls.Add(this.btnFull);
            this.nav.Dock = System.Windows.Forms.DockStyle.Top;
            this.nav.Location = new System.Drawing.Point(0, 0);
            this.nav.Name = "nav";
            this.nav.Size = new System.Drawing.Size(1100, 56);
            this.nav.TabIndex = 0;
            this.nav.Paint += new System.Windows.Forms.PaintEventHandler(GTYApp.Utils.Effects.PaintNavBarGradient);
            // 
            // lblBrand
            // 
            this.lblBrand.AutoSize = true;
            this.lblBrand.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblBrand.ForeColor = System.Drawing.Color.White;
            this.lblBrand.Location = new System.Drawing.Point(16, 12);
            this.lblBrand.Name = "lblBrand";
            this.lblBrand.Size = new System.Drawing.Size(106, 30);
            this.lblBrand.TabIndex = 0;
            this.lblBrand.Text = "TechStyle"; // default ปลอดภัยสำหรับ Designer
            // 
            // btnAbout
            // 
            this.btnAbout.Location = new System.Drawing.Point(220, 12);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(90, 32);
            this.btnAbout.TabIndex = 1;
            this.btnAbout.Text = "About";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.BtnAbout_Click);
            // 
            // btnContact
            // 
            this.btnContact.Location = new System.Drawing.Point(320, 12);
            this.btnContact.Name = "btnContact";
            this.btnContact.Size = new System.Drawing.Size(90, 32);
            this.btnContact.TabIndex = 2;
            this.btnContact.Text = "Contact";
            this.btnContact.UseVisualStyleBackColor = true;
            this.btnContact.Click += new System.EventHandler(this.BtnContact_Click);
            // 
            // picUser
            // 
            this.picUser.Location = new System.Drawing.Point(800, 12);
            this.picUser.Name = "picUser";
            this.picUser.Size = new System.Drawing.Size(32, 32);
            this.picUser.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picUser.TabIndex = 3;
            this.picUser.TabStop = false;
            this.picUser.Click += new System.EventHandler(this.PicUser_Click);
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.ForeColor = System.Drawing.Color.White;
            this.lblUsername.Location = new System.Drawing.Point(840, 20);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(67, 15);
            this.lblUsername.TabIndex = 4;
            this.lblUsername.Text = "Username";
            // 
            // btnFull
            // 
            this.btnFull.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFull.Location = new System.Drawing.Point(1000, 12);
            this.btnFull.Name = "btnFull";
            this.btnFull.Size = new System.Drawing.Size(80, 32);
            this.btnFull.TabIndex = 5;
            this.btnFull.Text = "Fullscreen";
            this.btnFull.UseVisualStyleBackColor = true;
            this.btnFull.Click += new System.EventHandler(this.BtnFull_Click);
            // 
            // menuUser
            // 
            this.menuUser.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSettings,
            this.miLogout});
            this.menuUser.Name = "menuUser";
            this.menuUser.Size = new System.Drawing.Size(122, 48);
            // 
            // miSettings
            // 
            this.miSettings.Name = "miSettings";
            this.miSettings.Size = new System.Drawing.Size(121, 22);
            this.miSettings.Text = "Settings";
            this.miSettings.Click += new System.EventHandler(this.MiSettings_Click);
            // 
            // miLogout
            // 
            this.miLogout.Name = "miLogout";
            this.miLogout.Size = new System.Drawing.Size(121, 22);
            this.miLogout.Text = "Logout";
            this.miLogout.Click += new System.EventHandler(this.MiLogout_Click);
            // 
            // page
            // 
            this.page.Dock = System.Windows.Forms.DockStyle.Fill;
            this.page.Location = new System.Drawing.Point(0, 56);
            this.page.Name = "page";
            this.page.Size = new System.Drawing.Size(1100, 644);
            this.page.TabIndex = 1;
            // 
            // anim
            // 
            this.anim.Enabled = true;
            this.anim.Interval = 16;
            this.anim.Tick += new System.EventHandler(this.Anim_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.Controls.Add(this.page);
            this.Controls.Add(this.nav);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Home • TechStyle";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.nav.ResumeLayout(false);
            this.nav.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUser)).EndInit();
            this.menuUser.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
    }
}