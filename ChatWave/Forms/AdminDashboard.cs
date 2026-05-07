using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChatWave.Forms
{
    public partial class AdminDashboard : Form
    {
        public AdminDashboard()
        {
            InitializeComponent();
            DesignUI();
        }

        private void DesignUI()
        {
            // 🎨 Culori (aceleași ca login)
            Color movHeader = Color.FromArgb(167, 147, 214);
            Color movCard = Color.FromArgb(235, 230, 250);
            Color movButton = Color.FromArgb(160, 130, 210);
            Color textDark = Color.FromArgb(70, 70, 70);

            // FORM
            this.Text = "Admin Dashboard";
            this.Size = new Size(900, 600);
            this.BackColor = Color.FromArgb(245, 245, 245);

            // HEADER
            Panel header = new Panel();
            header.BackColor = movHeader;
            header.Size = new Size(this.Width, 80);
            header.Location = new Point(0, 0);
            this.Controls.Add(header);

            Label title = new Label();
            title.Text = "📊 Admin Dashboard";
            title.ForeColor = Color.White;
            title.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            title.AutoSize = true;
            title.Location = new Point(20, 25);
            header.Controls.Add(title);

            // 🔲 CARD FUNCTION
            Panel CreateCard(string text, string value, int x, int y)
            {
                Panel card = new Panel();
                card.Size = new Size(200, 100);
                card.Location = new Point(x, y);
                card.BackColor = movCard;

                Label lblValue = new Label();
                lblValue.Text = value;
                lblValue.Font = new Font("Segoe UI", 16, FontStyle.Bold);
                lblValue.ForeColor = textDark;
                lblValue.Location = new Point(10, 10);
                lblValue.AutoSize = true;

                Label lblText = new Label();
                lblText.Text = text;
                lblText.ForeColor = textDark;
                lblText.Location = new Point(10, 50);
                lblText.AutoSize = true;

                card.Controls.Add(lblValue);
                card.Controls.Add(lblText);

                return card;
            }

            // 📊 STATISTICI
            this.Controls.Add(CreateCard("Utilizatori", "1,245", 50, 120));
            this.Controls.Add(CreateCard("Online acum", "312", 270, 120));
            this.Controls.Add(CreateCard("Timp mediu", "45 min", 490, 120));
            this.Controls.Add(CreateCard("Venit lunar", "$3,200", 710, 120));

            // 👥 LISTĂ UTILIZATORI
            Label usersLabel = new Label();
            usersLabel.Text = "👥 Utilizatori activi";
            usersLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            usersLabel.Location = new Point(50, 250);
            usersLabel.AutoSize = true;
            this.Controls.Add(usersLabel);

            ListBox usersList = new ListBox();
            usersList.Size = new Size(350, 200);
            usersList.Location = new Point(50, 280);
            usersList.BackColor = movCard;

            // exemplu date
            usersList.Items.Add("andreea123 - 2h");
            usersList.Items.Add("mihai_dev - 1h 20m");
            usersList.Items.Add("alex_ion - 50m");

            this.Controls.Add(usersList);

            // 💳 PLĂȚI PREMIUM
            Label paymentsLabel = new Label();
            paymentsLabel.Text = "💳 Plăți Premium";
            paymentsLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            paymentsLabel.Location = new Point(450, 250);
            paymentsLabel.AutoSize = true;
            this.Controls.Add(paymentsLabel);

            ListBox paymentsList = new ListBox();
            paymentsList.Size = new Size(350, 200);
            paymentsList.Location = new Point(450, 280);
            paymentsList.BackColor = movCard;

            paymentsList.Items.Add("andreea123 - $5");
            paymentsList.Items.Add("mihai_dev - $10");
            paymentsList.Items.Add("alex_ion - $5");

            this.Controls.Add(paymentsList);

            // 🔘 LOGOUT BUTTON
            Button logout = new Button();
            logout.Text = "Logout";
            logout.Size = new Size(100, 35);
            logout.Location = new Point(750, 20);
            logout.BackColor = movButton;
            logout.ForeColor = Color.White;
            logout.FlatStyle = FlatStyle.Flat;
            logout.FlatAppearance.BorderSize = 0;

            logout.Click += (s, e) =>
            {
                this.Hide();
                new LoginForm().Show();
            };

            header.Controls.Add(logout);
        }
    }
}