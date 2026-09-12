using System.Collections.Generic;
using System.Windows.Forms;

namespace PersianTypeHelper
{
    internal static class HotkeyFormatter
    {
        public static string Format(uint modifiers, Keys key)
        {
            var parts = new List<string>();
            if ((modifiers & NativeMethods.MOD_CONTROL) != 0) parts.Add("Ctrl");
            if ((modifiers & NativeMethods.MOD_SHIFT) != 0) parts.Add("Shift");
            if ((modifiers & NativeMethods.MOD_ALT) != 0) parts.Add("Alt");
            parts.Add(key.ToString());
            return string.Join(" + ", parts);
        }
    }
}