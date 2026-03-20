namespace GTYApp.Forms
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private CheckBox chkHidePassword;
        private CheckBox chkRemember;
        private Button btnLogin;
        private Button btnGoogle;
        private LinkLabel lnkForgot;
        private LinkLabel lnkRegister;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components is not null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtEmail = new TextBox();
            this.txtPassword = new TextBox();
            this.chkHidePassword = new CheckBox();
            this.chkRemember = new CheckBox();
            this.btnLogin = new Button();
            this.btnGoogle = new Button();
            this.lnkForgot = new LinkLabel();
            this.lnkRegister = new LinkLabel();
            this.SuspendLayout();
            // 
            // txtEmail
            // 
            this.txtEmail.PlaceholderText = "Email";
            this.txtEmail.Location = new Point(40, 40);
            this.txtEmail.Size = new Size(320, 27);
            // 
            // txtPassword
            // 
            this.txtPassword.PlaceholderText = "Password";
            this.txtPassword.Location = new Point(40, 80);
            this.txtPassword.Size = new Size(320, 27);
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // chkHidePassword
            // 
            this.chkHidePassword.Location = new Point(40, 115);
            this.chkHidePassword.Text = "Hide password";
            this.chkHidePassword.Checked = true;
            this.chkHidePassword.CheckedChanged += ChkHidePassword_CheckedChanged;
            // 
            // chkRemember
            // 
            this.chkRemember.Location = new Point(180, 115);
            this.chkRemember.Text = "Save password";
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new Point(40, 150);
            this.btnLogin.Size = new Size(150, 34);
            this.btnLogin.Text = "Login";
            this.btnLogin.Click += BtnLogin_Click;
            // 
            // btnGoogle
            // 
            this.btnGoogle.Location = new Point(210, 150);
            this.btnGoogle.Size = new Size(150, 34);
            this.btnGoogle.Text = "Login with Google";
            this.btnGoogle.Click += BtnGoogle_Click;
            // 
            // lnkForgot
            // 
            this.lnkForgot.Location = new Point(40, 195);
            this.lnkForgot.AutoSize = true;
            this.lnkForgot.Text = "Forgot password?";
            this.lnkForgot.LinkClicked += LnkForgot_LinkClicked;
            // 
            // lnkRegister
            // 
            this.lnkRegister.Location = new Point(200, 195);
            this.lnkRegister.AutoSize = true;
            this.lnkRegister.Text = "Register";
            this.lnkRegister.LinkClicked += LnkRegister_LinkClicked;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(400, 250);
            this.Controls.Add(this.lnkRegister);
            this.Controls.Add(this.lnkForgot);
            this.Controls.Add(this.btnGoogle);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.chkRemember);
            this.Controls.Add(this.chkHidePassword);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtEmail);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.Load += LoginForm_Load;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}