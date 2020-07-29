using System;
using System.Collections.Generic;
using System.Linq;
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
            Console("Отдельный запрос на авторизацию отправлен!");
        }
        private void sendConfig()
        {
            //Отправляем запрос на конфиг
            List<byte> listGetConfig = new List<byte>() { 0xF3, 0x60, 0x00, 0x00, 0x00 };
            listGetConfig.AddRange(CRC.Calc(listGetConfig.ToArray(), new Crc16Modbus()).CrcData);
            
            foreach (var cl in gModemClients)
            {
                cl.ns.Write(listGetConfig.ToArray(), 0, listGetConfig.Count);     // отправляем сообщение 
            }
            string tmpStr = string.Format("Отправлено {1} байт-> {0}", string.Join(" ", listGetConfig.ToArray().Take(listGetConfig.Count).Select(r => string.Format("{0:X}", r))), listGetConfig.Count);
            Console($"Отправлен запрос на конфиг!{listGetConfig[0]}-{listGetConfig[1]}-{listGetConfig[2]}-{listGetConfig[3]}-{listGetConfig[4]}-{listGetConfig[5]}-{listGetConfig[6]}");
            Console(tmpStr);
        }
        public void send()
        {
            Thread SendConfig = new Thread(sendCorrectTime);
            SendConfig.IsBackground = true;
            SendConfig.Start();
        }
        public void sendConfigForSaveThread()
        {
            Thread SendConfig = new Thread(sendConfigForSave);
            SendConfig.IsBackground = true;
            SendConfig.Start();
        }
        private void sendConfigForSave()
        {
            //Отправляем запрос на конфиг
            List<byte> listGetConfig = new List<byte>();
            listGetConfig.Add(0xf3);
            listGetConfig.Add(0x61);
            listGetConfig.AddRange(getBytes(gConfig));
            listGetConfig.AddRange(CRC.Calc(listGetConfig.ToArray(), new Crc16Modbus()).CrcData);

            foreach (var cl in gModemClients)
            {
                cl.ns.Write(listGetConfig.ToArray(), 0, listGetConfig.Count);     // отправляем сообщение 
            }
            string tmpStr = string.Format("Отправлено {1} байт-> {0}", string.Join(" ", listGetConfig.ToArray().Take(listGetConfig.Count).Select(r => string.Format("{0:X}", r))), listGetConfig.Count);
            Console(tmpStr);
        }
        public void sendCorrectTime()
        {
            List<byte> newList = new List<byte>();
            List<byte> networkAdress = new List<byte>() { 0xff, 0xff, 0x00, 0x31, 0x36, 0x37, 0x46, 0x46, 0x43, 0x10, 0x36, 0x44 };
            //List<byte> networkAdress = new List<byte>() { 0xf3 };
            newList.AddRange(Register.MakeCorrectTime(networkAdress));
            //newList.AddRange(CRC.Calc(networkAdress.ToArray(), new Crc16Modbus()).CrcData);
            //newList[10] = 0;
            foreach (var cl in gModemClients)
            {
                cl.ns.Write(newList.ToArray(), 0, newList.Count);   // отправляем сообщение 
            }
            string tmpStr = string.Format("Отправлено {1} байт-> {0}", string.Join(" ", newList.ToArray().Take(newList.Count).Select(r => string.Format("{0:X}", r))), newList.Count);
            Console(tmpStr);
        }
        private void StatusString_MouseMove(object sender, MouseEventArgs e)
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
                    string tmpLastCon = SQLite.Instance.GetModemsLastConnectionByImei(i.IMEI);

                    if (tmpImei == "000000000000000") tmpImei = "...............";
                    if (tmpPort == "0") tmpPort = ".....";
                    if (tmpName == "") tmpName = ".....";
                    if (tmpLastCon == String.Empty) tmpName = "..........";
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
    }
}
