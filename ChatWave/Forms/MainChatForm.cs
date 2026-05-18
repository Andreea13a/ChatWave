using ChatWave.Data;
using ChatWave.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Message = ChatWave.Models.Message;

namespace ChatWave.Forms
{
    public partial class MainChatForm : Form
    {
        ListBox conversations;
        ListBox chatBox;
        TextBox messageBox;
        private LoggedUser currentUser;

        Panel pnlUserProfile;
        Label lblProfileName;
        PictureBox pbAvatar;
        Label lblRoleRight;
        Label lblEmailRight;
        Label lblPhoneRight;
        Button btnChangePhoto;

        // Elemente de interfață principale
        Panel header;
        Panel chatHeader;
        Panel inputPanel;
        Panel dropdown;
        Button menuBtn;
        Button btnNewChat;
        Label lblChatTitle;
        Button sendButton;
        Label userLabel;

        // TIMER PENTRU REÎNCĂRCARE AUTOMATĂ (REAL-TIME CHAT)
        private Timer chatTimer;
        private int currentLoadedMessagesCount = 0;

        private Dictionary<int, Image> userImages = new Dictionary<int, Image>();
        private List<User> allUsers;
        private int selectedUserId = -1;
        private string selectedUsername = "";

        // Variabile pentru conversația curentă
        private int currentConversationPartnerId = -1;
        private string currentConversationPartnerName = "";

        public MainChatForm(LoggedUser user)
        {
            currentUser = user;
            InitializeComponent();
            allUsers = UserRepository.GetAllUsers();
            DesignUI();
            InitializeChatTimer();
            this.WindowState = FormWindowState.Maximized;

            // Aplică tema curentă
            RefreshTheme();
        }

        private void InitializeChatTimer()
        {
            chatTimer = new Timer();
            chatTimer.Interval = 1000;
            chatTimer.Tick += (s, e) => {
                if (currentConversationPartnerId != -1)
                {
                    LoadMessagesWithUser(currentConversationPartnerId, currentConversationPartnerName);
                }
            };
            chatTimer.Start();
        }

        private void DesignUI()
        {
            bool isAdmin = currentUser.Role == "admin";
            this.Controls.Clear();
            this.Text = "ChatWave - Chat";

            // ==========================================
            // 1. HEADER-UL PRINCIPAL (Bara de sus)
            // ==========================================
            header = new Panel();
            header.Height = 70;
            header.Dock = DockStyle.Top;
            header.BackColor = ThemeManager.Header;
            this.Controls.Add(header);

            Label title = new Label();
            title.Text = "💬 ChatWave";
            title.ForeColor = Color.White;
            title.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            title.AutoSize = true;
            title.Location = new Point(20, 22);
            header.Controls.Add(title);

            // BUTON NOU CONVERSATIE
            btnNewChat = new Button();
            btnNewChat.Text = "+ Conversație nouă";
            btnNewChat.Size = new Size(180, 38);
            btnNewChat.Location = new Point(200, 16);
            btnNewChat.BackColor = Color.FromArgb(100, 80, 150);
            btnNewChat.FlatStyle = FlatStyle.Flat;
            btnNewChat.FlatAppearance.BorderSize = 0;
            btnNewChat.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnNewChat.ForeColor = Color.White;
            btnNewChat.Cursor = Cursors.Hand;
            btnNewChat.Click += BtnNewChat_Click;

            // Efecte hover
            btnNewChat.MouseEnter += (s, e) => btnNewChat.BackColor = Color.FromArgb(80, 60, 130);
            btnNewChat.MouseLeave += (s, e) => btnNewChat.BackColor = Color.FromArgb(100, 80, 150);

            header.Controls.Add(btnNewChat);

            // Label utilizator
            userLabel = new Label();
            userLabel.Text = "👤 " + currentUser.Username;
            userLabel.ForeColor = Color.White;
            userLabel.Font = new Font("Segoe UI", 10);
            userLabel.AutoSize = true;
            userLabel.Location = new Point(this.Width - 180, 25);
            userLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            header.Controls.Add(userLabel);

            // BUTON MENIU (cele trei liniute)
            menuBtn = new Button();
            menuBtn.Text = "☰";
            menuBtn.Size = new Size(40, 38);
            menuBtn.Location = new Point(this.Width - 90, 16);
            menuBtn.BackColor = Color.FromArgb(100, 80, 150);
            menuBtn.FlatStyle = FlatStyle.Flat;
            menuBtn.FlatAppearance.BorderSize = 0;
            menuBtn.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            menuBtn.ForeColor = Color.White;
            menuBtn.Cursor = Cursors.Hand;
            menuBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            // Efecte hover
            menuBtn.MouseEnter += (s, e) => menuBtn.BackColor = Color.FromArgb(80, 60, 130);
            menuBtn.MouseLeave += (s, e) => menuBtn.BackColor = Color.FromArgb(100, 80, 150);

            header.Controls.Add(menuBtn);

            // ==========================================
            // 2. PANELUL DIN STÂNGA (Lista de utilizatori)
            // ==========================================
            Panel leftPanel = new Panel();
            leftPanel.Width = 300;
            leftPanel.Dock = DockStyle.Left;
            this.Controls.Add(leftPanel);

            Label convLabel = new Label();
            convLabel.Text = "👥 Conversațiile mele";
            convLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            convLabel.Location = new Point(15, 15);
            convLabel.AutoSize = true;
            leftPanel.Controls.Add(convLabel);

            TextBox searchBox = new TextBox();
            searchBox.Size = new Size(leftPanel.Width - 30, 30);
            searchBox.Location = new Point(15, 50);
            searchBox.Font = new Font("Segoe UI", 10);
            searchBox.Text = "🔍 Caută conversație...";
            searchBox.BorderStyle = BorderStyle.FixedSingle;
            leftPanel.Controls.Add(searchBox);

            searchBox.GotFocus += (s, e) =>
            {
                if (searchBox.Text == "🔍 Caută conversație...") searchBox.Text = "";
            };
            searchBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(searchBox.Text))
                {
                    searchBox.Text = "🔍 Caută conversație...";
                }
            };

            conversations = new ListBox();
            conversations.Width = leftPanel.Width - 30;
            conversations.Height = leftPanel.Height - 160;
            conversations.Location = new Point(15, 90);
            conversations.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            conversations.BorderStyle = BorderStyle.None;
            conversations.DrawMode = DrawMode.OwnerDrawFixed;
            conversations.ItemHeight = 55;
            conversations.Cursor = Cursors.Hand;
            leftPanel.Controls.Add(conversations);

            conversations.DrawItem += (s, e) =>
            {
                if (e.Index < 0) return;
                bool selected = (e.State & DrawItemState.Selected) != 0;

                Color bgColor = selected
                    ? (ThemeManager.IsDarkMode ? Color.FromArgb(70, 70, 90) : Color.FromArgb(235, 225, 255))
                    : (ThemeManager.IsDarkMode ? Color.FromArgb(45, 45, 65) : Color.White);

                e.Graphics.FillRectangle(new SolidBrush(bgColor), e.Bounds);

                string username = conversations.Items[e.Index].ToString();
                var user = allUsers.FirstOrDefault(u => u.Username == username);

                Rectangle avatarRect = new Rectangle(e.Bounds.Left + 8, e.Bounds.Top + 8, 38, 38);
                e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(200, 190, 220)), avatarRect);

                if (user != null)
                {
                    Image userImage = GetUserImage(user.Id);
                    if (userImage != null)
                        e.Graphics.DrawImage(userImage, avatarRect);
                    else
                    {
                        using (Font f = new Font("Segoe UI", 12, FontStyle.Bold))
                        using (StringFormat sf = new StringFormat())
                        {
                            sf.Alignment = StringAlignment.Center;
                            sf.LineAlignment = StringAlignment.Center;
                            e.Graphics.DrawString(
                                username.Substring(0, 1).ToUpper(),
                                f, Brushes.White, avatarRect, sf);
                        }
                    }
                }

                Rectangle textRect = new Rectangle(e.Bounds.Left + 55, e.Bounds.Top + 10, e.Bounds.Width - 65, 30);
                using (Font textFont = new Font("Segoe UI", selected ? 11 : 10, selected ? FontStyle.Bold : FontStyle.Regular))
                {
                    Color textColor = selected
                        ? ThemeManager.TextSecondary
                        : ThemeManager.TextPrimary;

                    e.Graphics.DrawString(username, textFont,
                        new SolidBrush(textColor), textRect);
                }

                e.Graphics.DrawLine(new Pen(ThemeManager.CardBackground),
                    e.Bounds.Left, e.Bounds.Bottom - 1,
                    e.Bounds.Right, e.Bounds.Bottom - 1);
            };

            // Încarcă toți utilizatorii în listă (excludem utilizatorul curent)
            var allUsernames = new List<string>();
            foreach (var user in allUsers)
            {
                if (user.Id != currentUser.Id)
                {
                    conversations.Items.Add(user.Username);
                    allUsernames.Add(user.Username);
                    GetUserImage(user.Id);
                }
            }

            searchBox.TextChanged += (s, e) =>
            {
                string filter = searchBox.Text.ToLower();
                if (filter == "🔍 caută conversație...") return;
                conversations.Items.Clear();
                foreach (var name in allUsernames)
                    if (name.ToLower().Contains(filter))
                        conversations.Items.Add(name);
            };

            // ==========================================
            // 3. PANELUL DIN DREAPTA (Profilul utilizatorului)
            // ==========================================
            pnlUserProfile = new Panel();
            pnlUserProfile.Width = 260;
            pnlUserProfile.Dock = DockStyle.Right;
            pnlUserProfile.Visible = false;
            this.Controls.Add(pnlUserProfile);

            pbAvatar = new PictureBox();
            pbAvatar.Size = new Size(110, 110);
            pbAvatar.Location = new Point((pnlUserProfile.Width - 110) / 2, 30);
            pbAvatar.SizeMode = PictureBoxSizeMode.Zoom;
            pbAvatar.BackColor = Color.Gainsboro;
            pbAvatar.Cursor = Cursors.Hand;
            pbAvatar.Click += (s, e) =>
            {
                if (lblProfileName.Text == currentUser.Username)
                    ChangeProfilePhoto();
            };
            pnlUserProfile.Controls.Add(pbAvatar);

            btnChangePhoto = new Button();
            btnChangePhoto.Text = "📷 Schimbă poza";
            btnChangePhoto.Size = new Size(120, 30);
            btnChangePhoto.Location = new Point((pnlUserProfile.Width - 120) / 2, 150);
            btnChangePhoto.FlatStyle = FlatStyle.Flat;
            btnChangePhoto.Font = new Font("Segoe UI", 9);
            btnChangePhoto.Click += (s, e) => ChangeProfilePhoto();
            pnlUserProfile.Controls.Add(btnChangePhoto);

            lblProfileName = new Label();
            lblProfileName.Text = "";
            lblProfileName.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblProfileName.Location = new Point(10, 200);
            lblProfileName.Size = new Size(pnlUserProfile.Width - 20, 30);
            lblProfileName.TextAlign = ContentAlignment.MiddleCenter;
            pnlUserProfile.Controls.Add(lblProfileName);

            lblRoleRight = new Label();
            lblRoleRight.Text = "";
            lblRoleRight.Font = new Font("Segoe UI", 10);
            lblRoleRight.Location = new Point(10, 235);
            lblRoleRight.Size = new Size(pnlUserProfile.Width - 20, 25);
            lblRoleRight.TextAlign = ContentAlignment.MiddleCenter;
            pnlUserProfile.Controls.Add(lblRoleRight);

            lblEmailRight = new Label();
            lblEmailRight.Text = "";
            lblEmailRight.Font = new Font("Segoe UI", 9);
            lblEmailRight.Location = new Point(10, 270);
            lblEmailRight.Size = new Size(pnlUserProfile.Width - 20, 20);
            lblEmailRight.TextAlign = ContentAlignment.MiddleCenter;
            pnlUserProfile.Controls.Add(lblEmailRight);

            lblPhoneRight = new Label();
            lblPhoneRight.Text = "";
            lblPhoneRight.Font = new Font("Segoe UI", 9);
            lblPhoneRight.Location = new Point(10, 300);
            lblPhoneRight.Size = new Size(pnlUserProfile.Width - 20, 20);
            lblPhoneRight.TextAlign = ContentAlignment.MiddleCenter;
            pnlUserProfile.Controls.Add(lblPhoneRight);

            // ==========================================
            // 4. BARA DE INPUT DE JOS (Ancorată fix la baza ferestrei)
            // ==========================================
            inputPanel = new Panel();
            inputPanel.Height = 65;
            inputPanel.Dock = DockStyle.Bottom;
            this.Controls.Add(inputPanel);

            sendButton = new Button();
            sendButton.Text = "✉ Trimite";
            sendButton.Size = new Size(110, 35);
            sendButton.FlatStyle = FlatStyle.Flat;
            sendButton.FlatAppearance.BorderSize = 0;
            sendButton.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            sendButton.Cursor = Cursors.Hand;
            sendButton.Click += SendMessage;
            inputPanel.Controls.Add(sendButton);

            messageBox = new TextBox();
            messageBox.Font = new Font("Segoe UI", 11);
            messageBox.BorderStyle = BorderStyle.FixedSingle;
            inputPanel.Controls.Add(messageBox);

            messageBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendMessage(s, e);
                    e.SuppressKeyPress = true;
                }
            };

            // ==========================================
            // 5. TITLUL CONVERSAȚIEI CURENTE (Sub Header)
            // ==========================================
            chatHeader = new Panel();
            chatHeader.Height = 50;
            chatHeader.Dock = DockStyle.Top;
            this.Controls.Add(chatHeader);

            lblChatTitle = new Label();
            lblChatTitle.Text = "Selectează un utilizator";
            lblChatTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblChatTitle.Location = new Point(15, 12);
            lblChatTitle.AutoSize = true;
            chatHeader.Controls.Add(lblChatTitle);

            // ==========================================
            // 6. ZONA CENTRALĂ DE MESAJE (Ocupă restul spațiului rămas)
            // ==========================================
            chatBox = new ListBox();
            chatBox.Dock = DockStyle.Fill;
            chatBox.BorderStyle = BorderStyle.None;
            chatBox.Font = new Font("Segoe UI", 10);
            chatBox.DrawMode = DrawMode.Normal;
            chatBox.ItemHeight = 35;
            this.Controls.Add(chatBox);

            // ==========================================
            // 7. MENIUL DROPDOWN (Plasat deasupra tuturor elementelor)
            // ==========================================
            dropdown = new Panel();
            dropdown.Size = new Size(200, isAdmin ? 210 : 130);
            dropdown.BorderStyle = BorderStyle.FixedSingle;
            dropdown.Visible = false;
            this.Controls.Add(dropdown);

            AddSectionLabel(dropdown, "👤 Cont", 6);
            AddMenuButton(dropdown, "  Profilul meu", 40, (s, e) =>
            {
                dropdown.Visible = false;
                ShowUserProfile(currentUser.Id);
            });
            AddMenuButton(dropdown, "  Setări", 75, (s, e) =>
            {
                dropdown.Visible = false;
                new SettingsForm(currentUser).ShowDialog();
                ShowUserProfile(currentUser.Id);
            });
            AddMenuButton(dropdown, "  🔓 Logout", 100, Color.FromArgb(192, 57, 43), (s, e) =>
            {
                dropdown.Visible = false;
                chatTimer.Stop();
                this.Hide();
                new LoginForm().Show();
            });

            if (isAdmin)
            {
                AddSeparator(dropdown, 135);
                AddSectionLabel(dropdown, "🛡️ Admin", 142);
                AddMenuButton(dropdown, "  Dashboard", 162, (s, e) =>
                {
                    dropdown.Visible = false;
                    new AdminDashboard().Show();
                });
            }

            // Gestionare comportament buton meniu
            menuBtn.Click += (s, e) =>
            {
                dropdown.Visible = !dropdown.Visible;
                if (dropdown.Visible) dropdown.BringToFront();
            };

            // Ascundere meniu la click pe zone libere
            this.Click += (s, e) => dropdown.Visible = false;
            chatBox.Click += (s, e) => dropdown.Visible = false;

            // Selectare utilizator din listă
            conversations.SelectedIndexChanged += Conversations_SelectedIndexChanged;

            // Recalculare dimensiuni la redimensionarea ferestrei
            this.Resize += (s, e) =>
            {
                UpdateInternalLayout();
            };
        }

        // Metodă pentru recalcularea pozițiilor
        private void UpdateInternalLayout()
        {
            if (header == null) return;

            // Actualizează doar elementele care trebuie să fie în dreapta
            if (menuBtn != null)
            {
                menuBtn.Location = new Point(header.Width - 90, 16);
            }

            if (userLabel != null)
            {
                userLabel.Location = new Point(header.Width - 180, 25);
            }

            if (dropdown != null)
            {
                dropdown.Location = new Point(this.Width - 230, 70);
                dropdown.BringToFront();
            }

            // Sincronizare lățime elemente din bara de input de jos
            if (inputPanel != null && messageBox != null && sendButton != null)
            {
                sendButton.Location = new Point(inputPanel.Width - 130, 15);
                messageBox.Location = new Point(15, 15);
                messageBox.Width = inputPanel.Width - 160;
            }
        }

        // ==========================================
        // METODE PENTRU CONVERSAȚII
        // ==========================================

        private void BtnNewChat_Click(object sender, EventArgs e)
        {
            using (var newChatForm = new NewConversationForm(currentUser))
            {
                if (newChatForm.ShowDialog() == DialogResult.OK)
                {
                    int selectedUserId = newChatForm.SelectedUserId;
                    string selectedUsername = newChatForm.SelectedUsername;

                    // Verifică dacă există deja o conversație
                    var existingMessages = MessageRepository.GetMessagesBetweenUsers(
                        currentUser.Id,
                        selectedUserId,
                        currentUser.Username,
                        selectedUsername);

                    if (existingMessages.Count > 0)
                    {
                        DialogResult result = MessageBox.Show(
                            $"Ai deja o conversație cu {selectedUsername}. Vrei să o deschizi?",
                            "Conversație existentă",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            currentConversationPartnerId = selectedUserId;
                            currentConversationPartnerName = selectedUsername;
                            this.selectedUserId = selectedUserId;
                            this.selectedUsername = selectedUsername;

                            LoadMessagesWithUser(selectedUserId, selectedUsername);
                            lblChatTitle.Text = "💬 " + selectedUsername;
                            ShowUserProfile(selectedUserId);
                            SelectUserInList(selectedUsername);
                        }
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show(
                            $"Începi o conversație cu {selectedUsername}?",
                            "Conversație nouă",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            currentConversationPartnerId = selectedUserId;
                            currentConversationPartnerName = selectedUsername;
                            this.selectedUserId = selectedUserId;
                            this.selectedUsername = selectedUsername;

                            chatBox.Items.Clear();
                            lblChatTitle.Text = "💬 " + selectedUsername;
                            ShowUserProfile(selectedUserId);
                            AddSystemMessage($"Ai început o conversație cu {selectedUsername}");
                            SelectUserInList(selectedUsername);
                        }
                    }
                }
            }
        }

        private void LoadMessagesWithUser(int userId, string username)
        {
            var messages = MessageRepository.GetMessagesBetweenUsers(
                currentUser.Id,
                userId,
                currentUser.Username,
                username);

            if (messages.Count == currentLoadedMessagesCount && currentLoadedMessagesCount > 0) return;

            chatBox.Items.Clear();
            currentLoadedMessagesCount = messages.Count;

            foreach (var msg in messages)
            {
                AddMessageToChat(msg);
            }

            if (chatBox.Items.Count > 0)
                chatBox.TopIndex = chatBox.Items.Count - 1;
        }

        private void SelectUserInList(string username)
        {
            for (int i = 0; i < conversations.Items.Count; i++)
            {
                if (conversations.Items[i].ToString() == username)
                {
                    conversations.SelectedIndex = i;
                    break;
                }
            }
        }

        private void AddSystemMessage(string text)
        {
            chatBox.Items.Add($"📢 {DateTime.Now:HH:mm}: {text}");
            if (chatBox.Items.Count > 0)
                chatBox.TopIndex = chatBox.Items.Count - 1;
        }

        private void Conversations_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (conversations.SelectedItem == null) return;
            selectedUsername = conversations.SelectedItem.ToString();
            var user = allUsers.FirstOrDefault(u => u.Username == selectedUsername);
            if (user != null)
            {
                currentConversationPartnerId = user.Id;
                currentConversationPartnerName = user.Username;
                selectedUserId = user.Id;
                currentLoadedMessagesCount = -1;
                ShowUserProfile(user.Id);
                LoadMessagesWithUser(user.Id, user.Username);
                lblChatTitle.Text = "💬 " + selectedUsername;
            }
        }

        public void RefreshTheme()
        {
            ThemeManager.ApplyTheme(this);

            if (header != null) header.BackColor = ThemeManager.Header;
            if (btnNewChat != null)
            {
                btnNewChat.BackColor = ThemeManager.ButtonColor;
                btnNewChat.ForeColor = Color.White;
            }
            if (menuBtn != null)
            {
                menuBtn.BackColor = ThemeManager.ButtonColor;
                menuBtn.ForeColor = Color.White;
            }
            if (conversations != null)
            {
                conversations.BackColor = ThemeManager.IsDarkMode ? Color.FromArgb(40, 40, 60) : Color.White;
                conversations.ForeColor = ThemeManager.TextPrimary;
                conversations.Invalidate();
            }
            if (chatBox != null)
            {
                chatBox.BackColor = ThemeManager.ChatBackground;
                chatBox.ForeColor = ThemeManager.TextPrimary;
            }
            if (inputPanel != null) inputPanel.BackColor = ThemeManager.CardBackground;
            if (chatHeader != null) chatHeader.BackColor = ThemeManager.IsDarkMode ? Color.FromArgb(40, 40, 60) : Color.FromArgb(245, 242, 255);
            if (pnlUserProfile != null) pnlUserProfile.BackColor = ThemeManager.CardBackground;
            if (dropdown != null)
            {
                dropdown.BackColor = ThemeManager.IsDarkMode ? Color.FromArgb(45, 45, 65) : Color.White;
                dropdown.ForeColor = ThemeManager.TextPrimary;
            }
            if (lblChatTitle != null) lblChatTitle.ForeColor = ThemeManager.TextSecondary;
            if (lblProfileName != null) lblProfileName.ForeColor = ThemeManager.TextPrimary;
            if (lblRoleRight != null) lblRoleRight.ForeColor = ThemeManager.TextSecondary;
            if (lblEmailRight != null) lblEmailRight.ForeColor = ThemeManager.TextPrimary;
            if (lblPhoneRight != null) lblPhoneRight.ForeColor = ThemeManager.TextPrimary;
            if (sendButton != null) sendButton.BackColor = ThemeManager.ButtonColor;
            if (btnChangePhoto != null) btnChangePhoto.BackColor = ThemeManager.ButtonColor;
            if (messageBox != null)
            {
                messageBox.BackColor = ThemeManager.InputBackground;
                messageBox.ForeColor = ThemeManager.TextPrimary;
            }

            this.Invalidate();
            this.Refresh();
            conversations?.Invalidate();
            chatBox?.Invalidate();
        }

        private void AddMessageToChat(Message msg)
        {
            string prefix = msg.SenderId == currentUser.Id ? "[Tu]" : "[" + msg.SenderName + "]";
            chatBox.Items.Add($"{prefix} {msg.SentAt:HH:mm}: {msg.Text}");

            if (chatBox.Items.Count > 0)
                chatBox.TopIndex = chatBox.Items.Count - 1;

            chatBox.Invalidate();
            chatBox.Refresh();
            chatBox.BringToFront();
            dropdown.BringToFront();
        }

        private void ShowUserProfile(int userId)
        {
            var user = allUsers.FirstOrDefault(u => u.Id == userId);
            if (user == null) return;

            pnlUserProfile.Visible = true;
            lblProfileName.Text = user.Username;
            lblRoleRight.Text = "Rol: " + (user.Role ?? "user");
            lblEmailRight.Text = "📧 " + (user.Email ?? "-");
            lblPhoneRight.Text = "📱 " + (user.Phone ?? "-");

            if (userImages.ContainsKey(userId))
                userImages.Remove(userId);

            LoadImageIntoPictureBox(pbAvatar, userId);
            btnChangePhoto.Visible = (userId == currentUser.Id);
        }

        private void SendMessage(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(messageBox.Text))
            {
                MessageBox.Show("⚠ Mesajul nu poate fi gol!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (currentConversationPartnerId == -1)
            {
                MessageBox.Show("⚠ Selectează sau creează o conversație!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var message = new Message
            {
                SenderId = currentUser.Id,
                SenderName = currentUser.Username,
                ReceiverId = currentConversationPartnerId,
                ReceiverName = currentConversationPartnerName,
                Text = messageBox.Text.Trim(),
                SentAt = DateTime.Now
            };

            bool success = MessageRepository.AddMessage(message);

            if (success)
            {
                messageBox.Clear();
                AddMessageToChat(message);
                currentLoadedMessagesCount = chatBox.Items.Count;
                messageBox.Focus();
            }
            else
            {
                MessageBox.Show("⚠ Eroare la trimiterea mesajului!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddSectionLabel(Panel parent, string text, int y)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lbl.ForeColor = Color.Gray;
            lbl.Location = new Point(8, y);
            lbl.AutoSize = true;
            parent.Controls.Add(lbl);
        }

        private void AddMenuButton(Panel parent, string text, int y, EventHandler onClick)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Size = new Size(196, 30);
            btn.Location = new Point(0, y);
            btn.BackColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 240, 255);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Font = new Font("Segoe UI", 9);
            btn.Click += onClick;
            parent.Controls.Add(btn);
        }

        private void AddMenuButton(Panel parent, string text, int y, Color color, EventHandler onClick)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Size = new Size(196, 30);
            btn.Location = new Point(0, y);
            btn.BackColor = Color.White;
            btn.ForeColor = color;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 240, 255);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Font = new Font("Segoe UI", 9);
            btn.Click += onClick;
            parent.Controls.Add(btn);
        }

        private void AddSeparator(Panel parent, int y)
        {
            Panel sep = new Panel();
            sep.Size = new Size(196, 1);
            sep.Location = new Point(0, y);
            sep.BackColor = Color.FromArgb(220, 215, 235);
            parent.Controls.Add(sep);
        }

        private Image ByteArrayToImage(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0) return null;
            using (MemoryStream ms = new MemoryStream(byteArray))
                return Image.FromStream(ms);
        }

        private byte[] ImageToByteArray(Image image)
        {
            if (image == null) return null;
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        private Image GenerateDefaultAvatar(string username, int size = 100)
        {
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                Rectangle rect = new Rectangle(0, 0, size, size);
                using (LinearGradientBrush brush = new LinearGradientBrush(rect,
                    Color.FromArgb(167, 147, 214), Color.FromArgb(130, 110, 180), 45))
                    g.FillEllipse(brush, rect);

                string initial = username.Length > 0 ? username.Substring(0, 1).ToUpper() : "?";
                using (Font avatarFont = new Font("Segoe UI", size / 2, FontStyle.Bold))
                using (StringFormat sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    g.DrawString(initial, avatarFont, Brushes.White, rect, sf);
                }

                using (Pen pen = new Pen(Color.White, 3))
                    g.DrawEllipse(pen, rect);
            }
            return bmp;
        }

        private Image GetUserImage(int userId)
        {
            if (userImages.ContainsKey(userId))
                return userImages[userId];

            byte[] imageData = UserRepository.GetProfileImage(userId);
            Image img = null;

            if (imageData != null && imageData.Length > 0)
            {
                try { img = ByteArrayToImage(imageData); }
                catch (Exception ex) { Console.WriteLine($"Eroare imagine: {ex.Message}"); }
            }

            if (img == null)
            {
                var user = allUsers.FirstOrDefault(u => u.Id == userId);
                img = GenerateDefaultAvatar(user?.Username ?? "User", 100);
            }

            userImages[userId] = img;
            return img;
        }

        private void LoadImageIntoPictureBox(PictureBox pb, int userId)
        {
            try
            {
                Image img = GetUserImage(userId);
                if (img != null) pb.Image = img;
            }
            catch (Exception ex) { Console.WriteLine($"Eroare PictureBox: {ex.Message}"); }
        }

        private void ChangeProfilePhoto()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Selectează poza de profil";
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (Image original = Image.FromFile(ofd.FileName))
                        {
                            Image resized = ResizeImage(original, 200, 200);
                            byte[] imageData = ImageToByteArray(resized);

                            if (UserRepository.SaveProfileImage(currentUser.Id, imageData))
                            {
                                if (userImages.ContainsKey(currentUser.Id))
                                    userImages.Remove(currentUser.Id);

                                LoadImageIntoPictureBox(pbAvatar, currentUser.Id);
                                conversations.Invalidate();

                                MessageBox.Show("✅ Poza actualizată cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("⚠ Eroare la salvarea pozei!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"⚠ Eroare: {ex.Message}", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private Image ResizeImage(Image image, int width, int height)
        {
            Bitmap resized = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(resized))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, 0, 0, width, height);
            }
            return resized;
        }

        public void RefreshUsers()
        {
            allUsers = UserRepository.GetAllUsers();
            conversations.Invalidate();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (chatTimer != null)
            {
                chatTimer.Stop();
                chatTimer.Dispose();
            }
            base.OnFormClosing(e);
        }
    }
}