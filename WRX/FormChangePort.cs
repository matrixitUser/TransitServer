using System;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
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

        private void BtOk_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void FormChangePort_Load(object sender, System.EventArgs e)
        {
            btOk.Enabled = false;
            tmpPort = txtPort.Text;
        }
        private void TxtPort_TextChanged(object sender, System.EventArgs e)
        {
            labЗPrompt.Text = "Это предыдущий порт,\nможно не сохранять";
            if (txtPort.Text == tmpPort) labЗPrompt.Visible = true;
            else labЗPrompt.Visible = false;

            Regex regex = new Regex(@"^\d{1,5}$");
            if (regex.IsMatch(txtPort.Text))
            {
                txtPort.BackColor = Color.White;
                btOk.Enabled = true;
            }
            else
            {
                txtPort.BackColor = Color.Red;
                btOk.Enabled = false;
            }
        }
    }
}
