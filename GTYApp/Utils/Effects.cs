using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using GTYApp.Services;

namespace GTYApp.Utils
{
    public static partial class Effects
    {
        // Windows 11 Mica / Acrylic
        private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;
        private const int DWMSBT_MAINWINDOW = 2; // Mica
        private const int DWMSBT_TRANSIENTWINDOW = 3; // Acrylic-like

        [LibraryImport("dwmapi.dll")]
        private static partial int DwmSetWindowAttribute(
            IntPtr hwnd,
            int dwAttribute,
            ref int pvAttribute,
            int cbAttribute);

        [LibraryImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static partial IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse);

        public static void TryApplyMica(Form form)
        {
            if (form is null)
                return;

            try
            {
                if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000))
                {
                    form.BackColor = Color.Transparent;
                    form.FormBorderStyle = FormBorderStyle.Sizable;

                    var hwnd = form.Handle;
                    var val = DWMSBT_MAINWINDOW;

                    _ = DwmSetWindowAttribute(
                        hwnd,
                        DWMWA_SYSTEMBACKDROP_TYPE,
                        ref val,
                        System.Runtime.InteropServices.Marshal.SizeOf<int>());
                }
            }
            catch
            {
                // ถ้าทำไม่สำเร็จไม่ต้องทำอะไรต่อ
            }
        }

        public static void EnableDoubleBuffer(Control control)
        {
            if (control is null)
                return;

            try
            {
                var type = control.GetType();
                var prop = type.GetProperty(
                    "DoubleBuffered",
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic);

                prop?.SetValue(control, true, null);
            }
            catch
            {
                // ignore
            }

            foreach (Control child in control.Controls)
                EnableDoubleBuffer(child);
        }

        public static void StyleSoftButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 255, 255, 255);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(60, 0, 0, 0);
            btn.BackColor = Color.Transparent;
        }

        public static void MakeCircularPictureBox(PictureBox pb)
        {
            if (pb == null || pb.Width <= 0 || pb.Height <= 0)
                return;

            int diameter = Math.Min(pb.Width, pb.Height);

            using var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(
                (pb.Width - diameter) / 2,
                (pb.Height - diameter) / 2,
                diameter,
                diameter);

            pb.Region = new Region(path);
        }

        // ---- แก้เฉพาะ method นี้ให้ gradient เนียน + ไม่บัคเวลา resize ----
        public static void PaintNavBarGradient(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel panel)
                return;

            var rect = panel.ClientRectangle;
            if (rect.Width < 2 || rect.Height < 2)
                return;

            // palette เดิม แต่ปรับให้สีเนียนขึ้นนิดหน่อย
            var light1 = Color.FromArgb(72, 140, 255);
            var light2 = Color.FromArgb(170, 110, 255);
            var dark1 = Color.FromArgb(22, 40, 70);
            var dark2 = Color.FromArgb(18, 18, 32);

            var c1 = ThemeService.Current == ThemeService.Theme.Light ? light1 : dark1;
            var c2 = ThemeService.Current == ThemeService.Theme.Light ? light2 : dark2;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            using var lg = new System.Drawing.Drawing2D.LinearGradientBrush(
                rect,
                c1,
                c2,
                0f);

            e.Graphics.FillRectangle(lg, rect);
        }

        // ---- Renderer ใหม่สำหรับ dropdown (Settings / Logout) ----
        public sealed class ModernToolStripRenderer : ToolStripProfessionalRenderer
        {
            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                // ไม่วาดกรอบแข็ง ๆ เพื่อให้ dropdown ดูสะอาด
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                var rect = new Rectangle(Point.Empty, e.Item.Bounds.Size);
                bool isSelected = e.Item.Selected;

                bool isDark = ThemeService.Current == ThemeService.Theme.Dark;

                Color bg;
                Color fg;

                if (isDark)
                {
                    bg = isSelected
                        ? Color.FromArgb(70, 90, 130)
                        : Color.FromArgb(25, 25, 40);
                    fg = Color.White;
                }
                else
                {
                    bg = isSelected
                        ? Color.FromArgb(220, 235, 255)
                        : Color.White;
                    fg = Color.FromArgb(20, 20, 40);
                }

                using var b = new SolidBrush(bg);
                e.Graphics.FillRectangle(b, rect);
                e.Item.ForeColor = fg;
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                var midY = e.Item.Bounds.Height / 2;
                using var p = new Pen(Color.FromArgb(80, 80, 100));
                e.Graphics.DrawLine(p, 8, midY, e.Item.Bounds.Width - 8, midY);
            }
        }
    }
}
