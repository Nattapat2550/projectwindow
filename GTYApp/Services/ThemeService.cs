using GTYApp.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GTYApp.Services
{
    public static class ThemeService
    {
        public enum Theme
        {
            Light,
            Dark
        }

        public static Theme Current { get; private set; } = Theme.Dark;

        // พาเล็ตสีหลัก (Dark / Light)
        private static readonly Color DarkBack = Color.FromArgb(18, 18, 24);
        private static readonly Color DarkBackAlt = Color.FromArgb(28, 28, 36);
        private static readonly Color DarkBorder = Color.FromArgb(45, 45, 60);
        private static readonly Color DarkFore = Color.WhiteSmoke;

        private static readonly Color LightBack = Color.White;
        private static readonly Color LightBackAlt = Color.FromArgb(245, 245, 250);
        private static readonly Color LightBorder = Color.FromArgb(210, 210, 220);
        private static readonly Color LightFore = Color.FromArgb(32, 32, 40);

        /// <summary>
        /// Apply ธีมให้ทั้งฟอร์ม (เรียกจาก Form_Load)
        /// </summary>
        public static void Apply(Form form)
        {
            if (form == null)
                return;

            // เปิด DoubleBuffer ให้ทั้งฟอร์ม + children ลดการกระพริบ
            Effects.EnableDoubleBuffer(form);

            var bg = Current == Theme.Light ? LightBack : DarkBack;
            var fg = Current == Theme.Light ? LightFore : DarkFore;

            form.BackColor = bg;
            form.ForeColor = fg;
            form.AutoScaleMode = AutoScaleMode.Dpi;

            // ลองใส่ Mica ถ้า Windows รองรับ
            try
            {
                Effects.TryApplyMica(form);
            }
            catch
            {
                // ถ้าไม่ได้ก็ไม่เป็นไร ไม่ต้องทำอะไรต่อ
            }

            foreach (Control child in form.Controls)
            {
                ApplyControl(child, bg, fg);
            }
        }

        /// <summary>
        /// Toggle Light/Dark theme และ re-apply ให้ทุกฟอร์มที่เปิดอยู่
        /// </summary>
        public static void Toggle()
        {
            Current = Current == Theme.Light ? Theme.Dark : Theme.Light;

            foreach (Form f in Application.OpenForms)
            {
                Apply(f);
                f.Invalidate(true);
            }
        }

        /// <summary>
        /// Toggle fullscreen สำหรับฟอร์มที่ส่งเข้ามาเท่านั้น
        /// ไม่ยุ่งกับฟอร์มอื่น
        /// </summary>
        public static void ToggleFullscreen(Form form)
        {
            if (form == null)
                return;

            if (form.Tag is FullscreenState state && state.IsFullscreen)
            {
                // restore
                form.FormBorderStyle = state.BorderStyle;
                form.WindowState = state.WindowState;
                form.Tag = state.OriginalTag;
            }
            else
            {
                var fs = new FullscreenState
                {
                    IsFullscreen = true,
                    WindowState = form.WindowState,
                    BorderStyle = form.FormBorderStyle,
                    OriginalTag = form.Tag
                };

                form.Tag = fs;
                form.FormBorderStyle = FormBorderStyle.None;
                form.WindowState = FormWindowState.Maximized;
            }
        }

        // เก็บ state เดิมไว้เวลาเข้า fullscreen
        private sealed class FullscreenState
        {
            public bool IsFullscreen { get; set; }
            public FormWindowState WindowState { get; set; }
            public FormBorderStyle BorderStyle { get; set; }
            public object? OriginalTag { get; set; }
        }

        /// <summary>
        /// ไล่ apply สี/สไตล์ให้ control ตามชนิดต่าง ๆ
        /// ไม่เพิ่ม/ลบ control แค่ปรับหน้าตา
        /// ใช้ if/else แทน switch เพื่อตัดปัญหา unreachable pattern
        /// </summary>
        public static void ApplyControl(Control control, Color bg, Color fg)
        {
            if (control == null)
                return;

            // --- จัดสไตล์ตามชนิด control ---

            if (control is Panel panel)
            {
                panel.BackColor = bg;
            }
            else if (control is GroupBox group)
            {
                group.BackColor = bg;
                group.ForeColor = fg;
            }
            else if (control is TabControl tabControl)
            {
                tabControl.BackColor = bg;
                tabControl.ForeColor = fg;
                foreach (TabPage page in tabControl.TabPages)
                {
                    page.BackColor = bg;
                    page.ForeColor = fg;
                }
            }
            else if (control is TabPage tabPage)
            {
                tabPage.BackColor = bg;
                tabPage.ForeColor = fg;
            }
            else if (control is Button button)
            {
                // ใช้ helper เดิมให้ปุ่มดูนุ่ม ๆ
                Effects.StyleSoftButton(button);
                button.BackColor = Current == Theme.Light ? LightBackAlt : DarkBackAlt;
                button.ForeColor = fg;
            }
            else if (control is LinkLabel link)
            {
                link.BackColor = Color.Transparent;
                link.LinkColor = Color.DeepSkyBlue;
                link.ActiveLinkColor = Color.DodgerBlue;
                link.VisitedLinkColor = Color.MediumPurple;
            }
            else if (control is Label label)
            {
                label.BackColor = Color.Transparent;
                label.ForeColor = fg;
            }
            else if (control is TextBox textBox)
            {
                textBox.BackColor = Current == Theme.Light ? Color.White : DarkBackAlt;
                textBox.ForeColor = fg;
                textBox.BorderStyle = BorderStyle.FixedSingle;
            }
            else if (control is DataGridView grid)
            {
                grid.BackgroundColor = bg;
                grid.GridColor = Current == Theme.Light ? LightBorder : DarkBorder;
                grid.EnableHeadersVisualStyles = false;

                grid.ColumnHeadersDefaultCellStyle.BackColor =
                    Current == Theme.Light ? LightBackAlt : DarkBackAlt;
                grid.ColumnHeadersDefaultCellStyle.ForeColor = fg;

                grid.DefaultCellStyle.BackColor = bg;
                grid.DefaultCellStyle.ForeColor = fg;
            }
            else
            {
                // default case: ใช้สีพื้น/ตัวอักษรตาม theme
                control.BackColor = bg;

                // บาง control ต้องโปร่งใส (เช่นวางบน panel gradient)
                if (control is not Panel &&
                    control is not GroupBox &&
                    control is not TabControl &&
                    control is not TabPage &&
                    control is not DataGridView &&
                    control is not Button)
                {
                    control.BackColor = Color.Transparent;
                }
            }

            // common foreground color
            control.ForeColor = fg;

            // ไล่ลงไป children ต่อ
            foreach (Control child in control.Controls)
            {
                ApplyControl(child, bg, fg);
            }
        }
    }
}
