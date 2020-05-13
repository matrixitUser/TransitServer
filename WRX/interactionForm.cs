using System;
using System.Threading;
using System.Windows.Forms;

namespace TransitServer
{
    public partial class Form1 : Form
    {
  
        private void sendAuth()
        {
            byte[] auth = new byte[28] { 0xC0, 0x00, 0x06, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE7, 0x48, 0xC2 };    //команда авторизации
            foreach (var cl in gModemClients)
            {
                cl.ns.Write(auth, 0, auth.Length);     // отправляем сообщение 
            }
            lbConsole.Invoke(new Action(() => lbConsole.Items.Add("Отдельный запрос на авторизацию отправлен!")));
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
                    string tmpPort = imeiDictinary.GetPortSql(i.IMEI).ToString();
                    string tmpName = imeiDictinary.GetNameSql(i.IMEI);
                    //string tmpLastCon = imeiDictinary.getLastConnection(i.IMEI).ToString();

                    if (tmpImei == "000000000000000") tmpImei = "...............";
                    if (tmpPort == "0") tmpPort = ".....";
                    if (tmpName == "") tmpName = ".....";
                    //if (tmpLastCon == DateTime.MinValue.ToString()) tmpName = "..........";
                    string tmp = $"Imei: {tmpImei}\n" +
                                    $"Порт: {tmpPort}\n" +
                                    $"Имя объекта: {tmpName}\n" +
                                    $"Последнее соединение: ----\n" +
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
    }
}
