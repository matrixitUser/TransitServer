using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;      
using System.Net.Sockets;    
using System.Threading;
using System.IO;
using System.Dynamic;
using System.Diagnostics;
using System.Data.SQLite;

namespace TransitServer
{
    public partial class Form1 : Form
    {
        static object locker = new object();
        bool isUsed = false; int count = 0;
        //public bool isWhileS = true;
        int localPort = 10114;
        List<GPRSclient> gModemClients = new List<GPRSclient>();
        List<AskueServer>gAskueServers = new List<AskueServer>();
        TcpListener transitServer;
        TcpListener gTcpAskueServer;
        ImeiDictinary imeiDictinary = new ImeiDictinary();
        BackgroundWorker worker = new BackgroundWorker();
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            transitServer = new TcpListener(IPAddress.Any, localPort);
            transitServer.Start();  // запускаем сервер
            Thread Sockets = new Thread(SocketsCreate);
            Sockets.IsBackground = true;
            //isWhileS = false;
            Sockets.Start();

            m_dbConn = new SQLiteConnection();
            m_sqlCmd = new SQLiteCommand();

            dbFileName = "transitServer.sqlite";
            CreateDB();
            EventsFromDB(GetNotQuite());
        }
        private void SocketsCreate()
        {
            statusString.Items.Add("Клиенты модемов: 0");
            statusString.Items.Add("Клиентов АСКУЭ: 0");
            //statusString.Items.Add("  Handle:");
            while (true)
            {
                if (!isUsed)
                {
                    isUsed = true;
                    //countTmp++;
                    Thread newSocket = new Thread(ModemSocketListening);
                    newSocket.IsBackground = true;
                    newSocket.Start();
                }
                Thread.Sleep(20);
                string statusStr = string.Format("Клиентов модемов:{0}", gModemClients.Count);
                //lvConsole.Invoke(new Action(() => lvConsole.Items.Add(statusStr)));

                statusString.Invoke(new Action(() => statusString.Items[0].Text = statusStr));
                Thread.Sleep(20);
            }
        }
        private void ModemSocketListening()
        {
            bool isWhile = true;
            TcpClient tcpClient = transitServer.AcceptTcpClient();  // ожидаем подключение клиента
            IntPtr clientHandle = tcpClient.Client.Handle;
            GPRSclient modemClient = new GPRSclient();
            modemClient.handle = tcpClient.Client.Handle;
            gModemClients.Add(modemClient);
            NetworkStream ns = tcpClient.GetStream(); // для получения и отправки сообщений
            gModemClients[gModemClients.Count - 1].nsSet(ns);
            isUsed = false;
            byte[] auth = new byte[28] { 0xC0, 0x00, 0x06, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE7, 0x48, 0xC2 };    //команда авторизации
            ns.Write(auth, 0, auth.Length);     // отправляем сообщение 
            lvConsole.Invoke(new Action(() => lvConsole.Items.Add("Запрос на авторизацию отправлен!")));
            while (tcpClient.Connected && isWhile)  // пока клиент подключен, ждем приходящие сообщения
            {
                byte[] msg = new byte[1024];     // готовим место для принятия сообщения
                try
                {

                    int count = ns.Read(msg, 0, msg.Length);   // читаем сообщение от клиента
                    if (count == 0) { isWhile = false; break; }

                    DateTime dateTime = DateTime.Now;
                    if (count == 2)
                    {
                        if (msg[0] + msg[1] == 0xFF)
                        {
                            ns.Write(msg, 0, 2);
                        }
                    }
                    else if (count == 3)
                    {
                        if ((byte)((msg[1] >> 4) | (msg[1] << 4)) == msg[2])
                        {
                            byte[] arrParams = new byte[] { 0x01, 0x02, 0x07, 0x08, 0x0A, 0x12 };//, 0x13 };
                            for (int i = 0; i < arrParams.Length; i++)
                            {
                                if (((byte)(msg[1] >> i) & 1) == 1)
                                {
                                    Event(imeiDictinary.getName(modemClient.IMEI), dateTime, ParameterName(arrParams[i]));
                                }
                            }

                        }
                    }
                    else if (count > 3)
                    {
                        answer(msg, count, tcpClient.Client.Handle);
                    }

                    string hexString = string.Format("{2}:receive {1} байт-> {0}", string.Join(" ", msg.Take(count).Select(r => string.Format("{0:X}", r))), count, dateTime.ToString());
                    lvConsole.Invoke(new Action(() => lvConsole.Items.Add(hexString)));
                }
                catch (Exception ex)
                {
                    isWhile = false;
                }
            }  
            string imei = findClientImeibyHandle(tcpClient.Client.Handle);
            tcpClientRemove(tcpClient.Client.Handle);
            tcpClient.Close();

            int indexServer = findServerIndexbyIMEI(imei);
            if (indexServer >= 0)
            {
                gAskueServers[indexServer].tcpListener.Stop();
                gAskueServers.RemoveAt(indexServer);
            }
            string statusStr = string.Format("Клиенты АСКУЭ:{0}", gAskueServers.Count);
            statusString.Invoke(new Action(() => statusString.Items[1].Text = statusStr));
        }

        public void Console(string str)
        {
            lvConsole.Invoke(new Action(() => lvConsole.Items.Add(str)));
        }
        public void Event(string name, DateTime dateTime, string param)
        {
            string strTmp = dateTime.ToString() + "| " + name + ": " + param;
            lbEvent.Invoke(new Action(() => lbEvent.Items.Add(strTmp)));
            NewEvent(name, dateTime, param);
        }
        const string COLNAME = "colEvent1Name";
        const string COLMESSAGE = "colEvent1Message";
        const string COLDATE = "colEvent1Date";
        public void EventsFromDB(List<dynamic> records)
        {
            if (!records.Any()) return;
            foreach(var rec in records)
            {
                dgvEvent.Rows.Add();
                dgvEvent.Rows[dgvEvent.RowCount - 1].Cells[COLNAME].Value = rec.name;
                dgvEvent.Rows[dgvEvent.RowCount - 1].Cells[COLMESSAGE].Value = rec.message;
                dgvEvent.Rows[dgvEvent.RowCount - 1].Cells[COLDATE].Value = rec.date;
            }
        }
        public void NewEvent(string name, DateTime date, string message)
        {
            InsertRow(name, date, message);
            dgvEvent.Invoke(new Action(() => dgvEvent.Rows.Add()));
            dgvEvent.Rows[dgvEvent.RowCount - 1].Cells[COLNAME].Value = name;
            dgvEvent.Rows[dgvEvent.RowCount - 1].Cells[COLMESSAGE].Value = message;
            dgvEvent.Rows[dgvEvent.RowCount - 1].Cells[COLDATE].Value = date.ToString();
        }
        public string ParameterName(byte param)
        {
            switch (param)
            {
                case 0x01:
                    return "включение/выключение прибора";
                case 0x02:
                    return "коррекция часов прибора";
                case 0x07:
                    return "коррекция тарифного расписания";
                case 0x08:
                    return "коррекция расписания праздничных дней";
                case 0x0A:
                    return "инициализация массива средних мощностей";
                case 0x12:
                    return "вскрытие/закрытие прибора";
                case 0x13:
                    return "перепрограммирование прибора";
                default:
                    return $"Неизвестный параметер: {param}";
            }
        }
        private int findServerIndexbyIMEI(string imei)
        {
            int i = 0;
            foreach(var server in gAskueServers)
            {
                if (server.IMEI == imei) return i;
                i++;
            }
            return -1;
        }

        private string findClientImeibyHandle(IntPtr clientHandle)
        {
            int i = 0;
            foreach (var cl in gModemClients)
            {
                if (cl.handle == clientHandle)
                {
                    break;
                }
                i++;
            }
            string imeiTmp = gModemClients[i].IMEI;
            return imeiTmp;
        }

        private void tcpClientRemove(IntPtr clientHandle) 
        {
            int i = 0;
            foreach (var cl in gModemClients)
            {
                if (cl.handle == clientHandle)
                {
                    break;
                }
                i++;
            }
            try
            {
                if ((i <= gModemClients.Count) && (gModemClients[i].handle == clientHandle))
                    gModemClients.RemoveAt(i);
            }
            catch { }
        }

        private void answer(byte[] msg, int len, IntPtr clientHandle)
        {
            byte[] bytes = new byte[512];
            byte[] crc = new byte[2];
            if (msg[0] == 0xC0)
            {
                int i;
                for (i = 0; (i < len); i++)
                {
                    if (msg[i] == 0xC2)
                    {
                        crc = CRC.CrcCalculate(msg, 1, i - 3);
                        if ((crc[0] == msg[i - 1]) && (crc[1] == msg[i - 2]))
                            answerTeleofic(msg, i, clientHandle);
                        return;
                    }
                }
            }
            //Отправка на сервер АСКУЭ
            if (gAskueServers.Count > 0)
            {
                gAskueServers[0].ns.Write(msg, 0, len);
            }



        }

        private void answerTeleofic(byte[] bytes, int len, IntPtr clientHandle)
        {
            byte protocol = bytes[1];
            byte doit = bytes[2];
            ushort cmd = bytes[3];
            if (protocol == 1)
            {
                cmd = (ushort)(cmd << 8 | bytes[4]);

            }
            byte datalen = bytes[4 + protocol];
            switch (cmd)
            {
                case 0:
                    int index = clientIndex(clientHandle);
                    string IMEI = "";
                    StringBuilder builder = new StringBuilder();
                    builder.Append(Encoding.UTF8.GetString(bytes, 6, 15));
                    IMEI = builder.ToString();
                    DateTime dateTime = DateTime.Now;
                    lvConsole.Invoke(new Action(() => lvConsole.Items.Add($"{dateTime.ToString()}: Ответ на запрос авторизации получен! IMEI: {IMEI}"))); 

                    if (index >= 0)
                    {
                        gModemClients[index].IMEI = IMEI;
                        int port = imeiDictinary.getPort(IMEI);
                        if (port == 0) port = imeiDictinary.setPort(IMEI);
                        gModemClients[index].PORT = port;

                        int indexSever = findServerIndexbyIMEI(IMEI);
                        if (indexSever < 0) 
                        {
                            Thread SocketsAskue = new Thread(createLServerToAskue);
                            SocketsAskue.IsBackground = true;
                            dynamic obj = new ExpandoObject();
                            obj.port = port;
                            obj.imei = IMEI;
                            SocketsAskue.Start(obj);
                        }
                    }
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                default:
                    throw new Exception($"неизвестная команда {cmd}");
            }
        }

        private int clientIndex(IntPtr clientHandle)
        {
            int index = 0;
            foreach (var cl in gModemClients)
            {
                if (cl.handle == clientHandle)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        private void dgvEvent1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == 3)
            {
                QuiteRow(dgvEvent.Rows[e.RowIndex].Cells[COLNAME].Value.ToString(), dgvEvent.Rows[e.RowIndex].Cells[COLDATE].Value.ToString(), dgvEvent.Rows[e.RowIndex].Cells[COLMESSAGE].Value.ToString());
                dgvEvent.Rows.RemoveAt(e.RowIndex);
            }
        }
    }
}
