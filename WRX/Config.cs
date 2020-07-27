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

    public struct tsConfig  //200 байт
    {
        public UInt16 u16FlashVer;          //2 
        public byte u8NetworkAddress;       //3
        public byte u8Mode;                 //4
        public UInt32 u32ReleaseTs;         //8

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public tsApnName[] apnName;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] u8client;      //Типы счетчиков   = 24 байта              //200

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] u8server;      //Типы счетчиков   = 24 байта              //200

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public tsUartConfig[] sUart;         //160

        public UInt16 PeriodEvent;      // Период опроса нештатных ситуации //178

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public UInt32[] u32CounterNA;     //Сетевые адреса счетчиков  	= 16 байтов   //196

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] u8CounterType;      //Типы счетчиков   = 4 байта              //200

        public byte u8ModemType;                 //4
        public byte u8currSimCard;                 //4
        public byte u8firstServer;                 //4
        public byte u8Reserved3;                 //4
    }
    //// CSDtoGPRS

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public struct tsProfiles
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] ip_port; // 24
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public struct tsApnName
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] APN; // 24
    }
}
