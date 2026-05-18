using System.Drawing;
using System.Windows.Forms;

namespace ChatWave
{
    public static class ThemeManager
    {
        public static bool IsDarkMode { get; private set; } = false;

        public static Color Background => IsDarkMode
            ? Color.FromArgb(30, 30, 45)
            : Color.White;

        public static Color CardBackground => IsDarkMode
            ? Color.FromArgb(45, 45, 65)
            : Color.FromArgb(235, 230, 250);

        public static Color Header => IsDarkMode
            ? Color.FromArgb(50, 40, 80)
            : Color.FromArgb(167, 147, 214);

        public static Color TextPrimary => IsDarkMode
            ? Color.White
            : Color.FromArgb(50, 50, 50);

        public static Color TextSecondary => IsDarkMode
            ? Color.FromArgb(180, 170, 200)
            : Color.FromArgb(120, 80, 180);

        public static Color InputBackground => IsDarkMode
            ? Color.FromArgb(55, 55, 75)
            : Color.FromArgb(243, 240, 255);

        public static Color ButtonColor => IsDarkMode
            ? Color.FromArgb(100, 80, 150)
            : Color.FromArgb(160, 130, 210);

        public static Color ChatBackground => IsDarkMode
            ? Color.FromArgb(35, 35, 50)
            : Color.FromArgb(250, 248, 255);

        public static void SetDarkMode(bool dark)
        {
            IsDarkMode = dark;
        }

        public static void ApplyTheme(Form form)
        {
            form.BackColor = Background;
            ApplyToControls(form.Controls);
        }

        private static void ApplyToControls(Control.ControlCollection controls)
        {
            foreach (Control ctrl in controls)
            {
                // Panel
                if (ctrl is Panel panel)
                {
                    Color bg = panel.BackColor;

                    if (bg == Color.White ||
                        bg == Color.FromArgb(245, 245, 245) ||
                        bg == Color.FromArgb(250, 248, 255) ||
                        bg == Color.FromArgb(245, 242, 255))
                        panel.BackColor = Background;

                    else if (bg == Color.FromArgb(235, 230, 250) ||
                             bg == Color.FromArgb(240, 235, 250))
                        panel.BackColor = CardBackground;

                    else if (bg == Color.FromArgb(167, 147, 214) ||
                             bg == Color.FromArgb(190, 170, 230) ||
                             bg == Color.FromArgb(50, 40, 80))
                        panel.BackColor = Header;
                }

                // Label
                else if (ctrl is Label lbl)
                {
                    if (lbl.ForeColor == Color.FromArgb(50, 50, 50) ||
                        lbl.ForeColor == Color.Black)
                        lbl.ForeColor = TextPrimary;
                    else if (lbl.ForeColor == Color.FromArgb(80, 80, 80) ||
                             lbl.ForeColor == Color.FromArgb(70, 70, 70))
                        lbl.ForeColor = TextPrimary;
                    else if (lbl.ForeColor == Color.FromArgb(120, 80, 180) ||
                             lbl.ForeColor == Color.FromArgb(90, 40, 160))
                        lbl.ForeColor = TextSecondary;
                }

                // TextBox
                else if (ctrl is TextBox txt)
                {
                    if (!txt.ReadOnly && txt.Enabled)
                    {
                        txt.BackColor = InputBackground;
                        txt.ForeColor = TextPrimary;
                    }
                }

                // Button - ACUM ESTE ÎN AFARA BLOCULUI DE PANEL
                else if (ctrl is Button btn)
                {
                    if (btn.BackColor == Color.White ||
                        btn.BackColor == Color.FromArgb(160, 130, 210) ||
                        btn.BackColor == Color.FromArgb(167, 147, 214) ||
                        btn.BackColor == Color.FromArgb(190, 170, 230))
                    {
                        btn.BackColor = ButtonColor;
                        btn.ForeColor = Color.White;
                    }
                    else if (btn.BackColor == Color.FromArgb(255, 249, 235))
                    {
                        btn.BackColor = IsDarkMode ? Color.FromArgb(60, 60, 80) : Color.FromArgb(255, 249, 235);
                        btn.ForeColor = IsDarkMode ? Color.FromArgb(180, 170, 200) : Color.FromArgb(180, 130, 20);
                    }
                }

                // ListBox
                else if (ctrl is ListBox lb)
                {
                    lb.BackColor = IsDarkMode
                        ? Color.FromArgb(40, 40, 60)
                        : Color.White;
                    lb.ForeColor = TextPrimary;
                }

                // TabControl - ACUM ESTE ÎN AFARA BLOCULUI DE PANEL
                else if (ctrl is TabControl tabCtrl)
                {
                    tabCtrl.BackColor = Background;
                    foreach (TabPage page in tabCtrl.TabPages)
                    {
                        page.BackColor = Background;
                        ApplyToControls(page.Controls);
                    }
                }

                // TabPage
                else if (ctrl is TabPage tp)
                {
                    tp.BackColor = Background;
                }

                // DataGridView
                else if (ctrl is DataGridView dgv)
                {
                    dgv.BackgroundColor = Background;
                    dgv.DefaultCellStyle.BackColor = Background;
                    dgv.DefaultCellStyle.ForeColor = TextPrimary;
                    dgv.AlternatingRowsDefaultCellStyle.BackColor = CardBackground;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = CardBackground;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextPrimary;
                    dgv.GridColor = IsDarkMode
                        ? Color.FromArgb(60, 60, 80)
                        : Color.FromArgb(220, 215, 235);
                }

                // Recursiv pentru copii
                if (ctrl.Controls.Count > 0)
                    ApplyToControls(ctrl.Controls);
            }
        }
    }
}