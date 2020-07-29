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
 //   u16 u16FlashVer;                    //0x00  (0)
 //   u8 u8NetworkAddress;                //0x02
 //   u8 u8Mode;                          // =4 байта

 //   uint32_t u32ReleaseTs;              //=4 байта

 //   u8 APN[2][32];
	//u8 client[32];
 //   u8 server[32];

 //   tsUartConfig sUart1;    //64 bit
 //   tsUartConfig sUart2;
 //   tsUartConfig sUart3;    //по 8 байт * 3 = 24 байта

 //   //Периоды, времена
 //   u16 PeriodEvent;    				// Период опроса нештатных ситуации
 //   u8 u8ModemType;
 //   u8 u8firstServer;

 //   u32 u32CounterNA[4];        		//Сетевые адреса счетчиков  	= 16 байтов ( по indexу счетчика

 //   u8 u8CounterType[4];                //Типы счетчиков   = 4 байта

 //   u8 u8currSimCard;
 //   u8 u8Reserved1;
 //   u8 u8Reserved2;
 //   u8 u8Reserved3;

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public struct tsApnName
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] APN; // 24
    }
}
