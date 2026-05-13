using System.Windows.Forms;

namespace ChatWave.Forms
{
    public class BaseForm : Form
    {
        public BaseForm()
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
        }
    }
}