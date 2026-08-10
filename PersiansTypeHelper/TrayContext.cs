using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PersianTypeHelper
{
    public class TrayContext : ApplicationContext
    {
        private readonly NotifyIcon trayIcon;
        private readonly HotkeyWindow hotkeyWindow;
        private readonly ToolStripMenuItem hotkeyMenuItem;
        private const int HOTKEY_ID = 9000;

        private HotkeySettings settings;

        public TrayContext()
        {
            settings = SettingsManager.Load();

            hotkeyMenuItem = new ToolStripMenuItem();
            hotkeyMenuItem.Click += (s, e) => ChangeHotkey();

            var menu = new ContextMenuStrip();
            menu.Items.Add(hotkeyMenuItem);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("خروج", null, (s, e) => ExitApp());

            trayIcon = new NotifyIcon
            {
                Icon = LoadAppIcon(),
                Visible = true,
                ContextMenuStrip = menu
            };

            hotkeyWindow = new HotkeyWindow();
            hotkeyWindow.HotkeyPressed += OnHotkeyPressed;

            RegisterCurrentHotkey(showErrorIfFailed: true);
            UpdateTrayTextAndMenu();

            trayIcon.ShowBalloonTip(
                3000,
                "Persian Type Helper فعال شد",
                $"برای تایپ فارسی کلید {FormatHotkey()} رو بزن.",
                ToolTipIcon.Info);
        }

        private static Icon LoadAppIcon()
        {
            try
            {
                
                var extracted = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                if (extracted != null)
                    return extracted;
            }
            catch
            {
                
            }

            try
            {
                string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");
                if (File.Exists(iconPath))
                    return new Icon(iconPath);
            }
            catch
            {
                
            }

            return SystemIcons.Application;
        }

        private void RegisterCurrentHotkey(bool showErrorIfFailed)
        {
            bool ok = NativeMethods.RegisterHotKey(
                hotkeyWindow.Handle,
                HOTKEY_ID,
                settings.Modifiers,
                settings.Key);

            if (!ok && showErrorIfFailed)
            {
                MessageBox.Show(
                    "ثبت هات‌کی ناموفق بود (شاید توسط برنامه‌ی دیگه‌ای گرفته شده).\nمی‌تونی از منوی تری کلید میانبر رو عوض کنی.");
            }
        }

        private void ChangeHotkey()
        {
            using var dlg = new HotkeyCaptureForm(settings.Modifiers, (Keys)settings.Key);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                NativeMethods.UnregisterHotKey(hotkeyWindow.Handle, HOTKEY_ID);

                settings.Modifiers = dlg.SelectedModifiers;
                settings.Key = (uint)dlg.SelectedKey;
                SettingsManager.Save(settings);

                RegisterCurrentHotkey(showErrorIfFailed: true);
                UpdateTrayTextAndMenu();

                trayIcon.ShowBalloonTip(2000, "کلید میانبر تغییر کرد", FormatHotkey(), ToolTipIcon.Info);
            }
        }

        private void UpdateTrayTextAndMenu()
        {
            
            trayIcon.Text = $"Persian Type Helper ({FormatHotkey()})";
            hotkeyMenuItem.Text = $"تغییر کلید میانبر (فعلی: {FormatHotkey()})";
        }

        private string FormatHotkey()
        {
            var parts = new List<string>();
            if ((settings.Modifiers & NativeMethods.MOD_CONTROL) != 0) parts.Add("Ctrl");
            if ((settings.Modifiers & NativeMethods.MOD_SHIFT) != 0) parts.Add("Shift");
            if ((settings.Modifiers & NativeMethods.MOD_ALT) != 0) parts.Add("Alt");
            parts.Add(((Keys)settings.Key).ToString());
            return string.Join("+", parts);
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
            ExitThread();
        }
    }
}