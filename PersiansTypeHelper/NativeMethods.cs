using System;
using System.Runtime.InteropServices;

namespace PersianTypeHelper
{
    internal static class NativeMethods
    {
       
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

       
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

       
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        public const uint MOD_CONTROL = 0x0002;
        public const uint MOD_SHIFT = 0x0004;
        public const uint MOD_ALT = 0x0001;
        public const int WM_HOTKEY = 0x0312;

        public const byte VK_CONTROL = 0x11;
        public const byte VK_V = 0x56;
        public const uint KEYEVENTF_KEYUP = 0x0002;
    }
}