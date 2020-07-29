using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransitServer
{
    class Register
    {
        static public byte[] MakeWriteBkpRequest(List<byte> NetworkAddress)
        {
            var Data = new List<byte>();

            //timestamp
            UInt32 ts;
    
            var span = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0));
            ts = (UInt32)span.TotalSeconds;
            //channels
            Data.AddRange(BitConverter.GetBytes(ts));
            return MakeWriteHoldingRegisterRequest(NetworkAddress, 0x32000, 4, Data);
        }

        static byte[] MakeWriteHoldingRegisterRequest(List<byte> NetworkAddress, UInt32 register, UInt16 registerCount, List<byte> wdata)
        {
            var Data = new List<byte>();

            UInt16 startRegister = (UInt16)register;
            Data.Add(Helper.GetHighByte(startRegister));
            Data.Add(Helper.GetLowByte(startRegister));

            Data.Add(Helper.GetHighByte(registerCount));
            Data.Add(Helper.GetLowByte(registerCount));

            Data.AddRange(wdata);

            return MakeBaseRequest(NetworkAddress, 16, Data);
        }
        static public byte[] MakeCorrectTime(List<byte> NetworkAddress)
        {
            var Data = new List<byte>();
            if (NetworkAddress.Count > 1)
            {
                Data.Add(251);
            }
            Data.AddRange(NetworkAddress);
            Data.Add(17);
            Data.Add(4);
            var span = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0));
            UInt32 ts = (UInt32)span.TotalSeconds;
            //channels
            Data.AddRange(BitConverter.GetBytes(ts));

            var crc = CRC.Calc(Data.ToArray(), new Crc16Modbus());
            Data.Add(crc.CrcData[0]);
            Data.Add(crc.CrcData[1]);

            return Data.ToArray();
        } 
        static public byte[] MakeBaseRequest(List<byte> NetworkAddress, byte Function, List<byte> Data)// = null
        {
            var bytes = new List<byte>();
            if (NetworkAddress.Count > 1)
            {
                bytes.Add(251);
            }
            bytes.AddRange(NetworkAddress);
            bytes.Add(Function);

            if (Data != null)
            {
                bytes.AddRange(Data);
            }

            var crc = CRC.Calc(bytes.ToArray(), new Crc16Modbus());
            bytes.Add(crc.CrcData[0]);
            bytes.Add(crc.CrcData[1]);

            return bytes.ToArray();
        }
    }
}
