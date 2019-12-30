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
        [FieldOffset(0)]
        public readonly byte SourcePort0;
        [FieldOffset(1)]
        public readonly byte SourcePort1;
        [FieldOffset(2)]
        public readonly byte DestPort0;
        [FieldOffset(3)]
        public readonly byte DestPort1;

        [FieldOffset(12)]
        public readonly byte OffsetAndReserved;

        public TCPDatagramHeader To()
        {
            ushort sourcePort = (ushort)( SourcePort0 * 256 + SourcePort1);
            ushort destPort = (ushort)( DestPort0 * 256 + DestPort1);
            byte offset = (byte)(OffsetAndReserved >> 4);

            return new TCPDatagramHeader(sourcePort, destPort, offset);
        }
    }


    public struct TCPDatagramHeader
    {
        public readonly ushort SourcePort;
        public readonly ushort DestPort;
        public readonly byte Offset;

        public TCPDatagramHeader(ushort sourcePort, ushort destPort, byte offset)
        {
            SourcePort = sourcePort;
            DestPort = destPort;
            Offset = offset;
        }
    }

    public class TCPDatagram : IInternetData
    {
        public TCPDatagramHeader Header { get; private set; }

        public ProtocalType ProtocalType => ProtocalType.TCP;

        public IInternetData Super { get; private set; }

        public bool HasSuper { get; private set; }

        public byte[] RawData { get; private set; }

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
                    type = ProtocalType.Unknown;
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
