using System;
using System.Drawing;
using System.Windows.Forms;
using ChatWave.Data;

namespace ChatWave.Forms
{
    public partial class ProfileForm : BaseForm
    {
        private LoggedUser currentUser;
        private Label lblEmailValue;
        private Label lblPhoneValue;
        private Label lblSuccessMsg;

        public ProfileForm(LoggedUser user)
        {
            currentUser = user;
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            Color movHeader = Color.FromArgb(167, 147, 214);
            Color movCard = Color.FromArgb(235, 230, 250);

            this.Text = "Profilul meu";
            this.Size = new Size(380, 600); // ← mărit
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // HEADER
            Panel pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 140;
            pnlHeader.BackColor = movHeader;

            // AVATAR
            Panel pnlAvatar = new Panel();
            pnlAvatar.Size = new Size(80, 80);
            pnlAvatar.Location = new Point(145, 20);
            pnlAvatar.BackColor = Color.White;

            Label lblInitials = new Label();
            lblInitials.Text = currentUser.Username.Substring(0, 1).ToUpper();
            lblInitials.Font = new Font("Segoe UI", 28, FontStyle.Bold);
            lblInitials.ForeColor = movHeader;
            lblInitials.TextAlign = ContentAlignment.MiddleCenter;
            lblInitials.Dock = DockStyle.Fill;
            pnlAvatar.Controls.Add(lblInitials);

            pnlAvatar.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.FillEllipse(Brushes.White,
                    0, 0, pnlAvatar.Width - 1, pnlAvatar.Height - 1);
            };

            Label lblUsername = new Label();
            lblUsername.Text = currentUser.Username;
            lblUsername.ForeColor = Color.White;
            lblUsername.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblUsername.TextAlign = ContentAlignment.MiddleCenter;
            lblUsername.Location = new Point(0, 105);
            lblUsername.Size = new Size(380, 30);

            pnlHeader.Controls.Add(pnlAvatar);
            pnlHeader.Controls.Add(lblUsername);

            // DETALII
            Panel pnlDetails = new Panel();
            pnlDetails.Location = new Point(20, 160);
            pnlDetails.Size = new Size(330, 290);
            pnlDetails.BackColor = Color.White;

            // Username row
            AddDetailRow(pnlDetails, "👤 Username", currentUser.Username, 0);

            // Rol row
            AddDetailRow(pnlDetails, "🛡️ Rol", currentUser.Role, 70);

            // Email row — cu referință la label valoare
            Panel emailRow = CreateRow(movCard, "📧 Email",
                currentUser.Email ?? "-", 140, out lblEmailValue);
            pnlDetails.Controls.Add(emailRow);

            // Telefon row — cu referință la label valoare
            Panel phoneRow = CreateRow(movCard, "📱 Telefon",
                currentUser.Phone ?? "-", 210, out lblPhoneValue);
            pnlDetails.Controls.Add(phoneRow);

            // MESAJ SUCCES
            lblSuccessMsg = new Label();
            lblSuccessMsg.Text = "";
            lblSuccessMsg.ForeColor = Color.FromArgb(39, 174, 96);
            lblSuccessMsg.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblSuccessMsg.Location = new Point(20, 460);
            lblSuccessMsg.Size = new Size(330, 20);
            lblSuccessMsg.TextAlign = ContentAlignment.MiddleCenter;

            // BUTON EDITARE
            Button btnSettings = new Button();
            btnSettings.Text = "⚙️ Editează profilul";
            btnSettings.Location = new Point(20, 490);
            btnSettings.Size = new Size(330, 42);
            btnSettings.BackColor = movHeader;
            btnSettings.ForeColor = Color.White;
            btnSettings.FlatStyle = FlatStyle.Flat;
            btnSettings.FlatAppearance.BorderSize = 0;
            btnSettings.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnSettings.Cursor = Cursors.Hand;
            btnSettings.MouseEnter += (s, e) =>
                btnSettings.BackColor = Color.FromArgb(140, 110, 190);
            btnSettings.MouseLeave += (s, e) =>
                btnSettings.BackColor = movHeader;
            btnSettings.Click += (s, e) =>
            {
                SettingsForm settings = new SettingsForm(currentUser);
                settings.ShowDialog();

                // Reîncarcă datele după editare
                lblUsername.Text = currentUser.Username;
                lblEmailValue.Text = currentUser.Email ?? "-";
                lblPhoneValue.Text = currentUser.Phone ?? "-";
                lblSuccessMsg.Text = "✅ Profilul a fost actualizat!";
            };

            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlDetails);
            this.Controls.Add(lblSuccessMsg);
            this.Controls.Add(btnSettings);
        }

        // Row simplu fara referinta
        private void AddDetailRow(Panel parent, string label, string value, int y)
        {
            Color movCard = Color.FromArgb(235, 230, 250);
            Panel row = new Panel();
            row.Location = new Point(0, y);
            row.Size = new Size(330, 60);
            row.BackColor = movCard;

            Label lblLabel = new Label();
            lblLabel.Text = label;
            lblLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lblLabel.ForeColor = Color.FromArgb(120, 80, 180);
            lblLabel.Location = new Point(12, 8);
            lblLabel.AutoSize = true;

            Label lblValue = new Label();
            lblValue.Text = value;
            lblValue.Font = new Font("Segoe UI", 11);
            lblValue.ForeColor = Color.FromArgb(50, 50, 50);
            lblValue.Location = new Point(12, 28);
            lblValue.AutoSize = true;

            row.Controls.Add(lblLabel);
            row.Controls.Add(lblValue);
            parent.Controls.Add(row);
        }

        // Row cu referinta la label valoare
        private Panel CreateRow(Color bgColor, string label,
            string value, int y, out Label valueLabel)
        {
            Panel row = new Panel();
            row.Location = new Point(0, y);
            row.Size = new Size(330, 60);
            row.BackColor = bgColor;

            Label lblLabel = new Label();
            lblLabel.Text = label;
            lblLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lblLabel.ForeColor = Color.FromArgb(120, 80, 180);
            lblLabel.Location = new Point(12, 8);
            lblLabel.AutoSize = true;

            valueLabel = new Label();
            valueLabel.Text = value;
            valueLabel.Font = new Font("Segoe UI", 11);
            valueLabel.ForeColor = Color.FromArgb(50, 50, 50);
            valueLabel.Location = new Point(12, 28);
            valueLabel.AutoSize = true;

            row.Controls.Add(lblLabel);
            row.Controls.Add(valueLabel);
            return row;
        }
    }
}