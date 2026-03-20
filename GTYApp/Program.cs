using GTYApp.Utils;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GTYApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            Application.ThreadException += (s, ex) =>
            {
                MessageBox.Show("UI Error:\n" + ex.Exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
            {
                var e = ex.ExceptionObject as Exception;
                MessageBox.Show("App Error:\n" + e?.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            // เช็ค Config
            try
            {
                AppConfig.LoadOrThrow();
                Services.PureApiClient.ConfigureFromAppSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Config Error:\n" + ex.Message, "Start-up Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // หยุดการทำงานถ้า Config พัง
            }

            // ⚠️ ปิดระบบ Auto-login ไว้ชั่วคราว เพื่อไม่ให้โปรแกรมไปค้างตอนเริ่ม 
            // สาเหตุที่ค้างเพราะการใช้ Task.Wait() ไปขวางการทำงานของ Thread หลัก 
            // แนะนำให้นำการตรวจเช็ค Token ด้านล่างนี้ ไปเรียกใช้แบบ "await" ใน Event "Load" 
            // ของหน้า Splash Screen หรือหน้าจอแรกของแอปพลิเคชันแทนครับ
            /*
            try
            {
                var token = CryptoStorage.TryRestoreRememberToken();
                if (!string.IsNullOrWhiteSpace(token))
                {
                    // ต้องหลีกเลี่ยงการใช้ .Wait() บน Thread หลัก
                    var resumeTask = Task.Run(() => Services.AuthService.TryResumeSessionAsync(token!));
                    resumeTask.Wait(TimeSpan.FromSeconds(3));
                }
            }
            catch { }
            */

            Application.Run(new AppContextEx());
        }
    }
}