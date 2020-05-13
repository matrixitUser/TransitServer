using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TransitServer
{
    public partial class Form1 : Form
    {
        static object locker = new object();
        bool isUsed = false;
        //public bool isWhileS = true;
        int localPort = 7003;
        List<GPRSclient> gModemClients = new List<GPRSclient>();
        List<AskueServer>gAskueServers = new List<AskueServer>();
        TcpListener transitServer;
        TcpListener gTcpAskueServer;
        ImeiDictinary imeiDictinary = new ImeiDictinary();
        BackgroundWorker worker = new BackgroundWorker();
        SongWMP song;
        bool isArcive = false;
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
            song = new SongWMP("song.mp3");
            SQLite.Instance.CreateDB();
            SQLite.Instance.CreateModemsTable();
            ViewModems(SQLite.Instance.GetModems());
            EventsFromDB(SQLite.Instance.GetNotQuite());
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
                //lbConsole.Invoke(new Action(() => lbConsole.Items.Add(statusStr)));

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
            lbConsole.Invoke(new Action(() => lbConsole.Items.Add("Запрос на авторизацию отправлен!")));
            while (tcpClient.Connected && isWhile)  // пока клиент подключен, ждем приходящие сообщения
            {
                byte[] bytes = new byte[1024];     // готовим место для принятия сообщения
                try
                {

                    int count = ns.Read(bytes, 0, bytes.Length);   // читаем сообщение от клиента
                    if (count == 0) { isWhile = false; break; }
                    bytes = bytes.Take(count).ToArray();
                    DateTime dateTime = DateTime.Now;
                    if (count == 2)
                    {
                        if (bytes[0] + bytes[1] == 0xFF)
                        {
                            ns.Write(bytes, 0, 2);
                        }
                    }
                    else if (count == 3)
                    {
                        if ((byte)((bytes[1] >> 4) | (bytes[1] << 4)) == bytes[2])
                        {
                            byte[] arrParams = new byte[] { 0x01, 0x02, 0x07, 0x08, 0x0A, 0x12 };//, 0x13 };
                            for (int i = 0; i < arrParams.Length; i++)
                            {
                                if (((byte)(bytes[1] >> i) & 1) == 1)
                                {
                                    NewEvent(imeiDictinary.GetNameSql(modemClient.IMEI), dateTime, ParameterName(arrParams[i]));
                                }
                            }
                        }
                    }
                    else if (count > 3)
                    {
                        if (CRC.CheckReverse(bytes, new Crc16Modbus()))
                        {
                            Guid nullGuid = new Guid();
                            DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                            //tsConfig conf = setBytes(bytes);
                            if (bytes[0] == 0xFB) //длинный СА
                            {
                                byte[] bytesCId = bytes.Skip(1).Take(12).ToArray();
                                string strCId = BitConverter.ToString(bytesCId).Replace("-", "");
                                byte func = bytes[13];
                                byte[] bytesObjectId = bytes.Skip(14).Take(16).ToArray();
                                Guid objectId = new Guid(bytesObjectId);
                                byte networkAddress = bytes[30];
                                byte[] byteTime = bytes.Skip(31).Take(4).ToArray();
                                UInt32 uInt32Time = (UInt32)(byteTime[3] << 24) | (UInt32)(byteTime[2] << 16) | (UInt32)(byteTime[1] << 8) | byteTime[0];
                                DateTime dtContollers = dt1970.AddSeconds(uInt32Time);
                                UInt16 code = Helper.ToUInt16(bytes, 35);
                                DateTime[] dtEvent = new DateTime[16];
                                for (int i = 0; i < 16; i++)
                                {
                                    //byte[] byteTime1 = msg.Skip(37 + i*4 ).Take(4).ToArray();
                                    //UInt32 uInt32Time1 = (UInt32)(byteTime1[3] << 24) | (UInt32)(byteTime1[2] << 16) | (UInt32)(byteTime1[1] << 8) | byteTime1[0];
                                    //dtEvent[i] = dt1970.AddSeconds(uInt32Time1);
                                    dtEvent[i] = u32ToBytes(bytes.Skip(37 + i * 4).Take(4).ToArray());
                                }
                                byte[] arrParams = new byte[] { 0x01, 0x02, 0x07, 0x08, 0x0A, 0x12 };//, 0x13 };
                                for (int i = 0; i < arrParams.Length; i++)
                                {
                                    if (((byte)(code >> i) & 1) == 1)
                                    {
                                        NewEvent(imeiDictinary.GetNameSql(modemClient.IMEI), dtEvent[i], ParameterName(arrParams[i]));
                                    }
                                }
                            }
                        }
                        else if (CRC.Check(bytes, new Crc16Modbus()))
                        {

                        }

                        answer(bytes, count, tcpClient.Client.Handle);

                    }

                    string hexString = string.Format("{2}:receive {1} байт-> {0}", string.Join(" ", bytes.Take(count).Select(r => string.Format("{0:X}", r))), count, dateTime.ToString());
                    lbConsole.Invoke(new Action(() => lbConsole.Items.Add(hexString)));
                }
                catch (Exception ex)
                {
                    isWhile = false;
                    //MessageBox.Show(ex.Message);
                }
            }  
            string imei = findClientImeibyHandle(tcpClient.Client.Handle);
            tcpClientRemove(tcpClient.Client.Handle);
            Invoke(new Action(() => SQLite.Instance.UpdateActiveConnectionModemsbyImei(imei, 0))); // придаем статус модема = ПОДКЛЮЧЕН
            Invoke(new Action(() => ViewModems(SQLite.Instance.GetModems())));  // обновляем таблицу
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
            lbConsole.Invoke(new Action(() => lbConsole.Items.Add(str)));
        }
        public DateTime u32ToBytes(byte[] bytes)
        {
            if (bytes[0] == 0xFF && bytes[1] == 0xFF && bytes[2] == 0xFF && bytes[3] == 0xFF) return DateTime.MinValue;
            UInt32 u32Date = BitConverter.ToUInt32(bytes, 0);
            int year = 2000 + (int)((u32Date >> 26) & 0x3F);//year 6
            int month = (int)((u32Date >> 22) & 0x0F);//mon  4
                                                      //u32Date = 1364791907;
            int day = (int)((u32Date >> 17) & 0x1F);//day   5
            int hour = (int)((u32Date >> 12) & 0x1F);//hour  5
            int minute = (int)((u32Date >> 6) & 0x3F);//min	 6
            int second = (int)((u32Date >> 0) & 0x3F);//sec	 6
            return new DateTime(year, month, day, hour, minute, second);
        }

        const string COLEVENTNAME = "colEventName";
        const string COLMESSAGE = "colEventMessage";
        const string COLDATE = "colEventDate";
        const string COLQUITE = "colEventQuite";
        public void EventsFromDB(List<dynamic> records)
        {
            dgvEvent.Rows.Clear();
            if (!records.Any())
            {
                song.Stop();
                return;
            }
            foreach(var rec in records)
            {
                dgvEvent.Rows.Add();
                dgvEvent.Rows[dgvEvent.RowCount - 1].Cells[COLEVENTNAME].Value = rec.name;
                dgvEvent.Rows[dgvEvent.RowCount - 1].Cells[COLMESSAGE].Value = rec.message;
                dgvEvent.Rows[dgvEvent.RowCount - 1].Cells[COLDATE].Value = rec.date;
                var dic = (IDictionary<string, object>)rec;
                if (dic.ContainsKey("quite") && rec.quite != null && rec.quite.ToString() != "")
                {
                    DataGridViewTextBoxCell tb = new DataGridViewTextBoxCell();
                    tb.Value = rec.quite;
                    dgvEvent.Rows[dgvEvent.RowCount - 1].Cells[COLQUITE] = tb;
                }
                else
                {
                    song.Play();
                }

            }
        }

        const string MODEMSCOLID = "id";
        const string MODEMSCOLIMEI = "imei";
        const string MODEMSCOLPORT = "port";
        const string MODEMSCOLNAME = "name";
        const string MODEMSCOLLASTCONNECTION = "lastConnection";
        const string MODEMSCOLACTIVECONNECTION = "activeConnection";
        public void ViewModems(List<dynamic> records)
        {
            try
            {
                dgvModems.Rows.Clear();
            }
            catch(Exception e)
            {
                //MessageBox.Show(e.Message);
            }
            
            if (!records.Any())
            {
                return;
            }
            foreach (var rec in records)
            {
                dgvModems.Rows.Add();
                dgvModems.Rows[dgvModems.RowCount - 1].Cells[MODEMSCOLID].Value = rec.id;
                dgvModems.Rows[dgvModems.RowCount - 1].Cells[MODEMSCOLIMEI].Value = rec.imei;
                dgvModems.Rows[dgvModems.RowCount - 1].Cells[MODEMSCOLPORT].Value = rec.port;
                dgvModems.Rows[dgvModems.RowCount - 1].Cells[MODEMSCOLNAME].Value = rec.name;
                dgvModems.Rows[dgvModems.RowCount - 1].Cells[MODEMSCOLLASTCONNECTION].Value = rec.lastConnection;
                dgvModems.Rows[dgvModems.RowCount - 1].Cells[MODEMSCOLACTIVECONNECTION].Value = rec.activeConnection;
                if ((int)rec.activeConnection == 1)
                    dgvModems.Rows[dgvModems.RowCount - 1].Cells[MODEMSCOLACTIVECONNECTION].Style.BackColor = System.Drawing.Color.Green;
                else dgvModems.Rows[dgvModems.RowCount - 1].Cells[MODEMSCOLACTIVECONNECTION].Style.BackColor = System.Drawing.Color.White;
            }
        }
        public void NewEvent(string name, DateTime date, string message)
        {
            List<dynamic> tmp = SQLite.Instance.GetRowForCheck(name, date, message);
            if (tmp.Count > 0)
            {

            }
            else
            {
                tcDown.Invoke(new Action(() => tcDown.SelectedTab = tpEvent));
                SQLite.Instance.InsertRow(name, date, message);
                dgvEvent.Invoke(new Action(() => dgvEvent.Rows.Add()));
                dgvEvent.Rows[dgvEvent.RowCount - 1].Cells[COLEVENTNAME].Value = name;
                dgvEvent.Rows[dgvEvent.RowCount - 1].Cells[COLMESSAGE].Value = message;
                dgvEvent.Rows[dgvEvent.RowCount - 1].Cells[COLDATE].Value = date.ToString();
                song.Play();
            }
            
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
                    Console($"{dateTime.ToString()}: Ответ на запрос авторизации получен! IMEI: {IMEI}");
                    if (index >= 0)
                    {
                        gModemClients[index].IMEI = IMEI;
                        int port = imeiDictinary.GetPortSql(IMEI);
                        imeiDictinary.SetPortSql(IMEI); // записываем модем в БД и задаем порт
                        Invoke(new Action(() => SQLite.Instance.UpdateActiveConnectionModemsbyImei(IMEI, 1))); // придаем статус модема = ПОДКЛЮЧЕН
                        gModemClients[index].PORT = port;
                        Invoke(new Action(() => ViewModems(SQLite.Instance.GetModems())));  // обновляем таблицу

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
                SQLite.Instance.QuiteRow(dgvEvent.Rows[e.RowIndex].Cells[COLEVENTNAME].Value.ToString(), dgvEvent.Rows[e.RowIndex].Cells[COLDATE].Value.ToString(), dgvEvent.Rows[e.RowIndex].Cells[COLMESSAGE].Value.ToString());
                dgvEvent.Rows.RemoveAt(e.RowIndex);
                if (dgvEvent.RowCount == 0) song.Stop();
            }
        }
        
        private void btnSongMute_Click(object sender, EventArgs e)
        {
            song.Mute();
            btnSongMute.Image = (song.IsMute()) ? Properties.Resources.sound_mute1 : Properties.Resources.sound1;
        }

        private void btnGetAllEvents_Click(object sender, EventArgs e)
        {
            if (isArcive)
            {
                EventsFromDB(SQLite.Instance.GetNotQuite());
            }
            else
            {
                EventsFromDB(SQLite.Instance.GetAll());
            }
            isArcive = !isArcive;
        }

        private void btn_AddModem_Click(object sender, EventArgs e)
        {
            FormAddModem formAddModem = new FormAddModem();
            if (formAddModem.ShowDialog() == DialogResult.OK)
            {
                SQLite.Instance.InsertModems(formAddModem.imei, formAddModem.port, formAddModem.name);
            }
            ViewModems(SQLite.Instance.GetModems());
        }

        private void updateDgvModems_Click(object sender, EventArgs e)
        {
            ViewModems(SQLite.Instance.GetModems());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread SendAuth = new Thread(sendAuth);
            SendAuth.IsBackground = true;
            SendAuth.Start();
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            lbConsole.Items.Clear();
        }
        private DataGridViewCellEventArgs mouseLocation;
        private void tsmiRedactorModema_Click(object sender, EventArgs e)
        {
            ActionModems redactorObj = new ActionModems();
            redactorObj.Owner = this;
            redactorObj.oldNameModem = dgvModems.Rows[mouseLocation.RowIndex].Cells[MODEMSCOLNAME].Value.ToString();
            redactorObj.Show();
            redactorObj.txtId.Text = dgvModems.Rows[mouseLocation.RowIndex].Cells[MODEMSCOLID].Value.ToString();
            redactorObj.txtImei.Text = dgvModems.Rows[mouseLocation.RowIndex].Cells[MODEMSCOLIMEI].Value.ToString();
            redactorObj.txtPort.Text = dgvModems.Rows[mouseLocation.RowIndex].Cells[MODEMSCOLPORT].Value.ToString();
            redactorObj.txtName.Text = dgvModems.Rows[mouseLocation.RowIndex].Cells[MODEMSCOLNAME].Value.ToString();
        }

        private void dgvModems_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            mouseLocation = e;
        }

        private void txtFinder_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
