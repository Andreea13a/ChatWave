using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ChatWave.Data;
using ChatWave.Models;

namespace ChatWave.Forms
{
    public partial class NewConversationForm : Form
    {
        private LoggedUser currentUser;
        private List<User> allUsers;
        private ListBox usersList;
        private TextBox searchBox;
        private Button btnStartChat;
        private Label lblTitle;
        private Panel header;

        public int SelectedUserId { get; private set; }
        public string SelectedUsername { get; private set; }
        public bool ConversationCreated { get; private set; }

        public NewConversationForm(LoggedUser user)
        {
            currentUser = user;
            allUsers = UserRepository.GetAllUsers();
            InitializeComponent();
            ApplyTheme();
        }

        private void InitializeComponent()
        {
            this.Text = "ChatWave - Conversație nouă";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Header
            header = new Panel();
            header.Dock = DockStyle.Top;
            header.Height = 60;
            header.BackColor = ThemeManager.Header;

            lblTitle = new Label();
            lblTitle.Text = "➕ Selectează un utilizator";
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.Location = new Point(20, 18);
            lblTitle.AutoSize = true;
            header.Controls.Add(lblTitle);

            // Search box
            searchBox = new TextBox();
            searchBox.Location = new Point(20, 80);
            searchBox.Size = new Size(345, 30);
            searchBox.Font = new Font("Segoe UI", 11);
            searchBox.Text = "🔍 Caută utilizator...";
            searchBox.ForeColor = Color.Gray;

            searchBox.GotFocus += (s, e) =>
            {
                if (searchBox.Text == "🔍 Caută utilizator...")
                {
                    searchBox.Text = "";
                    searchBox.ForeColor = ThemeManager.TextPrimary;
                }
            };

            searchBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(searchBox.Text))
                {
                    searchBox.Text = "🔍 Caută utilizator...";
                    searchBox.ForeColor = Color.Gray;
                }
            };

            searchBox.TextChanged += SearchBox_TextChanged;

            // Lista utilizatori
            usersList = new ListBox();
            usersList.Location = new Point(20, 120);
            usersList.Size = new Size(345, 280);
            usersList.Font = new Font("Segoe UI", 11);
            usersList.DrawMode = DrawMode.OwnerDrawFixed;
            usersList.ItemHeight = 50;
            usersList.Cursor = Cursors.Hand;

            usersList.DrawItem += (s, e) =>
            {
                if (e.Index < 0) return;

                bool selected = (e.State & DrawItemState.Selected) != 0;
                Color bgColor = selected ? ThemeManager.ButtonColor : ThemeManager.Background;
                Color textColor = selected ? Color.White : ThemeManager.TextPrimary;

                e.Graphics.FillRectangle(new SolidBrush(bgColor), e.Bounds);

                string username = usersList.Items[e.Index].ToString();
                var user = allUsers.FirstOrDefault(u => u.Username == username);

                // Desenează avatar
                Rectangle avatarRect = new Rectangle(e.Bounds.Left + 8, e.Bounds.Top + 8, 34, 34);
                e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(200, 190, 220)), avatarRect);

                string initial = username.Length > 0 ? username.Substring(0, 1).ToUpper() : "?";
                using (Font f = new Font("Segoe UI", 14, FontStyle.Bold))
                {
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    e.Graphics.DrawString(initial, f, Brushes.White, avatarRect, sf);
                }

                // Desenează numele
                Rectangle textRect = new Rectangle(e.Bounds.Left + 55, e.Bounds.Top + 12, e.Bounds.Width - 65, 25);
                using (Font textFont = new Font("Segoe UI", 11, FontStyle.Bold))
                {
                    e.Graphics.DrawString(username, textFont, new SolidBrush(textColor), textRect);
                }

                // Desenează rolul
                if (user != null && !string.IsNullOrEmpty(user.Role))
                {
                    Rectangle roleRect = new Rectangle(e.Bounds.Left + 55, e.Bounds.Top + 32, e.Bounds.Width - 65, 15);
                    using (Font roleFont = new Font("Segoe UI", 8))
                    {
                        string role = user.Role == "admin" ? "🛡️ Admin" : "👤 User";
                        e.Graphics.DrawString(role, roleFont, new SolidBrush(ThemeManager.TextSecondary), roleRect);
                    }
                }
            };

            usersList.SelectedIndexChanged += UsersList_SelectedIndexChanged;

            // Buton pornire conversație
            btnStartChat = new Button();
            btnStartChat.Text = "💬 Începe conversația";
            btnStartChat.Size = new Size(345, 45);
            btnStartChat.Location = new Point(20, 410);
            btnStartChat.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnStartChat.FlatStyle = FlatStyle.Flat;
            btnStartChat.BackColor = ThemeManager.ButtonColor;
            btnStartChat.ForeColor = Color.White;
            btnStartChat.Cursor = Cursors.Hand;
            btnStartChat.Enabled = false;
            btnStartChat.Click += BtnStartChat_Click;

            // Adaugă controale
            this.Controls.Add(header);
            this.Controls.Add(searchBox);
            this.Controls.Add(usersList);
            this.Controls.Add(btnStartChat);

            // Încarcă utilizatorii
            LoadUsers();
        }

        private void LoadUsers()
        {
            usersList.Items.Clear();
            foreach (var user in allUsers)
            {
                // Nu arată utilizatorul curent
                if (user.Id != currentUser.Id)
                {
                    usersList.Items.Add(user.Username);
                }
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            string filter = searchBox.Text.ToLower();
            if (filter == "🔍 caută utilizator..." || string.IsNullOrWhiteSpace(filter))
            {
                LoadUsers();
                return;
            }

            usersList.Items.Clear();
            foreach (var user in allUsers)
            {
                if (user.Id != currentUser.Id && user.Username.ToLower().Contains(filter))
                {
                    usersList.Items.Add(user.Username);
                }
            }
        }

        private void UsersList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (usersList.SelectedItem != null)
            {
                btnStartChat.Enabled = true;
                btnStartChat.BackColor = Color.FromArgb(100, 200, 100);
            }
            else
            {
                btnStartChat.Enabled = false;
                btnStartChat.BackColor = ThemeManager.ButtonColor;
            }
        }

        private void BtnStartChat_Click(object sender, EventArgs e)
        {
            if (usersList.SelectedItem == null) return;

            SelectedUsername = usersList.SelectedItem.ToString();
            var selectedUser = allUsers.FirstOrDefault(u => u.Username == SelectedUsername);

            if (selectedUser != null)
            {
                SelectedUserId = selectedUser.Id;
                ConversationCreated = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void ApplyTheme()
        {
            this.BackColor = ThemeManager.Background;
            header.BackColor = ThemeManager.Header;
            lblTitle.ForeColor = Color.White;
            searchBox.BackColor = ThemeManager.InputBackground;
            searchBox.ForeColor = ThemeManager.TextPrimary;
            usersList.BackColor = ThemeManager.Background;
            usersList.ForeColor = ThemeManager.TextPrimary;
            btnStartChat.BackColor = ThemeManager.ButtonColor;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ApplyTheme();
        }
    }
}