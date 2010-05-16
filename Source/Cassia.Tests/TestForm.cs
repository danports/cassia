using System.Windows.Forms;
using AxMSTSCLib;

namespace Cassia.Tests
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        public AxMsRdpClient7NotSafeForScripting RdpControl
        {
            get { return rdpControl; }
        }
    }
}