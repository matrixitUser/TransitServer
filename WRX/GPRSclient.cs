using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace TransitServer
{
    class GPRSclient
    {
        public TcpClient tcpClient = new TcpClient();
        //public string IMEI { set; get;}
        public string IMEI;
        public IntPtr handle;
        public int PORT = 0;
        public bool isAuth;
        public NetworkStream ns;
        public GPRSclient()
        {
            IMEI = "000000000000000";
            isAuth = false;
        }
        public void nsSet(NetworkStream ns)
        {
            this.ns = ns;
        }
        public void IMEIset(string IMEI )
        {
            this.IMEI = IMEI;
        }
    }
}

