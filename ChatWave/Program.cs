using System;
using System.Windows.Forms;
using ChatWave.Forms;

namespace ChatWave
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainChatForm());


        }
    }
}