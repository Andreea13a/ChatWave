using System;
using System.Drawing;
using System.Windows.Forms;
using ChatWave.Data;

namespace ChatWave.Forms
{
    public partial class SettingsForm : Form
    {
        private LoggedUser currentUser;

        private Panel pnlHeader;
        private Label lblTitle;
        private TabControl tabControl;
        private TabPage tabProfile;
        private TabPage tabPassword;
        private TabPage tabTheme;

        private TextBox txtUsername;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private Label lblProfileError;
        private Label lblProfileSuccess;
        private Button btnSaveProfile;

        private TextBox txtOldPassword;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private Label lblPasswordError;
        private Label lblPasswordSuccess;
        private Button btnSavePassword;

        private Button btnLightMode;
        private Button btnDarkMode;
        private Label lblThemeStatus;

        public SettingsForm(LoggedUser user)
        {
            currentUser = user;
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            Color movHeader = Color.FromArgb(167, 147, 214);
            Color movButton = Color.FromArgb(160, 130, 210);

            this.Text = "ChatWave - Setări";
            this.Size = new Size(500, 480);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // HEADER
            pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 60;
            pnlHeader.BackColor = movHeader;

            lblTitle = new Label();
            lblTitle.Text = "⚙️ Setări";
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.Location = new Point(20, 15);
            lblTitle.AutoSize = true;
            pnlHeader.Controls.Add(lblTitle);

            // TAB CONTROL
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new Font("Segoe UI", 10);

            // ── TAB PROFIL ──────────────────────────────────────────
            tabProfile = new TabPage();
            tabProfile.Text = "👤 Profil";
            tabProfile.BackColor = Color.White;

            AddLabel(tabProfile, "👤 Username", 20, 20);
            txtUsername = AddTextBox(tabProfile, currentUser.Username, 20, 48);
            txtUsername.Enabled = false;
            txtUsername.BackColor = Color.FromArgb(245, 245, 245);

            AddLabel(tabProfile, "📧 Email", 20, 95);
            txtEmail = AddTextBox(tabProfile, currentUser.Email ?? "", 20, 123);

            AddLabel(tabProfile, "📱 Telefon", 20, 170);
            txtPhone = AddTextBox(tabProfile, currentUser.Phone ?? "", 20, 198);

            lblProfileError = new Label();
            lblProfileError.Text = "";
            lblProfileError.ForeColor = Color.FromArgb(192, 57, 43);
            lblProfileError.Font = new Font("Segoe UI", 9);
            lblProfileError.Location = new Point(20, 245);
            lblProfileError.Size = new Size(420, 20);
            tabProfile.Controls.Add(lblProfileError);

            lblProfileSuccess = new Label();
            lblProfileSuccess.Text = "";
            lblProfileSuccess.ForeColor = Color.FromArgb(39, 174, 96);
            lblProfileSuccess.Font = new Font("Segoe UI", 9);
            lblProfileSuccess.Location = new Point(20, 245);
            lblProfileSuccess.Size = new Size(420, 20);
            tabProfile.Controls.Add(lblProfileSuccess);

            btnSaveProfile = new Button();
            btnSaveProfile.Text = "💾 Salvează profilul";
            btnSaveProfile.Location = new Point(20, 275);
            btnSaveProfile.Size = new Size(420, 42);
            btnSaveProfile.BackColor = movButton;
            btnSaveProfile.ForeColor = Color.White;
            btnSaveProfile.FlatStyle = FlatStyle.Flat;
            btnSaveProfile.FlatAppearance.BorderSize = 0;
            btnSaveProfile.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnSaveProfile.Cursor = Cursors.Hand;
            btnSaveProfile.Click += BtnSaveProfile_Click;
            btnSaveProfile.MouseEnter += (s, e) =>
                btnSaveProfile.BackColor = Color.FromArgb(140, 110, 190);
            btnSaveProfile.MouseLeave += (s, e) =>
                btnSaveProfile.BackColor = movButton;
            tabProfile.Controls.Add(btnSaveProfile);

            // ── TAB PAROLA ──────────────────────────────────────────
            tabPassword = new TabPage();
            tabPassword.Text = "🔒 Parolă";
            tabPassword.BackColor = Color.White;

            AddLabel(tabPassword, "🔑 Parola veche", 20, 20);
            txtOldPassword = AddTextBox(tabPassword, "", 20, 48);
            txtOldPassword.PasswordChar = '*';

            AddLabel(tabPassword, "🔒 Parola nouă", 20, 95);
            txtNewPassword = AddTextBox(tabPassword, "", 20, 123);
            txtNewPassword.PasswordChar = '*';

            AddLabel(tabPassword, "🔒 Confirmă parola nouă", 20, 170);
            txtConfirmPassword = AddTextBox(tabPassword, "", 20, 198);
            txtConfirmPassword.PasswordChar = '*';

            lblPasswordError = new Label();
            lblPasswordError.Text = "";
            lblPasswordError.ForeColor = Color.FromArgb(192, 57, 43);
            lblPasswordError.Font = new Font("Segoe UI", 9);
            lblPasswordError.Location = new Point(20, 245);
            lblPasswordError.Size = new Size(420, 20);
            tabPassword.Controls.Add(lblPasswordError);

            lblPasswordSuccess = new Label();
            lblPasswordSuccess.Text = "";
            lblPasswordSuccess.ForeColor = Color.FromArgb(39, 174, 96);
            lblPasswordSuccess.Font = new Font("Segoe UI", 9);
            lblPasswordSuccess.Location = new Point(20, 245);
            lblPasswordSuccess.Size = new Size(420, 20);
            tabPassword.Controls.Add(lblPasswordSuccess);

            btnSavePassword = new Button();
            btnSavePassword.Text = "🔒 Schimbă parola";
            btnSavePassword.Location = new Point(20, 275);
            btnSavePassword.Size = new Size(420, 42);
            btnSavePassword.BackColor = movButton;
            btnSavePassword.ForeColor = Color.White;
            btnSavePassword.FlatStyle = FlatStyle.Flat;
            btnSavePassword.FlatAppearance.BorderSize = 0;
            btnSavePassword.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnSavePassword.Cursor = Cursors.Hand;
            btnSavePassword.Click += BtnSavePassword_Click;
            btnSavePassword.MouseEnter += (s, e) =>
                btnSavePassword.BackColor = Color.FromArgb(140, 110, 190);
            btnSavePassword.MouseLeave += (s, e) =>
                btnSavePassword.BackColor = movButton;
            tabPassword.Controls.Add(btnSavePassword);

            // ── TAB TEMA ────────────────────────────────────────────
            tabTheme = new TabPage();
            tabTheme.Text = "🌙 Temă";
            tabTheme.BackColor = Color.White;

            Label lblThemeTitle = new Label();
            lblThemeTitle.Text = "Alege tema aplicației:";
            lblThemeTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblThemeTitle.ForeColor = Color.FromArgb(70, 70, 70);
            lblThemeTitle.Location = new Point(20, 30);
            lblThemeTitle.AutoSize = true;
            tabTheme.Controls.Add(lblThemeTitle);

            lblThemeStatus = new Label();
            lblThemeStatus.Text = ChatWave.ThemeManager.IsDarkMode
                ? "Tema curentă: 🌙 Dark Mode"
                : "Tema curentă: ☀️ Light Mode";
            lblThemeStatus.Font = new Font("Segoe UI", 10);
            lblThemeStatus.ForeColor = Color.FromArgb(120, 80, 180);
            lblThemeStatus.Location = new Point(20, 65);
            lblThemeStatus.AutoSize = true;
            tabTheme.Controls.Add(lblThemeStatus);

            btnLightMode = new Button();
            btnLightMode.Text = "☀️ Light Mode";
            btnLightMode.Location = new Point(20, 110);
            btnLightMode.Size = new Size(190, 80);
            btnLightMode.BackColor = Color.FromArgb(255, 249, 235);
            btnLightMode.ForeColor = Color.FromArgb(180, 130, 20);
            btnLightMode.FlatStyle = FlatStyle.Flat;
            btnLightMode.FlatAppearance.BorderColor = Color.FromArgb(220, 180, 50);
            btnLightMode.FlatAppearance.BorderSize = 2;
            btnLightMode.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnLightMode.Cursor = Cursors.Hand;
            btnLightMode.Click += BtnLightMode_Click;
            tabTheme.Controls.Add(btnLightMode);

            btnDarkMode = new Button();
            btnDarkMode.Text = "🌙 Dark Mode";
            btnDarkMode.Location = new Point(240, 110);
            btnDarkMode.Size = new Size(190, 80);
            btnDarkMode.BackColor = Color.FromArgb(40, 40, 60);
            btnDarkMode.ForeColor = Color.White;
            btnDarkMode.FlatStyle = FlatStyle.Flat;
            btnDarkMode.FlatAppearance.BorderColor = Color.FromArgb(100, 80, 160);
            btnDarkMode.FlatAppearance.BorderSize = 2;
            btnDarkMode.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnDarkMode.Cursor = Cursors.Hand;
            btnDarkMode.Click += BtnDarkMode_Click;
            tabTheme.Controls.Add(btnDarkMode);

            Label lblNote = new Label();
            lblNote.Text = "⚠️ Tema se aplică imediat pentru toată aplicația.";
            lblNote.Font = new Font("Segoe UI", 9);
            lblNote.ForeColor = Color.FromArgb(150, 150, 150);
            lblNote.Location = new Point(20, 210);
            lblNote.AutoSize = true;
            tabTheme.Controls.Add(lblNote);

            // ADAUGA TABURILE — ordinea conteaza!
            tabControl.TabPages.Add(tabProfile);
            tabControl.TabPages.Add(tabPassword);
            tabControl.TabPages.Add(tabTheme);

            this.Controls.Add(tabControl);
            this.Controls.Add(pnlHeader);
        }

        private Label AddLabel(TabPage tab, string text, int x, int y)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lbl.ForeColor = Color.FromArgb(70, 70, 70);
            lbl.Location = new Point(x, y);
            lbl.AutoSize = true;
            tab.Controls.Add(lbl);
            return lbl;
        }

        private TextBox AddTextBox(TabPage tab, string text, int x, int y)
        {
            TextBox txt = new TextBox();
            txt.Text = text;
            txt.Location = new Point(x, y);
            txt.Size = new Size(420, 35);
            txt.Font = new Font("Segoe UI", 11);
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.BackColor = Color.FromArgb(243, 240, 255);
            txt.GotFocus += (s, e) => txt.BackColor = Color.FromArgb(237, 231, 255);
            txt.LostFocus += (s, e) => txt.BackColor = Color.FromArgb(243, 240, 255);
            tab.Controls.Add(txt);
            return txt;
        }

        private void BtnSaveProfile_Click(object sender, EventArgs e)
        {
            lblProfileError.Text = "";
            lblProfileSuccess.Text = "";

            if (string.IsNullOrEmpty(txtEmail.Text))
            {
                lblProfileError.Text = "⚠ Email-ul nu poate fi gol!";
                return;
            }

            if (string.IsNullOrEmpty(txtPhone.Text))
            {
                lblProfileError.Text = "⚠ Telefonul nu poate fi gol!";
                return;
            }

            bool success = UserRepository.UpdateProfile(
                currentUser.Id,
                txtEmail.Text.Trim(),
                txtPhone.Text.Trim());

            if (success)
            {
                currentUser.Email = txtEmail.Text.Trim();
                currentUser.Phone = txtPhone.Text.Trim();
                lblProfileSuccess.Text = "✅ Profil actualizat cu succes!";

                if (this.Owner is MainChatForm mainForm)
                    mainForm.RefreshUsers();
            }
            else
            {
                lblProfileError.Text = "⚠ Eroare la salvare!";
            }
        }

        private void BtnSavePassword_Click(object sender, EventArgs e)
        {
            lblPasswordError.Text = "";
            lblPasswordSuccess.Text = "";

            if (string.IsNullOrEmpty(txtOldPassword.Text) ||
                string.IsNullOrEmpty(txtNewPassword.Text) ||
                string.IsNullOrEmpty(txtConfirmPassword.Text))
            {
                lblPasswordError.Text = "⚠ Completează toate câmpurile!";
                return;
            }

            if (txtOldPassword.Text != currentUser.Password)
            {
                lblPasswordError.Text = "⚠ Parola veche este incorectă!";
                return;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                lblPasswordError.Text = "⚠ Parolele noi nu coincid!";
                return;
            }

            if (txtNewPassword.Text.Length < 6)
            {
                lblPasswordError.Text = "⚠ Parola trebuie să aibă minim 6 caractere!";
                return;
            }

            bool success = UserRepository.UpdatePassword(
                currentUser.Id,
                txtNewPassword.Text);

            if (success)
            {
                currentUser.Password = txtNewPassword.Text;
                lblPasswordSuccess.Text = "✅ Parola schimbată cu succes!";
                txtOldPassword.Text = "";
                txtNewPassword.Text = "";
                txtConfirmPassword.Text = "";
            }
            else
            {
                lblPasswordError.Text = "⚠ Eroare la schimbarea parolei!";
            }
        }

        private void BtnLightMode_Click(object sender, EventArgs e)
        {
            ChatWave.ThemeManager.SetDarkMode(false);
            lblThemeStatus.Text = "Tema curentă: ☀️ Light Mode";
            ApplyThemeToAllForms();
        }

        private void BtnDarkMode_Click(object sender, EventArgs e)
        {
            ChatWave.ThemeManager.SetDarkMode(true);
            lblThemeStatus.Text = "Tema curentă: 🌙 Dark Mode";
            ApplyThemeToAllForms();
        }

        private void ApplyThemeToAllForms()
        {
            foreach (Form form in Application.OpenForms)
            {
                ThemeManager.ApplyTheme(form);

                // Dacă formularul este MainChatForm, apelează metoda specifică de reîmprospătare
                if (form is MainChatForm mainForm)
                {
                    mainForm.RefreshTheme();
                }

                form.Invalidate(true);
                form.Refresh();
            }
        }
    }
}