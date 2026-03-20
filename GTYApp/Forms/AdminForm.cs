using GTYApp.Models;
using GTYApp.Services;
using GTYApp.Utils;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GTYApp.Forms
{
    public partial class AdminForm : Form
    {
        private List<User> _users = new List<User>();

        public AdminForm()
        {
            InitializeComponent();
        }

        private void AdminForm_Load(object? sender, EventArgs e)
        {
            // ตรวจสิทธิ์ admin
            if (!Session.IsAuthenticated ||
                Session.CurrentUser is null ||
                !string.Equals(Session.CurrentUser.Role, "admin", StringComparison.OrdinalIgnoreCase))
            {
                // ⭐ เปลี่ยนจากการพากลับหน้า Home ปกติ ไปยังหน้า Login แทน
                AppContextEx.Instance!.NavigateToLogin();
                return;
            }

            ThemeService.Apply(this);

            var map = ContentService.GetHomeSections();

            if (map.TryGetValue("hero", out var hero))
                txtHero.Text = hero;
            else
                txtHero.Clear();

            if (map.TryGetValue("features", out var features))
                txtFeatures.Text = features;
            else
                txtFeatures.Clear();

            if (map.TryGetValue("cta", out var cta))
                txtCta.Text = cta;
            else
                txtCta.Clear();

            _users = UserService.GetAllUsers();
            gridUsers.AutoGenerateColumns = true;
            gridUsers.DataSource = _users;
        }

        private void BtnSaveHome_Click(object? sender, EventArgs e)
        {
            ContentService.UpsertSection("hero", txtHero.Text);
            ContentService.UpsertSection("features", txtFeatures.Text);
            ContentService.UpsertSection("cta", txtCta.Text);

            MessageBox.Show("Homepage saved.", "Saved",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSaveUsers_Click(object? sender, EventArgs e)
        {
            gridUsers.EndEdit();

            foreach (var u in _users)
            {
                UserService.UpdateUser(u);
            }

            MessageBox.Show("Users saved.", "Saved",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}