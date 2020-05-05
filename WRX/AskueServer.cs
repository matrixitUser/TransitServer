using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace TransitServer
{
    class AskueServer
    {
        public TcpListener tcpListener;
        //public string IMEI { set; get;}
        public string IMEI;
        public bool isAuth;
        public NetworkStream ns;
        public AskueServer()
        {
            IMEI = "000";
            isAuth = false;
        }
        public void nsSet(NetworkStream ns)
        {
            this.ns = ns;
        }
        public void IMEIset(string IMEI)
        {
            this.IMEI = IMEI;
        }
    }
}
