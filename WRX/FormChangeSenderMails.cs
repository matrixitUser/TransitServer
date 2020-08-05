using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TransitServer
{
    public partial class FormChangeSenderMails : Form
    {
        public bool isCustomGroup = false;
        public FormChangeSenderMails()
        {
            InitializeComponent();
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            if(isCustomGroup == true)
            {

            }
            else
            {
                SQLite.Instance.UpdateDbMails(labelImeiModem.Text, txtSenderMail.Text, txtNameSenderMail.Text, txtRecieverMail.Text, txtSubjectMail.Text, txtSmtpClient.Text, txtSmtpPort.Text, txtSenderPassword.Text);
            }
            DialogResult = DialogResult.OK;
        }

        private void TxtSmtpPort_TextChanged(object sender, EventArgs e)
        {
            Regex regex = new Regex(@"^\d+$");
            if (regex.IsMatch(txtSmtpPort.Text))
            {
                txtSmtpPort.BackColor = Color.White;
                buttonSave.Enabled = true;
            }
            else
            {
                txtSmtpPort.BackColor = Color.Red;
                buttonSave.Enabled = false;
            }
        }

        private void FormChangeSenderMails_Load(object sender, EventArgs e)
        {
            
        }

        private void TxtSenderMail_TextChanged(object sender, EventArgs e)
        {
            if (txtSenderMail.Text!=String.Empty)
            {
                txtSenderMail.BackColor = Color.White;
                buttonSave.Enabled = true;
            }
            else
            {
                txtSenderMail.BackColor = Color.Red;
                buttonSave.Enabled = false;
            }
        }

        private void TxtNameSenderMail_TextChanged(object sender, EventArgs e)
        {
            if (txtNameSenderMail.Text != String.Empty)
            {
                txtNameSenderMail.BackColor = Color.White;
                buttonSave.Enabled = true;
            }
            else
            {
                txtNameSenderMail.BackColor = Color.Red;
                buttonSave.Enabled = false;
            }
        }

        private void TxtRecieverMail_TextChanged(object sender, EventArgs e)
        {
            if (txtRecieverMail.Text != String.Empty)
            {
                txtRecieverMail.BackColor = Color.White;
                buttonSave.Enabled = true;
            }
            else
            {
                txtRecieverMail.BackColor = Color.Red;
                buttonSave.Enabled = false;
            }
        }

        private void TxtSubjectMail_TextChanged(object sender, EventArgs e)
        {
            if (txtSubjectMail.Text != String.Empty)
            {
                txtSubjectMail.BackColor = Color.White;
                buttonSave.Enabled = true;
            }
            else
            {
                txtSubjectMail.BackColor = Color.Red;
                buttonSave.Enabled = false;
            }
        }

        private void TxtSmtpClient_TextChanged(object sender, EventArgs e)
        {
            if (txtSmtpClient.Text != String.Empty)
            {
                txtSmtpClient.BackColor = Color.White;
                buttonSave.Enabled = true;
            }
            else
            {
                txtSmtpClient.BackColor = Color.Red;
                buttonSave.Enabled = false;
            }
        }

        private void TxtSenderPassword_TextChanged(object sender, EventArgs e)
        {
            if (txtSenderPassword.Text != String.Empty)
            {
                txtSenderPassword.BackColor = Color.White;
                buttonSave.Enabled = true;
            }
            else
            {
                txtSenderPassword.BackColor = Color.Red;
                buttonSave.Enabled = false;
            }
        }
    }
}
