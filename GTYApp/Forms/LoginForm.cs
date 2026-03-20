using GTYApp.Services;
using GTYApp.Utils;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace GTYApp.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm() { InitializeComponent(); }

        private void LoginForm_Load(object? sender, EventArgs e) => ThemeService.Apply(this);

        private async void BtnLogin_Click(object? sender, EventArgs e)
        {
            btnLogin.Enabled = false;
            try
            {
                var ok = await Services.AuthService.LoginWithPasswordAsync(
                    txtEmail.Text.Trim(),
                    txtPassword.Text,
                    chkRemember.Checked);

                if (!ok)
                {
                    MessageBox.Show(
                        Services.AuthService.LastError ?? "อีเมลหรือรหัสผ่านไม่ถูกต้อง",
                        "Login failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                AppContextEx.Instance?.NavigateByRole();
                DialogResult = DialogResult.OK;
                Close();
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }

        private async void BtnGoogle_Click(object? sender, EventArgs e)
        {
            btnGoogle.Enabled = false;
            try
            {
                var ok = await Services.AuthService.LoginWithGoogleLoopbackAsync();

                if (!ok)
                {
                    MessageBox.Show(
                     Services.AuthService.LastError ?? "Google Login ไม่สำเร็จ",
                     "Login failed",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Warning);
                    return;
                }

                AppContextEx.Instance?.NavigateByRole();
                DialogResult = DialogResult.OK;
                Close();
            }
            finally
            {
                btnGoogle.Enabled = true;
            }
        }

        private void ChkHidePassword_CheckedChanged(object? sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = chkHidePassword.Checked;
        }

        private void LnkForgot_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://frontendlogins.onrender.com",
                UseShellExecute = true
            });
        }

        private void LnkRegister_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://frontendlogins.onrender.com/register.html",
                UseShellExecute = true
            });
        }
    }
}