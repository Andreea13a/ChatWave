using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChatWave.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            DesignUI();
        }

        private void DesignUI()
        {
            // FORM
            this.Text = "Login";
            this.Size = new Size(400, 520);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 245, 245);

            // HEADER
            Panel header = new Panel();
            header.BackColor = Color.FromArgb(167, 147, 214); // mov pastel
            header.Size = new Size(this.Width, 130);
            header.Location = new Point(0, 0);
            this.Controls.Add(header);

            Label title = new Label();
            title.Text = "✨ ChatWave";
            title.ForeColor = Color.White;
            title.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            title.AutoSize = true;
            title.Location = new Point(100, 40);
            header.Controls.Add(title);

            // USERNAME PANEL
            Panel userPanel = new Panel();
            userPanel.Size = new Size(260, 40);
            userPanel.Location = new Point(70, 170);
            userPanel.BackColor = Color.FromArgb(235, 230, 250);
            userPanel.BorderStyle = BorderStyle.None;
            this.Controls.Add(userPanel);

            Label userIcon = new Label();
            userIcon.Text = "👤";
            userIcon.Location = new Point(5, 8);
            userIcon.AutoSize = true;
            userPanel.Controls.Add(userIcon);

            TextBox txtUser = new TextBox();
            txtUser.BorderStyle = BorderStyle.None;
            txtUser.Text = "Username";
            txtUser.ForeColor = Color.Gray;

            txtUser.Enter += (s, e) =>
            {
                if (txtUser.Text == "Username")
                {
                    txtUser.Text = "";
                    txtUser.ForeColor = Color.Black;
                }
            };

            txtUser.Leave += (s, e) =>
            {
                if (txtUser.Text == "")
                {
                    txtUser.Text = "Username";
                    txtUser.ForeColor = Color.Gray;
                }
            };
            txtUser.Location = new Point(35, 10);
            txtUser.Width = 200;
            userPanel.Controls.Add(txtUser);

            // PASSWORD PANEL
            Panel passPanel = new Panel();
            passPanel.Size = new Size(260, 40);
            passPanel.Location = new Point(70, 230);
            passPanel.BackColor = Color.White;
            passPanel.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(passPanel);

            Label passIcon = new Label();
            passIcon.Text = "🔒";
            passIcon.Location = new Point(5, 8);
            passIcon.AutoSize = true;
            passPanel.Controls.Add(passIcon);

            TextBox txtPass = new TextBox();
            txtPass.BorderStyle = BorderStyle.None;
            txtPass.Text = "Parolă";
            txtPass.ForeColor = Color.Gray;

            txtPass.Enter += (s, e) =>
            {
                if (txtPass.Text == "Parolă")
                {
                    txtPass.Text = "";
                    txtPass.ForeColor = Color.Black;
                    txtPass.UseSystemPasswordChar = true;
                }
            };

            txtPass.Leave += (s, e) =>
            {
                if (txtPass.Text == "")
                {
                    txtPass.UseSystemPasswordChar = false;
                    txtPass.Text = "Parolă";
                    txtPass.ForeColor = Color.Gray;
                }
            };
            txtPass.UseSystemPasswordChar = true;
            txtPass.Location = new Point(35, 10);
            txtPass.Width = 200;
            passPanel.Controls.Add(txtPass);

            // LOGIN BUTTON
            Button btnLogin = new Button();
            btnLogin.Text = "✔ Autentifică-te";
            btnLogin.Size = new Size(260, 45);
            btnLogin.Location = new Point(70, 310);
            btnLogin.BackColor = Color.FromArgb(160, 130, 210);
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            // HOVER EFFECT
            btnLogin.MouseEnter += (s, e) =>
                btnLogin.BackColor = Color.FromArgb(140, 110, 200);

            btnLogin.MouseLeave += (s, e) =>
                btnLogin.BackColor = Color.FromArgb(160, 130, 210);

            btnLogin.Click += (s, e) =>
            {
                MessageBox.Show("Login apăsat!");
            };

            this.Controls.Add(btnLogin);

            // REGISTER TEXT
            Label register = new Label();
            register.Text = "Nu ai cont? Înregistrează-te";
            register.ForeColor = Color.Gray;
            register.AutoSize = true;
            register.Location = new Point(100, 370);
            this.Controls.Add(register);

            txtUser.BackColor = userPanel.BackColor;
            txtPass.BackColor = passPanel.BackColor;
            txtUser.ForeColor = Color.FromArgb(70, 70, 70);
            txtPass.ForeColor = Color.FromArgb(70, 70, 70);
            Panel line1 = new Panel();
            line1.BackColor = Color.FromArgb(200, 190, 230);
            line1.Size = new Size(260, 2);
            line1.Location = new Point(70, 210);
            this.Controls.Add(line1);

            Panel line2 = new Panel();
            line2.BackColor = Color.FromArgb(200, 190, 230);
            line2.Size = new Size(260, 2);
            line2.Location = new Point(70, 270);
            this.Controls.Add(line2);
        }
    }
    
}