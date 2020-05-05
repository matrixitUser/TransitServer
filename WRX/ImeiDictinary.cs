using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace TransitServer
{
    class ImeiDictinary
    {
        byte[] busyPort = new byte[5000];
        int startPort = 20000;
        int endPort = 60000;
        public string path = @"DictinaryImeiPort.txt";
        public Dictionary<string, string> imeiDict = new Dictionary<string, string>();
        public ImeiDictinary()
        {
            readDict();
        }

        public int readDict()
        {
            try
            {
                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] tmpDict = line.Split('~');
                        if (tmpDict.Length < 2) continue;
                        string tmpKey = tmpDict[0];
                        string tmpValue = tmpDict[1];
                        string[] tmpDictValue = tmpValue.Split('-');
                        int tmpPort = 0;
                        string tmpName = "";
                        DateTime tmpDate = DateTime.MinValue;
                        string tmpDate1 = "---";
                        try { tmpPort = Int32.Parse(tmpDictValue[0]);  }
                        catch(Exception e) {  }
                        try { tmpName = tmpDictValue[1]; }
                        catch { tmpName = "not name"; }
                        try { tmpDate = Convert.ToDateTime(tmpDictValue[2]); }
                        catch { }
                        if (tmpDate == DateTime.MinValue) tmpDate1 = "..........";
                        tmpValue = tmpPort.ToString() + "-" + tmpName + "-" + tmpDate1;
                        if (!imeiDict.ContainsKey(tmpKey))
                        {
                            imeiDict.Add(tmpKey, tmpValue);
                            toArraybusyPort(tmpPort);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return 0;
            }
            return imeiDict.Count;
        }

        public int writeDict()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
                    foreach (var item in imeiDict)
                    {
                        sw.WriteLine($"{item.Key}" + "~" + $"{item.Value}");
                    }

            }
            catch (Exception e)
            {
            }
            return 0;
        }


        public int writeItem(string IMEI)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
                    sw.WriteLine($"{IMEI}" + "~" + $"{ imeiDict[IMEI]}");
            }
            catch (Exception e)
            {
                return 0;
            }
            return 1;
        }
        public void removeItem(string IMEI)
        {
            if (imeiDict.ContainsKey(IMEI))
            {
                imeiDict.Remove(IMEI);
            }
        }
        public string getName(string IMEI)
        {
            string tmpName = "";
            if (imeiDict.ContainsKey(IMEI))
            {
                string[] tmpPortNameLastCon = imeiDict[IMEI].Split('-');
                try
                {
                    tmpName = tmpPortNameLastCon[1];
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                return tmpName;
            }
            return tmpName;
        }
        public void setName(string IMEI, string nameModem)
        {
            if (imeiDict.ContainsKey(IMEI))
            {
                string[] tmpPortNameLastCon = imeiDict[IMEI].Split('-');
                try
                {
                    tmpPortNameLastCon[1] = nameModem;
                    imeiDict[IMEI] = tmpPortNameLastCon[0] + "-" + tmpPortNameLastCon[1] + "-" + tmpPortNameLastCon[2];
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
        public DateTime getLastConnection(string IMEI)
        {
            DateTime tmpDate  = DateTime.MinValue;
            if (imeiDict.ContainsKey(IMEI))
            {
                string[] tmpPortNameLastCon = imeiDict[IMEI].Split('-');
                try
                {
                    tmpDate = Convert.ToDateTime(tmpPortNameLastCon[2]);
                }
                catch (Exception e)
                {
                    tmpDate = DateTime.MinValue;
                    //MessageBox.Show(e.Message);
                }

                return tmpDate;
            }
            return tmpDate;
        }
        public void setLastConnection(string IMEI, DateTime lastConnection)
        {
            if (imeiDict.ContainsKey(IMEI))
            {
                string[] tmpPortNameLastCon = imeiDict[IMEI].Split('-');
                try
                {
                    tmpPortNameLastCon[2] = lastConnection.ToString();
                    imeiDict[IMEI] = tmpPortNameLastCon[0] + "-" + tmpPortNameLastCon[1] + "-" + tmpPortNameLastCon[2];
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
        public int getPort(string IMEI)
        {
            int tmpPort = 0;
            if (imeiDict.ContainsKey(IMEI)) 
            {
                string[] tmpPortNameLastCon = imeiDict[IMEI].Split('-');
                try
                {
                    tmpPort = Int32.Parse(tmpPortNameLastCon[0]);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
               
                return tmpPort;
            }
            return 0;
        }
        public void setNewPort(string IMEI, int newPort)
        {
            if (imeiDict.ContainsKey(IMEI))
            {
                string[] tmpPortNameLastCon = imeiDict[IMEI].Split('-');
                try
                {
                    tmpPortNameLastCon[0] = newPort.ToString();
                    imeiDict[IMEI] = tmpPortNameLastCon[0] + "-" + tmpPortNameLastCon[1] + "-" + tmpPortNameLastCon[2];
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        public int setPort(string IMEI)
        {
            int port = 0;
            int tmpPort = 0;
            if (imeiDict.ContainsKey(IMEI))
            {
                string[] tmpPortNameLastCon = imeiDict[IMEI].Split('-');
                try
                {
                    tmpPort = Int32.Parse(tmpPortNameLastCon[0]);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                return tmpPort;
            }
            else 
            {
                port = findFreePort();
                //string[] tmpPortNameLastCon = imeiDict[IMEI].Split('-');
                //tmpPortNameLastCon[0] = port.ToString();
                imeiDict[IMEI] = port.ToString() + "-" + "Not Name" + "-" + DateTime.MinValue.ToString();
                try
                {
                    imeiDict.Add(IMEI, imeiDict[IMEI]);
                }
                catch (Exception e)
                {
                }
                toArraybusyPort(port);
                writeItem(IMEI); 
            }
            return port;
        }

        private void toArraybusyPort(int port) 
        {
            port = port - startPort;   //20310-20000=310
            int cel = port / 8;   //38
            int drop = port % 8;  //6     это 
            int indexPort = cel;  
            byte tmpByte = busyPort[indexPort]; 
            tmpByte = (byte)(tmpByte | (1 << drop));
            busyPort[indexPort] = tmpByte;
        }

        private int findFreePort()
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
