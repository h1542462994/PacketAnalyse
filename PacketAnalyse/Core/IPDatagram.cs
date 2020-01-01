using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PacketAnalyse.Core
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct IPDatagramHeaderRaw
    {
        /// <summary>
        /// 版本号和首部长度
        /// </summary>
        [FieldOffset(0)]
        public readonly byte VersionAndLength;
        /// <summary>
        /// 区分服务
        /// </summary>
        [FieldOffset(1)]
        public readonly byte Services;
        /// <summary>
        /// 总长度
        /// </summary>
        [FieldOffset(2)]
        public readonly byte Length0;
        [FieldOffset(3)]
        public readonly byte Length1;
        /// <summary>
        /// 标识0, 1
        /// </summary>
        [FieldOffset(4)]
        public readonly byte Identification0;
        [FieldOffset(5)]
        public readonly byte Identification1;
        [FieldOffset(6)]
        public readonly byte FlagOffset0;
        [FieldOffset(7)]
        public readonly byte FlagOffset1;
        [FieldOffset(8)]
        public readonly byte TTL;
        [FieldOffset(9)]
        public readonly byte Protocal;
        [FieldOffset(10)]
        public readonly byte CheckSum0;
        [FieldOffset(11)]
        public readonly byte CheckSum1;
        [FieldOffset(12)]
        public readonly uint Source;
        [FieldOffset(16)]
        public readonly uint Dest;

        public IPDatagramHeader To(byte[] data)
        {
            IPAddress source = new IPAddress(Source);
            IPAddress dest = new IPAddress(Dest);
            byte version = (byte)((VersionAndLength & 0xf0) >> 4);
            byte headerLength = (byte)(VersionAndLength & 0x0f);
            byte flag = (byte)(FlagOffset0 >> 5);
            byte rf = (byte)((flag & 0b100) >> 2);
            byte df = (byte)((flag & 0b010) >> 1);
            byte mf = (byte)(flag & 0b001);
            ushort id = (ushort)((Identification0 << 8) + Identification1);
            ushort length = (ushort)((Length0 << 4) + Length1);
            ushort checkSum = (ushort)((CheckSum0 << 4) + CheckSum1);
            ushort offset = (ushort)(((FlagOffset0 & 0b00011111) << 8) + FlagOffset1);

            int headerL = headerLength * 4;

            byte[] optionRaw = new ArraySegment<byte>(data, 20, headerL - 20).ToArray();

            ProtocalType type;
            if (Protocal == 1)
            {
                type = ProtocalType.ICMP;
            }
            else if (Protocal == 2)
            {
                type = ProtocalType.IGMP;
            }
            else if (Protocal == 4)
            {
                type = ProtocalType.IP;
            }
            else if (Protocal == 6)
            {
                type = ProtocalType.TCP;
            }
            else if (Protocal == 17)
            {
                type = ProtocalType.UDP;
            }
            else if (Protocal == 255)
            {
                type = ProtocalType.NoSuper;
            }
            else
            {
                type = ProtocalType._;
            }

            return new IPDatagramHeader(version, headerLength, Services, length, id, flag, rf, df, mf, offset, TTL,Protocal, type, checkSum, source, dest, optionRaw);

        }
    }

    public struct IPDatagramHeader
    {
        public byte Version;
        public byte HeaderLength;
        public byte Service;
        public ushort Length;
        public ushort Identification;
        public byte Flag;
        public byte RFFlag;
        public byte DFFlag;
        public byte MFFlag;
        public ushort Offset;
        public byte TTL;
        public byte ProtocalRaw;
        public ProtocalType Type;
        public ushort CheckSum;
        public IPAddress Source;
        public IPAddress Dest;
        public byte[] OptionRaw;

        public IPDatagramHeader(byte version, byte headerLength, byte service, ushort length, ushort identification, byte flag, byte rFFlag, byte dFFlag, byte mFFlag, ushort offset, byte tTL, byte protacalRaw, ProtocalType type, ushort checkSum, IPAddress source, IPAddress dest, byte[] optionRaw)
        {
            Version = version;
            HeaderLength = headerLength;
            Service = service;
            Length = length;
            Identification = identification;
            Flag = flag;
            RFFlag = rFFlag;
            DFFlag = dFFlag;
            MFFlag = mFFlag;
            Offset = offset;
            TTL = tTL;
            ProtocalRaw = protacalRaw;
            Type = type;
            CheckSum = checkSum;
            Source = source;
            Dest = dest;
            OptionRaw = optionRaw;
        }

        public override string ToString()
        {
            return $"{Source}=>{Dest}\nversion:{Version}\t,headerLength:{HeaderLength}\t,tos:{Service}\t,length:{Length}\n"
                + $"id:{Identification}\t,r:{RFFlag}\t,df:{DFFlag}\t,mf:{MFFlag}\t,offset:{Offset}\n" +
                 $"ttl:{TTL}\t,protocal:{ProtocalRaw}({Type})\t,checksum:{CheckSum}";
        }

        public bool IsInnerDatagram()
        {
            return Source.IsInnerIP() && Dest.IsInnerIP();
        }

        public bool IsOneAddressOf(IPAddress iPAddress)
        {
            return Source.Equals(iPAddress) || Dest.Equals(iPAddress);
        }
    }

    /// <summary>
    /// IP数据包
    /// </summary>
    public class IPDatagram : IInternetData
    {
        public DateTime Tick { get; private set; }

        public ProtocalType ProtocalType { get => ProtocalType.IP; }
        public IInternetData Super { get; private set; }
        public bool HasSuper { get; private set; }
        public byte[] RawData { get; private set; }

        public IPDatagramScope Scope { get => new IPDatagramScope(this); }

        public IPDatagramHeader Header { get; private set; }

        public override string ToString()
        {
            return $"{this.Scope()}\n{Header.ToString()}";
        }

        public unsafe static IPDatagram Parse(byte[] data)
        {
            if (data == null) return null;
              
            fixed (byte* pBuffer = data)
            {
                var headerRaw = (IPDatagramHeaderRaw*)pBuffer;
                var header = headerRaw->To(data);
                int headerL = header.HeaderLength * 4;

                byte[] superData = new ArraySegment<byte>(data, headerL, data.Length - headerL).ToArray();

                var ipDatagram = new IPDatagram()
                {
                    Header = header,
                    Tick = DateTime.Now
                };
                ipDatagram.RawData = data;
                if (header.Type == ProtocalType.NoSuper)
                {
                    ipDatagram.HasSuper = false;
                    ipDatagram.Super = null;
                }
                else if (header.Type == ProtocalType._ || header.Type == ProtocalType.ICMP || header.Type == ProtocalType.IGMP)
                {
                    ipDatagram.HasSuper = true;
                    ipDatagram.Super = new NoAnalyseDatagram(superData, header.Type);
                }
                else if(header.Type == ProtocalType.UDP)
                {
                    ipDatagram.HasSuper = true;
                    ipDatagram.Super = UDPDatagram.Parse(superData);
                }
                else if(header.Type == ProtocalType.TCP)
                {
                    ipDatagram.HasSuper = true;
                    ipDatagram.Super = TCPDatagram.Parse(superData);
                }

                Console.WriteLine();
                Console.WriteLine(ipDatagram);

                return ipDatagram;
            }
        }

        public string getInfo()
        {
            if (this.Super is TCPDatagram gram)
            {
                return gram.ToString();
            }

            return "";
        }
    }

    public class IPDatagramScope
    {

        private IPDatagram data;
        public string 协议栈 => data.Scope();
        public string 时间戳 => data.Tick.ToTickTimeString();
        public IPAddress 源地址 => data.Header.Source;
        public IPAddress 目的地址 => data.Header.Dest;
        public int 首部长度 => data.Header.HeaderLength;
        public int 长度 => data.Header.Length;
        public int 标识 => data.Header.Identification;
        public int DF => data.Header.DFFlag;
        public int MF => data.Header.MFFlag;
        public int 片偏移 => data.Header.Offset;
        public int TTL => data.Header.TTL;
        public string 上层协议 => $"{data.Header.ProtocalRaw}({data.Header.Type})";

        public string 详细信息 => data.getInfo();
        //public string 数据 => data.RawData.Scope();

        public IPDatagramScope(IPDatagram data)
        {
            this.data = data;
        }
    }


}
