using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PersianTypeHelper
{
    public class InputForm : Form
    {
        private TextBox txtInput;
        private NumericUpDown numMaxChars;
        private Panel titleBar;
        private Panel pinIcon;
        private Panel closeIcon;
        private Panel themeIcon;
        private Label lblTitle;
        private Label lblMax;
        private Panel contentPanel;
        private TableLayoutPanel optionsRow;

        private bool isPinned = false;
        private bool isDarkMode;
        private Point dragStart;
        private ColorPalette palette;

        private readonly IntPtr targetWindow;
        private readonly HotkeySettings settings;

        public InputForm(IntPtr targetWindow)
        {
            this.targetWindow = targetWindow;
            this.settings = SettingsManager.Load();

            isDarkMode = Theme.ResolveIsDark((ThemeMode)settings.ThemeMode);
            palette = isDarkMode ? Theme.Dark : Theme.Light;

            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            TopMost = true;
            ShowInTaskbar = false;
            Text = "تایپ فارسی";
            Width = 560;
            Height = 148;
            AutoScaleMode = AutoScaleMode.Font;
            Padding = new Padding(1); 

            BuildTitleBar();
            BuildContent();
            ApplyTheme();

            Shown += (s, e) => txtInput.Focus();
        }

        private void BuildTitleBar()
        {
            titleBar = new Panel { Dock = DockStyle.Top, Height = 34 };
            titleBar.MouseDown += TitleBar_MouseDown;
            titleBar.MouseMove += TitleBar_MouseMove;

            lblTitle = new Label
            {
                Text = "تایپ فارسی",
                Font = new Font("Tahoma", 9.5f),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 10, 0),
                RightToLeft = RightToLeft.Yes
            };
            lblTitle.MouseDown += TitleBar_MouseDown;
            lblTitle.MouseMove += TitleBar_MouseMove;

            closeIcon = new Panel { Dock = DockStyle.Right, Width = 40, Cursor = Cursors.Hand };
            closeIcon.Paint += CloseIcon_Paint;
            closeIcon.MouseEnter += (s, e) => { closeIcon.BackColor = palette.CloseHover; closeIcon.Invalidate(); };
            closeIcon.MouseLeave += (s, e) => { closeIcon.BackColor = palette.Surface; closeIcon.Invalidate(); };
            closeIcon.Click += (s, e) => Close();

            pinIcon = new Panel { Dock = DockStyle.Right, Width = 40, Cursor = Cursors.Hand };
            pinIcon.Paint += PinIcon_Paint;
            pinIcon.MouseEnter += (s, e) => { if (!isPinned) pinIcon.BackColor = palette.HoverOverlay; pinIcon.Invalidate(); };
            pinIcon.MouseLeave += (s, e) => { pinIcon.BackColor = isPinned ? palette.Accent : palette.Surface; pinIcon.Invalidate(); };
            pinIcon.Click += (s, e) =>
            {
                isPinned = !isPinned;
                pinIcon.BackColor = isPinned ? palette.Accent : palette.Surface;
                pinIcon.Invalidate();
            };

            themeIcon = new Panel { Dock = DockStyle.Right, Width = 40, Cursor = Cursors.Hand };
            themeIcon.Paint += ThemeIcon_Paint;
            themeIcon.MouseEnter += (s, e) => { themeIcon.BackColor = palette.HoverOverlay; themeIcon.Invalidate(); };
            themeIcon.MouseLeave += (s, e) => { themeIcon.BackColor = palette.Surface; themeIcon.Invalidate(); };
            themeIcon.Click += ThemeIcon_Click;

            
            titleBar.Controls.Add(lblTitle);
            titleBar.Controls.Add(themeIcon);
            titleBar.Controls.Add(pinIcon);
            titleBar.Controls.Add(closeIcon);
        }

        private void ThemeIcon_Click(object? sender, EventArgs e)
        {
            isDarkMode = !isDarkMode;
            palette = isDarkMode ? Theme.Dark : Theme.Light;

            settings.ThemeMode = isDarkMode ? (int)ThemeMode.Dark : (int)ThemeMode.Light;
            SettingsManager.Save(settings);

            ApplyTheme();
        }

        private void CloseIcon_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = closeIcon.ClientRectangle;
            using var pen = new Pen(palette.TextPrimary, 1.6f);
            int pad = 13;
            g.DrawLine(pen, pad, pad, rect.Width - pad, rect.Height - pad);
            g.DrawLine(pen, rect.Width - pad, pad, pad, rect.Height - pad);
        }

        private void PinIcon_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = pinIcon.ClientRectangle;
            var color = isPinned ? Color.White : palette.TextPrimary;
            using var brush = new SolidBrush(color);

            var state = g.Save();
            g.TranslateTransform(rect.Width / 2f, rect.Height / 2f);
            g.RotateTransform(45f);
            g.FillEllipse(brush, -4f, -9f, 8f, 8f);        
            g.FillRectangle(brush, -1.3f, -2f, 2.6f, 10f); 
            g.Restore(state);
        }

        private void ThemeIcon_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = themeIcon.ClientRectangle;
            var center = new PointF(rect.Width / 2f, rect.Height / 2f);
            using var brush = new SolidBrush(palette.TextPrimary);


            if (isDarkMode)
            {
                float r = 4.2f;
                g.FillEllipse(brush, center.X - r, center.Y - r, r * 2, r * 2);
                using var pen = new Pen(palette.TextPrimary, 1.5f);
                for (int i = 0; i < 8; i++)
                {
                    double angle = i * Math.PI / 4;
                    float x1 = center.X + (float)(Math.Cos(angle) * (r + 2.5));
                    float y1 = center.Y + (float)(Math.Sin(angle) * (r + 2.5));
                    float x2 = center.X + (float)(Math.Cos(angle) * (r + 6));
                    float y2 = center.Y + (float)(Math.Sin(angle) * (r + 6));
                    g.DrawLine(pen, x1, y1, x2, y2);
                }
            }
            else
            {
                float r = 7f;
                g.FillEllipse(brush, center.X - r, center.Y - r, r * 2, r * 2);
                using var cutBrush = new SolidBrush(themeIcon.BackColor);
                g.FillEllipse(cutBrush, center.X - r + 4.5f, center.Y - r - 2.5f, r * 2, r * 2);
            }
        }

        private void TitleBar_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                dragStart = e.Location;
        }

        private void TitleBar_MouseMove(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Location = new Point(Location.X + e.X - dragStart.X, Location.Y + e.Y - dragStart.Y);
        }

        private void BuildContent()
        {
            contentPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 8, 10, 10) };

            optionsRow = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 3,
                RowCount = 1
            };
            optionsRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); 
            optionsRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));     
            optionsRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));    

            lblMax = new Label
            {
                Text = "حداکثر کاراکتر:",
                AutoSize = true,
                Anchor = AnchorStyles.Right,
                Margin = new Padding(6, 8, 4, 0)
            };

            numMaxChars = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 5000,
                Value = Math.Clamp(settings.MaxChars, 1, 5000),
                Width = 70,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = HorizontalAlignment.Center,
                Anchor = AnchorStyles.Right,
                Margin = new Padding(0, 4, 0, 6)
            };
            numMaxChars.ValueChanged += NumMaxChars_ValueChanged;

            optionsRow.Controls.Add(lblMax, 1, 0);
            optionsRow.Controls.Add(numMaxChars, 2, 0);

            txtInput = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Tahoma", 14),
                RightToLeft = RightToLeft.Yes,
                TextAlign = HorizontalAlignment.Right,
                MaxLength = (int)numMaxChars.Value,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtInput.KeyDown += TxtInput_KeyDown;

            contentPanel.Controls.Add(txtInput);
            contentPanel.Controls.Add(optionsRow);

            
            Controls.Add(contentPanel);
            Controls.Add(titleBar);
        }

        private void ApplyTheme()
        {
            BackColor = palette.Border;

            titleBar.BackColor = palette.Surface;
            lblTitle.ForeColor = palette.TextPrimary;

            closeIcon.BackColor = palette.Surface;
            pinIcon.BackColor = isPinned ? palette.Accent : palette.Surface;
            themeIcon.BackColor = palette.Surface;

            contentPanel.BackColor = palette.Background;
            optionsRow.BackColor = palette.Background;
            lblMax.ForeColor = palette.TextSecondary;

            numMaxChars.BackColor = palette.Surface;
            numMaxChars.ForeColor = palette.TextPrimary;

            txtInput.BackColor = palette.Surface;
            txtInput.ForeColor = palette.TextPrimary;

            closeIcon.Invalidate();
            pinIcon.Invalidate();
            themeIcon.Invalidate();
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
                Close();
            }
        }

        private void Submit()
        {
            string raw = txtInput.Text;
            if (string.IsNullOrEmpty(raw))
                return;

            string reshaped = PersianReshaper.ProcessText(raw, e_numbers: true, f_numbers: true, e_harakat: true);

            Clipboard.SetText(reshaped);

            Hide();

           
            NativeMethods.SetForegroundWindow(targetWindow);
            System.Threading.Thread.Sleep(150); 

            SendPaste();

            if (isPinned)
            {
              
                txtInput.Clear();
                Show();
                Activate();
                NativeMethods.SetForegroundWindow(Handle);
                txtInput.Focus();
            }
            else
            {
                Close();
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