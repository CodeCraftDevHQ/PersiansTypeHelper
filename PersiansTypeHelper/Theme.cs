using System.Drawing;
using Microsoft.Win32;

namespace PersianTypeHelper
{
    public enum ThemeMode
    {
        System = 0,
        Light = 1,
        Dark = 2
    }

 
    internal class ColorPalette
    {
        public Color Background = Color.White;
        public Color Surface = Color.White;
        public Color Border = Color.Gray;
        public Color TextPrimary = Color.Black;
        public Color TextSecondary = Color.Gray;
        public Color Accent = Color.Blue;
        public Color CloseHover = Color.Red;
        public Color HoverOverlay = Color.LightGray;
    }

    internal static class Theme
    {
        public static readonly ColorPalette Dark = new ColorPalette
        {
            Background = Color.FromArgb(30, 30, 32),
            Surface = Color.FromArgb(43, 43, 47),
            Border = Color.FromArgb(62, 62, 68),
            TextPrimary = Color.FromArgb(235, 235, 240),
            TextSecondary = Color.FromArgb(160, 160, 168),
            Accent = Color.FromArgb(94, 114, 235),
            CloseHover = Color.FromArgb(196, 43, 28),
            HoverOverlay = Color.FromArgb(60, 60, 66)
        };

        public static readonly ColorPalette Light = new ColorPalette
        {
            Background = Color.FromArgb(246, 246, 248),
            Surface = Color.White,
            Border = Color.FromArgb(214, 214, 219),
            TextPrimary = Color.FromArgb(28, 28, 30),
            TextSecondary = Color.FromArgb(96, 96, 102),
            Accent = Color.FromArgb(74, 94, 215),
            CloseHover = Color.FromArgb(196, 43, 28),
            HoverOverlay = Color.FromArgb(226, 226, 231)
        };


        public static bool IsSystemDarkTheme()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                var value = key?.GetValue("AppsUseLightTheme");
                if (value is int lightThemeValue)
                    return lightThemeValue == 0; 
            }
            catch
            {
               
            }
            return true;
        }


        public static ColorPalette Resolve(ThemeMode mode) => mode switch
        {
            ThemeMode.Light => Light,
            ThemeMode.Dark => Dark,
            _ => IsSystemDarkTheme() ? Dark : Light
        };

        public static bool ResolveIsDark(ThemeMode mode) => mode switch
        {
            ThemeMode.Light => false,
            ThemeMode.Dark => true,
            _ => IsSystemDarkTheme()
        };
    }
}