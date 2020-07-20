using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TransitServer
{
    public partial class Form1 : Form
    {
        byte[] getBytes(tsConfig str)
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }
        tsConfig setBytes(byte[] arr)
        {
            tsConfig str = new tsConfig();

            int size = Marshal.SizeOf(str);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            str = (tsConfig)Marshal.PtrToStructure(ptr, str.GetType());
            Marshal.FreeHGlobal(ptr);

            return str;
        }
    }
    public struct tsUartConfig
    {
        public UInt32 u32BaudRate;
        public byte u8WordLen;
        public byte u8StopBits;
        public byte u8Parity;
        public byte reserved;
    }

    public struct tsLigthtChannel
    {
        public byte u8ControlMode;
        public byte u8beforeSunRise;
        public byte u8afterSunSet;
        public byte reserved;
        public UInt32 on1;
        public UInt32 off1;
        public UInt32 on2;
        public UInt32 off2;
    }

    public struct tsConfig
    {
        public UInt16 u16FlashVer;
        public byte u8NetworkAddress;
        public byte u8Mode;

        public tsUartConfig sUart1;
        public tsUartConfig sUart2;
        public tsUartConfig sUart3;

        public UInt32 u32ReleaseTs;

        public UInt16 u16TimeOut;
        public byte u8IsRtcError;
        public byte u8timeDiff;//+

        public UInt32 u32lat;//+
        public UInt32 u32lon; //+

        public tsLigthtChannel ligthtChannels1;//+
        public tsLigthtChannel ligthtChannels2;//+
        public tsLigthtChannel ligthtChannels3;//+
        public tsLigthtChannel ligthtChannels4;//+

        public byte u8hardware;
        public byte u8reserved;
        public UInt16 u16reserved;
    }
}
