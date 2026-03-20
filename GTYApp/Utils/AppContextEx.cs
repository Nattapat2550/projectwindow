using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using GTYApp.Forms;

namespace GTYApp.Utils
{
    public class AppContextEx : ApplicationContext
    {
        public static AppContextEx? Instance { get; private set; }

        public AppContextEx()
        {
            Instance = this;
            NavigateToLogin();
        }

        public void NavigateByRole()
        {
            if (!Session.IsAuthenticated || Session.CurrentUser == null)
            {
                NavigateToLogin();
                return;
            }

            if (Session.IsAdmin)
            {
                SwitchForm(new AdminForm());
            }
            else
            {
                NavigateToMain();
            }
        }

        public void NavigateToLogin() => SwitchForm(new LoginForm());

        public void NavigateToMain() => SwitchForm(new MainForm());

        public void Navigate(Form newForm)
        {
            SwitchForm(newForm);
        }

        private void SwitchForm(Form newForm)
        {
            var oldForm = MainForm;

            MainForm = newForm;
            MainForm.FormClosed += OnMainFormClosed;
            MainForm.Show();

            if (oldForm != null)
            {
                oldForm.FormClosed -= OnMainFormClosed;
                oldForm.Hide(); // ซ่อนฟอร์มเก่าทันทีเพื่อให้เปลี่ยนหน้าลื่นไหลไม่ค้าง

                // ⭐ ป้องกันอาการจอค้าง/Deadlock ตอนเปลี่ยนหน้า หรือ Logout 
                // โดยการใช้ Application.Idle เพื่อรอให้คิวงานของ UI ว่างจริงๆ ก่อนแล้วค่อยทำการปิดฟอร์มเดิม
                EventHandler? idleHandler = null;
                idleHandler = (s, e) =>
                {
                    Application.Idle -= idleHandler;
                    if (!oldForm.IsDisposed)
                    {
                        oldForm.Close();
                        oldForm.Dispose();
                    }
                };
                Application.Idle += idleHandler;
            }
        }

        private void OnMainFormClosed(object? sender, FormClosedEventArgs e)
        {
            ExitThread();
            Environment.Exit(0);
        }
    }
}