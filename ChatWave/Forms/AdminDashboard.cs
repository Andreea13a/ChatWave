using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ChatWave.Data;
using ChatWave.Models;
using Message = ChatWave.Models.Message;

namespace ChatWave.Forms
{
    public partial class AdminDashboard : BaseForm
    {
        public TabControl tabControl;
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblAdminName;
        private TabPage tabUsers;
        private TabPage tabMessages;
        private TabPage tabStats;
        private DataGridView dgvUsers;
        private DataGridView dgvMessages;
        private Button btnDeleteUser;
        private Button btnAddUser;
        private Button btnDeleteMessage;
        private TextBox txtSearch;
        private Panel pnlUsersBottom;
        private Panel pnlMessagesBottom;

        public AdminDashboard()
        {
            InitializeComponent();
            InitializeUI();
            LoadUsers();
            LoadMessages();
            LoadStats();
        }

        private void InitializeUI()
        {
            Color movHeader = Color.FromArgb(167, 147, 214);
            Color movButton = Color.FromArgb(160, 130, 210);

            this.Text = "ChatWave - Admin";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // HEADER
            pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 55;
            pnlHeader.BackColor = movHeader;

            lblTitle = new Label();
            lblTitle.Text = "📊 ChatWave — Admin";
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.Location = new Point(15, 14);
            lblTitle.AutoSize = true;

            lblAdminName = new Label();
            lblAdminName.Text = "Admin";
            lblAdminName.ForeColor = Color.White;
            lblAdminName.Font = new Font("Segoe UI", 10);
            lblAdminName.Location = new Point(720, 18);
            lblAdminName.AutoSize = true;

            Button btnBack = new Button();
            btnBack.Text = "← Chat";
            btnBack.Size = new Size(80, 32);
            btnBack.Location = new Point(800, 12);
            btnBack.BackColor = Color.FromArgb(140, 110, 190);
            btnBack.ForeColor = Color.White;
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Font = new Font("Segoe UI", 9);
            btnBack.Cursor = Cursors.Hand;
            btnBack.Click += (s, e) => this.Close();

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblAdminName);
            pnlHeader.Controls.Add(btnBack);

            // TAB CONTROL
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new Font("Segoe UI", 10);

            // TAB UTILIZATORI
            tabUsers = new TabPage();
            tabUsers.Text = "👥 Utilizatori";
            tabUsers.BackColor = Color.White;

            txtSearch = new TextBox();
            txtSearch.Dock = DockStyle.Top;
            txtSearch.Height = 35;
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.Text = "Caută utilizator...";
            txtSearch.ForeColor = Color.Gray;
            txtSearch.GotFocus += (s, e) => {
                if (txtSearch.Text == "Caută utilizator...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = Color.Black;
                }
            };
            txtSearch.LostFocus += (s, e) => {
                if (txtSearch.Text == "")
                {
                    txtSearch.Text = "Caută utilizator...";
                    txtSearch.ForeColor = Color.Gray;
                }
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            dgvUsers = new DataGridView();
            dgvUsers.Dock = DockStyle.Fill;
            dgvUsers.BackgroundColor = Color.White;
            dgvUsers.BorderStyle = BorderStyle.None;
            dgvUsers.RowHeadersVisible = false;
            dgvUsers.AllowUserToAddRows = false;
            dgvUsers.ReadOnly = true;
            dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsers.Font = new Font("Segoe UI", 10);
            dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(235, 230, 250);
            dgvUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvUsers.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 242, 255);

            pnlUsersBottom = new Panel();
            pnlUsersBottom.Dock = DockStyle.Bottom;
            pnlUsersBottom.Height = 50;
            pnlUsersBottom.BackColor = Color.FromArgb(235, 230, 250);

            btnAddUser = new Button();
            btnAddUser.Text = "＋ Adaugă";
            btnAddUser.Size = new Size(120, 34);
            btnAddUser.Location = new Point(10, 8);
            btnAddUser.BackColor = movButton;
            btnAddUser.ForeColor = Color.White;
            btnAddUser.FlatStyle = FlatStyle.Flat;
            btnAddUser.FlatAppearance.BorderSize = 0;
            btnAddUser.Font = new Font("Segoe UI", 10);
            btnAddUser.Cursor = Cursors.Hand;
            btnAddUser.Click += BtnAddUser_Click;

            btnDeleteUser = new Button();
            btnDeleteUser.Text = "🗑 Șterge";
            btnDeleteUser.Size = new Size(120, 34);
            btnDeleteUser.Location = new Point(140, 8);
            btnDeleteUser.BackColor = Color.FromArgb(192, 57, 43);
            btnDeleteUser.ForeColor = Color.White;
            btnDeleteUser.FlatStyle = FlatStyle.Flat;
            btnDeleteUser.FlatAppearance.BorderSize = 0;
            btnDeleteUser.Font = new Font("Segoe UI", 10);
            btnDeleteUser.Cursor = Cursors.Hand;
            btnDeleteUser.Click += BtnDeleteUser_Click;

            pnlUsersBottom.Controls.Add(btnAddUser);
            pnlUsersBottom.Controls.Add(btnDeleteUser);

            tabUsers.Controls.Add(dgvUsers);
            tabUsers.Controls.Add(txtSearch);
            tabUsers.Controls.Add(pnlUsersBottom);

            // TAB MESAJE
            tabMessages = new TabPage();
            tabMessages.Text = "💬 Mesaje";
            tabMessages.BackColor = Color.White;

            dgvMessages = new DataGridView();
            dgvMessages.Dock = DockStyle.Fill;
            dgvMessages.BackgroundColor = Color.White;
            dgvMessages.BorderStyle = BorderStyle.None;
            dgvMessages.RowHeadersVisible = false;
            dgvMessages.AllowUserToAddRows = false;
            dgvMessages.ReadOnly = true;
            dgvMessages.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMessages.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMessages.Font = new Font("Segoe UI", 10);
            dgvMessages.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(235, 230, 250);
            dgvMessages.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvMessages.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 242, 255);

            pnlMessagesBottom = new Panel();
            pnlMessagesBottom.Dock = DockStyle.Bottom;
            pnlMessagesBottom.Height = 50;
            pnlMessagesBottom.BackColor = Color.FromArgb(235, 230, 250);

            btnDeleteMessage = new Button();
            btnDeleteMessage.Text = "🗑 Șterge mesaj";
            btnDeleteMessage.Size = new Size(150, 34);
            btnDeleteMessage.Location = new Point(10, 8);
            btnDeleteMessage.BackColor = Color.FromArgb(192, 57, 43);
            btnDeleteMessage.ForeColor = Color.White;
            btnDeleteMessage.FlatStyle = FlatStyle.Flat;
            btnDeleteMessage.FlatAppearance.BorderSize = 0;
            btnDeleteMessage.Font = new Font("Segoe UI", 10);
            btnDeleteMessage.Cursor = Cursors.Hand;
            btnDeleteMessage.Click += BtnDeleteMessage_Click;

            pnlMessagesBottom.Controls.Add(btnDeleteMessage);

            tabMessages.Controls.Add(dgvMessages);
            tabMessages.Controls.Add(pnlMessagesBottom);

            // TAB STATISTICI
            tabStats = new TabPage();
            tabStats.Text = "📊 Statistici";
            tabStats.BackColor = Color.White;

            tabControl.TabPages.Add(tabUsers);
            tabControl.TabPages.Add(tabMessages);
            tabControl.TabPages.Add(tabStats);

            this.Controls.Add(tabControl);
            this.Controls.Add(pnlHeader);
        }

        private void LoadUsers()
        {
            dgvUsers.DataSource = null;
            List<User> users = UserRepository.GetAllUsers();

            DataTable table = new DataTable();
            table.Columns.Add("Id");
            table.Columns.Add("Username");
            table.Columns.Add("Email");
            table.Columns.Add("Telefon");
            table.Columns.Add("Rol");
            table.Columns.Add("Data înregistrării");

            foreach (User u in users)
            {
                table.Rows.Add(
                    u.Id,
                    u.Username,
                    u.Email ?? "-",
                    u.Phone ?? "-",
                    u.Role,
                    u.CreatedAt.ToString("dd.MM.yyyy HH:mm")
                );
            }
            dgvUsers.DataSource = table;
        }

        private void LoadMessages()
        {
            dgvMessages.DataSource = null;
            List<Message> messages = MessageRepository.GetAllMessages();

            DataTable table = new DataTable();
            table.Columns.Add("Id");
            table.Columns.Add("Expeditor");
            table.Columns.Add("Mesaj");
            table.Columns.Add("Data");

            foreach (Message m in messages)
            {
                table.Rows.Add(
                    m.Id,
                    m.SenderName,
                    m.Text,
                    m.SentAt.ToString("dd.MM.yyyy HH:mm")
                );
            }
            dgvMessages.DataSource = table;
        }

        private void LoadStats()
        {
            tabStats.Controls.Clear();

            Panel statsPanel = new Panel();
            statsPanel.Dock = DockStyle.Fill;
            statsPanel.BackColor = Color.White;

            int totalUsers = UserRepository.GetAllUsers().Count;
            int totalMessages = MessageRepository.GetAllMessages().Count;
            int adminCount = 0;
            foreach (User u in UserRepository.GetAllUsers())
                if (u.Role == "admin") adminCount++;

            statsPanel.Controls.Add(CreateStatCard(
                "👥 Total utilizatori", totalUsers.ToString(),
                Color.FromArgb(167, 147, 214), 20, 20));

            statsPanel.Controls.Add(CreateStatCard(
                "💬 Total mesaje", totalMessages.ToString(),
                Color.FromArgb(100, 180, 210), 250, 20));

            statsPanel.Controls.Add(CreateStatCard(
                "🛡️ Administratori", adminCount.ToString(),
                Color.FromArgb(100, 180, 130), 480, 20));

            statsPanel.Controls.Add(CreateStatCard(
                "👤 Utilizatori simpli", (totalUsers - adminCount).ToString(),
                Color.FromArgb(210, 150, 100), 20, 160));

            tabStats.Controls.Add(statsPanel);
        }

        private Panel CreateStatCard(string title, string value, Color color, int x, int y)
        {
            Panel card = new Panel();
            card.Size = new Size(200, 120);
            card.Location = new Point(x, y);
            card.BackColor = Color.FromArgb(235, 230, 250);

            Panel colorBar = new Panel();
            colorBar.Size = new Size(200, 8);
            colorBar.Location = new Point(0, 0);
            colorBar.BackColor = color;
            card.Controls.Add(colorBar);

            Label lblValue = new Label();
            lblValue.Text = value;
            lblValue.Font = new Font("Segoe UI", 28, FontStyle.Bold);
            lblValue.ForeColor = color;
            lblValue.Location = new Point(15, 20);
            lblValue.AutoSize = true;
            card.Controls.Add(lblValue);

            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 9);
            lblTitle.ForeColor = Color.FromArgb(100, 100, 100);
            lblTitle.Location = new Point(15, 75);
            lblTitle.AutoSize = true;
            card.Controls.Add(lblTitle);

            return card;
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Caută utilizator..." || txtSearch.Text == "")
            {
                LoadUsers();
                return;
            }

            List<User> allUsers = UserRepository.GetAllUsers();
            DataTable table = new DataTable();
            table.Columns.Add("Id");
            table.Columns.Add("Username");
            table.Columns.Add("Email");
            table.Columns.Add("Telefon");
            table.Columns.Add("Rol");
            table.Columns.Add("Data înregistrării");

            foreach (User u in allUsers)
            {
                if (u.Username.ToLower().Contains(txtSearch.Text.ToLower()))
                {
                    table.Rows.Add(
                        u.Id,
                        u.Username,
                        u.Email ?? "-",
                        u.Phone ?? "-",
                        u.Role,
                        u.CreatedAt.ToString("dd.MM.yyyy HH:mm")
                    );
                }
            }
            dgvUsers.DataSource = table;
        }

        private void BtnAddUser_Click(object sender, EventArgs e)
        {
            RegisterForm register = new RegisterForm();
            register.ShowDialog();
            LoadUsers();
            LoadStats();
        }

        private void BtnDeleteUser_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("⚠ Selectează un utilizator!",
                    "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Ești sigur că vrei să ștergi utilizatorul selectat?",
                "Confirmare ștergere",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells[0].Value);
                bool success = UserRepository.DeleteUser(id);
                if (success)
                {
                    MessageBox.Show("✅ Utilizator șters cu succes!",
                        "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadUsers();
                    LoadStats();
                }
                else
                {
                    MessageBox.Show("⚠ Eroare la ștergere!",
                        "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnDeleteMessage_Click(object sender, EventArgs e)
        {
            if (dgvMessages.SelectedRows.Count == 0)
            {
                MessageBox.Show("⚠ Selectează un mesaj!",
                    "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Ești sigur că vrei să ștergi mesajul selectat?",
                "Confirmare ștergere",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dgvMessages.SelectedRows[0].Cells["Id"].Value);
                bool success = MessageRepository.DeleteMessage(id);
                if (success)
                {
                    MessageBox.Show("✅ Mesaj șters cu succes!",
                        "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadMessages();
                    LoadStats();
                }
                else
                {
                    MessageBox.Show("⚠ Eroare la ștergere!",
                        "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}