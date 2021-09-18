
using System.Windows.Forms;

namespace RunFFMPEG
{
    public partial class TheForm : Form
    {
        public TheForm(string msg)
        {
                InitializeComponent();
            this.Text = "error occured:::";
            this.WindowState = FormWindowState.Maximized;
            tbOne.Text = msg;
        }

    }
}
