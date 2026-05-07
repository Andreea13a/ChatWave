namespace ChatWave.Forms
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // LoginForm
            this.ClientSize = new System.Drawing.Size(400, 520);
            this.Name = "LoginForm";
            this.Text = "Login";

            this.ResumeLayout(false);
        }
    }
}