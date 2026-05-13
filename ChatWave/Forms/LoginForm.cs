using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChatWave.Forms
{
    public partial class LoginForm : Form
    {
        private TextBox txtUser;
        private TextBox txtPass;
        private Label lblError;

        public LoginForm()
        {
            InitializeComponent();
            DesignUI();
            this.WindowState = FormWindowState.Maximized;
        }

        private void DesignUI()
        {
            // FORM - SETĂRI FULLSCREEN
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;

            // HEADER (ACEEAȘI CULOARE CA LA REGISTER)
            Panel pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 120;
            pnlHeader.BackColor = ColorTranslator.FromHtml("#B39DDB");

            Label lblTitle = new Label();
            lblTitle.Text = "✨ ChatWave";
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Location = new Point(0, 20);
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 45;

            Label lblSubtitle = new Label();
            lblSubtitle.Text = "Autentifică-te în cont 🚀";
            lblSubtitle.ForeColor = ColorTranslator.FromHtml("#EDE7F6");
            lblSubtitle.Font = new Font("Segoe UI", 11);
            lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            lblSubtitle.Location = new Point(0, 70);
            lblSubtitle.Dock = DockStyle.Top;
            lblSubtitle.Height = 40;

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);

            // CENTER PANEL
            Panel pnlCenter = new Panel();
            pnlCenter.Dock = DockStyle.Fill;
            pnlCenter.BackColor = Color.White;
            pnlCenter.AutoScroll = true;

            // USERNAME
            Label lblUsername = new Label();
            lblUsername.Text = "👤  Username";
            lblUsername.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblUsername.ForeColor = ColorTranslator.FromHtml("#4A4A4A");
            lblUsername.Location = new Point(40, 25);
            lblUsername.AutoSize = true;

            txtUser = new TextBox();
            txtUser.Location = new Point(40, 50);
            txtUser.Size = new Size(340, 35);
            txtUser.Font = new Font("Segoe UI", 11);
            txtUser.BorderStyle = BorderStyle.FixedSingle;
            txtUser.BackColor = ColorTranslator.FromHtml("#F3F0FF");
            StyleTextBox(txtUser);

            // PASSWORD
            Label lblPassword = new Label();
            lblPassword.Text = "🔒  Parolă";
            lblPassword.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblPassword.ForeColor = ColorTranslator.FromHtml("#4A4A4A");
            lblPassword.Location = new Point(40, 100);
            lblPassword.AutoSize = true;

            txtPass = new TextBox();
            txtPass.Location = new Point(40, 125);
            txtPass.Size = new Size(340, 35);
            txtPass.Font = new Font("Segoe UI", 11);
            txtPass.BorderStyle = BorderStyle.FixedSingle;
            txtPass.PasswordChar = '*';
            txtPass.BackColor = ColorTranslator.FromHtml("#F3F0FF");
            StyleTextBox(txtPass);

            // ERROR LABEL
            lblError = new Label();
            lblError.Text = "";
            lblError.ForeColor = ColorTranslator.FromHtml("#E74C3C");
            lblError.Font = new Font("Segoe UI", 9);
            lblError.Location = new Point(40, 175);
            lblError.Size = new Size(340, 20);

            // LOGIN BUTTON
            Button btnLogin = new Button();
            btnLogin.Text = "✔  Autentifică-te";
            btnLogin.Location = new Point(40, 205);
            btnLogin.Size = new Size(340, 45);
            btnLogin.BackColor = ColorTranslator.FromHtml("#B39DDB");
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.Click += BtnLogin_Click;
            btnLogin.MouseEnter += (s, e) =>
                btnLogin.BackColor = ColorTranslator.FromHtml("#9575CD");
            btnLogin.MouseLeave += (s, e) =>
                btnLogin.BackColor = ColorTranslator.FromHtml("#B39DDB");

            // SEPARATOR
            Label lblOr = new Label();
            lblOr.Text = "─────────── sau ───────────";
            lblOr.Font = new Font("Segoe UI", 9);
            lblOr.ForeColor = ColorTranslator.FromHtml("#AAAAAA");
            lblOr.Location = new Point(55, 265);
            lblOr.AutoSize = true;

            // REGISTER BUTTON
            Button btnGoRegister = new Button();
            btnGoRegister.Text = "📝  Nu ai cont? Înregistrează-te";
            btnGoRegister.Location = new Point(40, 295);
            btnGoRegister.Size = new Size(340, 42);
            btnGoRegister.BackColor = Color.White;
            btnGoRegister.ForeColor = ColorTranslator.FromHtml("#B39DDB");
            btnGoRegister.FlatStyle = FlatStyle.Flat;
            btnGoRegister.FlatAppearance.BorderSize = 1;
            btnGoRegister.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#B39DDB");
            btnGoRegister.Font = new Font("Segoe UI", 11);
            btnGoRegister.Cursor = Cursors.Hand;
            btnGoRegister.Click += (s, e) =>
            {
                this.Hide();
                RegisterForm register = new RegisterForm();
                register.Show();
            };
            btnGoRegister.MouseEnter += (s, e) =>
                btnGoRegister.BackColor = ColorTranslator.FromHtml("#F3F0FF");
            btnGoRegister.MouseLeave += (s, e) =>
                btnGoRegister.BackColor = Color.White;

            pnlCenter.Controls.Add(lblUsername);
            pnlCenter.Controls.Add(txtUser);
            pnlCenter.Controls.Add(lblPassword);
            pnlCenter.Controls.Add(txtPass);
            pnlCenter.Controls.Add(lblError);
            pnlCenter.Controls.Add(btnLogin);
            pnlCenter.Controls.Add(lblOr);
            pnlCenter.Controls.Add(btnGoRegister);

            this.Controls.Add(pnlCenter);
            this.Controls.Add(pnlHeader);

            // CENTRALIZARE CONTROALE
            pnlCenter.Resize += (s, e) =>
            {
                int centerX = (pnlCenter.Width - 340) / 2;

                txtUser.Left = centerX;
                txtPass.Left = centerX;
                lblUsername.Left = centerX;
                lblPassword.Left = centerX;
                lblError.Left = centerX;
                btnLogin.Left = centerX;
                btnGoRegister.Left = centerX;
                lblOr.Left = centerX + 15;
            };
        }

        private void StyleTextBox(TextBox txt)
        {
            txt.Enter += (s, e) =>
                txt.BackColor = ColorTranslator.FromHtml("#EDE7F6");
            txt.Leave += (s, e) =>
                txt.BackColor = ColorTranslator.FromHtml("#F3F0FF");
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            lblError.Text = "";

            string username = txtUser.Text.Trim();
            string password = txtPass.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "⚠ Completează toate câmpurile!";
                return;
            }

            try
            {
                var user = UserService.Login(username, password);

                if (user == null)
                {
                    lblError.Text = "❌ Username sau parolă incorectă!";
                    return;
                }

                this.Hide();
                MainChatForm mainForm = new MainChatForm(user);
                mainForm.FormClosed += (snd, args) => this.Close();
                mainForm.Show();
            }
            catch (Exception ex)
            {
                lblError.Text = $"⚠ Eroare: {ex.Message}";
            }
        }
    }
}