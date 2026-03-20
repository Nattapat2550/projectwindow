using GTYApp.Services;
using GTYApp.Utils;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GTYApp.Forms
{
    public partial class MainForm : Form
    {
        private static readonly HttpClient _httpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        private string? _loadedAvatarSrc;

        public MainForm()
        {
            InitializeComponent();

            if (menuUser != null)
            {
                menuUser.Renderer = new Effects.ModernToolStripRenderer();
                menuUser.ShowImageMargin = true;
                menuUser.ShowCheckMargin = false;
                menuUser.Padding = new Padding(4);
                menuUser.Font = new Font("Segoe UI", 10f, FontStyle.Regular);
            }

            var iconSettings = TryLoadIcon("setting.png");
            if (miSettings != null)
            {
                miSettings.Text = "Settings";
                if (iconSettings != null) miSettings.Image = iconSettings;
            }

            var iconLogout = TryLoadIcon("logout.png");
            if (miLogout != null)
            {
                miLogout.Text = "Logout";
                if (iconLogout != null) miLogout.Image = iconLogout;
            }

            var iconAbout = TryLoadIcon("about.png");
            if (btnAbout != null && iconAbout != null)
            {
                btnAbout.Image = iconAbout;
                btnAbout.ImageAlign = ContentAlignment.MiddleLeft;
                btnAbout.TextImageRelation = TextImageRelation.ImageBeforeText;
            }

            var iconContact = TryLoadIcon("contact.png");
            if (btnContact != null && iconContact != null)
            {
                btnContact.Image = iconContact;
                btnContact.ImageAlign = ContentAlignment.MiddleLeft;
                btnContact.TextImageRelation = TextImageRelation.ImageBeforeText;
            }
        }

        private async void MainForm_Load(object? sender, EventArgs e)
        {
            ThemeService.Apply(this);
            Effects.TryApplyMica(this);
            Effects.EnableDoubleBuffer(this);

            Effects.StyleSoftButton(btnAbout);
            Effects.StyleSoftButton(btnContact);
            Effects.StyleSoftButton(btnFull);

            lblBrand.Cursor = Cursors.Hand;
            lblBrand.Click += LblBrand_Click;

            nav.Resize += (_, _) => nav.Invalidate();

            if (!Session.IsAuthenticated || Session.CurrentUser is null)
            {
                AppContextEx.Instance!.NavigateToLogin();
                return;
            }

            var brand = AppConfig.GetString("Ui", "BrandLeft");
            if (!string.IsNullOrWhiteSpace(brand))
            {
                lblBrand.Text = brand!;
            }

            await RefreshHeaderAsync();

            picUser.Resize += (_, _) => MakeUserPictureCircular();
            MakeUserPictureCircular();

            RelayoutHeader();
        }

        private void LblBrand_Click(object? sender, EventArgs e) => ShowHome();

        private void ShowHome()
        {
            if (page == null) return;
            // แก้บัค Collection was modified
            while (page.Controls.Count > 0)
            {
                var c = page.Controls[0];
                page.Controls.RemoveAt(0);
                c.Dispose();
            }
        }

        private async Task RefreshHeaderAsync()
        {
            if (!Session.IsAuthenticated || Session.CurrentUser is null)
            {
                lblUsername.Text = "";
                SafeSetPicture(null);
                _loadedAvatarSrc = null;
                return;
            }

            var user = Session.CurrentUser;
            lblUsername.Text = !string.IsNullOrWhiteSpace(user!.Username) ? user.Username : user.Email;

            picUser.SizeMode = PictureBoxSizeMode.Zoom;
            string defaultPath = Path.Combine(AppContext.BaseDirectory, "Resources", "user.png");

            try
            {
                var src = user.ProfilePictureUrl;

                if (src == _loadedAvatarSrc) return;

                if (string.IsNullOrWhiteSpace(src))
                {
                    _loadedAvatarSrc = null;
                    SafeSetPictureFromFile(defaultPath);
                    return;
                }

                src = src.Trim();

                if (src.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var img = ImageCodec.FromDataUrl(src);
                        SafeSetPicture(img);
                        _loadedAvatarSrc = src;
                        return;
                    }
                    catch
                    {
                        SafeSetPictureFromFile(defaultPath);
                        return;
                    }
                }

                if (src.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    src.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    var success = await TryLoadImageFromUrlAsync(src);
                    if (success)
                    {
                        _loadedAvatarSrc = src;
                        return;
                    }
                    SafeSetPictureFromFile(defaultPath);
                    return;
                }

                if (File.Exists(src))
                {
                    SafeSetPictureFromFile(src);
                    _loadedAvatarSrc = src;
                    return;
                }

                SafeSetPictureFromFile(defaultPath);
            }
            catch
            {
                SafeSetPictureFromFile(defaultPath);
            }
        }

        private void SafeSetPictureFromFile(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    SafeSetPicture(null);
                    return;
                }

                // แก้ไข Memory Leak + GDI Error โดยการโคลน Bitmap
                using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var ms = new MemoryStream();
                fs.CopyTo(ms);
                ms.Position = 0;

                using var tempImg = Image.FromStream(ms);
                var img = new Bitmap(tempImg); // คลายล็อคไฟล์ทันที

                SafeSetPicture(img);
            }
            catch
            {
                SafeSetPicture(null);
            }
        }

        private void SafeSetPicture(Image? image)
        {
            try
            {
                var old = picUser.Image;
                picUser.Image = null;
                old?.Dispose();

                picUser.Image = image;
            }
            catch { }
        }

        private async Task<bool> TryLoadImageFromUrlAsync(string url)
        {
            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Get, url);
                using var resp = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

                if (!resp.IsSuccessStatusCode) return false;

                using var ms = new MemoryStream();
                using (var stream = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    await stream.CopyToAsync(ms).ConfigureAwait(false);
                }
                ms.Position = 0;

                Image img;
                try
                {
                    using var tempImg = Image.FromStream(ms);
                    img = new Bitmap(tempImg); // ป้องกัน Memory Leak
                }
                catch
                {
                    return false;
                }

                if (picUser.InvokeRequired)
                {
                    picUser.Invoke(new Action(() => SafeSetPicture(img)));
                }
                else
                {
                    SafeSetPicture(img);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void PicUser_Click(object? sender, EventArgs e)
        {
            if (menuUser is null) return;
            var p = picUser.PointToScreen(new Point(0, picUser.Height));
            menuUser.Show(p);
        }

        private void MiSettings_Click(object? sender, EventArgs e)
        {
            using (var dlg = new SettingsForm())
            {
                dlg.ShowDialog(this);
            }
            _loadedAvatarSrc = null;
            _ = RefreshHeaderAsync();
        }

        private async void MiLogout_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show(
                "คุณต้องการออกจากระบบหรือไม่?",
                "Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                await AuthService.LogoutAsync();
                AppContextEx.Instance!.NavigateToLogin();
            }
        }

        private void BtnAbout_Click(object? sender, EventArgs e) => ShowPage(new AboutForm());

        private void BtnContact_Click(object? sender, EventArgs e) => ShowPage(new ContactForm());

        private void BtnFull_Click(object? sender, EventArgs e) => ThemeService.ToggleFullscreen(this);

        private void Anim_Tick(object? sender, EventArgs e)
        {
            _ = RefreshHeaderAsync();
        }

        private void MainForm_Resize(object? sender, EventArgs e)
        {
            RelayoutHeader();
            nav.Invalidate();
        }

        private void RelayoutHeader()
        {
            if (nav == null || lblBrand == null || btnAbout == null || btnContact == null ||
                picUser == null || lblUsername == null || btnFull == null)
                return;

            const int marginLeft = 16;
            const int marginRight = 16;
            const int gap = 12;
            int w = nav.Width;

            if (w <= 0) return;

            lblBrand.Left = marginLeft;
            lblBrand.Top = (nav.Height - lblBrand.Height) / 2;

            btnFull.Top = (nav.Height - btnFull.Height) / 2;
            btnFull.Left = w - marginRight - btnFull.Width;

            bool showUserName = w >= 820;
            lblUsername.Visible = showUserName;

            if (showUserName)
            {
                lblUsername.AutoSize = true;
                lblUsername.Top = (nav.Height - lblUsername.Height) / 2;
                int nameWidth = lblUsername.PreferredWidth;
                lblUsername.Left = btnFull.Left - gap - nameWidth;

                picUser.Top = (nav.Height - picUser.Height) / 2;
                picUser.Left = lblUsername.Left - gap - picUser.Width;
            }
            else
            {
                picUser.Top = (nav.Height - picUser.Height) / 2;
                picUser.Left = btnFull.Left - gap - picUser.Width;
            }

            int minRightStart = lblBrand.Right + 32;
            if (picUser.Left < minRightStart) picUser.Left = minRightStart;

            int areaLeft = lblBrand.Right + 32;
            int areaRight = picUser.Left - 32;

            btnAbout.Top = (nav.Height - btnAbout.Height) / 2;
            btnContact.Top = btnAbout.Top;

            int buttonsWidth = btnAbout.Width + gap + btnContact.Width;

            if (areaRight > areaLeft)
            {
                int areaWidth = areaRight - areaLeft;
                int startX = areaLeft + (areaWidth - buttonsWidth) / 2;

                if (startX < areaLeft) startX = areaLeft;
                if (startX + buttonsWidth > areaRight) startX = areaLeft;

                btnAbout.Left = startX;
                btnContact.Left = btnAbout.Right + gap;
            }
            else
            {
                btnAbout.Left = lblBrand.Right + 24;
                btnContact.Left = btnAbout.Right + gap;
            }
        }

        private const int DefaultIconSize = 24;

        private static Bitmap? TryLoadIcon(string fileName, int size)
        {
            try
            {
                var path = Path.Combine(AppContext.BaseDirectory, "Resources", fileName);
                if (!File.Exists(path)) return null;

                using var src = Image.FromFile(path);

                var bmp = new Bitmap(size, size);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    g.Clear(Color.Transparent);
                    var dest = new Rectangle(0, 0, size, size);
                    g.DrawImage(src, dest);
                }
                return bmp;
            }
            catch
            {
                return null;
            }
        }

        private static Bitmap? TryLoadIcon(string fileName) => TryLoadIcon(fileName, DefaultIconSize);

        private void MakeUserPictureCircular()
        {
            if (picUser.Width <= 0 || picUser.Height <= 0) return;

            int diameter = Math.Min(picUser.Width, picUser.Height);
            using var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(
                (picUser.Width - diameter) / 2,
                (picUser.Height - diameter) / 2,
                diameter,
                diameter);

            var oldRegion = picUser.Region;
            picUser.Region = new Region(path);
            oldRegion?.Dispose();
        }

        private void ShowPage(Form child)
        {
            if (page == null) return;

            // แก้บัค Collection was modified เช่นเดียวกัน
            while (page.Controls.Count > 0)
            {
                var c = page.Controls[0];
                page.Controls.RemoveAt(0);
                c.Dispose();
            }

            child.TopLevel = false;
            child.FormBorderStyle = FormBorderStyle.None;
            child.Dock = DockStyle.Fill;

            page.Controls.Add(child);
            child.Show();
        }
    }
}