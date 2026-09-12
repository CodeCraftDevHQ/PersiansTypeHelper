using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PersianTypeHelper
{
    public class TrayContext : ApplicationContext
    {
        private readonly NotifyIcon trayIcon;
        private readonly HotkeyWindow hotkeyWindow;
        private readonly ToolStripMenuItem settingsMenuItem;
        private readonly ToolStripMenuItem exitMenuItem;
        private const int HOTKEY_ID = 9000;

        private HotkeySettings settings;
        private SettingsForm? settingsForm;

        public TrayContext()
        {
            settings = SettingsManager.Load();
          
            settingsMenuItem = new ToolStripMenuItem();
            settingsMenuItem.Click += (s, e) => OpenSettings();

            exitMenuItem = new ToolStripMenuItem();
            exitMenuItem.Click += (s, e) => ExitApp();

            var menu = new ContextMenuStrip();
            menu.Items.Add(settingsMenuItem);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(exitMenuItem);

            trayIcon = new NotifyIcon
            {
                Icon = LoadAppIcon(),
                Visible = true,
                ContextMenuStrip = menu
            };

            hotkeyWindow = new HotkeyWindow();
            hotkeyWindow.HotkeyPressed += OnHotkeyPressed;

            RegisterCurrentHotkey(showErrorIfFailed: true);
            UpdateTrayTexts();

            trayIcon.ShowBalloonTip(
                3000,
                Loc.S(settings.AppLanguage, "Persian Type Helper فعال شد", "Persian Type Helper is active"),
                Loc.S(settings.AppLanguage,
                    $"برای تایپ فارسی کلید {FormatHotkey()} رو بزن.",
                    $"Press {FormatHotkey()} to start typing Persian."),
                ToolTipIcon.Info);
        }

        private static Icon LoadAppIcon()
        {
            try
            {
                // آیکونی که با ApplicationIcon در csproj داخل خودِ exe جاسازی شده
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
                MessageBox.Show(Loc.S(settings.AppLanguage,
                    "ثبت هات‌کی ناموفق بود (شاید توسط برنامه‌ی دیگه‌ای گرفته شده).\nمی‌تونی از تنظیمات کلید میانبر رو عوض کنی.",
                    "Registering the hotkey failed (it may be used by another app).\nYou can change it from Settings."));
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
                UpdateTrayTexts();

                trayIcon.ShowBalloonTip(
                    2000,
                    Loc.S(settings.AppLanguage, "کلید میانبر تغییر کرد", "Hotkey changed"),
                    FormatHotkey(),
                    ToolTipIcon.Info);
            }
        }

        private void OpenSettings()
        {
            if (settingsForm == null || settingsForm.IsDisposed)
            {
                settingsForm = new SettingsForm(ChangeHotkey);
                settingsForm.FormClosed += (s, e) =>
                {
                    settingsForm = null;
                
                    settings = SettingsManager.Load();
                    UpdateTrayTexts();
                };
                settingsForm.Show();
            }
            else
            {
                settingsForm.Activate();
            }
        }

        private void UpdateTrayTexts()
        {
            
            trayIcon.Text = $"Persian Type Helper ({FormatHotkey()})";
            settingsMenuItem.Text = Loc.S(settings.AppLanguage, "تنظیمات...", "Settings...");
            exitMenuItem.Text = Loc.S(settings.AppLanguage, "خروج", "Exit");
        }

        private string FormatHotkey() => HotkeyFormatter.Format(settings.Modifiers, (Keys)settings.Key);

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