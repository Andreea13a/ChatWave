using ChatWave.Data;
using ChatWave.Models;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ChatWave.Forms
{
    public partial class RegisterForm : Form
    {
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private Panel pnlCenter;
        private Label lblUsername;
        private TextBox txtUsername;
        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblPhone;
        private TextBox txtPhone;
        private Label lblPassword;
        private TextBox txtPassword;
        private Label lblConfirm;
        private TextBox txtConfirmPassword;
        private Button btnRegister;
        private Button btnGoLogin;
        private Label lblError;
        private Label lblSuccess;
        private Label lblOr;

        public RegisterForm()
        {
            InitializeComponent();
            InitializeUI();
            this.WindowState = FormWindowState.Maximized;
        }

        private void InitializeUI()
        {
            // FORM

            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            // HEADER
            pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 120;
            pnlHeader.BackColor = ColorTranslator.FromHtml("#B39DDB");

            lblTitle = new Label();
            lblTitle.Text = "✨ ChatWave";
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Location = new Point(0, 20);
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 45;

            lblSubtitle = new Label();
            lblSubtitle.Text = "Creează un cont nou 🚀";
            lblSubtitle.ForeColor = ColorTranslator.FromHtml("#EDE7F6");
            lblSubtitle.Font = new Font("Segoe UI", 11);
            lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            lblSubtitle.Location = new Point(0, 70);
            lblSubtitle.Dock = DockStyle.Bottom;
            lblSubtitle.Height = 40;

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);

            // CENTER PANEL
            pnlCenter = new Panel();
            pnlCenter.Dock = DockStyle.Fill;
            pnlCenter.BackColor = Color.White;
            pnlCenter.AutoScroll = true;

            // USERNAME
            lblUsername = new Label();
            lblUsername.Text = "👤  Username";
            lblUsername.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblUsername.ForeColor = ColorTranslator.FromHtml("#4A4A4A");
            lblUsername.Location = new Point(40, 25);
            lblUsername.AutoSize = true;

            txtUsername = new TextBox();
            txtUsername.Location = new Point(40, 50);
            txtUsername.Size = new Size(340, 35);
            txtUsername.Font = new Font("Segoe UI", 11);
            txtUsername.BorderStyle = BorderStyle.FixedSingle;
            txtUsername.BackColor = ColorTranslator.FromHtml("#F3F0FF");
            StyleTextBox(txtUsername);

            // EMAIL
            lblEmail = new Label();
            lblEmail.Text = "📧  Email";
            lblEmail.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblEmail.ForeColor = ColorTranslator.FromHtml("#4A4A4A");
            lblEmail.Location = new Point(40, 100);
            lblEmail.AutoSize = true;

            txtEmail = new TextBox();
            txtEmail.Location = new Point(40, 125);
            txtEmail.Size = new Size(340, 35);
            txtEmail.Font = new Font("Segoe UI", 11);
            txtEmail.BorderStyle = BorderStyle.FixedSingle;
            txtEmail.BackColor = ColorTranslator.FromHtml("#F3F0FF");
            StyleTextBox(txtEmail);

            // PHONE
            lblPhone = new Label();
            lblPhone.Text = "📱  Număr de telefon";
            lblPhone.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblPhone.ForeColor = ColorTranslator.FromHtml("#4A4A4A");
            lblPhone.Location = new Point(40, 175);
            lblPhone.AutoSize = true;

            txtPhone = new TextBox();
            txtPhone.Location = new Point(40, 200);
            txtPhone.Size = new Size(340, 35);
            txtPhone.Font = new Font("Segoe UI", 11);
            txtPhone.BorderStyle = BorderStyle.FixedSingle;
            txtPhone.BackColor = ColorTranslator.FromHtml("#F3F0FF");
            StyleTextBox(txtPhone);

            // PASSWORD
            lblPassword = new Label();
            lblPassword.Text = "🔒  Parolă";
            lblPassword.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblPassword.ForeColor = ColorTranslator.FromHtml("#4A4A4A");
            lblPassword.Location = new Point(40, 250);
            lblPassword.AutoSize = true;

            txtPassword = new TextBox();
            txtPassword.Location = new Point(40, 275);
            txtPassword.Size = new Size(340, 35);
            txtPassword.Font = new Font("Segoe UI", 11);
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.PasswordChar = '*';
            txtPassword.BackColor = ColorTranslator.FromHtml("#F3F0FF");
            StyleTextBox(txtPassword);

            // CONFIRM PASSWORD
            lblConfirm = new Label();
            lblConfirm.Text = "🔑  Confirmă Parola";
            lblConfirm.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblConfirm.ForeColor = ColorTranslator.FromHtml("#4A4A4A");
            lblConfirm.Location = new Point(40, 325);
            lblConfirm.AutoSize = true;

            txtConfirmPassword = new TextBox();
            txtConfirmPassword.Location = new Point(40, 350);
            txtConfirmPassword.Size = new Size(340, 35);
            txtConfirmPassword.Font = new Font("Segoe UI", 11);
            txtConfirmPassword.BorderStyle = BorderStyle.FixedSingle;
            txtConfirmPassword.PasswordChar = '*';
            txtConfirmPassword.BackColor = ColorTranslator.FromHtml("#F3F0FF");
            StyleTextBox(txtConfirmPassword);

            // ERROR LABEL
            lblError = new Label();
            lblError.Text = "";
            lblError.ForeColor = ColorTranslator.FromHtml("#E74C3C");
            lblError.Font = new Font("Segoe UI", 9);
            lblError.Location = new Point(40, 398);
            lblError.Size = new Size(340, 20);

            // SUCCESS LABEL
            lblSuccess = new Label();
            lblSuccess.Text = "";
            lblSuccess.ForeColor = ColorTranslator.FromHtml("#27AE60");
            lblSuccess.Font = new Font("Segoe UI", 9);
            lblSuccess.Location = new Point(40, 398);
            lblSuccess.Size = new Size(340, 20);

            // REGISTER BUTTON
            btnRegister = new Button();
            btnRegister.Text = "✔  Înregistrează-te";
            btnRegister.Location = new Point(40, 428);
            btnRegister.Size = new Size(340, 45);
            btnRegister.BackColor = ColorTranslator.FromHtml("#B39DDB");
            btnRegister.ForeColor = Color.White;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnRegister.Cursor = Cursors.Hand;
            btnRegister.Click += BtnRegister_Click;
            btnRegister.MouseEnter += (s, e) =>
                btnRegister.BackColor = ColorTranslator.FromHtml("#9575CD");
            btnRegister.MouseLeave += (s, e) =>
                btnRegister.BackColor = ColorTranslator.FromHtml("#B39DDB");

            // SEPARATOR
            lblOr = new Label();
            lblOr.Text = "─────────── sau ───────────";
            lblOr.Font = new Font("Segoe UI", 9);
            lblOr.ForeColor = ColorTranslator.FromHtml("#AAAAAA");
            lblOr.Location = new Point(55, 485);
            lblOr.AutoSize = true;

            // BACK TO LOGIN BUTTON
            btnGoLogin = new Button();
            btnGoLogin.Text = "🔑  Ai cont? Autentifică-te";
            btnGoLogin.Location = new Point(40, 515);
            btnGoLogin.Size = new Size(340, 42);
            btnGoLogin.BackColor = Color.White;
            btnGoLogin.ForeColor = ColorTranslator.FromHtml("#B39DDB");
            btnGoLogin.FlatStyle = FlatStyle.Flat;
            btnGoLogin.FlatAppearance.BorderSize = 1;
            btnGoLogin.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#B39DDB");
            btnGoLogin.Font = new Font("Segoe UI", 11);
            btnGoLogin.Cursor = Cursors.Hand;
            btnGoLogin.Click += BtnGoLogin_Click;
            btnGoLogin.MouseEnter += (s, e) =>
                btnGoLogin.BackColor = ColorTranslator.FromHtml("#F3F0FF");
            btnGoLogin.MouseLeave += (s, e) =>
                btnGoLogin.BackColor = Color.White;

            pnlCenter.Controls.Add(lblUsername);
            pnlCenter.Controls.Add(txtUsername);
            pnlCenter.Controls.Add(lblEmail);
            pnlCenter.Controls.Add(txtEmail);
            pnlCenter.Controls.Add(lblPhone);
            pnlCenter.Controls.Add(txtPhone);
            pnlCenter.Controls.Add(lblPassword);
            pnlCenter.Controls.Add(txtPassword);
            pnlCenter.Controls.Add(lblConfirm);
            pnlCenter.Controls.Add(txtConfirmPassword);
            pnlCenter.Controls.Add(lblError);
            pnlCenter.Controls.Add(lblSuccess);
            pnlCenter.Controls.Add(btnRegister);
            pnlCenter.Controls.Add(lblOr);
            pnlCenter.Controls.Add(btnGoLogin);

            this.Controls.Add(pnlCenter);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlCenter);
            this.Controls.Add(pnlHeader);

            pnlCenter.Resize += (s, e) =>
            {
                int centerX = (pnlCenter.Width - 340) / 2;

                txtUsername.Left = centerX;
                txtEmail.Left = centerX;
                txtPhone.Left = centerX;
                txtPassword.Left = centerX;
                txtConfirmPassword.Left = centerX;

                lblUsername.Left = centerX;
                lblEmail.Left = centerX;
                lblPhone.Left = centerX;
                lblPassword.Left = centerX;
                lblConfirm.Left = centerX;

                lblError.Left = centerX;
                lblSuccess.Left = centerX;

                btnRegister.Left = centerX;
                btnGoLogin.Left = centerX;

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

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            lblSuccess.Text = "";

            if (string.IsNullOrEmpty(txtUsername.Text) ||
                string.IsNullOrEmpty(txtEmail.Text) ||
                string.IsNullOrEmpty(txtPhone.Text) ||
                string.IsNullOrEmpty(txtPassword.Text) ||
                string.IsNullOrEmpty(txtConfirmPassword.Text))
            {
                lblError.Text = "⚠ Completează toate câmpurile!";
                return;
            }

            if (!Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                lblError.Text = "⚠ Email invalid!";
                return;
            }

            if (!Regex.IsMatch(txtPhone.Text, @"^\+?[0-9]{9,13}$"))
            {
                lblError.Text = "⚠ Număr de telefon invalid!";
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                lblError.Text = "⚠ Parolele nu coincid!";
                return;
            }

            if (txtPassword.Text.Length < 6)
            {
                lblError.Text = "⚠ Parola trebuie să aibă minim 6 caractere!";
                return;
            }
            User user = new User
            {
                Username = txtUsername.Text,
                Email = txtEmail.Text,
                Phone = txtPhone.Text,
                Password = txtPassword.Text,
                Role = "user",
                CreatedAt = DateTime.Now
            };

            bool success = UserRepository.AddUser(user);

            if (!success)
            {
                lblError.Text = "⚠ Eroare la salvare!";
                return;
            }

            lblSuccess.Text = "✔ Cont creat cu succes!";
            MessageBox.Show("✔ Cont creat cu succes! Te redirecționăm la login...");
            this.Hide();
            LoginForm login = new LoginForm();
            login.Show();
           

        }
        

        private void BtnGoLogin_Click(object sender, EventArgs e)
        {
            this.Hide();
            LoginForm login = new LoginForm();
            login.Show();
          
        }
    }
}