using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PersianTypeHelper
{
    public class HotkeyCaptureForm : Form
    {
        public uint SelectedModifiers { get; private set; }
        public Keys SelectedKey { get; private set; }

        private readonly Label lblCurrent;

        public HotkeyCaptureForm(uint currentModifiers, Keys currentKey)
        {
            SelectedModifiers = currentModifiers;
            SelectedKey = currentKey;

            this.Text = "تغییر کلید میانبر";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.TopMost = true;
            this.Width = 380;
            this.Height = 170;
            this.KeyPreview = true;

            var lblInfo = new Label
            {
                Text = "کلید ترکیبی جدید رو فشار بده (مثلاً Ctrl+Shift+P)",
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                RightToLeft = RightToLeft.Yes,
                Font = new Font("Tahoma", 9)
            };

            lblCurrent = new Label
            {
                Text = FormatHotkey(currentModifiers, currentKey),
                Dock = DockStyle.Top,
                Height = 50,
                Font = new Font("Tahoma", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var btnCancel = new Button
            {
                Text = "انصراف",
                Dock = DockStyle.Bottom,
                DialogResult = DialogResult.Cancel
            };

            this.Controls.Add(lblCurrent);
            this.Controls.Add(lblInfo);
            this.Controls.Add(btnCancel);

            this.KeyDown += HotkeyCaptureForm_KeyDown;
            this.Shown += (s, e) => this.Focus();
        }

        private void HotkeyCaptureForm_KeyDown(object? sender, KeyEventArgs e)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;

           
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Menu)
                return;

            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return;
            }

            uint modifiers = 0;
            if (e.Control) modifiers |= NativeMethods.MOD_CONTROL;
            if (e.Shift) modifiers |= NativeMethods.MOD_SHIFT;
            if (e.Alt) modifiers |= NativeMethods.MOD_ALT;

            if (modifiers == 0)
            {
                lblCurrent.Text = "باید حداقل یک Ctrl/Shift/Alt باشه";
                return;
            }

            SelectedModifiers = modifiers;
            SelectedKey = e.KeyCode;
            lblCurrent.Text = FormatHotkey(modifiers, e.KeyCode);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private static string FormatHotkey(uint modifiers, Keys key)
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