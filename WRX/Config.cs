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
        public tsConfig setBytes(byte[] arr)
        {
            tsConfig str = new tsConfig();

            int size = Marshal.SizeOf(str);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            str = (tsConfig)Marshal.PtrToStructure(ptr, str.GetType());
            Marshal.FreeHGlobal(ptr);

            return str;
        }

        public T setBytesFromConfig<T>(byte[] arr, T type)
        {
            int size = Marshal.SizeOf(type);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            type = (T)Marshal.PtrToStructure(ptr, type.GetType());
            Marshal.FreeHGlobal(ptr);

            return type;
        }
        public byte[] getBytes<T>(T str)
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
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

    public struct tsConfig  //200 байт
    {
        public UInt16 u16FlashVer;          //2 
        public byte u8NetworkAddress;       //3
        public byte u8Mode;                 //4
        public UInt32 u32ReleaseTs;         //8

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public tsApnName[] apnName;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] u8client;      

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] u8server;      

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public tsUartConfig[] sUart;        

        public UInt16 PeriodEvent;
        public byte u8ModemType;
        public byte u8firstServer;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public UInt32[] u32CounterNA;     

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] u8CounterType;     
              
        public byte u8currSimCard;
        public byte u8Reserved1;
        public byte u8Reserved2;
        public byte u8Reserved3;                
    }
    
    public struct tsCurrent
    {

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public UInt32[] chipId;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] objectId;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
        public byte[] imei;
        public byte u8Reserved1;

        public UInt32 counterTime;

        public UInt16 event_;
        public byte u8Reserved2;
        public byte u8Reserved3;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public UInt32[] timeEvent;

        public UInt32 timePoll;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public struct tsApnName
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] APN; // 24
    }
}
