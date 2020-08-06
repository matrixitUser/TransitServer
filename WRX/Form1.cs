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
using System.Configuration;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Net.Mail;

namespace TransitServer
{
    public partial class Form1 : Form
    {
        static object locker = new object();
        bool isUsed = false;
        public bool isCustomGroup = false;
        //int localPort = 10114;
        readonly int localPort = Int32.Parse(ConfigurationManager.AppSettings.Get("port"));
        List<GPRSclient> gModemClients = new List<GPRSclient>();
        List<AskueServer>gAskueServers = new List<AskueServer>();
        TcpListener transitServer;
        TcpListener gTcpAskueServer;
        ImeiDictinary imeiDictinary = new ImeiDictinary();
        BackgroundWorker worker = new BackgroundWorker();
        SongWMP song;
        tsConfig gConfig;
        bool isArcive = false;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            if (!CheckFreePort(localPort))
            {
                MessageBox.Show($"Внутренний порт {localPort} занят другим приложением!\nПоменяйте порт или завершите работу другой программы.");
            }
            try
            {
                transitServer = new TcpListener(IPAddress.Any, localPort);
                transitServer.Start();  // запускаем сервер
                Thread Sockets = new Thread(SocketsCreate);
                Sockets.IsBackground = true;
                Sockets.Start();
                song = new SongWMP("song.mp3");
                SQLite.Instance.CreateDB();
                SQLite.Instance.ZeroingActiveConnection();
                ViewModems(SQLite.Instance.GetModems());
                EventsFromDB(SQLite.Instance.GetNotQuite());
                InitTree(trView1);
            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message);
                Application.Exit();
            }
            
        }

        #region SocketCreated
        public bool CheckFreePort(int localPort)
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port == localPort)
                {
                    return false;
                }
            }
            return true;
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
            Console("Запрос на авторизацию отправлен!");
            while (tcpClient.Connected && isWhile)  // пока клиент подключен, ждем приходящие сообщения
            {
                byte[] bytes = new byte[1024];     // готовим место для принятия сообщения
                try
                {

                    int count = ns.Read(bytes, 0, bytes.Length);   // читаем сообщение от клиента
                    if (count == 0) { isWhile = false; break; }
                    bytes = bytes.Take(count).ToArray();
                    DateTime dateTime = DateTime.Now;
                    string hexString = string.Format("получено {1} байт-> {0}", string.Join(" ", bytes.Take(count).Select(r => string.Format("{0:X}", r))), count);
                    Console(hexString);
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
                                    NewEvent(imeiDictinary.GetNameSql(modemClient.IMEI), dateTime, ParameterName(arrParams[i]), modemClient.IMEI);
                                }
                            }
                        }
                    }
                    else if (count > 3)
                    {
                        if (CRC.CheckReverse(bytes, new Crc16Modbus()))
                        {
                            try
                            {
                                if (bytes[0] == 0xFB) //длинный СА
                                {
                                    byte[] bytesCId = bytes.Skip(1).Take(12).ToArray();
                                    string strCId = BitConverter.ToString(bytesCId).Replace("-", "");
                                    ParseAnswerFromModem(bytes.Skip(13).ToArray(), modemClient);
                                }
                                else if (bytes[0] == 0xF3) //считываем конфиг
                                {
                                    ParseAnswerFromModem(bytes.Skip(1).ToArray(), modemClient);
                                }
                            }
                            catch(Exception exc)
                            {
                                Console($"Ошибка входных данных! {exc.Message}");
                            }
                        }
                        else 
                        {
                            Console("Контрольная сумма не сходится!");
                        }
                        
                        Answer(bytes, count, tcpClient.Client.Handle, ns);
                        
                    }

                    //string hexString = string.Format("получено {1} байт-> {0}", string.Join(" ", bytes.Take(count).Select(r => string.Format("{0:X}", r))), count);
                    //Console(hexString);
                }
                catch (Exception ex)
                {
                    //isWhile = false;
                    //MessageBox.Show(ex.Message + " 1");
                    Console(ex.Message);
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

        private void ParseAnswerFromModem(byte[] data, GPRSclient modemClient)
        {

            DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            switch (data[0])
            {
                case 0x4D:
                    {
                        byte[] config = data.Skip(1).ToArray();
                        tsCurrent cur = setBytesFromConfig<tsCurrent>(config, new tsCurrent());
                        string imei = Encoding.ASCII.GetString(cur.imei);
                        Guid objectId = new Guid(cur.objectId);
                        DateTime dtContollers = dt1970.AddSeconds(cur.counterTime);
                        DateTime[] dtEvent = new DateTime[16];
                        for (int i = 0; i < 16; i++)
                        {
                            if (cur.timeEvent[i] == 0xFFFFFFFF) dtEvent[i] = dt1970;
                            else dtEvent[i] = dt1970.AddSeconds(cur.timeEvent[i]);
                        }
                        byte[] arrParams = new byte[] { 0x01, 0x02, 0x07, 0x08, 0x0A, 0x12, 0x13, 0x00 };
                        List<dynamic> records = new List<dynamic>();
                        for (int i = 0; i < arrParams.Length; i++)
                        {
                            if (((byte)(cur.event_ >> i) & 1) == 1 && dtEvent[i] != DateTime.MinValue)
                            {
                                NewEvent(imeiDictinary.GetNameSql(imei), dtEvent[i], ParameterName(arrParams[i]), imei);
                            }
                        }
                        break;
                    }
                case 0x5F:
                    {
                        byte[] config = data.Skip(1).ToArray();
                        tsCurrent conf = setBytesFromConfig<tsCurrent>(config, new tsCurrent());
                        string strTmp = string.Format(string.Join(" ", config.Take(config.Length).Select(r => string.Format("{0:X}", r))), config.Length);
                        break;
                    }
                case 0x60://96
                    {
                        byte[] config = data.Skip(1).ToArray();
                        tsConfig conf = setBytes(config);
                        gConfig = conf;
                        //gConfig.u8ModemType = 5;
                        string configForSQl = string.Format(string.Join("-", config.Take(config.Length).Select(r => string.Format("{0:X}", r))), config.Length);
                        SQLite.Instance.UpdateConfigModems(modemClient.IMEI, configForSQl);
                        break;
                    }
                case 0x61: // 97
                    //byte[] config1 = data.Skip(1).ToArray();
                    //string configForSQl1 = string.Format(string.Join(" ", config1.Take(config1.Length).Select(r => string.Format("{0:X}", r))), config1.Length);
                    //byte[] configTmp = getBytes(config);
                    //string configForSQl = string.Format(string.Join(" ", configTmp.Take(configTmp.Length).Select(r => string.Format("{0:X}", r))), configTmp.Length);
                    break;
            }
        }
        private void Answer(byte[] msg, int len, IntPtr clientHandle, NetworkStream ns)
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
                            AnswerTeleofic(msg, i, clientHandle, ns);
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

        private void AnswerTeleofic(byte[] bytes, int len, IntPtr clientHandle, NetworkStream ns)
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
                    int index = ClientIndex(clientHandle);
                    string IMEI = "";
                    StringBuilder builder = new StringBuilder();
                    builder.Append(Encoding.UTF8.GetString(bytes, 6, 15));
                    IMEI = builder.ToString();
                    DateTime dateTime = DateTime.Now;
                    Console($"Ответ на запрос авторизации получен! IMEI: {IMEI}");

                    //Отправляем запрос на конфиг
                    List<byte> listGetConfig = new List<byte>() { 0xF3, 0x60, 0x00, 0x00, 0x00 };
                    listGetConfig.AddRange(CRC.Calc(listGetConfig.ToArray(), new Crc16Modbus()).CrcData);
                    ns.Write(listGetConfig.ToArray(), 0, listGetConfig.Count);
                    string tmpStr = string.Format("Отправлено {1} байт-> {0}", string.Join(" ", listGetConfig.ToArray().Take(listGetConfig.Count).Select(r => string.Format("{0:X}", r))), listGetConfig.Count);
                    Console(tmpStr);
                    if (index >= 0)
                    {
                        gModemClients[index].IMEI = IMEI;
                        int port = imeiDictinary.GetPortSql(IMEI);
                        imeiDictinary.SetPortSql(IMEI); // записываем модем в БД и задаем порт
                        InsertMailSender(IMEI); // добавляем в БД в таблицу dbMail данные об отправителе, значения по умолчанию берем с конфига
                        Invoke(new Action(() => SQLite.Instance.UpdateActiveConnectionModemsbyImei(IMEI, 1))); // придаем статус модема = ПОДКЛЮЧЕН
                        SQLite.Instance.UpdateLastConnectionModemsbyImei(IMEI, DateTime.Now.ToString());
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
        private int findServerIndexbyIMEI(string imei)
        {
            int i = 0;
            foreach (var server in gAskueServers)
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
        private int ClientIndex(IntPtr clientHandle)
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

        #endregion //  //

        #region Events
        public void NewEvent(string name, DateTime date, string message, string imei)
        {
            List<dynamic> tmp = SQLite.Instance.GetRowForCheck(name, date, message);
            if (tmp.Count > 0)
            {

            }
            else
            {
                SendEmailAsync(name, date, message, imei).GetAwaiter();
                Console("Письмо отпралено.");
                tcDown.Invoke(new Action(() => tcDown.SelectedTab = tpEvent));
                SQLite.Instance.InsertRow(name, date, message, imei);
                dgvEvent.Invoke(new Action(() => dgvEvent.Rows.Add()));
                dgvEvent.Rows[dgvEvent.RowCount - 1].Cells[COLEVENTNAME].Value = name;
                dgvEvent.Rows[dgvEvent.RowCount - 1].Cells[COLMESSAGE].Value = message;
                dgvEvent.Rows[dgvEvent.RowCount - 1].Cells[COLDATE].Value = date;
                dgvEvent.Rows[dgvEvent.RowCount - 1].Cells[COLEVENTIMEIMODEM].Value = imei;
                song.Play();
            }
        }
        public string ParameterName(byte param)
        {
            switch (param)
            {
                case 0x00:
                    return "вскрытие шкафа";
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

        const string COLEVENTNAME = "colEventNameModem";
        const string COLMESSAGE = "colEventMessage";
        const string COLDATE = "colEventDate";
        const string COLQUITE = "colEventQuite";
        const string COLEVENTIMEIMODEM = "colEventImeiModem";
        public void EventsFromDB(List<dynamic> records)
        {
            dgvEvent.Rows.Clear();
            if (!records.Any())
            {
                song.Stop();
                return;
            }
            foreach (var rec in records)
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
        private void DgvEvent1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                SQLite.Instance.QuiteRow(dgvEvent.Rows[e.RowIndex].Cells[COLEVENTNAME].Value.ToString(), dgvEvent.Rows[e.RowIndex].Cells[COLDATE].Value.ToString(), dgvEvent.Rows[e.RowIndex].Cells[COLMESSAGE].Value.ToString());
                dgvEvent.Rows.RemoveAt(e.RowIndex);
                if (dgvEvent.RowCount == 0) song.Stop();
            }
        }

        private void BtnSongMute_Click(object sender, EventArgs e)
        {
            song.Mute();
            btnSongMute.Image = (song.IsMute()) ? Properties.Resources.sound_mute1 : Properties.Resources.sound1;
        }

        private void BtnGetAllEvents_Click(object sender, EventArgs e)
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
        #endregion 

        #region ModemsWork
        const string MODEMSCOLID = "id";
        const string MODEMSCOLIMEI = "imei";
        const string MODEMSCOLPORT = "port";
        const string MODEMSCOLNAME = "name";
        const string MODEMSCOLLASTCONNECTION = "lastConnection";
        const string MODEMSCOLACTIVECONNECTION = "activeConnection";
        private DataGridViewCellEventArgs mouseLocation;
        private void dgvModems_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            mouseLocation = e;
        }
        public void ViewModems(List<dynamic> records)
        {
            try
            {
                dgvModems.Rows.Clear();
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
                Console(e.Message);
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
                if ((int)rec.activeConnection == 1)
                    dgvModems.Rows[dgvModems.RowCount - 1].Cells[MODEMSCOLACTIVECONNECTION].Value = Properties.Resources.tick;
                else dgvModems.Rows[dgvModems.RowCount - 1].Cells[MODEMSCOLACTIVECONNECTION].Value = Properties.Resources.cross;
            }
        }

        private void Btn_AddModem_Click(object sender, EventArgs e)
        {
            FormAddModem formAddModem = new FormAddModem();
            if (formAddModem.ShowDialog() == DialogResult.OK)
            {
                SQLite.Instance.InsertModems(formAddModem.imei, formAddModem.port, formAddModem.name);
            }
            ViewModems(SQLite.Instance.GetModems());
        }

        private void UpdateDgvModems_Click(object sender, EventArgs e)
        {
            ViewModems(SQLite.Instance.GetModems());
        }
        private void TsmiRedactorModema_Click(object sender, EventArgs e)
        {
            FormRedactorModems redactorObj = new FormRedactorModems();
            redactorObj.Owner = this;
            redactorObj.oldNameModem = dgvModems.Rows[mouseLocation.RowIndex].Cells[MODEMSCOLNAME].Value.ToString();
            redactorObj.idModem = dgvModems.Rows[mouseLocation.RowIndex].Cells[MODEMSCOLID].Value.ToString();
            redactorObj.txtLastConnection.Text = dgvModems.Rows[mouseLocation.RowIndex].Cells[MODEMSCOLLASTCONNECTION].Value.ToString();
            redactorObj.txtImei.Text = dgvModems.Rows[mouseLocation.RowIndex].Cells[MODEMSCOLIMEI].Value.ToString();
            redactorObj.txtPort.Text = dgvModems.Rows[mouseLocation.RowIndex].Cells[MODEMSCOLPORT].Value.ToString();
            redactorObj.txtNameModem.Text = dgvModems.Rows[mouseLocation.RowIndex].Cells[MODEMSCOLNAME].Value.ToString();
            redactorObj.Show();
        }
        #endregion

        #region TreeView
        private TreeNodeMouseClickEventArgs nodeLocation;
        private void TrView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Name == "0")
            {
                tsmiCustomGroupSendMail.Enabled = false;
            }
            else
            {
                tsmiCustomGroupSendMail.Enabled = true;
            }
            nodeLocation = e;
            List<string> tmp = SQLite.Instance.GetParentsNodeForCheck(e.Node.Text);
            if (tmp.Count > 0) tsmiDelNode.Enabled = false;
            else tsmiDelNode.Enabled = true;

            List<dynamic> listModems = SQLite.Instance.GetModems();
            List<dynamic> listViewModems = new List<dynamic>();
            List<dynamic> listNodesTree = SQLite.Instance.GetAllNodesTree();
            TreeNode selectedNode;
            if (e.Node.Name == "0")
            {
                ViewModems(SQLite.Instance.GetModems());
            }
            else
            {
                selectedNode = e.Node;
                foreach (var obj in listModems)
                {
                    if (selectedNode.Name == obj.group) listViewModems.Add(obj);
                }
                if (selectedNode.FirstNode != null) listViewModems.AddRange(RoundOnNodeOut(listModems, selectedNode.FirstNode));
                ViewModems(listViewModems);
            }
        }
        public List<dynamic> RoundOnNodeOut(List<dynamic> listModems, TreeNode selectedNode)
        {
            List<dynamic> listViewModems = new List<dynamic>();
            for (; ; )
            {
                foreach (var obj1 in listModems)
                {
                    if (selectedNode.Name == obj1.group) listViewModems.Add(obj1);
                }

                selectedNode = selectedNode.NextNode;
                if (selectedNode == null) return listViewModems;
                RoundOnNodeOut(listModems, selectedNode);
            }
        }

        public void InitTree(TreeView treeView)
        {
            treeView.Nodes.Clear();
            treeView.Nodes.Add("0", "Все");
            List<dynamic> listNodesTree = SQLite.Instance.GetAllNodesTree();
            AtChildrenToTree(listNodesTree, "Все", treeView.Nodes["0"]);
            treeView.Nodes["0"].ExpandAll();
        }
        public void AtChildrenToTree(List<dynamic> listNodesTree, string parent, TreeNode selectedNode)
        {
            List<dynamic> listChildrens = listNodesTree.FindAll(x => x.parent == parent);
            foreach (var child in listChildrens)
            {
                selectedNode.Nodes.Add(child.id.ToString(), child.name);
                AtChildrenToTree(listNodesTree, child.name, selectedNode.Nodes[child.id.ToString()]);
            }
        }


        private void ToolStripMenuItem_Click(object sender, EventArgs e) // добавить группу
        {
            FormAddNode formAddNode = new FormAddNode();
            if (formAddNode.ShowDialog() == DialogResult.OK)
            {
                TreeNode newNode = new TreeNode(formAddNode.txtNameGroup.Text);
                nodeLocation.Node.Nodes.Add(newNode);
                nodeLocation.Node.Expand();

                string senderMail = ConfigurationManager.AppSettings.Get("senderMail");
                string nameSenderMail = ConfigurationManager.AppSettings.Get("nameSenderMail");
                string recieverMail = ConfigurationManager.AppSettings.Get("recieverMail");
                string subjectMail = ConfigurationManager.AppSettings.Get("subjectMail");
                string smtpClient = ConfigurationManager.AppSettings.Get("smtpClient");
                Int32.TryParse(ConfigurationManager.AppSettings.Get("smtpPort"), out int smtpPort);
                string senderPassword = ConfigurationManager.AppSettings.Get("senderPassword");

                SQLite.Instance.InsertNode(newNode.Text, newNode.Parent.Text, senderMail, recieverMail, nameSenderMail, subjectMail, smtpClient, smtpPort.ToString(), senderPassword);

            }
        }
        private void DelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SQLite.Instance.DeleteNode(nodeLocation.Node.Name);
            InitTree(trView1);
        }

        private void TsmiAddGroup_Click(object sender, EventArgs e)
        {
            FormAddInGroup formAddInGroup = new FormAddInGroup();
            formAddInGroup.labNameModem.Text = dgvModems.Rows[mouseLocation.RowIndex].Cells[MODEMSCOLNAME].Value.ToString();
            formAddInGroup.labImei.Text = dgvModems.Rows[mouseLocation.RowIndex].Cells[MODEMSCOLIMEI].Value.ToString();
            formAddInGroup.labLastConnection.Text = dgvModems.Rows[mouseLocation.RowIndex].Cells[MODEMSCOLLASTCONNECTION].Value.ToString();
            string idGroup = SQLite.Instance.GetIdGroup(formAddInGroup.labImei.Text);
            if (idGroup == "0") formAddInGroup.labCurrentGroup.Text = "Все";
            else formAddInGroup.labCurrentGroup.Text = SQLite.Instance.GetCurrentGroup(idGroup);
            InitTree(formAddInGroup.treeViewAdd);
            if (formAddInGroup.ShowDialog() == DialogResult.OK)
            {
                Console($"Объект {formAddInGroup.labNameModem.Text} был успешно добавлен в группу {formAddInGroup.nameNode}");
            }
        }
        #endregion

        #region ButtonsAndFunccionForm1
        private void BtSendAuth_Click(object sender, EventArgs e)
        {
            Thread SendAuth = new Thread(sendAuth);
            SendAuth.IsBackground = true;
            SendAuth.Start();
        }
        private void BtnClear_Click(object sender, EventArgs e)
        {
            lbConsole.Items.Clear();
        }
        private void TxtFinder_TextChanged(object sender, EventArgs e)
        {

        }
        private void BtChangePort_Click(object sender, EventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            FormChangePort formChangePort = new FormChangePort();
            formChangePort.txtPort.Text = ConfigurationManager.AppSettings.Get("port");
            if (formChangePort.ShowDialog() == DialogResult.OK)
            {
                config.AppSettings.Settings["port"].Value = formChangePort.txtPort.Text;
                config.Save();
                ConfigurationManager.RefreshSection("appSettings");
                Application.Restart();
            }
        }
        public void Console(string str)
        {
            tcDown.Invoke(new Action(() => tcDown.SelectedTab = tpConsole));
            string textConsole = $"{DateTime.Now}: {str}";
            lbConsole.Invoke(new Action(() => lbConsole.Items.Add(textConsole)));
        }
        private void BtSendConfig_Click(object sender, EventArgs e)
        {
            Thread SendConfig = new Thread(sendConfig);
            SendConfig.IsBackground = true;
            SendConfig.Start();
        }

        private void BtSendMail_Click(object sender, EventArgs e)
        {
            try
            {
                SendEmailAsync("Тестовый объект", DateTime.Now, "Открытие-закрытие шкафа", "852585258525852").GetAwaiter();
                Console("Письмо отправлено успешно.");
            }
            catch (Exception exc)
            {
                Console(exc.Message);
            }
        }
        #endregion

        #region SendMail
        private void InsertMailSender(string imei)
        {
            // взятие переменных из app.config
            string senderMail = ConfigurationManager.AppSettings.Get("senderMail");
            string nameSenderMail = ConfigurationManager.AppSettings.Get("nameSenderMail");
            string recieverMail = ConfigurationManager.AppSettings.Get("recieverMail");
            string subjectMail = ConfigurationManager.AppSettings.Get("subjectMail");
            string smtpClient = ConfigurationManager.AppSettings.Get("smtpClient");
            Int32.TryParse(ConfigurationManager.AppSettings.Get("smtpPort"), out int smtpPort);
            string senderPassword = ConfigurationManager.AppSettings.Get("senderPassword");

            SQLite.Instance.InsertSenderMail(imei, "0", senderMail, recieverMail, nameSenderMail, subjectMail, smtpClient, smtpPort.ToString(), senderPassword);
        }

        private static async Task SendEmailAsync(string nameModem, DateTime dateEvent, string message, string imei)
        {
            // взятие переменных из Базы Данных
            dynamic record = SQLite.Instance.GetAllParamSenderMail(imei);
            string senderMail = record.senderMail;
            string nameSenderMail = record.nameSenderMail;
            string recieverMail = record.recieverMail;
            List<string> listRecievers = new List<string>();
            if (recieverMail.Contains(','))
            {
                listRecievers.AddRange(recieverMail.Split(','));
            }
            else 
            {
                listRecievers.Add(recieverMail);
            }
            string subjectMail = record.subjectMail;
            string smtpClient = record.smtpClient;
            Int32.TryParse(record.smtpPort, out int smtpPort);
            string senderPassword = record.senderPassword;

            // настройка почты
            MailAddress from = new MailAddress(senderMail, nameSenderMail); // отправитель и имя отправителя
            MailAddress to = new MailAddress(recieverMail); // получатель
            MailMessage m = new MailMessage(from, to); // объект сообщения
            foreach(var list in listRecievers)
            {
                m.CC.Add(list);
            }
            m.Subject = subjectMail; // тема
            m.Body = $"{nameModem}: {message} {dateEvent}\nImei: {imei}";
            SmtpClient smtp = new SmtpClient(smtpClient, smtpPort); // адрес smtp-сервера и порт, с которого будем отправлять письмо
            smtp.Credentials = new NetworkCredential(senderMail, senderPassword); // логин и пароль
            smtp.EnableSsl = true;

            await smtp.SendMailAsync(m);
        }
        private void TsmiMailSendCustom_Click(object sender, EventArgs e)
        {
            FormChangeSenderMails form = new FormChangeSenderMails();
            form.isCustomGroup = false;
            form.labelNameModem.Text = dgvModems.Rows[mouseLocation.RowIndex].Cells[MODEMSCOLNAME].Value.ToString();
            form.labelImeiModem.Text = dgvModems.Rows[mouseLocation.RowIndex].Cells[MODEMSCOLIMEI].Value.ToString();
            form.labelInfoText.Visible = true;
            form.labelInfoText.Text = "Настройки данной формы\nбудут действовать только на\nданный объект";

            dynamic record = SQLite.Instance.GetAllParamSenderMail(form.labelImeiModem.Text);
            if (record.group == "0") form.txtGroup.Text = "Все";
            else
            {
                form.txtGroup.Text = SQLite.Instance.GetCurrentGroup(record.group);
            }
            form.txtNameSenderMail.Text = record.nameSenderMail;
            form.txtRecieverMail.Text = record.recieverMail;
            form.txtSenderMail.Text = record.senderMail;
            form.txtSmtpClient.Text = record.smtpClient;
            form.txtSmtpPort.Text = record.smtpPort;
            form.txtSubjectMail.Text = record.subjectMail;
            form.txtSenderPassword.Text = record.senderPassword;

            if (form.ShowDialog() == DialogResult.OK)
            {

            }
        }
        private void TsmiCustomGroupSendMail_Click(object sender, EventArgs e)
        {
            FormChangeSenderMails form = new FormChangeSenderMails();
            form.isCustomGroup = true;
            form.labelNameModem.Visible = false;
            form.labelImeiModem.Visible = false;
            form.labImei.Visible = false;
            form.labName.Visible = false;
            form.labelInfoText.Visible = true;
            form.labelInfoText.Text = "Настройки данной формы\nбудут действовать на все\nдочерние узлы группы";
            form.labelInfoText.Location = new System.Drawing.Point(20, 20);
            form.txtGroup.Text = nodeLocation.Node.Text;

            dynamic record = SQLite.Instance.GetAllParamNodesTree(nodeLocation.Node.Name);
            // взятие переменных из app.config
            form.txtNameSenderMail.Text = record.nameSenderMail;
            form.txtRecieverMail.Text = record.recieverMail;
            form.txtSenderMail.Text = record.senderMail;
            form.txtSmtpClient.Text = record.smtpClient;
            form.txtSmtpPort.Text = record.smtpPort;
            form.txtSubjectMail.Text = record.subjectMail;
            form.txtSenderPassword.Text = record.senderPassword;

            // получаем список imei модемов дочерних узлов
            List<dynamic> listModems = GetImeiModemFromNodes(nodeLocation);

            if (form.ShowDialog() == DialogResult.OK)
            {
                foreach (var list in listModems)
                {
                    SQLite.Instance.UpdateDbMails(list.imei, form.txtSenderMail.Text, form.txtNameSenderMail.Text, form.txtRecieverMail.Text, form.txtSubjectMail.Text, form.txtSmtpClient.Text, form.txtSmtpPort.Text, form.txtSenderPassword.Text);
                }
            }
        }
        private List<dynamic> GetImeiModemFromNodes(TreeNodeMouseClickEventArgs e)
        {
            List<dynamic> listModems = SQLite.Instance.GetModems();
            List<dynamic> listViewModems = new List<dynamic>();
            TreeNode selectedNode;
            if (e.Node.Name == "0")
            {
                return SQLite.Instance.GetModems();
            }
            else
            {
                selectedNode = e.Node;
                foreach (var obj in listModems)
                {
                    if (selectedNode.Name == obj.group) listViewModems.Add(obj);
                }
                if (selectedNode.FirstNode != null) listViewModems.AddRange(RoundOnNodeOutImei(listModems, selectedNode.FirstNode));
                return listViewModems;
            }
        }
        public List<dynamic> RoundOnNodeOutImei(List<dynamic> listModems, TreeNode selectedNode)
        {
            List<dynamic> listViewModems = new List<dynamic>();
            for (; ; )
            {
                foreach (var obj1 in listModems)
                {
                    if (selectedNode.Name == obj1.group) listViewModems.Add(obj1);
                }

                selectedNode = selectedNode.NextNode;
                if (selectedNode == null) return listViewModems;
                RoundOnNodeOut(listModems, selectedNode);
            }
        }
        #endregion
    }
}
