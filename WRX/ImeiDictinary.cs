using System;
using System.Collections.Generic;

namespace TransitServer
{
    class ImeiDictinary
    {
        byte[] busyPort = new byte[5000];
        int startPort = 20000;
        public ImeiDictinary()
        {
            ReadDictSql();
        }
        public void ReadDictSql()
        {
            List<int> ports = SQLite.Instance.GetModemsPort();
            foreach (var port in ports)
            {
                ToArraybusyPort(port);
            } 
        }
        public void WriteItemSql(string IMEI, int port)
        {
            SQLite.Instance.InsertModems(IMEI, port.ToString());
        }
        public string GetNameSql(string IMEI)
        {
            return SQLite.Instance.GetModemsNameByImei(IMEI);
        }
        public void SetNameSql(string IMEI, string nameModem)
        {
            SQLite.Instance.UpdateNameModemsbyImei(IMEI, nameModem);
        }
        public int GetPortSql(string IMEI)
        {
            try
            {
                int tmpPort = SQLite.Instance.GetModemsPortByImei(IMEI);
                return tmpPort;
            }
            catch
            {
                return 0;
            }
        }
        public int SetPortSql(string IMEI)
        {
            List<string> tmpImei = SQLite.Instance.GetModemsImei();
            foreach(var i in tmpImei)
            {
                if (i == IMEI) //проверяю на наличие порта
                {
                    return SQLite.Instance.GetModemsPortByImei(IMEI);
                }
            }
            int port = FindFreePort();
            ToArraybusyPort(port);
            WriteItemSql(IMEI, port);
            return port;
        }

        private void ToArraybusyPort(int port) 
        {
            port = port - startPort;   //20310-20000=310
            int cel = port / 8;   //38
            int drop = port % 8;  //6     это 
            int indexPort = cel;  
            byte tmpByte = busyPort[indexPort]; 
            tmpByte = (byte)(tmpByte | (1 << drop));
            busyPort[indexPort] = tmpByte;
        }

        private int FindFreePort()
        {
            int i = 0;
            while ((i < 5000) && (busyPort[i] == 0xFF)) i++;
            if ((i < 5000) && (busyPort[i] != 0xFF))
            {
                for (int nBit = 0; nBit < 8; nBit++)
                {
                    if (((1 << nBit) & busyPort[i]) == 0)
                    {
                        return    i * 8 + nBit + startPort;
                    }
                }
            }
            return  0;
        }
    }
}
