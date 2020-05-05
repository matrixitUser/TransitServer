using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;      // потребуется
using System.Net.Sockets;    // потребуется
using System.Windows.Forms;


namespace TransitServer
{
    public partial class Form1 : Form
    {

        private void createLServerToAskue(dynamic obj)
        {
            dynamic objTmp = (dynamic)obj;
            int port = (int)objTmp.port;
            string IMEI = (string)objTmp.imei;
            gTcpAskueServer = new TcpListener(IPAddress.Any, port);
            gTcpAskueServer.Start();  // запускаем сервер

            //   Thread newSocketAskue = new Thread(socketListeningAskue);
            //   newSocketAskue.IsBackground = true;
            //   newSocketAskue.Start();


            AskueServer server = new AskueServer();
            server.IMEI = IMEI;
            server.tcpListener = gTcpAskueServer;
            gAskueServers.Add(server);
            socketListeningAskue(gAskueServers.Count - 1);
        }

        private void socketListeningAskue(int indexServer)
        {
            bool isWhileS = true;
            while (isWhileS)
            {
                TcpClient tcpclient = new TcpClient();
                try { tcpclient = gTcpAskueServer.AcceptTcpClient(); }
                catch (Exception e)
                {
                    tcpclient.Close();
                    break;  //TODO 
                }
                string statusStr = string.Format("Клиенты АСКУЭ:{0}", gAskueServers.Count);
                statusString.Invoke(new Action(() => statusString.Items[1].Text = statusStr));

                NetworkStream ns = tcpclient.GetStream(); // для получения и отправки сообщений
                gAskueServers[indexServer].nsSet(ns);

                while (tcpclient.Connected && isWhileS)  // пока клиент подключен, ждем приходящие сообщения
                {
                    StringBuilder builder = new StringBuilder();
                    byte[] msg = new byte[1024];     // готовим место для принятия сообщения
                    try
                    {
                        int count = ns.Read(msg, 0, msg.Length);   // читаем сообщение от клиента
                                                                   //ns.Write(msg, 0, msg.Length);
                                                                   //lbConsole.Invoke(new Action(() => lbConsole.Items.Add))
                        if (count == 0) { isWhileS = false; break; }
                        retransmit(msg, count, tcpclient.Client.Handle);
                    }
                    catch
                    {
                        isWhileS = false;
                    }

                }
                tcpclient.Close();
                isWhileS = true;
            }

        }
        
        private void retransmit(byte[] msg, int len, IntPtr clientHandle)
        {
            byte[] bytes = new byte[512];
            byte[] crc = new byte[2];
            //Сделано только для одного модема
           if (gModemClients.Count > 0)
            {
                gModemClients[0].ns.Write(msg, 0, len);
            }
        }
    }
}
