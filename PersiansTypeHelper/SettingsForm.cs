using Microsoft.VisualBasic.Logging;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PersianTypeHelper
{
    public class SettingsForm : Form
    {
        private readonly Action onChangeHotkeyRequested;
        private HotkeySettings settings;
        private ColorPalette palette;

        private Panel scrollHost;
        private TableLayoutPanel root;

        private Label lblHotkeyTitle;
        private Label lblHotkeyValue;
        private Button btnChangeHotkey;

        private Label lblMaxCharsTitle;
        private NumericUpDown numMaxChars;

        private Label lblDigitsTitle;
        private RadioButton radDigitsEnglish;
        private RadioButton radDigitsPersian;
        private RadioButton radDigitsArabic;

        private CheckBox chkKeepHarakat;

        private Label lblThemeTitle;
        private RadioButton radThemeAuto;
        private RadioButton radThemeLight;
        private RadioButton radThemeDark;

        private Label lblLanguageTitle;
        private RadioButton radLangFa;
        private RadioButton radLangEn;

        private Button btnClose;

        public SettingsForm(Action onChangeHotkeyRequested)
        {
            this.onChangeHotkeyRequested = onChangeHotkeyRequested;
            settings = SettingsManager.Load();
            palette = Theme.Resolve((ThemeMode)settings.ThemeMode);

            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            MinimizeBox = false;
            TopMost = true;
            Width = 440;
            Height = 580;
            Font = new Font("Tahoma", 9);

            BuildUi();
            ApplyTheme();
            ApplyLanguage();
        }

        private string T(string fa, string en) => Loc.S(settings.AppLanguage, fa, en);

        private void BuildUi()
        {
            scrollHost = new Panel { Dock = DockStyle.Fill, AutoScroll = true };

            root = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                Padding = new Padding(18, 16, 18, 16)
            };

            // --- کلید میانبر ---
            lblHotkeyTitle = new Label { AutoSize = true, Font = new Font("Tahoma", 9.5f, FontStyle.Bold), Margin = new Padding(0, 0, 0, 6) };

            var hotkeyRow = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 18)
            };
            btnChangeHotkey = new Button { AutoSize = true, FlatStyle = FlatStyle.Flat, Padding = new Padding(10, 3, 10, 3), Margin = new Padding(0, 0, 10, 0) };
            btnChangeHotkey.Click += BtnChangeHotkey_Click;
            lblHotkeyValue = new Label { AutoSize = true, Font = new Font("Tahoma", 11, FontStyle.Bold), Margin = new Padding(0, 6, 0, 0) };
            hotkeyRow.Controls.Add(btnChangeHotkey);
            hotkeyRow.Controls.Add(lblHotkeyValue);

            // --- حداکثر کاراکتر ---
            lblMaxCharsTitle = new Label { AutoSize = true, Font = new Font("Tahoma", 9.5f, FontStyle.Bold), Margin = new Padding(0, 0, 0, 6) };
            numMaxChars = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 5000,
                Value = Math.Clamp(settings.MaxChars, 1, 5000),
                Width = 90,
                Margin = new Padding(0, 0, 0, 18)
            };
            numMaxChars.ValueChanged += (s, e) =>
            {
                settings.MaxChars = (int)numMaxChars.Value;
                SettingsManager.Save(settings);
            };

            // --- شیوه‌ی نمایش اعداد ---
            lblDigitsTitle = new Label { AutoSize = true, Font = new Font("Tahoma", 9.5f, FontStyle.Bold), Margin = new Padding(0, 0, 0, 6) };
            radDigitsEnglish = new RadioButton { AutoSize = true, Margin = new Padding(0, 2, 0, 2) };
            radDigitsPersian = new RadioButton { AutoSize = true, Margin = new Padding(0, 2, 0, 2) };
            radDigitsArabic = new RadioButton { AutoSize = true, Margin = new Padding(0, 2, 0, 18) };
            switch (settings.DigitMode)
            {
                case 2: radDigitsArabic.Checked = true; break;
                case 0: radDigitsEnglish.Checked = true; break;
                default: radDigitsPersian.Checked = true; break;
            }
            radDigitsEnglish.CheckedChanged += (s, e) => { if (radDigitsEnglish.Checked) SaveDigitMode(0); };
            radDigitsPersian.CheckedChanged += (s, e) => { if (radDigitsPersian.Checked) SaveDigitMode(1); };
            radDigitsArabic.CheckedChanged += (s, e) => { if (radDigitsArabic.Checked) SaveDigitMode(2); };

            // --- اعراب ---
            chkKeepHarakat = new CheckBox { AutoSize = true, Checked = settings.KeepHarakat, Margin = new Padding(0, 0, 0, 18) };
            chkKeepHarakat.CheckedChanged += (s, e) =>
            {
                settings.KeepHarakat = chkKeepHarakat.Checked;
                SettingsManager.Save(settings);
            };

            // --- ظاهر (تم) ---
            lblThemeTitle = new Label { AutoSize = true, Font = new Font("Tahoma", 9.5f, FontStyle.Bold), Margin = new Padding(0, 0, 0, 6) };
            radThemeAuto = new RadioButton { AutoSize = true, Margin = new Padding(0, 2, 0, 2) };
            radThemeLight = new RadioButton { AutoSize = true, Margin = new Padding(0, 2, 0, 2) };
            radThemeDark = new RadioButton { AutoSize = true, Margin = new Padding(0, 2, 0, 18) };
            switch (settings.ThemeMode)
            {
                case 1: radThemeLight.Checked = true; break;
                case 2: radThemeDark.Checked = true; break;
                default: radThemeAuto.Checked = true; break;
            }
            radThemeAuto.CheckedChanged += (s, e) => { if (radThemeAuto.Checked) SaveTheme(0); };
            radThemeLight.CheckedChanged += (s, e) => { if (radThemeLight.Checked) SaveTheme(1); };
            radThemeDark.CheckedChanged += (s, e) => { if (radThemeDark.Checked) SaveTheme(2); };

            // --- زبان برنامه ---
            lblLanguageTitle = new Label { AutoSize = true, Font = new Font("Tahoma", 9.5f, FontStyle.Bold), Margin = new Padding(0, 0, 0, 6) };
            radLangFa = new RadioButton { AutoSize = true, Text = "فارسی", Margin = new Padding(0, 2, 0, 2) };
            radLangEn = new RadioButton { AutoSize = true, Text = "English", Margin = new Padding(0, 2, 0, 4) };
            radLangFa.Checked = settings.AppLanguage == 0;
            radLangEn.Checked = settings.AppLanguage == 1;
            radLangFa.CheckedChanged += (s, e) => { if (radLangFa.Checked) SaveLanguage(0); };
            radLangEn.CheckedChanged += (s, e) => { if (radLangEn.Checked) SaveLanguage(1); };

            root.Controls.Add(lblHotkeyTitle);
            root.Controls.Add(hotkeyRow);
            root.Controls.Add(lblMaxCharsTitle);
            root.Controls.Add(numMaxChars);
            root.Controls.Add(lblDigitsTitle);
            root.Controls.Add(radDigitsEnglish);
            root.Controls.Add(radDigitsPersian);
            root.Controls.Add(radDigitsArabic);
            root.Controls.Add(chkKeepHarakat);
            root.Controls.Add(lblThemeTitle);
            root.Controls.Add(radThemeAuto);
            root.Controls.Add(radThemeLight);
            root.Controls.Add(radThemeDark);
            root.Controls.Add(lblLanguageTitle);
            root.Controls.Add(radLangFa);
            root.Controls.Add(radLangEn);

            scrollHost.Controls.Add(root);

            var closeRow = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(18, 8, 18, 8) };
            btnClose = new Button { Dock = DockStyle.Right, Width = 100, FlatStyle = FlatStyle.Flat };
            btnClose.Click += (s, e) => Close();
            closeRow.Controls.Add(btnClose);

            Controls.Add(scrollHost);
            Controls.Add(closeRow);
        }

        private void SaveDigitMode(int mode)
        {
            settings.DigitMode = mode;
            SettingsManager.Save(settings);
        }

        private void SaveTheme(int mode)
        {
            settings.ThemeMode = mode;
            SettingsManager.Save(settings);
            palette = Theme.Resolve((ThemeMode)mode);
            ApplyTheme();
        }

        private void SaveLanguage(int lang)
        {
            settings.AppLanguage = lang;
            SettingsManager.Save(settings);
            ApplyLanguage();
        }

        private void BtnChangeHotkey_Click(object? sender, EventArgs e)
        {
         
            onChangeHotkeyRequested?.Invoke();

     
            settings = SettingsManager.Load();
            UpdateHotkeyLabel();
        }

        private void UpdateHotkeyLabel()
        {
            lblHotkeyValue.Text = HotkeyFormatter.Format(settings.Modifiers, (Keys)settings.Key);
        }

        private void ApplyTheme()
        {
            BackColor = palette.Background;
            scrollHost.BackColor = palette.Background;
            root.BackColor = palette.Background;

            foreach (Control c in AllControls(root))
            {
                switch (c)
                {
                    case Label lbl:
                        lbl.ForeColor = palette.TextPrimary;
                        break;
                    case RadioButton rb:
                        rb.ForeColor = palette.TextPrimary;
                        break;
                    case CheckBox cb:
                        cb.ForeColor = palette.TextPrimary;
                        break;
                    case NumericUpDown nud:
                        nud.BackColor = palette.Surface;
                        nud.ForeColor = palette.TextPrimary;
                        break;
                    case FlowLayoutPanel flp:
                        flp.BackColor = palette.Background;
                        break;
                }
            }

            btnChangeHotkey.BackColor = palette.Surface;
            btnChangeHotkey.ForeColor = palette.TextPrimary;
            btnChangeHotkey.FlatAppearance.BorderColor = palette.Border;

            btnClose.BackColor = palette.Surface;
            btnClose.ForeColor = palette.TextPrimary;
            btnClose.FlatAppearance.BorderColor = palette.Border;

            foreach (Control c in Controls)
                if (c is Panel p && p.Dock == DockStyle.Bottom)
                    p.BackColor = palette.Background;
        }

        private static System.Collections.Generic.IEnumerable<Control> AllControls(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                yield return c;
                foreach (var inner in AllControls(c))
                    yield return inner;
            }
        }

        private void ApplyLanguage()
        {
            Text = T("تنظیمات", "Settings");

            lblHotkeyTitle.Text = T("کلید میانبر:", "Hotkey:");
            btnChangeHotkey.Text = T("تغییر", "Change");
            UpdateHotkeyLabel();

            lblMaxCharsTitle.Text = T("حداکثر تعداد کاراکتر مجاز:", "Max allowed characters:");

            lblDigitsTitle.Text = T("شیوه‌ی نمایش اعداد:", "Digit style:");
            radDigitsEnglish.Text = T("انگلیسی (0123)", "English (0123)");
            radDigitsPersian.Text = T("فارسی (۰۱۲۳)", "Persian (۰۱۲۳)");
            radDigitsArabic.Text = T("عربی (٠١٢٣)", "Arabic (٠١٢٣)");

            chkKeepHarakat.Text = T("نگه‌داشتن اعراب حروف (ً ٌ ٍ َ ُ ِ ّ ْ)", "Keep Arabic diacritics (harakat)");

            lblThemeTitle.Text = T("ظاهر برنامه:", "Appearance:");
            radThemeAuto.Text = T("خودکار (پیرو تم ویندوز)", "Automatic (follow Windows)");
            radThemeLight.Text = T("روشن", "Light");
            radThemeDark.Text = T("تاریک", "Dark");

            lblLanguageTitle.Text = T("زبان برنامه:", "App language:");

            btnClose.Text = T("بستن", "Close");
        }
    }
}