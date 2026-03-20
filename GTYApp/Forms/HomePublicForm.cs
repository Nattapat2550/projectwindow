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
    public partial class HomePublicForm : Form
    {
        private float _t; // เวลาอนิเมชัน

        public HomePublicForm() { InitializeComponent(); }

        private void HomePublicForm_Load(object? sender, EventArgs e)
        {
            ThemeService.Apply(this);
            Effects.TryApplyMica(this);
            Effects.EnableDoubleBuffer(this);

            // ทำปุ่มบน navbar ให้สไตล์เดียวกับ MainForm
            Effects.StyleSoftButton(btnAbout);
            Effects.StyleSoftButton(btnContact);
            Effects.StyleSoftButton(btnLogin);
            Effects.StyleSoftButton(btnFull);

            lblBrand.Cursor = Cursors.Hand;
            lblBrand.Click += LblBrand_Click;
            // ใส่ไอคอนให้ปุ่มบน navbar จากโฟลเดอร์ Resources
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

            var iconHome = TryLoadIcon("home.png");
            if (btnLogin != null && iconHome != null)
            {
                btnLogin.Image = iconHome;
                btnLogin.ImageAlign = ContentAlignment.MiddleLeft;
                btnLogin.TextImageRelation = TextImageRelation.ImageBeforeText;
            }

            // ให้ navbar วาด gradient ใหม่เสมอเมื่อมีการ resize
            nav.Resize += (_, _) => nav.Invalidate();

            RenderHero();
            anim.Start();
            RelayoutHeader();
        }
        private void LblBrand_Click(object? sender, EventArgs e)
        {
            anim.Stop();             // รีเซ็ตอนิเมชันเก่า
            _t = 0f;                 // รีเซ็ตเวลาอนิเมชัน
            content.Controls.Clear();
            RenderHero();
            anim.Start();            // เริ่ม animate ใหม่
        }
        private void RelayoutHeader()
        {
            if (nav == null || lblBrand == null || btnAbout == null || btnContact == null ||
                btnLogin == null || btnFull == null)
            {
                return;
            }

            const int marginLeft = 16;
            const int marginRight = 16;
            const int gap = 12;

            int w = ClientSize.Width;
            if (w <= 0)
                return;

            // ซ้าย: โลโก้ / ชื่อแบรนด์
            lblBrand.Left = marginLeft;
            lblBrand.Top = (nav.Height - lblBrand.Height) / 2;

            // ขวา: ปุ่ม Fullscreen + Login
            int fullWidth = btnFull.Width;
            btnFull.Top = (nav.Height - btnFull.Height) / 2;
            btnFull.Left = w - marginRight - fullWidth;

            int loginWidth = btnLogin.Width;
            btnLogin.Top = (nav.Height - btnLogin.Height) / 2;
            btnLogin.Left = btnFull.Left - gap - loginWidth;

            // กันไม่ให้ปุ่มไปทับโลโก้ / หลุดออกนอกขอบซ้าย
            int minLoginLeft = lblBrand.Right + 32;
            if (btnLogin.Left < minLoginLeft)
            {
                btnLogin.Left = minLoginLeft;

                // ถ้าจอแคบมาก ปุ่ม Fullscreen ขยับตามแต่ยังอยู่ในจอ
                btnFull.Left = btnLogin.Right + gap;
                if (btnFull.Left + fullWidth > w - marginRight)
                {
                    btnFull.Left = w - marginRight - fullWidth;
                }
            }

            // กลาง: ปุ่ม About / Contact
            int areaLeft = lblBrand.Right + 32;
            int areaRight = btnLogin.Left - 32;

            btnAbout.Top = (nav.Height - btnAbout.Height) / 2;
            btnContact.Top = btnAbout.Top;

            int buttonsWidth = btnAbout.Width + gap + btnContact.Width;

            if (areaRight > areaLeft)
            {
                int areaWidth = areaRight - areaLeft;
                int startX = areaLeft + (areaWidth - buttonsWidth) / 2;

                if (startX < areaLeft)
                    startX = areaLeft;
                if (startX + buttonsWidth > areaRight)
                    startX = areaLeft;

                btnAbout.Left = startX;
                btnContact.Left = btnAbout.Right + gap;
            }
            else
            {
                // ถ้าพื้นที่กลางไม่มีเลย ให้ปุ่มลากไปชิดโลโก้
                btnAbout.Left = lblBrand.Right + 24;
                btnContact.Left = btnAbout.Right + gap;
            }
        }

        private Label lblH = null!, lblF = null!, lblC = null!;
        private void RenderHero()
        {
            content.Controls.Clear();

            var sections = Services.ContentService.GetHomeSections();
            lblH = MakeHero(sections.TryGetValue("hero", out var h) ? h : "Welcome to TechStyle!");
            lblF = MakeSub(sections.TryGetValue("features", out var f) ? f : "• Fast • Secure • Beautiful UI");
            lblC = MakeCta(sections.TryGetValue("cta", out var c) ? c : "Get started today!");

            content.Controls.Add(lblH);
            content.Controls.Add(lblF);
            content.Controls.Add(lblC);

            LayoutHero();
        }

        private Label MakeHero(string text) => new()
        {
            AutoSize = false,
            Left = 40,
            Top = 90,
            Width = content.Width - 80,
            Height = 160,
            Font = new Font("Segoe UI Semibold", 34f, FontStyle.Bold),
            Text = text,
            TextAlign = ContentAlignment.MiddleLeft
        };

        private Label MakeSub(string text) => new()
        {
            AutoSize = false,
            Left = 40,
            Top = 220,
            Width = content.Width - 80,
            Height = 80,
            Font = new Font("Segoe UI", 16f, FontStyle.Regular),
            Text = text,
            ForeColor = Color.FromArgb(220, 220, 230),
            TextAlign = ContentAlignment.MiddleLeft
        };

        private Label MakeCta(string text) => new()
        {
            AutoSize = false,
            Left = 40,
            Top = 310,
            Width = content.Width - 80,
            Height = 60,
            Font = new Font("Segoe UI Semibold", 18f, FontStyle.Bold),
            Text = text,
            ForeColor = Color.FromArgb(120, 200, 255),
            TextAlign = ContentAlignment.MiddleLeft
        };

        private void LayoutHero()
        {
            if (lblH is null || lblF is null || lblC is null)
                return;

            lblH.Width = content.Width - 80;
            lblF.Width = content.Width - 80;
            lblC.Width = content.Width - 80;
        }

        private void Anim_Tick(object? sender, EventArgs e)
        {
            _t += anim.Interval / 1000f;

            if (lblH is null || lblF is null || lblC is null)
                return;

            // easing เล็ก ๆ ให้ hero ขยับนิด ๆ
            static float k(float x) => (float)(0.5 - 0.5 * Math.Cos(Math.Min(1, x) * Math.PI));

            var a = Math.Min(1f, _t / 0.8f);
            lblH.Left = 40 - (int)(30 * (1 - k(a)));

            var b = Math.Min(1f, Math.Max(0, (_t - 0.15f) / 0.8f));
            lblF.Left = 40 - (int)(28 * (1 - k(b)));

            var c = Math.Min(1f, Math.Max(0, (_t - 0.30f) / 0.8f));
            lblC.Left = 40 - (int)(26 * (1 - k(c)));
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            lblH?.Width = content.Width - 80;
            lblF?.Width = content.Width - 80;
        }

        private void BtnAbout_Click(object? sender, EventArgs e)
        {
            ShowContentForm(new AboutForm());
        }

        private void BtnContact_Click(object? sender, EventArgs e)
        {
            ShowContentForm(new ContactForm());
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            using var dlg = new LoginForm();
            dlg.ShowDialog(this);
        }

        private void BtnFull_Click(object? sender, EventArgs e)
        {
            ThemeService.ToggleFullscreen(this);
        }

        private void HomePublicForm_Resize(object? sender, EventArgs e)
        {
            RelayoutHeader();
            nav.Invalidate();
        }

        private const int DefaultIconSize = 24;

        // เปลี่ยนจาก Image? → Bitmap?
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

        // overload
        private static Bitmap? TryLoadIcon(string fileName)
            => TryLoadIcon(fileName, DefaultIconSize);
        private void ShowContentForm(Form child)
        {
            if (content == null)
                return;

            // ถ้ามีอนิเมชัน hero อยู่ ให้หยุดก่อน
            anim.Stop();

            foreach (Control c in content.Controls)
                c.Dispose();
            content.Controls.Clear();

            child.TopLevel = false;
            child.FormBorderStyle = FormBorderStyle.None;
            child.Dock = DockStyle.Fill;

            content.Controls.Add(child);
            child.Show();
        }
    }
}
