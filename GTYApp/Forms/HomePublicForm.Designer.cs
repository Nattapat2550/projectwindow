namespace GTYApp.Forms
{
    partial class HomePublicForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel nav;
        private Label lblBrand;
        private Button btnAbout;
        private Button btnContact;
        private Button btnLogin;
        private Button btnFull;
        private Panel content;
        private System.Windows.Forms.Timer anim;

        protected override void Dispose(bool disposing)
        { if (disposing && (components is not null)) components.Dispose(); base.Dispose(disposing); }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            nav = new Panel();
            lblBrand = new Label();
            btnAbout = new Button();
            btnContact = new Button();
            btnLogin = new Button();
            btnFull = new Button();
            content = new Panel();
            anim = new System.Windows.Forms.Timer(components);

            SuspendLayout();

            // nav
            nav.Dock = DockStyle.Top;
            nav.Height = 56;
            nav.Paint += Utils.Effects.PaintNavBarGradient;

            // lblBrand
            lblBrand.AutoSize = true;
            lblBrand.Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold);
            lblBrand.ForeColor = Color.White;
            lblBrand.Location = new Point(16, 12);
            lblBrand.Text = "TechStyle";

            // btnAbout
            btnAbout.Text = "About";
            btnAbout.Size = new Size(90, 32);
            btnAbout.Location = new Point(220, 12);
            btnAbout.Click += BtnAbout_Click;

            // btnContact
            btnContact.Text = "Contact";
            btnContact.Size = new Size(90, 32);
            btnContact.Location = new Point(318, 12);
            btnContact.Click += BtnContact_Click;

            // btnLogin
            btnLogin.Text = "Login";
            btnLogin.Size = new Size(120, 32);
            btnLogin.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLogin.Location = new Point(Width - 280, 12);
            btnLogin.Click += BtnLogin_Click;

            // btnFull
            btnFull.Text = "Fullscreen";
            btnFull.Size = new Size(60, 32);
            btnFull.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFull.Location = new Point(Width - 150, 12);
            btnFull.Click += BtnFull_Click;

            nav.Controls.Add(lblBrand);
            nav.Controls.Add(btnAbout);
            nav.Controls.Add(btnContact);
            nav.Controls.Add(btnLogin);
            nav.Controls.Add(btnFull);

            // content
            content.Dock = DockStyle.Fill;

            // anim
            anim.Interval = 16; // ~60fps
            anim.Tick += Anim_Tick;

            // Form
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1000, 640);
            Controls.Add(content);
            Controls.Add(nav);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Home • TechStyle";
            Load += HomePublicForm_Load;
            Resize += HomePublicForm_Resize;

            ResumeLayout(false);
        }
    }
}