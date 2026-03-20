namespace GTYApp.Forms
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtUsername;
        private PictureBox picProfile;
        private Button btnUpload;
        private Button btnSave;
        private Button btnToggleTheme;
        private Button btnDeleteAccount;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components is not null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtUsername = new TextBox();
            this.picProfile = new PictureBox();
            this.btnUpload = new Button();
            this.btnSave = new Button();
            this.btnToggleTheme = new Button();
            this.btnDeleteAccount = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.picProfile)).BeginInit();
            this.SuspendLayout();
            // 
            this.txtUsername.Location = new Point(24, 24);
            this.txtUsername.Size = new Size(260, 27);
            this.txtUsername.PlaceholderText = "Display name";
            // 
            this.picProfile.Location = new Point(24, 70);
            this.picProfile.Size = new Size(120, 120);
            this.picProfile.SizeMode = PictureBoxSizeMode.Zoom;
            // 
            this.btnUpload.Location = new Point(160, 70);
            this.btnUpload.Size = new Size(124, 34);
            this.btnUpload.Text = "Upload photo";
            this.btnUpload.Click += BtnUpload_Click;
            // 
            this.btnSave.Location = new Point(24, 208);
            this.btnSave.Size = new Size(120, 34);
            this.btnSave.Text = "Save";
            this.btnSave.Click += BtnSave_Click;
            // 
            this.btnToggleTheme.Location = new Point(160, 208);
            this.btnToggleTheme.Size = new Size(124, 34);
            this.btnToggleTheme.Text = "Toggle Light/Dark";
            this.btnToggleTheme.Click += BtnToggleTheme_Click;
            // 
            this.btnDeleteAccount.Location = new Point(24, 252);
            this.btnDeleteAccount.Size = new Size(260, 34);
            this.btnDeleteAccount.Text = "Delete my account";
            this.btnDeleteAccount.Click += BtnDeleteAccount_Click;
            // 
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(312, 310);
            this.Controls.Add(this.btnDeleteAccount);
            this.Controls.Add(this.btnToggleTheme);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.picProfile);
            this.Controls.Add(this.txtUsername);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += SettingsForm_Load;
            ((System.ComponentModel.ISupportInitialize)(this.picProfile)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}