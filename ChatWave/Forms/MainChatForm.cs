using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChatWave.Forms
{
    public partial class MainChatForm : Form
    {
        ListBox conversations;
        ListBox chatBox;
        TextBox messageBox;

        public MainChatForm()
        {
            InitializeComponent();
            DesignUI();
        }

        private void DesignUI()
        {
            // 🎨 Culori
            Color movHeader = Color.FromArgb(167, 147, 214);
            Color movCard = Color.FromArgb(235, 230, 250);
            Color movButton = Color.FromArgb(160, 130, 210);

            // FORM
            this.Text = "ChatWave - Chat";
            this.Size = new Size(1000, 650);
            this.BackColor = Color.FromArgb(245, 245, 245);

            // ================= HEADER =================
            Panel header = new Panel();
            header.BackColor = movHeader;
            header.Size = new Size(this.Width, 70);
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
            userLabel.Text = "👤 Andreea";
            userLabel.ForeColor = Color.White;
            userLabel.Font = new Font("Segoe UI", 10);
            userLabel.AutoSize = true;
            userLabel.Location = new Point(700, 25);
            header.Controls.Add(userLabel);

            Button logout = new Button();
            logout.Text = "Logout";
            logout.Size = new Size(90, 30);
            logout.Location = new Point(850, 20);
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

            // ================= LEFT: CONVERSAȚII =================
            Panel leftPanel = new Panel();
            leftPanel.Size = new Size(250, this.Height);
            leftPanel.Location = new Point(0, 70);
            leftPanel.BackColor = movCard;
            this.Controls.Add(leftPanel);

            Label convLabel = new Label();
            convLabel.Text = "💬 Conversații";
            convLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            convLabel.Location = new Point(10, 10);
            convLabel.AutoSize = true;
            leftPanel.Controls.Add(convLabel);

            conversations = new ListBox();
            conversations.Size = new Size(230, 500);
            conversations.Location = new Point(10, 40);
            conversations.BackColor = Color.White;

            conversations.Items.Add("mihai_dev");
            conversations.Items.Add("alex_ion");
            conversations.Items.Add("support");

            leftPanel.Controls.Add(conversations);

            // ================= CHAT =================
            Panel chatPanel = new Panel();
            chatPanel.Size = new Size(700, 520);
            chatPanel.Location = new Point(250, 70);
            chatPanel.BackColor = Color.White;
            this.Controls.Add(chatPanel);

            chatBox = new ListBox();
            chatBox.Size = new Size(670, 420);
            chatBox.Location = new Point(15, 15);
            chatBox.BackColor = movCard;

            chatBox.Items.Add("Bună! 👋");
            chatBox.Items.Add("Salut!");
            chatPanel.Controls.Add(chatBox);

            // ================= INPUT =================
            messageBox = new TextBox();
            messageBox.Size = new Size(500, 35);
            messageBox.Location = new Point(15, 450);
            chatPanel.Controls.Add(messageBox);

            Button sendButton = new Button();
            sendButton.Text = "Send";
            sendButton.Size = new Size(120, 35);
            sendButton.Location = new Point(530, 450);
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
        }

        private void SendMessage(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(messageBox.Text))
            {
                chatBox.Items.Add("Tu: " + messageBox.Text);
                messageBox.Clear();
            }
        }
    }
}