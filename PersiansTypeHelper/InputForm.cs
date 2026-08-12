using System;
using System.Drawing;
using System.Windows.Forms;

namespace PersianTypeHelper
{
    public class InputForm : Form
    {
        private TextBox txtInput;
        private CheckBox chkPin;
        private NumericUpDown numMaxChars;
        private IntPtr targetWindow;
        private HotkeySettings settings;

        public InputForm(IntPtr targetWindow)
        {
            this.targetWindow = targetWindow;
            this.settings = SettingsManager.Load();

            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.Text = "تایپ فارسی";
            this.Width = 560;
            this.Height = 140;
            this.AutoScaleMode = AutoScaleMode.Font;


            var topPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 4,
                RowCount = 1,
                Padding = new Padding(8, 6, 8, 6)
            };
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));      
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); 
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));      
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));      
            topPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            chkPin = new CheckBox
            {
                Appearance = Appearance.Button,
                Text = "سنجاق",
                AutoSize = true,
                Padding = new Padding(10, 4, 10, 4),
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 0, 8, 0)
            };
            chkPin.CheckedChanged += (s, e) =>
            {
                chkPin.Text = chkPin.Checked ? "سنجاق شده ✓" : "سنجاق";
            };

            var lblMax = new Label
            {
                Text = "حداکثر کاراکتر:",
                AutoSize = true,
                Anchor = AnchorStyles.Right,
                TextAlign = ContentAlignment.MiddleRight,
                Margin = new Padding(8, 6, 4, 0)
            };

            numMaxChars = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 5000,
                Value = Math.Clamp(settings.MaxChars, 1, 5000),
                Width = 70,
                Anchor = AnchorStyles.Right,
                Margin = new Padding(0, 3, 0, 0),
                TextAlign = HorizontalAlignment.Center
            };
            numMaxChars.ValueChanged += NumMaxChars_ValueChanged;

            topPanel.Controls.Add(chkPin, 0, 0);
     
            topPanel.Controls.Add(lblMax, 2, 0);
            topPanel.Controls.Add(numMaxChars, 3, 0);

     
            txtInput = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Tahoma", 14),
                RightToLeft = RightToLeft.Yes,
                TextAlign = HorizontalAlignment.Right,
                MaxLength = (int)numMaxChars.Value
            };
            txtInput.KeyDown += TxtInput_KeyDown;

            this.Controls.Add(txtInput);
            this.Controls.Add(topPanel);

            this.Shown += (s, e) => txtInput.Focus();
        }

        private void NumMaxChars_ValueChanged(object? sender, EventArgs e)
        {
            int value = (int)numMaxChars.Value;
            txtInput.MaxLength = value;

            settings.MaxChars = value;
            SettingsManager.Save(settings);
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
            if (string.IsNullOrEmpty(raw))
                return;

            string reshaped = PersianReshaper.ProcessText(raw, e_numbers: true, f_numbers: true, e_harakat: true);

            Clipboard.SetText(reshaped);

            this.Hide();

          
            NativeMethods.SetForegroundWindow(targetWindow);
            System.Threading.Thread.Sleep(150); 

            SendPaste();

            if (chkPin.Checked)
            {
                
                txtInput.Clear();
                this.Show();
                this.Activate();
                NativeMethods.SetForegroundWindow(this.Handle);
                txtInput.Focus();
            }
            else
            {
                this.Close();
            }
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