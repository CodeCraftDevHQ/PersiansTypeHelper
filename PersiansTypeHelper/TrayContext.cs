using System;
using System.Drawing;
using System.Windows.Forms;

namespace PersianTypeHelper
{
    public class TrayContext : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private HotkeyWindow hotkeyWindow;
        private const int HOTKEY_ID = 9000;

        public TrayContext()
        {
            trayIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application, 
                Visible = true,
                Text = "Persian Type Helper (Ctrl+Shift+P)"
            };

            var menu = new ContextMenuStrip();
            menu.Items.Add("خروج", null, (s, e) => ExitApp());
            trayIcon.ContextMenuStrip = menu;

            hotkeyWindow = new HotkeyWindow();
            hotkeyWindow.HotkeyPressed += OnHotkeyPressed;

            bool ok = NativeMethods.RegisterHotKey(
                hotkeyWindow.Handle,
                HOTKEY_ID,
                NativeMethods.MOD_CONTROL | NativeMethods.MOD_SHIFT,
                (uint)Keys.P);

            if (!ok)
            {
                MessageBox.Show("ثبت هات‌کی ناموفق بود (شاید توسط برنامه‌ی دیگه‌ای گرفته شده).");
            }
        }

        private void OnHotkeyPressed()
        {
            IntPtr active = NativeMethods.GetForegroundWindow();
            var form = new InputForm(active);
            form.Show();
        }

        private void ExitApp()
        {
            NativeMethods.UnregisterHotKey(hotkeyWindow.Handle, HOTKEY_ID);
            trayIcon.Visible = false;
            Application.Exit();
        }
    }
}