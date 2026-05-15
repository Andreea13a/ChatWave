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
    public partial class MainChatForm : BaseForm
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

        private Dictionary<int, Image> userImages = new Dictionary<int, Image>();
        private List<User> allUsers;
        private int selectedUserId = -1;
        private string selectedUsername = "";

        public MainChatForm(LoggedUser user)
        {
            currentUser = user;
            InitializeComponent();
            allUsers = UserRepository.GetAllUsers();
            DesignUI();
            this.WindowState = FormWindowState.Maximized;
        }

        private void DesignUI()
        {
            Color movHeader = Color.FromArgb(167, 147, 214);
            Color movCard = Color.FromArgb(235, 230, 250);
            Color movButton = Color.FromArgb(160, 130, 210);
            bool isAdmin = currentUser.Role == "admin";

            this.Text = "ChatWave - Chat";
            this.BackColor = Color.FromArgb(245, 245, 245);

            // HEADER
            Panel header = new Panel();
            header.BackColor = movHeader;
            header.Height = 70;
            header.Dock = DockStyle.Top;
            this.Controls.Add(header);

            Label title = new Label();
            title.Text = "💬 ChatWave";
            title.ForeColor = Color.White;
            title.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            title.AutoSize = true;
            title.Location = new Point(20, 20);
            header.Controls.Add(title);

            Label userLabel = new Label();
            userLabel.Text = "👤 " + currentUser.Username;
            userLabel.ForeColor = Color.White;
            userLabel.Font = new Font("Segoe UI", 10);
            userLabel.AutoSize = true;
            userLabel.Location = new Point(this.Width - 180, 25);
            userLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            header.Controls.Add(userLabel);

            Button menuBtn = new Button();
            menuBtn.Text = "☰";
            menuBtn.Size = new Size(40, 35);
            menuBtn.Location = new Point(this.Width - 90, 18);
            menuBtn.BackColor = Color.FromArgb(190, 170, 230);
            menuBtn.ForeColor = Color.White;
            menuBtn.FlatStyle = FlatStyle.Flat;
            menuBtn.FlatAppearance.BorderSize = 0;
            menuBtn.Font = new Font("Segoe UI", 14);
            menuBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            header.Controls.Add(menuBtn);

            // DROPDOWN
            Panel dropdown = new Panel();
            dropdown.Size = new Size(200, isAdmin ? 210 : 130);
            dropdown.AutoScroll = true;
            dropdown.BackColor = Color.White;
            dropdown.BorderStyle = BorderStyle.FixedSingle;
            dropdown.Visible = false;
            dropdown.Location = new Point(this.Width - 210, 70);
            dropdown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            dropdown.BringToFront();
            this.Controls.Add(dropdown);

            AddSectionLabel(dropdown, "👤 Cont", 6);
            AddMenuButton(dropdown, "  Profilul meu", 40, movButton, (s, e) =>
            {
                dropdown.Visible = false;
                ShowUserProfile(currentUser.Id);
            });
            AddMenuButton(dropdown, "  Setări", 75, movButton, (s, e) =>
            {
                dropdown.Visible = false;
                new SettingsForm(currentUser).ShowDialog();
            });
            AddMenuButton(dropdown, "  🔓 Logout", 100, Color.FromArgb(192, 57, 43), (s, e) =>
            {
                dropdown.Visible = false;
                this.Hide();
                new LoginForm().Show();
            });

            if (isAdmin)
            {
                AddSeparator(dropdown, 135);
                AddSectionLabel(dropdown, "🛡️ Admin", 142);
                AddMenuButton(dropdown, "  Dashboard", 162, movButton, (s, e) =>
                {
                    dropdown.Visible = false;
                    new AdminDashboard().Show();
                });
            }

            menuBtn.Click += (s, e) =>
            {
                dropdown.Visible = !dropdown.Visible;
                dropdown.BringToFront();
            };
            this.Click += (s, e) => dropdown.Visible = false;

            // ==========================================
            // 1. PANEL PROFIL DREAPTA (Se adaugă primul)
            // ==========================================
            pnlUserProfile = new Panel();
            pnlUserProfile.Width = 250;
            pnlUserProfile.Dock = DockStyle.Right;
            pnlUserProfile.BackColor = movCard;
            pnlUserProfile.Visible = false;
            this.Controls.Add(pnlUserProfile);

            pbAvatar = new PictureBox();
            pbAvatar.Size = new Size(100, 100);
            pbAvatar.Location = new Point((pnlUserProfile.Width - 100) / 2, 20);
            pbAvatar.SizeMode = PictureBoxSizeMode.Zoom;
            pbAvatar.BackColor = movHeader;
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
            btnChangePhoto.Location = new Point((pnlUserProfile.Width - 120) / 2, 130);
            btnChangePhoto.BackColor = movButton;
            btnChangePhoto.ForeColor = Color.White;
            btnChangePhoto.FlatStyle = FlatStyle.Flat;
            btnChangePhoto.Font = new Font("Segoe UI", 9);
            btnChangePhoto.Click += (s, e) => ChangeProfilePhoto();
            pnlUserProfile.Controls.Add(btnChangePhoto);

            lblProfileName = new Label();
            lblProfileName.Text = "";
            lblProfileName.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblProfileName.ForeColor = Color.FromArgb(90, 40, 160);
            lblProfileName.Location = new Point(10, 175);
            lblProfileName.Size = new Size(pnlUserProfile.Width - 20, 30);
            lblProfileName.TextAlign = ContentAlignment.MiddleCenter;
            pnlUserProfile.Controls.Add(lblProfileName);

            lblRoleRight = new Label();
            lblRoleRight.Text = "";
            lblRoleRight.Font = new Font("Segoe UI", 10);
            lblRoleRight.ForeColor = Color.FromArgb(120, 80, 180);
            lblRoleRight.Location = new Point(10, 210);
            lblRoleRight.Size = new Size(pnlUserProfile.Width - 20, 25);
            lblRoleRight.TextAlign = ContentAlignment.MiddleCenter;
            pnlUserProfile.Controls.Add(lblRoleRight);

            lblEmailRight = new Label();
            lblEmailRight.Text = "";
            lblEmailRight.Font = new Font("Segoe UI", 9);
            lblEmailRight.ForeColor = Color.FromArgb(80, 80, 80);
            lblEmailRight.Location = new Point(10, 245);
            lblEmailRight.Size = new Size(pnlUserProfile.Width - 20, 20);
            lblEmailRight.TextAlign = ContentAlignment.MiddleCenter;
            pnlUserProfile.Controls.Add(lblEmailRight);

            lblPhoneRight = new Label();
            lblPhoneRight.Text = "";
            lblPhoneRight.Font = new Font("Segoe UI", 9);
            lblPhoneRight.ForeColor = Color.FromArgb(80, 80, 80);
            lblPhoneRight.Location = new Point(10, 275);
            lblPhoneRight.Size = new Size(pnlUserProfile.Width - 20, 20);
            lblPhoneRight.TextAlign = ContentAlignment.MiddleCenter;
            pnlUserProfile.Controls.Add(lblPhoneRight);

            // ==========================================
            // 2. LEFT PANEL (Se adaugă al doilea)
            // ==========================================
            Panel leftPanel = new Panel();
            leftPanel.Width = 300;
            leftPanel.Dock = DockStyle.Left;
            leftPanel.BackColor = movCard;
            this.Controls.Add(leftPanel);

            Label convLabel = new Label();
            convLabel.Text = "👥 Toți utilizatorii";
            convLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            convLabel.Location = new Point(15, 15);
            convLabel.AutoSize = true;
            leftPanel.Controls.Add(convLabel);

            TextBox searchBox = new TextBox();
            searchBox.Size = new Size(leftPanel.Width - 30, 30);
            searchBox.Location = new Point(15, 50);
            searchBox.Font = new Font("Segoe UI", 10);
            searchBox.ForeColor = Color.FromArgb(150, 150, 150);
            searchBox.Text = "🔍 Caută utilizator...";
            searchBox.BorderStyle = BorderStyle.FixedSingle;
            leftPanel.Controls.Add(searchBox);

            searchBox.GotFocus += (s, e) =>
            {
                if (searchBox.Text == "🔍 Caută utilizator...") searchBox.Text = "";
                searchBox.ForeColor = Color.FromArgb(80, 50, 120);
            };
            searchBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(searchBox.Text))
                {
                    searchBox.Text = "🔍 Caută utilizator...";
                    searchBox.ForeColor = Color.FromArgb(150, 150, 150);
                }
            };

            conversations = new ListBox();
            conversations.Width = leftPanel.Width - 30;
            conversations.Height = leftPanel.Height - 100;
            conversations.Location = new Point(15, 90);
            conversations.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            conversations.BackColor = Color.White;
            conversations.BorderStyle = BorderStyle.None;
            conversations.DrawMode = DrawMode.OwnerDrawFixed;
            conversations.ItemHeight = 55;
            conversations.Cursor = Cursors.Hand;
            leftPanel.Controls.Add(conversations);

            conversations.DrawItem += (s, e) =>
            {
                if (e.Index < 0) return;
                bool selected = (e.State & DrawItemState.Selected) != 0;
                e.Graphics.FillRectangle(
                    selected ? new SolidBrush(Color.FromArgb(235, 225, 255)) : Brushes.White,
                    e.Bounds);

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
                    e.Graphics.DrawString(username, textFont,
                        new SolidBrush(selected ? Color.FromArgb(90, 40, 160) : Color.FromArgb(120, 80, 180)),
                        textRect);
                }

                e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 235, 250)),
                    e.Bounds.Left, e.Bounds.Bottom - 1,
                    e.Bounds.Right, e.Bounds.Bottom - 1);
            };

            // Încarcă utilizatorii
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
                if (filter == "🔍 caută utilizator...") return;
                conversations.Items.Clear();
                foreach (var name in allUsernames)
                    if (name.ToLower().Contains(filter))
                        conversations.Items.Add(name);
            };

            // ==========================================
            // 3. CHAT PANEL (Se adaugă ULTIMUL - Dock Fill)
            // ==========================================
            Panel chatPanel = new Panel();
            chatPanel.Dock = DockStyle.Fill;
            chatPanel.BackColor = Color.White;
            this.Controls.Add(chatPanel);
            chatPanel.BringToFront();

            // INPUT PANEL — Dock Bottom
            Panel inputPanel = new Panel();
            inputPanel.Height = 65;
            inputPanel.Dock = DockStyle.Bottom;
            inputPanel.BackColor = Color.FromArgb(235, 230, 250);
            chatPanel.Controls.Add(inputPanel);

            // BUTTON SEND
            Button sendButton = new Button();
            sendButton.Text = "✉ Trimite";
            sendButton.Size = new Size(110, 35);
            sendButton.Location = new Point(chatPanel.Width - 130, 15);
            sendButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            sendButton.BackColor = Color.FromArgb(167, 147, 214);
            sendButton.ForeColor = Color.White;
            sendButton.FlatStyle = FlatStyle.Flat;
            sendButton.FlatAppearance.BorderSize = 0;
            sendButton.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            sendButton.Cursor = Cursors.Hand;
            sendButton.Click += SendMessage;
            inputPanel.Controls.Add(sendButton);

            // TEXTBOX MESSAGE
            messageBox = new TextBox();
            messageBox.Location = new Point(15, 15);
            messageBox.Size = new Size(chatPanel.Width - 160, 35);
            messageBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            messageBox.Font = new Font("Segoe UI", 11);
            messageBox.BorderStyle = BorderStyle.FixedSingle;
            messageBox.BackColor = Color.White;
            inputPanel.Controls.Add(messageBox);

            messageBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendMessage(s, e);
                    e.SuppressKeyPress = true;
                }
            };

            // CHAT HEADER — Dock Top
            Panel chatHeader = new Panel();
            chatHeader.Height = 50;
            chatHeader.Dock = DockStyle.Top;
            chatHeader.BackColor = Color.FromArgb(245, 242, 255);
            chatPanel.Controls.Add(chatHeader);

            Label lblChatTitle = new Label();
            lblChatTitle.Text = "Selectează un utilizator";
            lblChatTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblChatTitle.ForeColor = Color.FromArgb(90, 40, 160);
            lblChatTitle.Location = new Point(15, 12);
            lblChatTitle.AutoSize = true;
            chatHeader.Controls.Add(lblChatTitle);

            // CHAT BOX — Dock Fill
            chatBox = new ListBox();
            chatBox.Dock = DockStyle.Fill;
            chatBox.BackColor = Color.FromArgb(250, 248, 255);
            chatBox.BorderStyle = BorderStyle.None;
            chatBox.Font = new Font("Segoe UI", 10);
            chatBox.DrawMode = DrawMode.OwnerDrawFixed;
            chatBox.ItemHeight = 35;
            chatPanel.Controls.Add(chatBox);

            inputPanel.BringToFront();

            chatBox.DrawItem += (s, e) =>
            {
                if (e.Index < 0) return;
                string item = chatBox.Items[e.Index].ToString();
                bool isMine = item.StartsWith("[Tu]");

                e.Graphics.FillRectangle(
                    new SolidBrush(isMine ? Color.FromArgb(235, 225, 255) : Color.FromArgb(250, 248, 255)),
                    e.Bounds);

                e.Graphics.DrawString(item,
                    new Font("Segoe UI", 10),
                    new SolidBrush(isMine ? Color.FromArgb(90, 40, 160) : Color.FromArgb(50, 50, 50)),
                    new Rectangle(e.Bounds.Left + 10, e.Bounds.Top + 5, e.Bounds.Width - 20, e.Bounds.Height - 5));
            };

            // SELECT USER
            conversations.SelectedIndexChanged += (s, e) =>
            {
                if (conversations.SelectedItem == null) return;
                selectedUsername = conversations.SelectedItem.ToString();
                var user = allUsers.FirstOrDefault(u => u.Username == selectedUsername);
                if (user != null)
                {
                    selectedUserId = user.Id;
                    ShowUserProfile(user.Id);
                    LoadMessages();
                    lblChatTitle.Text = "💬 " + selectedUsername;
                }
            };

            // RESIZE EVENT
            this.Resize += (s, e) =>
            {
                userLabel.Location = new Point(this.Width - 180, 25);
                menuBtn.Location = new Point(this.Width - 90, 18);
                dropdown.Location = new Point(this.Width - 210, 70);

                if (messageBox != null && sendButton != null && inputPanel != null)
                {
                    sendButton.Location = new Point(inputPanel.Width - 130, 15);
                    messageBox.Width = inputPanel.Width - 160;
                }
            };
        }

        private void LoadMessages()
        {
            chatBox.Items.Clear();
            if (selectedUserId == -1) return;

            var messages = MessageRepository.GetMessagesBetweenUsers(currentUser.Id, selectedUserId);

            foreach (var msg in messages)
            {
                string prefix = msg.SenderId == currentUser.Id ? "[Tu]" : "[" + msg.SenderName + "]";
                chatBox.Items.Add($"{prefix} {msg.SentAt:HH:mm}: {msg.Text}");
            }

            if (chatBox.Items.Count > 0)
                chatBox.TopIndex = chatBox.Items.Count - 1;
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

            if (selectedUserId == -1)
            {
                MessageBox.Show("⚠ Selectează un utilizator!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var message = new Message
            {
                SenderId = currentUser.Id,
                SenderName = currentUser.Username,
                ReceiverId = selectedUserId,
                ReceiverName = selectedUsername,
                Text = messageBox.Text.Trim(),
                SentAt = DateTime.Now
            };

            bool success = MessageRepository.AddMessage(message);

            if (success)
            {
                messageBox.Clear();
                LoadMessages();
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
    }
}