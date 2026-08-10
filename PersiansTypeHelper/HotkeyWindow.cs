using System;
using System.Windows.Forms;

namespace PersianTypeHelper
{
    public class HotkeyWindow : NativeWindow
    {
        public event Action? HotkeyPressed;

        public HotkeyWindow()
        {
            CreateHandle(new CreateParams());
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_HOTKEY)
            {
                HotkeyPressed?.Invoke();
            }
            base.WndProc(ref m);
        }
    }
}