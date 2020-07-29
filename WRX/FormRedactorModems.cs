using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
            toolTipInput.AutoPopDelay = 5000;
            toolTipInput.InitialDelay = 100;
            toolTipInput.ReshowDelay = 500;
            toolTipInput.ShowAlways = true;
            string textIpToolTip = "Правильная форма записи IP-адреса является запись\n в виде четырёх десятичных чисел значением от 0 до 255,\n разделённых точками, например, 192.168.0.3";
            toolTipInput.SetToolTip(txtTestIp, textIpToolTip);
            toolTipInput.SetToolTip(txtIp1, textIpToolTip);
            toolTipInput.SetToolTip(txtIp2, textIpToolTip);
            toolTipInput.SetToolTip(txtIp3, textIpToolTip);
            string textPortToolTip = "Правильная форма записи порта является запись\n в виде пяти целых чисел, например 12345";
            toolTipInput.SetToolTip(txtTestPort, textPortToolTip);
            toolTipInput.SetToolTip(txtPort1, textPortToolTip);
            toolTipInput.SetToolTip(txtPort2, textPortToolTip);
            toolTipInput.SetToolTip(txtPort3, textPortToolTip);
            string textNetworkAdressToolTip = "Правильная форма записи сетевого адреса счетчика\n является запись в виде любых целых чисел, например 48";
            toolTipInput.SetToolTip(txtTestNA, textNetworkAdressToolTip);
            toolTipInput.SetToolTip(txtCounterNa1, textPortToolTip);
            toolTipInput.SetToolTip(txtCounterNa2, textPortToolTip);
            toolTipInput.SetToolTip(txtCounterNa3, textPortToolTip);
            toolTipInput.SetToolTip(txtCounterNa4, textPortToolTip);

            tsConfig tmpConfig = config;
            object[] dropList = new object[] { "server", "listener", "not use" };
            cbChannel1.Items.AddRange(dropList);
            cbChannel2.Items.AddRange(dropList);
            cbChannel3.Items.AddRange(dropList);
            cbChannel1.DropDownStyle = ComboBoxStyle.DropDownList;
            cbChannel2.DropDownStyle = ComboBoxStyle.DropDownList;
            cbChannel3.DropDownStyle = ComboBoxStyle.DropDownList;

            if (tmpConfig.u8server != null)
            {
                if (Encoding.UTF8.GetString(tmpConfig.u8server).Contains(':'))
                {
                    txtIp1.Text = Encoding.UTF8.GetString(tmpConfig.u8server).Split(':')[0];
                    txtPort1.Text = Encoding.UTF8.GetString(tmpConfig.u8server).Split(':')[1];
                    cbChannel1.SelectedItem = dropList[0];
                }
            }
            if (tmpConfig.u8client != null)
            {
                if (Encoding.UTF8.GetString(tmpConfig.u8client).Contains(':'))
                {
                    txtIp2.Text = Encoding.UTF8.GetString(tmpConfig.u8client).Split(':')[0];
                    txtPort2.Text = Encoding.UTF8.GetString(tmpConfig.u8client).Split(':')[1];
                    cbChannel2.SelectedItem = dropList[1];
                }
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
            else
            {
                txtCounterNa1.Enabled = false;
                txtCounterNa2.Enabled = false;
                txtCounterNa3.Enabled = false;
                txtCounterNa4.Enabled = false;
            }
            
            txtPeriodEvent.Text = tmpConfig.PeriodEvent.ToString();
            txtNetworkAdres.Text = tmpConfig.u8NetworkAddress.ToString();
            txtMode.Text = tmpConfig.u8Mode.ToString();
            txtRelease.Text = tmpConfig.u32ReleaseTs.ToString();
            txtFlashVer.Text = tmpConfig.u16FlashVer.ToString();
            if(tmpConfig.apnName != null)
            {
                txtApn1.Text = Encoding.UTF8.GetString(tmpConfig.apnName[0].APN);
                txtApn2.Text = Encoding.UTF8.GetString(tmpConfig.apnName[1].APN);
            }
                
            if (tmpConfig.sUart != null)
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

        #region ComboBoxIndexChanged
        private void CbChannel1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxIndexChanged(cbChannel1, txtIp1, txtPort1);
        }
        private void CbChannel2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxIndexChanged(cbChannel2, txtIp2, txtPort2);
        }
        private void cbChannel3_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxIndexChanged(cbChannel3, txtIp3, txtPort3);
        }
        private void ComboBoxIndexChanged(ComboBox comboBox, TextBox txtIP, TextBox txtPort)
        {
            if (comboBox.SelectedItem.ToString() == "server")
            {
                txtIP.Enabled = true;
                //txtIP.Text = Encoding.UTF8.GetString(config.profile[0].ip_port).Split(':')[0];
                txtPort.Enabled = true;
                //txtPort.Text = Encoding.UTF8.GetString(config.profile[0].ip_port).Split(':')[1];
                txtIP.BackColor = Color.White;
                txtPort.BackColor = Color.White;
            }
            if (comboBox.SelectedItem.ToString() == "listener")
            {
                txtIP.Enabled = true;
                //txtIP.Text = Encoding.UTF8.GetString(config.profile[0].ip_port).Split(':')[0];
                txtPort.Enabled = true;
               // txtPort.Text = Encoding.UTF8.GetString(config.profile[0].ip_port).Split(':')[1];
                txtIP.BackColor = Color.White;
                txtPort.BackColor = Color.White;
            }
            if (comboBox.SelectedItem.ToString() == "not use")
            {
                txtIP.Enabled = false;
                txtIP.Text = String.Empty;
                txtPort.Enabled = false;
                txtPort.Text = String.Empty;
                txtIP.BackColor = Color.White;
                txtPort.BackColor = Color.White;
            }
        }
        #endregion

        #region TextBoxTextChanged
        private void TextBoxChanged(TextBox textBox, Button buttonSave, string oldName)
        {
            if (textBox.Text == String.Empty)
            {
                textBox.BackColor = Color.Red;
                buttonSave.Enabled = false;
            }
            else
            {
                textBox.BackColor = Color.White;
                if (textBox.Text != oldName) buttonSave.Enabled = true;
            }
        }
        private void TextBoxPortChanged(TextBox textBox, Button buttonSave, string oldName)
        {
            Regex regex = new Regex(@"^\d{5}$");
            if (regex.IsMatch(textBox.Text) & textBox.Text != oldName)
            {
                textBox.BackColor = Color.White;
                buttonSave.Enabled = true;
            }
            else
            {
                textBox.BackColor = Color.Red;
                buttonSave.Enabled = false;
            }
        }
        private void TextBoxCounterNaChanged(TextBox textBox, Button buttonSave, string oldName)
        {
            Regex regex = new Regex(@"^\d+$");
            if (regex.IsMatch(textBox.Text) & textBox.Text != oldName)
            {
                textBox.BackColor = Color.White;
                buttonSave.Enabled = true;
            }
            else
            {
                textBox.BackColor = Color.Red;
                buttonSave.Enabled = false;
            }
        }
        private void TextBoxIpChanged(TextBox textBox, Button buttonSave, string oldName)
        {
            Regex regex = new Regex(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$");
            if (regex.IsMatch(textBox.Text) & textBox.Text != oldName)
            {
                textBox.BackColor = Color.White;
                buttonSave.Enabled = true;
            }
            else
            {
                textBox.BackColor = Color.Red;
                buttonSave.Enabled = false;
            }
        }
        private void TxtName_TextChanged(object sender, EventArgs e)
        {
            TextBoxChanged(txtNameModem, btnSaveChanges, oldNameModem);
        }
        private void TxtApn1_TextChanged(object sender, EventArgs e)
        {
            string oldNameApn1 = Encoding.UTF8.GetString(config.apnName[0].APN);
            TextBoxChanged(txtApn1, btnSaveChanges, oldNameApn1);
        }
        private void TxtApn2_TextChanged(object sender, EventArgs e)
        {
            string oldNameApn2 = Encoding.UTF8.GetString(config.apnName[0].APN);
            TextBoxChanged(txtApn2, btnSaveChanges, oldNameApn2);
        }
        private void TxtIp1_TextChanged(object sender, EventArgs e)
        {
            string oldNameTxtIp1 = Encoding.UTF8.GetString(config.u8server).Split(':')[0];
            TextBoxIpChanged(txtIp1, btnSaveChanges, oldNameTxtIp1);
        }
        private void TxtPort1_TextChanged(object sender, EventArgs e)
        {
            //string oldNameTxtPort1 = Encoding.UTF8.GetString(config.profile[0].ip_port).Split(':')[1];
            //TextBoxPortChanged(txtPort1, btnSaveChanges, oldNameTxtPort1);
        }
        private void TxtIp2_TextChanged(object sender, EventArgs e)
        {
            //string oldNameTxtIp2 = Encoding.UTF8.GetString(config.profile[1].ip_port).Split(':')[0];
            //TextBoxIpChanged(txtIp2, btnSaveChanges, oldNameTxtIp2);
        }
        private void TxtPort2_TextChanged(object sender, EventArgs e)
        {
            //string oldNameTxtPort2 = Encoding.UTF8.GetString(config.profile[1].ip_port).Split(':')[1];
            //TextBoxPortChanged(txtPort2, btnSaveChanges, oldNameTxtPort2);
        }
        private void TxtIp3_TextChanged(object sender, EventArgs e)
        {
            //string oldNameTxtIp3 = Encoding.UTF8.GetString(config.profile[2].ip_port).Split(':')[0];
            //TextBoxIpChanged(txtIp3, btnSaveChanges, oldNameTxtIp3);
        }
        private void TxtPort3_TextChanged(object sender, EventArgs e)
        {
            //string oldNameTxtPort3 = Encoding.UTF8.GetString(config.profile[2].ip_port).Split(':')[1];
            //TextBoxPortChanged(txtPort3, btnSaveChanges, oldNameTxtPort3);
        }
        private void TxtCounterNa1_TextChanged(object sender, EventArgs e)
        {
            string oldTxtCounterNa1 = config.u32CounterNA[0].ToString();
            TextBoxCounterNaChanged(txtCounterNa1, btnSaveChanges, oldTxtCounterNa1);
        }
        private void TxtCounterNa2_TextChanged(object sender, EventArgs e)
        {
            string oldTxtCounterNa2 = config.u32CounterNA[1].ToString();
            TextBoxCounterNaChanged(txtCounterNa2, btnSaveChanges, oldTxtCounterNa2);
        }
        private void TxtCounterNa3_TextChanged(object sender, EventArgs e)
        {
            string oldTxtCounterNa3 = config.u32CounterNA[2].ToString();
            TextBoxCounterNaChanged(txtCounterNa3, btnSaveChanges, oldTxtCounterNa3);
        }
        private void TxtCounterNa4_TextChanged(object sender, EventArgs e)
        {
            string oldTxtCounterNa4 = config.u32CounterNA[3].ToString();
            TextBoxCounterNaChanged(txtCounterNa4, btnSaveChanges, oldTxtCounterNa4);
        }

        #endregion

        private void TxtTestIp_TextChanged(object sender, EventArgs e)
        {
            Regex regex = new Regex(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$");
            if (regex.IsMatch(txtTestIp.Text))
            {
                txtTestIp.BackColor = Color.White;
            }
            else
            {
                txtTestIp.BackColor = Color.Red;
            }
        }

        private void TxtTesPort_TextChanged(object sender, EventArgs e)
        {
            Regex regex = new Regex(@"^\d{5}$");
            if (regex.IsMatch(txtTestPort.Text))
            {
                txtTestPort.BackColor = Color.White;
            }
            else
            {
                txtTestPort.BackColor = Color.Red;
            }
        }

        private void TxtTestNA_TextChanged(object sender, EventArgs e)
        {
            Regex regex = new Regex(@"^\d+$");
            if (regex.IsMatch(txtTestNA.Text))
            {
                txtTestNA.BackColor = Color.White;
            }
            else
            {
                txtTestNA.BackColor = Color.Red;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 form1 = this.Owner as Form1;
            form1.send();
        }

        private void sendConfig_Click(object sender, EventArgs e)
        {
            Form1 form1 = this.Owner as Form1;
            form1.sendConfigForSaveThread();
        }
    }
}
