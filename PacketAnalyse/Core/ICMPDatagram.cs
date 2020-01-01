using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketAnalyse.Core
{
    class ICMPDatagram : IInternetData
    {
        byte Type;
        byte Code;

        public ICMPDatagram(byte type, byte code)
        {
            Type = type;
            Code = code;
        }

        public ProtocalType ProtocalType => ProtocalType.ICMP;

        public IInternetData Super { get; private set; }

        public bool HasSuper { get; private set; }

        public byte[] RawData { get; private set; }

        public override string ToString()
        {
            string info = "";
            if(Type == 3)
            {
                info += "目的不可达";
            }
            else if (Type == 11 && Code == 0)
            {
                info += "超时(路由器)";
            }
            else if (Type == 11 && Code == 1)
            {
                info += "超时(目标主机)";
            }
            else if(Type == 8 && Code == 0)
            {
                info += "回显请求";
            } else if (Type == 0 && Code == 0)
            {
                info += "回显应答";
            } else
            {
                info += "其他";
            }
            info += $" Type: {Type} Code: {Code}";

            return info;
        }

        internal static IInternetData Parse(byte[] superData)
        {
            return new ICMPDatagram(superData[0], superData[1])
            {
                Super = null,
                HasSuper = false,
                RawData = superData
            };
        }
    }
}
