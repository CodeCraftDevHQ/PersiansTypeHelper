using System;
using System.Windows.Forms;

namespace PersianTypeHelper
{
    public class InputForm : Form
    {
        private TextBox txtInput;
        private IntPtr targetWindow;

        public InputForm(IntPtr targetWindow)
        {
            this.targetWindow = targetWindow;

            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.Text = "تایپ فارسی";
            this.Width = 500;
            this.Height = 100;

            txtInput = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Tahoma", 14),
                RightToLeft = RightToLeft.Yes,
                TextAlign = HorizontalAlignment.Right
            };
            txtInput.KeyDown += TxtInput_KeyDown;
            this.Controls.Add(txtInput);

            this.Shown += (s, e) => txtInput.Focus();
        }

        private void TxtInput_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Submit();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void Submit()
        {
            string raw = txtInput.Text;
            string reshaped = PersianReshaper.ProcessText(raw, e_numbers: true, f_numbers: true, e_harakat: true);

            Clipboard.SetText(reshaped);

            this.Hide();

         
            NativeMethods.SetForegroundWindow(targetWindow);
            System.Threading.Thread.Sleep(150); 

            SendPaste();

            this.Close();
        }

        private void SendPaste()
        {
            NativeMethods.keybd_event(NativeMethods.VK_CONTROL, 0, 0, UIntPtr.Zero);
            NativeMethods.keybd_event(NativeMethods.VK_V, 0, 0, UIntPtr.Zero);
            NativeMethods.keybd_event(NativeMethods.VK_V, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);
            NativeMethods.keybd_event(NativeMethods.VK_CONTROL, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);
        }
    }
}