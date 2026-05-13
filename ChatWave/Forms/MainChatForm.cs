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

namespace ChatWave.Forms
{
    public partial class MainChatForm : BaseForm
    {
        ListBox conversations;
        ListBox chatBox;
        TextBox messageBox;
        private LoggedUser currentUser;

        // Profile panel components
        Panel pnlUserProfile;
        Label lblProfileName;
        PictureBox pbAvatar;
        Label lblRoleRight;
        Label lblEmailRight;
        Label lblPhoneRight;
        Button btnChangePhoto;

        // Cache pentru imagini
        private Dictionary<int, Image> userImages = new Dictionary<int, Image>();

        // Lista completă de utilizatori
        private List<User> allUsers;

        public MainChatForm(LoggedUser user)
        {
            currentUser = user;
            InitializeComponent();

            // Încarcă toți utilizatorii o singură dată
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
                // Deschide propriul profil
                ShowUserProfile(currentUser.Id);
            });
            AddMenuButton(dropdown, "  Setări", 75, movButton, (s, e) =>
            {
                dropdown.Visible = false;
                SettingsForm settings = new SettingsForm(currentUser);
                settings.ShowDialog();
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

            // ── PANEL PROFIL DREAPTA ─────────────────────────────────
            pnlUserProfile = new Panel();
            pnlUserProfile.Width = 250;
            pnlUserProfile.Height = this.Height - 70;
            pnlUserProfile.Location = new Point(this.Width - 250, 70);
            pnlUserProfile.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            pnlUserProfile.BackColor = movCard;
            pnlUserProfile.Visible = false;
            this.Controls.Add(pnlUserProfile);

            // Avatar PictureBox
            pbAvatar = new PictureBox();
            pbAvatar.Size = new Size(100, 100);
            pbAvatar.Location = new Point((pnlUserProfile.Width - 100) / 2, 20);
            pbAvatar.SizeMode = PictureBoxSizeMode.Zoom;
            pbAvatar.BackColor = movHeader;
            pbAvatar.Cursor = Cursors.Hand;
            pbAvatar.Click += (s, e) =>
            {
                // Permite schimbarea pozei doar pentru profilul curent
                if (lblProfileName.Text == currentUser.Username)
                {
                    ChangeProfilePhoto();
                }
            };
            pnlUserProfile.Controls.Add(pbAvatar);

            // Buton schimbare poză
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

            // ── LEFT PANEL (toți utilizatorii) ───────────────────────
            Panel leftPanel = new Panel();
            leftPanel.Width = 300;
            leftPanel.Height = this.Height - 70;
            leftPanel.Location = new Point(0, 70);
            leftPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
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

            // Desenare personalizată cu poză
            conversations.DrawItem += (s, e) =>
            {
                if (e.Index < 0) return;
                bool selected = (e.State & DrawItemState.Selected) != 0;
                e.Graphics.FillRectangle(
                    selected ? new SolidBrush(Color.FromArgb(235, 225, 255)) : Brushes.White,
                    e.Bounds);

                string username = conversations.Items[e.Index].ToString();
                var user = allUsers.FirstOrDefault(u => u.Username == username);

                // Desenează poza de profil
                Rectangle avatarRect = new Rectangle(e.Bounds.Left + 8, e.Bounds.Top + 8, 38, 38);
                e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(200, 190, 220)), avatarRect);

                if (user != null)
                {
                    Image userImage = GetUserImage(user.Id);
                    if (userImage != null)
                    {
                        e.Graphics.DrawImage(userImage, avatarRect);
                    }
                    else
                    {
                        using (Font avatarFont = new Font("Segoe UI", 12, FontStyle.Bold))
                        using (StringFormat sf = new StringFormat())
                        {
                            sf.Alignment = StringAlignment.Center;
                            sf.LineAlignment = StringAlignment.Center;
                            e.Graphics.DrawString(username.Substring(0, 1).ToUpper(),
                                avatarFont, Brushes.White, avatarRect, sf);
                        }
                    }
                }

                // Desenează numele
                Rectangle textRect = new Rectangle(e.Bounds.Left + 55, e.Bounds.Top + 10, e.Bounds.Width - 65, 30);
                using (Font textFont = new Font("Segoe UI", selected ? 11 : 10, selected ? FontStyle.Bold : FontStyle.Regular))
                {
                    Color textColor = selected
                        ? Color.FromArgb(90, 40, 160)
                        : Color.FromArgb(120, 80, 180);
                    e.Graphics.DrawString(username, textFont, new SolidBrush(textColor), textRect);
                }

                e.Graphics.DrawLine(
                    new Pen(Color.FromArgb(240, 235, 250)),
                    e.Bounds.Left, e.Bounds.Bottom - 1,
                    e.Bounds.Right, e.Bounds.Bottom - 1);
            };

            // SelectedIndexChanged - arată profilul utilizatorului selectat
            conversations.SelectedIndexChanged += (s, e) =>
            {
                if (conversations.SelectedItem == null) return;
                string selectedUsername = conversations.SelectedItem.ToString();
                var user = allUsers.FirstOrDefault(u => u.Username == selectedUsername);
                if (user != null)
                {
                    ShowUserProfile(user.Id);
                }
            };

            // Încarcă TOȚI utilizatorii în listă (nu doar conversații)
            var allUsernames = new List<string>();
            foreach (var user in allUsers)
            {
                // Excludem utilizatorul curent din listă
                if (user.Id != currentUser.Id)
                {
                    conversations.Items.Add(user.Username);
                    allUsernames.Add(user.Username);
                    // Preîncarcă imaginea
                    GetUserImage(user.Id);
                }
            }

            // Caută utilizatori
            searchBox.TextChanged += (s, e) =>
            {
                string filter = searchBox.Text.ToLower();
                if (filter == "🔍 caută utilizator...") return;
                conversations.Items.Clear();
                foreach (var name in allUsernames)
                {
                    if (name.ToLower().Contains(filter))
                        conversations.Items.Add(name);
                }
            };

            // ── CHAT PANEL ───────────────────────────────────────────
            Panel chatPanel = new Panel();
            chatPanel.Left = leftPanel.Right;
            chatPanel.Top = 70;
            chatPanel.Width = this.Width - leftPanel.Width;
            chatPanel.Height = this.Height - 70;
            chatPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            chatPanel.BackColor = Color.White;
            this.Controls.Add(chatPanel);

            chatBox = new ListBox();
            chatBox.Left = 15;
            chatBox.Top = 15;
            chatBox.Width = chatPanel.Width - 30;
            chatBox.Height = chatPanel.Height - 80;
            chatBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            chatBox.BackColor = movCard;
            chatPanel.Controls.Add(chatBox);

            messageBox = new TextBox();
            messageBox.Left = 15;
            messageBox.Top = chatPanel.Height - 55;
            messageBox.Width = chatPanel.Width - 140;
            messageBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            chatPanel.Controls.Add(messageBox);

            Button sendButton = new Button();
            sendButton.Text = "Send";
            sendButton.Left = chatPanel.Width - 120;
            sendButton.Top = chatPanel.Height - 55;
            sendButton.Width = 105;
            sendButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            sendButton.BackColor = movButton;
            sendButton.ForeColor = Color.White;
            sendButton.FlatStyle = FlatStyle.Flat;
            sendButton.FlatAppearance.BorderSize = 0;
            sendButton.Click += SendMessage;
            chatPanel.Controls.Add(sendButton);

            messageBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendMessage(s, e);
                    e.SuppressKeyPress = true;
                }
            };

            // Eveniment de redimensionare
            this.Resize += (s, e) =>
            {
                userLabel.Location = new Point(this.Width - 180, 25);
                menuBtn.Location = new Point(this.Width - 90, 18);
                dropdown.Location = new Point(this.Width - 210, 70);
                pnlUserProfile.Location = new Point(this.Width - 250, 70);
                pnlUserProfile.Height = this.Height - 70;
                leftPanel.Height = this.Height - 70;
                chatPanel.Width = this.Width - leftPanel.Width;
                chatPanel.Height = this.Height - 70;
                chatBox.Width = chatPanel.Width - 30;
                chatBox.Height = chatPanel.Height - 80;
                messageBox.Top = chatPanel.Height - 55;
                messageBox.Width = chatPanel.Width - 140;
                sendButton.Left = chatPanel.Width - 120;
                sendButton.Top = chatPanel.Height - 55;
            };
        }

        // Arată profilul unui utilizator
        private void ShowUserProfile(int userId)
        {
            var user = allUsers.FirstOrDefault(u => u.Id == userId);
            if (user == null) return;

            pnlUserProfile.Visible = true;
            lblProfileName.Text = user.Username;
            lblRoleRight.Text = "Rol: " + (user.Role ?? "user");
            lblEmailRight.Text = "📧 " + (user.Email ?? "-");
            lblPhoneRight.Text = "📱 " + (user.Phone ?? "-");

            // Încarcă imaginea (șterge din cache pentru a reîncărca)
            if (userImages.ContainsKey(userId))
                userImages.Remove(userId);

            LoadImageIntoPictureBox(pbAvatar, userId);

            // Arată butonul de schimbare poză DOAR pentru utilizatorul curent
            btnChangePhoto.Visible = (userId == currentUser.Id);
        }

        // ==================== METODE AJUTĂTOARE ====================

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

        private void AddMenuButton(Panel parent, string text, int y,
                                   Color color, EventHandler onClick)
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

        private void SendMessage(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(messageBox.Text))
            {
                chatBox.Items.Add("Tu: " + messageBox.Text);
                messageBox.Clear();
            }
        }

        // ==================== METODE PENTRU POZE ====================

        private Image ByteArrayToImage(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
                return null;

            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }

        private byte[] ImageToByteArray(Image image)
        {
            if (image == null)
                return null;

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
                {
                    g.FillEllipse(brush, rect);
                }

                string initial = username.Length > 0 ? username.Substring(0, 1).ToUpper() : "?";
                using (Font avatarFont = new Font("Segoe UI", size / 2, FontStyle.Bold))
                using (StringFormat sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    g.DrawString(initial, avatarFont, Brushes.White, rect, sf);
                }

                using (Pen pen = new Pen(Color.White, 3))
                {
                    g.DrawEllipse(pen, rect);
                }
            }
            return bmp;
        }

        private Image GetUserImage(int userId)
        {
            // Verifică în cache
            if (userImages.ContainsKey(userId))
                return userImages[userId];

            // Încarcă din baza de date
            byte[] imageData = UserRepository.GetProfileImage(userId);
            Image img = null;

            if (imageData != null && imageData.Length > 0)
            {
                try
                {
                    img = ByteArrayToImage(imageData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Eroare la conversie imagine: {ex.Message}");
                }
            }

            if (img == null)
            {
                var user = allUsers.FirstOrDefault(u => u.Id == userId);
                string username = user?.Username ?? "User";
                img = GenerateDefaultAvatar(username, 100);
            }

            // Salvează în cache
            userImages[userId] = img;
            return img;
        }

        private void LoadImageIntoPictureBox(PictureBox pb, int userId)
        {
            try
            {
                Image img = GetUserImage(userId);
                if (img != null)
                {
                    pb.Image = img;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare la încărcarea imaginii: {ex.Message}");
            }
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
                                // Șterge din cache pentru a reîncărca
                                if (userImages.ContainsKey(currentUser.Id))
                                    userImages.Remove(currentUser.Id);

                                // Reîncarcă imaginea
                                LoadImageIntoPictureBox(pbAvatar, currentUser.Id);

                                // Reîmprospătează lista de conversații
                                conversations.Invalidate();

                                MessageBox.Show("Poza de profil a fost actualizată cu succes!",
                                    "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Eroare la salvarea pozei în baza de date!",
                                    "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Eroare la salvarea pozei: {ex.Message}",
                            "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
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