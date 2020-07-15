using System.Windows.Forms;

namespace TransitServer
{
    public partial class FormChangePort : Form
    {
        string tmpPort;
        public FormChangePort()
        {
            InitializeComponent();
        }

        private void btOk_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void FormChangePort_Load(object sender, System.EventArgs e)
        {
            btOk.Enabled = false;
            tmpPort = txtPort.Text;
        }

        private void txtPort_TextChanged(object sender, System.EventArgs e)
        {
            if (tmpPort!=txtPort.Text) btOk.Enabled = true;
        }
    }
}
