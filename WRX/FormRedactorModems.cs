using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TransitServer
{
    public partial class FormRedactorModems : Form
    {
        public string idModem;
        public tsConfig config;
        public FormRedactorModems()
        {
            InitializeComponent();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            ViewModems(SQLite.Instance.GetModems());
            Close();
        }

        private void BtnDeleteModem_Click(object sender, EventArgs e)
        {
            DialogFormOk dialogFormOk = new DialogFormOk();
            if(dialogFormOk.ShowDialog() == DialogResult.OK)
            {
                SQLite.Instance.DeleteModems(idModem);
                ViewModems(SQLite.Instance.GetModems());
                Close();
                MessageBox.Show("Объект удален!");
            }
        }

        const string MODEMSCOLID = "id";
        const string MODEMSCOLIMEI = "imei";
        const string MODEMSCOLPORT = "port";
        const string MODEMSCOLNAME = "name";
        const string MODEMSCOLLASTCONNECTION = "lastConnection";
        const string MODEMSCOLLACTIVECONNECTION = "activeConnection";
        public string oldNameModem;
        public void ViewModems(List<dynamic> records)
        {
            Form1 form1 = this.Owner as Form1;
            form1.dgvModems.Rows.Clear();
            if (!records.Any())
            {
                return;
            }
            foreach (var rec in records)
            {
                form1.dgvModems.Rows.Add();
                form1.dgvModems.Rows[form1.dgvModems.RowCount - 1].Cells[MODEMSCOLID].Value = rec.id;
                form1.dgvModems.Rows[form1.dgvModems.RowCount - 1].Cells[MODEMSCOLIMEI].Value = rec.imei;
                form1.dgvModems.Rows[form1.dgvModems.RowCount - 1].Cells[MODEMSCOLPORT].Value = rec.port;
                form1.dgvModems.Rows[form1.dgvModems.RowCount - 1].Cells[MODEMSCOLNAME].Value = rec.name;
                form1.dgvModems.Rows[form1.dgvModems.RowCount - 1].Cells[MODEMSCOLLASTCONNECTION].Value = rec.lastConnection;
                if ((int)rec.activeConnection == 1)
                    form1.dgvModems.Rows[form1.dgvModems.RowCount - 1].Cells[MODEMSCOLLACTIVECONNECTION].Value = Properties.Resources.tick;
                else form1.dgvModems.Rows[form1.dgvModems.RowCount - 1].Cells[MODEMSCOLLACTIVECONNECTION].Value = Properties.Resources.cross;
            }
        }

        private void BtnSaveChanges_Click(object sender, EventArgs e)
        {
            SQLite.Instance.UpdateNameModemsbyImei(txtImei.Text, txtNameModem.Text);
            MessageBox.Show("Изменения сохранены!");
            //MessageBox.Show("Кнопка в разработке!");
        }

        private void FormRedactorModems_Load(object sender, EventArgs e)
        {
            tsConfig tmpConfig = config;
            cbChannel1.Items.AddRange(new object[] { "server", "listener", "not use" });
            cbChannel2.Items.AddRange(new object[] { "server", "listener", "not use" });
            cbChannel3.Items.AddRange(new object[] { "server", "listener", "not use" });
            cbChannel1.DropDownStyle = ComboBoxStyle.DropDownList;
            cbChannel2.DropDownStyle = ComboBoxStyle.DropDownList;
            cbChannel3.DropDownStyle = ComboBoxStyle.DropDownList;

            if (tmpConfig.profile != null)
            {
                if (Encoding.UTF8.GetString(tmpConfig.profile[0].ip_port).Contains(':'))
                {
                    txtIp1.Text = Encoding.UTF8.GetString(tmpConfig.profile[0].ip_port).Split(':')[0];
                    txtPort1.Text = Encoding.UTF8.GetString(tmpConfig.profile[0].ip_port).Split(':')[1];
                    cbChannel1.SelectedItem = "server";
                }
                if (Encoding.UTF8.GetString(tmpConfig.profile[1].ip_port).Contains(':'))
                {
                    txtIp2.Text = Encoding.UTF8.GetString(tmpConfig.profile[1].ip_port).Split(':')[0];
                    txtPort2.Text = Encoding.UTF8.GetString(tmpConfig.profile[1].ip_port).Split(':')[1];
                }
                if (Encoding.UTF8.GetString(tmpConfig.profile[2].ip_port).Contains(':'))
                {
                    txtIp3.Text = Encoding.UTF8.GetString(tmpConfig.profile[2].ip_port).Split(':')[0];
                    txtPort3.Text = Encoding.UTF8.GetString(tmpConfig.profile[2].ip_port).Split(':')[1];
                }
            }

            if(tmpConfig.profileCount == 0)
            {
                cbChannel1.SelectedItem = "not use";
                cbChannel2.SelectedItem = "not use";
                cbChannel3.SelectedItem = "not use";
                cbChannel1.Enabled = false;
                cbChannel2.Enabled = false;
                txtIp1.Enabled = false;
                txtIp2.Enabled = false;
                txtPort1.Enabled = false;
                txtPort2.Enabled = false;
                cbChannel3.Enabled = false;
                txtIp3.Enabled = false;
                txtPort3.Enabled = false;
            }
            if (tmpConfig.profileCount == 1)
            {
                cbChannel2.SelectedItem = "not use";
                cbChannel3.SelectedItem = "not use";
                cbChannel2.Enabled = false;
                txtIp2.Enabled = false;
                txtPort2.Enabled = false;
                cbChannel3.Enabled = false;
                txtIp3.Enabled = false;
                txtPort3.Enabled = false;
            }
            if (tmpConfig.profileCount == 2)
            {
                cbChannel3.SelectedItem = "not use";
                cbChannel3.Enabled = false;
                txtIp3.Enabled = false;
                txtPort3.Enabled = false;
            }

            if(tmpConfig.apnName != null)
            {
                txtApn1.Text = Encoding.UTF8.GetString(tmpConfig.apnName[0].APN);
                if (tmpConfig.apnCount > 1)
                    txtApn2.Text = Encoding.UTF8.GetString(tmpConfig.apnName[1].APN);
                else txtApn2.Enabled = false;
            }

            cbType1.DropDownStyle = ComboBoxStyle.DropDownList;
            cbType2.DropDownStyle = ComboBoxStyle.DropDownList;
            cbType3.DropDownStyle = ComboBoxStyle.DropDownList;
            cbType4.DropDownStyle = ComboBoxStyle.DropDownList;

            if (tmpConfig.u32CounterNA != null)
            {
                txtCounterNa1.Text = tmpConfig.u32CounterNA[0].ToString();
                txtCounterNa2.Text = tmpConfig.u32CounterNA[1].ToString();
                txtCounterNa3.Text = tmpConfig.u32CounterNA[2].ToString();
                txtCounterNa4.Text = tmpConfig.u32CounterNA[3].ToString();
            }
            
            txtPeriodEvent.Text = tmpConfig.PeriodEvent.ToString();
            txtNetworkAdres.Text = tmpConfig.u8NetworkAddress.ToString();
            txtMode.Text = tmpConfig.u8Mode.ToString();
            txtRelease.Text = tmpConfig.u32ReleaseTs.ToString();
            txtFlashVer.Text = tmpConfig.u16FlashVer.ToString();
            if(tmpConfig.sUart != null)
            {
                cbBaudRate1.SelectedItem = (UInt32)tmpConfig.sUart[0].u32BaudRate;
                cbBaudRate2.SelectedItem = (UInt32)tmpConfig.sUart[1].u32BaudRate;
                cbBaudRate3.SelectedItem = (UInt32)tmpConfig.sUart[3].u32BaudRate;
                txtWordLen1.Text = tmpConfig.sUart[0].u8WordLen.ToString();
                txtWordLen2.Text = tmpConfig.sUart[1].u8WordLen.ToString();
                txtWordLen3.Text = tmpConfig.sUart[2].u8WordLen.ToString();
                txtStopBits1.Text = tmpConfig.sUart[0].u8StopBits.ToString();
                txtStopBits2.Text = tmpConfig.sUart[1].u8StopBits.ToString();
                txtStopBits3.Text = tmpConfig.sUart[2].u8StopBits.ToString();
                txtParity1.Text = tmpConfig.sUart[0].u8Parity.ToString();
                txtParity2.Text = tmpConfig.sUart[1].u8Parity.ToString();
                txtParity3.Text = tmpConfig.sUart[2].u8Parity.ToString();
            }
            btnSaveChanges.Enabled = false;
            cbBaudRate1.Items.AddRange(new object[] { 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 56000 });
            cbBaudRate1.SelectedItem = 4800;
            cbBaudRate2.Items.AddRange(new object[] { 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 56000 });
            cbBaudRate2.SelectedItem = 4800;
            cbBaudRate3.Items.AddRange(new object[] { 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 56000 });
            cbBaudRate3.SelectedItem = 4800;
        }

        private void TxtName_TextChanged(object sender, EventArgs e)
        {
            if (txtNameModem.Text != "" && txtNameModem.Text != oldNameModem) btnSaveChanges.Enabled = true;
        }
    }
}
