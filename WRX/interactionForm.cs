using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TransitServer
{
    public partial class Form1 : Form
    {
        private void comboBox3_TextChanged(object sender, EventArgs e)
        {
            string tmpFind = cmbIMEI.Text;
            do
            {
                if (cmbIMEI.Items.Count > 0)
                    cmbIMEI.Items.RemoveAt(0);
            } while (cmbIMEI.Items.Count > 0);

            foreach (var item in imeiDictinary.imeiDict)
            {
                if (item.Key.Contains(tmpFind) || item.Value.ToString().Contains(tmpFind))
                {
                    try
                    {
                        string[] tmp = item.Value.Split('-');
                        string tmp1 = tmp[0].PadRight(5) + " - " + tmp[2].Substring(0, 8) + " - " + tmp[1].PadRight(20);
                        cmbIMEI.Items.Add(item.Key + ":" + tmp1);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Thread SendAuth = new Thread(sendAuth);
            SendAuth.IsBackground = true;
            SendAuth.Start();
        }
        private void sendAuth()
        {
            byte[] auth = new byte[28] { 0xC0, 0x00, 0x06, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE7, 0x48, 0xC2 };    //команда авторизации
            foreach (var cl in gModemClients)
            {
                cl.ns.Write(auth, 0, auth.Length);     // отправляем сообщение 
            }
            lbConsole.Invoke(new Action(() => lbConsole.Items.Add("Отдельный запрос на авторизацию отправлен!")));
        }
        private void cmbIMEI_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string[] tmpKey = cmbIMEI.SelectedItem.ToString().Split(':');
            string[] tmpValue = tmpKey[1].Split('-');
            txtIMEI.Text = tmpKey[0];
            txtPort.Text = tmpValue[0].Trim(' ');
            txtName.Text = tmpValue[2].Trim(' ');
            txtLastCon.Text = tmpValue[1];
            btSave.Enabled = false;
            btDelete.Enabled = true;
        }

        private void btQuitWithoutSave_Click(object sender, EventArgs e)
        {
            txtIMEI.Text = "";
            txtPort.Text = "";
            txtName.Text = "";
            txtLastCon.Text = "";
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            int cmpCurPort = 0;
            bool okCompare = true;
            bool okPars = Int32.TryParse(txtPort.Text, out cmpCurPort);
            //int errorNumber = 0;
            if (okPars)
            {
                cmpCurPort = Int32.Parse(txtPort.Text);
                foreach (var i in imeiDictinary.imeiDict.Keys)
                {
                    string tmpImei = i;
                    if (cmpCurPort == imeiDictinary.getPort(i))
                    {
                        MessageBox.Show("Порт с таким номером уже существует, введите другой номер 'Порта'!");
                        okCompare = false;
                        break;
                    }
                }
            }
            imeiDictinary.setName(txtIMEI.Text, txtName.Text);
            if ((cmpCurPort > 20000) && (cmpCurPort < 60000) && (okPars) && (okCompare))
            {

                imeiDictinary.setNewPort(txtIMEI.Text, cmpCurPort);
                imeiDictinary.writeDict();
            }
            else
            {
                if (okCompare) MessageBox.Show("Неверный диапозон или формат данных для поля 'Порт' !");
            }
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            imeiDictinary.removeItem(txtIMEI.Text);
            imeiDictinary.writeDict();
        }

        private void txtName_Leave(object sender, EventArgs e)
        {
            int tmpPort = imeiDictinary.getPort(txtIMEI.Text);
            try { tmpPort = Int32.Parse(txtPort.Text); } catch { }
            if ((imeiDictinary.getPort(txtIMEI.Text) != tmpPort) || (imeiDictinary.getName(txtIMEI.Text) != txtName.Text)) btSave.Enabled = true;
            else btSave.Enabled = false;
            if ((imeiDictinary.getPort(txtIMEI.Text) == tmpPort) && (imeiDictinary.getName(txtIMEI.Text) == txtName.Text)) btSave.Enabled = false;
        }
       
        private void txtPort_Leave(object sender, EventArgs e)
        {
            int tmpPort = imeiDictinary.getPort(txtIMEI.Text);
            try { tmpPort = Int32.Parse(txtPort.Text); } catch { }
            if ((imeiDictinary.getPort(txtIMEI.Text) != tmpPort) || (imeiDictinary.getName(txtIMEI.Text) != txtName.Text)) btSave.Enabled = true;
            else btSave.Enabled = false;
        }

        private void statusString_MouseMove(object sender, MouseEventArgs e)
        {
            var localPosition = this.PointToClient(Cursor.Position);
            int ow = localPosition.X;
            if (localPosition.X > 0 && localPosition.X < 122)
            {
                toolTip1.AutoPopDelay = 5000;
                toolTip1.InitialDelay = 1000;
                toolTip1.ReshowDelay = 500;
                string statusBar = "Подключенные модемы:\n" +
                    "------------------------------\n";
                foreach (var i in gModemClients)
                {
                    string tmpImei = i.IMEI;
                    string tmpPort = imeiDictinary.getPort(i.IMEI).ToString();
                    string tmpName = imeiDictinary.getName(i.IMEI);
                    string tmpLastCon = imeiDictinary.getLastConnection(i.IMEI).ToString();

                    if (tmpImei == "000000000000000") tmpImei = "...............";
                    if (tmpPort == "0") tmpPort = ".....";
                    if (tmpName == "") tmpName = ".....";
                    if (tmpLastCon == DateTime.MinValue.ToString()) tmpName = "..........";
                    string tmp = $"Imei: {tmpImei}\n" +
                                    $"Порт: {tmpPort}\n" +
                                    $"Имя объекта: {tmpName}\n" +
                                    $"Последнее соединение: {tmpLastCon}\n" +
                                    "------------------------------\n";
                    statusBar += tmp;
                }

                toolTip1.SetToolTip(statusString, statusBar);
            }
            if (localPosition.X > 123 && localPosition.X < 250)
            {
                toolTip1.AutoPopDelay = 5000;
                toolTip1.InitialDelay = 1000;
                toolTip1.ReshowDelay = 500;
                string statusBar = "Подключенные АСКУЭ:\n" +
                    "------------------------------\n";
                foreach (var i in gAskueServers)
                {
                    string tmp = $"Данный АСКУЭ сервер подключен к модему:" + 
                                    "------------------------------\n";
                    statusBar += tmp;
                }

                toolTip1.SetToolTip(statusString, statusBar);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lbConsole.Items.Clear();
        }
    }
}
