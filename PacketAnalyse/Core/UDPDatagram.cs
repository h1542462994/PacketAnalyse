using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PacketAnalyse.Core
{
    [StructLayout(LayoutKind.Explicit)]
    struct UDPDatagramHeaderRaw
    {
        [FieldOffset(0)]
        public readonly byte SourcePort0;
        [FieldOffset(1)]
        public readonly byte SourcePort1;
        [FieldOffset(2)]
        public readonly byte DestPort0;
        [FieldOffset(3)]
        public readonly byte DestPort1;
        [FieldOffset(4)]
        public readonly byte Length0;
        [FieldOffset(5)]
        public readonly byte Length1;
        [FieldOffset(6)]
        public readonly byte CheckSum0;
        [FieldOffset(7)]
        public readonly byte CheckSum1;

        public UDPDatagramHeader To()
        {
            ushort sourcePort = (ushort)((SourcePort0 << 4) + SourcePort1);
            ushort destPort = (ushort)((DestPort0 << 4) + DestPort1);
            ushort length = (ushort)((Length0 << 4) + Length1);
            ushort checkSum = (ushort)((CheckSum0 << 4) + CheckSum1);
            return new UDPDatagramHeader(sourcePort, destPort, length, checkSum);
        }
    }

    public struct UDPDatagramHeader
    {
        public readonly ushort SourcePort;
        public readonly ushort DestPort;
        public readonly ushort Length;
        public readonly ushort CheckSum;

        public UDPDatagramHeader(ushort sourcePort, ushort destPort, ushort length, ushort checkSum)
        {
            SourcePort = sourcePort;
            DestPort = destPort;
            Length = length;
            CheckSum = checkSum;
        }
    }

    class UDPDatagram : IInternetData
    {
        public UDPDatagramHeader Header { get; private set; }

        public ProtocalType ProtocalType => ProtocalType.UDP;

        public IInternetData Super { get; private set; }

        public bool HasSuper { get; private set; }

        public byte[] RawData { get; private set; }


        public unsafe static UDPDatagram Parse(byte[] data)
        {
            if (data == null)
            {
                return null;
            }
            fixed (byte* pBuffer = data)
            {
                var headerRaw = (UDPDatagramHeaderRaw*)pBuffer;
                var header = headerRaw->To();

                ProtocalType type;
                ushort port = header.DestPort < header.SourcePort? header.DestPort : header.SourcePort;
                if (port == 53)
                {
                    type = ProtocalType.DNS;
                }
                else if (port == 67 || port == 68)
                {
                    type = ProtocalType.DHCP;
                }
                else if (port == 220)
                {
                    type = ProtocalType.IMAP3;
                }
                else 
                {
                    type = ProtocalType.Unknown;
                }

                ArraySegment<byte> superData = new ArraySegment<byte>(data, 8, data.Length - 8);
                UDPDatagram uDPDatagram = new UDPDatagram()
                {
                    Super = new NoAnalyseDatagram(superData.ToArray(), type),
                    HasSuper = true,
                    RawData = data,
                    Header = header
                };

                return uDPDatagram;
            }
        }
    }
}
