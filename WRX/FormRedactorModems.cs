using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TransitServer
{
    public partial class FormRedactorModems : Form
    {
        public string idModem;
        public tsConfig config;
        public tsConfig configForSave;
        public FormRedactorModems()
        {
            InitializeComponent();
        }

        const string MODEMSCOLID = "id";
        const string MODEMSCOLIMEI = "imei";
        const string MODEMSCOLPORT = "port";
        const string MODEMSCOLNAME = "name";
        const string MODEMSCOLLASTCONNECTION = "lastConnection";
        const string MODEMSCOLLACTIVECONNECTION = "activeConnection";
        public string oldNameModem;
        public Dictionary<int, string> modemType = new Dictionary<int, string>();
        public Dictionary<int, string> counterType = new Dictionary<int, string>();


        #region ViewModems

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
        #endregion

        #region FormLoad and help funccion
        private void FormRedactorModems_Load(object sender, EventArgs e)
        {
            modemType.Add(1, "CINTERION_BG52");
            counterType.Add(0, "not use");
            counterType.Add(1, "MERCURY230");
            counterType.Add(2, "MERCURY206");
            counterType.Add(3, "ENERGOMERA_CE303");
            toolTipInput.AutoPopDelay = 5000;
            toolTipInput.InitialDelay = 100;
            toolTipInput.ReshowDelay = 500;
            toolTipInput.ShowAlways = true;
            string textIpToolTip = " Правильная форма записи IP-адреса является запись\n в виде четырёх десятичных чисел значением от 0 до 255,\n разделённых точками, например, 192.168.0.3";
            toolTipInput.SetToolTip(txtIp1, textIpToolTip);
            toolTipInput.SetToolTip(txtIp2, textIpToolTip);
            string textPortToolTip = " Правильная форма записи порта является запись\n в виде пяти целых чисел, например 12345";
            toolTipInput.SetToolTip(txtPort1, textPortToolTip);
            toolTipInput.SetToolTip(txtPort2, textPortToolTip);
            string textNetworkAdressToolTip = " Правильная форма записи сетевого адреса счетчика\n является запись в виде любых целых чисел, например 48";
            toolTipInput.SetToolTip(txtCounterNa1, textNetworkAdressToolTip);
            toolTipInput.SetToolTip(txtCounterNa2, textNetworkAdressToolTip);
            toolTipInput.SetToolTip(txtCounterNa3, textNetworkAdressToolTip);
            toolTipInput.SetToolTip(txtCounterNa4, textNetworkAdressToolTip);

            string strConfigTmp = SQLite.Instance.GetConfigFromSql(txtImei.Text);
            tsConfig tmpConfig = new tsConfig();
            if (strConfigTmp != string.Empty)
            {
                string[] strConfig = strConfigTmp.Split('-');
                List<byte> listConfig = new List<byte>();
                for (int i = 0; i < strConfig.Length; i++)
                {
                    try
                    {
                        listConfig.Add(Convert.ToByte(strConfig[i], 16));
                    }
                    catch
                    {
                        MessageBox.Show($"Ошибка в индексе: {i}");
                    }
                }
                byte[] byteConfig = listConfig.ToArray();

                Form1 form1 = new Form1();
                tmpConfig = form1.setBytes(byteConfig);   
            }
            config = tmpConfig;
            configForSave = tmpConfig;
            cbChannel1.DropDownStyle = ComboBoxStyle.DropDownList;
            cbChannel2.DropDownStyle = ComboBoxStyle.DropDownList;
            string[] dropList = new string[] { "listener", "client", "not use" };
            cbChannel2.Items.AddRange(dropList);
            cbChannel1.Items.AddRange(dropList);

            if (tmpConfig.u8server != null)
            {
                if (Encoding.UTF8.GetString(tmpConfig.u8server).Contains(':'))
                {
                    txtIp1.Text = Encoding.UTF8.GetString(tmpConfig.u8server).Split(':')[0];
                    txtPort1.Text = Encoding.UTF8.GetString(tmpConfig.u8server).Split(':')[1];
                    cbChannel1.SelectedItem = "listener";
                }
            }
            if (tmpConfig.u8client != null)
            {
                if (Encoding.UTF8.GetString(tmpConfig.u8client).Contains(':'))
                {
                    txtIp2.Text = Encoding.UTF8.GetString(tmpConfig.u8client).Split(':')[0];
                    txtPort2.Text = Encoding.UTF8.GetString(tmpConfig.u8client).Split(':')[1];
                    cbChannel2.SelectedItem = "client";
                }
            }

            txtTypeModem.Text = SwitchModemType((Int32)tmpConfig.u8ModemType).ToString();
            
            List<string> dropListForCb = new List<string>();
            foreach(var dict in counterType.Values)
            {
                dropListForCb.Add(dict);
            }
            object[] dropListcbType = dropListForCb.ToArray();
            cbType1.Items.AddRange(dropListcbType);
            cbType2.Items.AddRange(dropListcbType);
            cbType3.Items.AddRange(dropListcbType);
            cbType4.Items.AddRange(dropListcbType);
            cbType1.DropDownStyle = ComboBoxStyle.DropDownList;
            cbType2.DropDownStyle = ComboBoxStyle.DropDownList;
            cbType3.DropDownStyle = ComboBoxStyle.DropDownList;
            cbType4.DropDownStyle = ComboBoxStyle.DropDownList;
            if(tmpConfig.u8CounterType != null)
            {
                cbType1.SelectedItem = SwitchCounterType((Int32)tmpConfig.u8CounterType[0]);
                cbType2.SelectedItem = SwitchCounterType((Int32)tmpConfig.u8CounterType[1]);
                cbType3.SelectedItem = SwitchCounterType((Int32)tmpConfig.u8CounterType[2]);
                cbType4.SelectedItem = SwitchCounterType((Int32)tmpConfig.u8CounterType[3]);
            }

            if (tmpConfig.u32CounterNA != null)
            {
                if (tmpConfig.u32CounterNA[0] == 4294967295) txtCounterNa1.Text = "not use";
                else txtCounterNa1.Text = tmpConfig.u32CounterNA[0].ToString();
                if (tmpConfig.u32CounterNA[1] == 4294967295) txtCounterNa2.Text = "not use";
                else txtCounterNa2.Text = tmpConfig.u32CounterNA[1].ToString();
                if (tmpConfig.u32CounterNA[2] == 4294967295) txtCounterNa3.Text = "not use";
                else txtCounterNa3.Text = tmpConfig.u32CounterNA[2].ToString();
                if (tmpConfig.u32CounterNA[3] == 4294967295) txtCounterNa4.Text = "not use";
                else txtCounterNa4.Text = tmpConfig.u32CounterNA[3].ToString();
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
            cbBaudRate1.Items.AddRange(new object[] { 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 57600 });
            cbBaudRate2.Items.AddRange(new object[] { 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 57600 });
            cbBaudRate3.Items.AddRange(new object[] { 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 57600 });
            if (tmpConfig.sUart != null)
            {
                cbBaudRate1.SelectedItem = (Int32)tmpConfig.sUart[0].u32BaudRate;
                cbBaudRate2.SelectedItem = (Int32)tmpConfig.sUart[1].u32BaudRate;
                cbBaudRate3.SelectedItem = (Int32)tmpConfig.sUart[2].u32BaudRate;
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
            else
            {
                cbBaudRate1.SelectedItem = 4800;
                cbBaudRate2.SelectedItem = 4800;
                cbBaudRate3.SelectedItem = 4800;
            }
            btnSaveChanges.Enabled = false;
        }
        private string SwitchCounterType(int counetrType)
        {
            string value = string.Empty;
            switch (counetrType)
            {
                case 0:
                    counterType.TryGetValue(0, out value);
                    return value;
                case 1:
                    counterType.TryGetValue(1, out value);
                    return value;
                case 2:
                    counterType.TryGetValue(2, out value);
                    return value;
                case 3:
                    counterType.TryGetValue(3, out value);
                    return value;
                default:
                    return value;
            }
        }
        private string SwitchModemType(int counetrType)
        {
            string value = string.Empty;
            switch (counetrType)
            {
                case 1:
                    modemType.TryGetValue(1, out value);
                    return value;
                default:
                    return value;
            }
        }
        #endregion

        #region ComboBoxIndexChanged
        private void CbChannel1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxIndexChanged(cbChannel1, txtIp1, txtPort1);
        }
        private void CbChannel2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxIndexChanged(cbChannel2, txtIp2, txtPort2);
        }
        private void ComboBoxIndexChanged(ComboBox comboBox, TextBox txtIP, TextBox txtPort)
        {
            if (comboBox.SelectedItem.ToString() == "listener")
            {
                txtIP.Enabled = true;
                txtIP.Text = Encoding.UTF8.GetString(config.u8server).Split(':')[0];
                txtPort.Enabled = true;
                txtPort.Text = Encoding.UTF8.GetString(config.u8server).Split(':')[1];
                txtIP.BackColor = Color.White;
                txtPort.BackColor = Color.White;
            }
            if (comboBox.SelectedItem.ToString() == "client")
            {
                txtIP.Enabled = true;
                txtIP.Text = Encoding.UTF8.GetString(config.u8client).Split(':')[0];
                txtPort.Enabled = true;
                txtPort.Text = Encoding.UTF8.GetString(config.u8client).Split(':')[1];
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
            Regex regex = new Regex(@"^\d+$");
            if (regex.IsMatch(textBox.Text))
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
            if (regex.IsMatch(textBox.Text) || textBox.Text == "not use")
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
            if (regex.IsMatch(textBox.Text) || textBox.Text == "listener")
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
            string oldNameTxtPort1 = Encoding.UTF8.GetString(config.u8server).Split(':')[1];
            TextBoxPortChanged(txtPort1, btnSaveChanges, oldNameTxtPort1);
        }
        private void TxtIp2_TextChanged(object sender, EventArgs e)
        {
            string oldNameTxtIp2 = Encoding.UTF8.GetString(config.u8client).Split(':')[0];
            TextBoxIpChanged(txtIp2, btnSaveChanges, oldNameTxtIp2);
        }
        private void TxtPort2_TextChanged(object sender, EventArgs e)
        {
            string oldNameTxtPort2 = Encoding.UTF8.GetString(config.u8client).Split(':')[1];
            TextBoxPortChanged(txtPort2, btnSaveChanges, oldNameTxtPort2);
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

        #region ButtonsWork
        private void BtnExit_Click(object sender, EventArgs e)
        {
            ViewModems(SQLite.Instance.GetModems());
            Close();
        }

        private void BtnDeleteModem_Click(object sender, EventArgs e)
        {
            DialogFormOk dialogFormOk = new DialogFormOk();
            if (dialogFormOk.ShowDialog() == DialogResult.OK)
            {
                SQLite.Instance.DeleteModems(idModem);
                ViewModems(SQLite.Instance.GetModems());
                Close();
                MessageBox.Show("Объект удален!");
            }
        }

        private void BtnToCorrectTime_Click(object sender, EventArgs e)
        {
            Form1 form1 = this.Owner as Form1;
            form1.SendToCorrectTime_Thread(txtImei.Text);
        }

        private void BtnGetCurrent_Click(object sender, EventArgs e)
        {
            Form1 form1 = this.Owner as Form1;
            form1.SendConfigForCurrent(txtImei.Text);
        }
        #endregion

        private void BtnSaveChanges_Click(object sender, EventArgs e)
        {
            tsConfig tsConfig = config;

            configForSave.apnName[0].APN = AdditionStr(txtApn1.Text);
            configForSave.apnName[1].APN = AdditionStr(txtApn2.Text);

            string server = txtIp1.Text + ":" + txtPort1.Text;
            configForSave.u8server = AdditionStr(server);

            string client = txtIp2.Text + ":" + txtPort2.Text;
            configForSave.u8client = AdditionStr(client);

            configForSave.sUart[0].u32BaudRate = UInt32.Parse(cbBaudRate1.Text);
            configForSave.sUart[1].u32BaudRate = UInt32.Parse(cbBaudRate2.Text);
            configForSave.sUart[2].u32BaudRate = UInt32.Parse(cbBaudRate3.Text);

            configForSave.PeriodEvent = ushort.Parse(txtPeriodEvent.Text);

            configForSave.u8ModemType = (byte)modemType.First(x => x.Value == txtTypeModem.Text).Key;
            configForSave.u8CounterType[0] = (byte)counterType.First(x => x.Value == cbType1.Text).Key;
            configForSave.u8CounterType[1] = (byte)counterType.First(x => x.Value == cbType2.Text).Key;
            configForSave.u8CounterType[2] = (byte)counterType.First(x => x.Value == cbType3.Text).Key;
            configForSave.u8CounterType[3] = (byte)counterType.First(x => x.Value == cbType4.Text).Key;

            configForSave.u32CounterNA[0] = (txtCounterNa1.Text == "not use") ? UInt32.Parse("4294967295") : UInt32.Parse(txtCounterNa1.Text);
            configForSave.u32CounterNA[1] = (txtCounterNa2.Text == "not use") ? UInt32.Parse("4294967295") : UInt32.Parse(txtCounterNa2.Text);
            configForSave.u32CounterNA[2] = (txtCounterNa3.Text == "not use") ? UInt32.Parse("4294967295") : UInt32.Parse(txtCounterNa3.Text);
            configForSave.u32CounterNA[3] = (txtCounterNa4.Text == "not use") ? UInt32.Parse("4294967295") : UInt32.Parse(txtCounterNa4.Text);

            string strChipId = SQLite.Instance.GetModemsChipId(txtImei.Text);
            byte[] byteChipId = new byte[] { };
            if (strChipId != string.Empty)
            {
                string[] arrChipId = strChipId.Split('-');
                
                List<byte> listChipId = new List<byte>();
                for (int i = 0; i < arrChipId.Length; i++)
                {
                    try
                    {
                        listChipId.Add(Convert.ToByte(arrChipId[i], 16));
                    }
                    catch
                    {
                        MessageBox.Show($"Ошибка в индексе: {i}");
                    }
                }
                byteChipId = listChipId.ToArray();
            }
            Form1 form1 = this.Owner as Form1;
            byte[] bytes = form1.getBytes(configForSave);
            string configForSaveSQl = string.Format(string.Join("-", bytes.Take(bytes.Length).Select(r => string.Format("{0:X}", r))), bytes.Length);
            SQLite.Instance.UpdateConfigForSaveModems(txtImei.Text, configForSaveSQl);
            List<byte> listBytesForSend = new List<byte>();
            if (strChipId != string.Empty)
            {
                listBytesForSend.Add(0xfb);
                listBytesForSend.AddRange(byteChipId);
            }
            else
            {
                listBytesForSend.Add(0xf3);
            }
            listBytesForSend.Add(0x61);
            listBytesForSend.AddRange(bytes);
            listBytesForSend.AddRange(CRC.Calc(listBytesForSend.ToArray(), new Crc16Modbus()).CrcData);
            foreach (var cl in form1.gModemClients)
            {
                if(cl.IMEI == txtImei.Text)
                {
                    cl.ns.Write(listBytesForSend.ToArray(), 0, listBytesForSend.Count);     // отправляем конфиг на сохранение

                    form1.Console("Измененный конфиг отправлен на потверждение!");
                }
            }
            SQLite.Instance.UpdateNameModemsbyImei(txtImei.Text, txtNameModem.Text);
            MessageBox.Show("Изменения сохранены!");
        }

        private byte[] AdditionStr(string inputStr)
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(Encoding.ASCII.GetBytes(inputStr));
            while (bytes.Count < 32)
            {
                bytes.Add(0);
            }
            return bytes.ToArray();
        }

        private void BtnRequestConfig_Click(object sender, EventArgs e)
        {
            Form1 form1 = this.Owner as Form1;
            //form1.sendConfigForSaveThread();
        }
    }
}
