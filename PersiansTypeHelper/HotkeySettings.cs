using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace PersianTypeHelper
{
    public class HotkeySettings
    {
        public uint Modifiers { get; set; } = NativeMethods.MOD_CONTROL | NativeMethods.MOD_SHIFT;
        public uint Key { get; set; } = (uint)Keys.P;
        public int MaxChars { get; set; } = 200;

        
        public int ThemeMode { get; set; } = 0;
    }

    public static class SettingsManager
    {
        private static readonly string SettingsDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PersianTypeHelper");

        private static readonly string SettingsPath = Path.Combine(SettingsDir, "settings.json");

        public static HotkeySettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    var settings = JsonSerializer.Deserialize<HotkeySettings>(json);
                    if (settings != null)
                        return settings;
                }
            }
            catch
            {
                
            }

            return new HotkeySettings();
        }

        public static void Save(HotkeySettings settings)
        {
            try
            {
                Directory.CreateDirectory(SettingsDir);
                string json = JsonSerializer.Serialize(settings);
                File.WriteAllText(SettingsPath, json);
            }
            catch
            {
                
            }
        }
    }
}