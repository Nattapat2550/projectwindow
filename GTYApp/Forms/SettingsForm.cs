using GTYApp.Services;
using GTYApp.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GTYApp.Forms
{
    public partial class SettingsForm : Form
    {
        private Image? _img;

        public SettingsForm() { InitializeComponent(); }

        private void SettingsForm_Load(object? sender, EventArgs e)
        {
            ThemeService.Apply(this);
            var u = Session.CurrentUser!;
            txtUsername.Text = u.Username ?? u.Email;

            if (!string.IsNullOrWhiteSpace(u.ProfilePictureUrl) && u.ProfilePictureUrl!.StartsWith("data:image"))
                _img = ImageCodec.FromDataUrl(u.ProfilePictureUrl!);
            else
                _img = Image.FromFile(Path.Combine(AppContext.BaseDirectory, "Resources", "user.png"));

            picProfile.Image = _img;
        }

        private void BtnUpload_Click(object? sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog { Filter = "Images|*.png;*.jpg;*.jpeg" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _img = Image.FromFile(ofd.FileName);
                picProfile.Image = _img;
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            var u = Session.CurrentUser!;
            if (!string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                if (!UserService.UpdateUsername(u.Id, txtUsername.Text.Trim()))
                {
                    MessageBox.Show("บันทึกชื่อไม่สำเร็จ"); return;
                }
                u.Username = txtUsername.Text.Trim();
            }

            if (_img is not null)
            {
                var dataUrl = ImageCodec.ToDataUrl(_img);
                if (!UserService.UpdateProfilePicture(u.Id, dataUrl))
                {
                    MessageBox.Show("บันทึกรูปไม่สำเร็จ"); return;
                }
                u.ProfilePictureUrl = dataUrl;
            }

            MessageBox.Show("Saved.");
            this.Close();
        }

        private void BtnToggleTheme_Click(object? sender, EventArgs e)
        {
            ThemeService.Toggle();
            foreach (Form f in Application.OpenForms)
            {
                ThemeService.Apply(f);
                f.Invalidate(true); // บังคับให้ redraw ใหม่
            }
        }

        private void BtnDeleteAccount_Click(object? sender, EventArgs e)
        {
            var u = Session.CurrentUser;
            if (u is null) { Close(); return; }
            if (MessageBox.Show("ยืนยันการลบบัญชี? (ถาวร)", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (UserService.DeleteAccount(u.Id))
                {
                    Session.SignOut();
                    MessageBox.Show("Account deleted.");
                    this.Close();
                    foreach (Form f in Application.OpenForms) if (f != this) f.Hide();
                    new HomePublicForm().Show();
                }
                else
                {
                    MessageBox.Show("ลบไม่สำเร็จ");
                }
            }
        }
    }
}
