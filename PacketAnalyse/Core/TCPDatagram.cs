using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PacketAnalyse.Core
{
    [StructLayout(LayoutKind.Explicit)]
    struct TCPDatagramHeaderRaw
    {
        /// <summary>
        /// 源端口
        /// </summary>
        [FieldOffset(0)]
        public readonly byte SourcePort0;
        [FieldOffset(1)]
        public readonly byte SourcePort1;
        /// <summary>
        /// 目标端口
        /// </summary>
        [FieldOffset(2)]
        public readonly byte DestPort0;
        [FieldOffset(3)]
        public readonly byte DestPort1;
        /// <summary>
        /// 序号
        /// </summary>
        [FieldOffset(4)]
        public readonly byte Sequence0;
        [FieldOffset(5)]
        public readonly byte Sequence1;
        [FieldOffset(6)]
        public readonly byte Sequence2;
        [FieldOffset(7)]
        public readonly byte Sequence3;
        /// <summary>
        /// 确认号
        /// </summary>
        [FieldOffset(8)]
        public readonly byte Acknowledgment0;
        [FieldOffset(9)]
        public readonly byte Acknowledgment1;
        [FieldOffset(10)]
        public readonly byte Acknowledgment2;
        [FieldOffset(11)]
        public readonly byte Acknowledgment3;
        /// <summary>
        /// 4位首部偏移长度
        /// </summary>
        [FieldOffset(12)]
        public readonly byte Offset4AndReserved4;
        /// <summary>
        /// 控制位 URG,ACK,PSH,RST,SYN,FIN
        /// </summary>
        [FieldOffset(13)]
        public readonly byte Reserved2AndControlFlags6;
        /// <summary>
        /// 窗口数
        /// </summary>
        [FieldOffset(14)]
        public readonly byte Windows0;
        [FieldOffset(15)]
        public readonly byte Windows1;
        /// <summary>
        /// 校验和
        /// </summary>
        [FieldOffset(16)]
        public readonly byte CheckSum0;
        [FieldOffset(17)]
        public readonly byte CheckSum1;
        /// <summary>
        /// 紧急指针
        /// </summary>
        [FieldOffset(18)]
        public readonly byte Urgent0;
        [FieldOffset(19)]
        public readonly byte Urgent1;

        /// <summary>
        /// 利用二进制位运算进行TCP报文解析
        /// </summary>
        /// <returns>返回解析好的TCP报文首部</returns>
        public TCPDatagramHeader To()
        {
            ushort sourcePort = (ushort)(SourcePort0 * 256 + SourcePort1);
            ushort destPort = (ushort)(DestPort0 * 256 + DestPort1);
            uint sequenceNumber = (uint)((Sequence0 << 24) + (Sequence1 << 16) + (Sequence2 << 8) + (Sequence3 << 0));
            uint acknowledgmentNumber = (uint)((Acknowledgment0 << 24) + (Acknowledgment1 << 16) + (Acknowledgment2 << 8) + (Acknowledgment3 << 0));
            byte offset = (byte)(Offset4AndReserved4 >> 4);
            byte URG = (byte)((Reserved2AndControlFlags6 & 0b00111111) >> 5);
            byte ACK = (byte)((Reserved2AndControlFlags6 & 0b00011111) >> 4);
            byte PSH = (byte)((Reserved2AndControlFlags6 & 0b00001111) >> 3);
            byte RST = (byte)((Reserved2AndControlFlags6 & 0b00000111) >> 2);
            byte SYN = (byte)((Reserved2AndControlFlags6 & 0b00000011) >> 1);
            byte FIN = (byte)((Reserved2AndControlFlags6 & 0b00000001) >> 0);
            ushort windows = (ushort)((Windows0 << 8) + Windows1);
            ushort checkSum = (ushort)((CheckSum0 << 8) + CheckSum1);
            ushort urgent = (ushort)((Urgent0 << 8) + Urgent1);
            return new TCPDatagramHeader(sourcePort, destPort, sequenceNumber, acknowledgmentNumber, offset, URG, ACK, PSH, RST, SYN, FIN, windows, checkSum, urgent);
        }
    }


    public struct TCPDatagramHeader
    {
        public readonly ushort SourcePort;
        public readonly ushort DestPort;
        public readonly uint SequenceNumber;
        public readonly uint AcknowledgmentNumber;
        public readonly byte Offset;
        public readonly byte URG;
        public readonly byte ACK;
        public readonly byte PSH;
        public readonly byte RST;
        public readonly byte SYN;
        public readonly byte FIN;
        public readonly ushort Windows;
        public readonly ushort CheckSum;
        public readonly ushort Urgent;

        public TCPDatagramHeader(
            ushort sourcePort, 
            ushort destPort, 
            uint sequenceNumber, 
            uint acknowledgmentNumber, 
            byte offset, 
            byte uRG, 
            byte aCK, 
            byte pSH, 
            byte rST, 
            byte sYN, 
            byte fIN, 
            ushort windows, 
            ushort checkSum, 
            ushort urgent
        )
        {
            SourcePort = sourcePort;
            DestPort = destPort;
            SequenceNumber = sequenceNumber;
            AcknowledgmentNumber = acknowledgmentNumber;
            Offset = offset;
            URG = uRG;
            ACK = aCK;
            PSH = pSH;
            RST = rST;
            SYN = sYN;
            FIN = fIN;
            Windows = windows;
            CheckSum = checkSum;
            Urgent = urgent;
        }

        /// <summary>
        /// 将常用的端口号标记出来
        /// </summary>
        /// <param name="port">端口号</param>
        /// <returns>端口用途</returns>
        public static string ParsePortToName(ushort port)
        {
            switch(port)
            {
                case 20:
                    return "ftp-data";
                case 21:
                    return "ftp";
                case 13:
                    return "daytime";
                case 22:
                    return "ssh";
                case 23:
                    return "telnet";
                case 25:
                    return "smtp";
                case 80:
                    return "http";
                case 443:
                    return "https";
                case 3306:
                    return "mysql";
                default:
                    return "";
            }
        }

        // 将TCP报文转换为文本摘要
        public override string ToString()
        {
            string info = "";
            info += $"{SourcePort}";
            var srcPortName = ParsePortToName(SourcePort);
            if (srcPortName.Length > 0)
            {
                info += $"({srcPortName})";
            }
            info += $" -> {DestPort} ";
            var destPortName = ParsePortToName(DestPort);
            if (destPortName.Length > 0)
            {
                info += $"({destPortName})";
            }
            info += " [";
            if (URG == 1)
            {
                info += "URG ";
            }
            if (ACK == 1)
            {
                info += "ACK ";
            }
            if (PSH == 1)
            {
                info += "PSH ";
            }
            if (RST == 1)
            {
                info += "RST ";
            }
            if (SYN == 1)
            {
                info += "SYN ";
            }
            if (FIN == 1)
            {
                info += "FIN ";
            }
            info += "] ";
            info += $" Seq: {SequenceNumber} Ack: {AcknowledgmentNumber} HLen: {Offset*4} Win: {Windows} CheckSum: {CheckSum}";
            return info;
        }
    }

    public class TCPDatagram : IInternetData
    {
        public TCPDatagramHeader Header { get; private set; }

        public ProtocalType ProtocalType => ProtocalType.TCP;

        public IInternetData Super { get; private set; }

        public bool HasSuper { get; private set; }

        public byte[] RawData { get; private set; }

        public override string ToString()
        {
            return Header.ToString();
        }

        public unsafe static TCPDatagram Parse(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            fixed (byte* pBuffer = data)
            {
                var headerRaw = (TCPDatagramHeaderRaw*)pBuffer;
                var header = headerRaw->To();
                ushort port = header.DestPort < header.SourcePort ? header.DestPort : header.SourcePort;
                ProtocalType type;
                // 根据端口标记协议
                if (port == 80)
                {
                    type = ProtocalType.Http;
                }
                else if (port == 443)
                {
                    type = ProtocalType.Https;
                }
                else if (port == 22)
                {
                    type = ProtocalType.Ssh;
                }
                else if (port == 20 || port == 21)
                {
                    type = ProtocalType.Ftp;
                }
                else
                {
                    type = ProtocalType._;
                }

                int headerL = header.Offset * 4;

                ArraySegment<byte> superData = new ArraySegment<byte>(data, headerL, data.Length - headerL);

                return new TCPDatagram()
                {
                    Header = header,
                    Super = new NoAnalyseDatagram(superData.ToArray(), type),
                    HasSuper = true,
                    RawData = data
                };
            }
        }
    }
}
