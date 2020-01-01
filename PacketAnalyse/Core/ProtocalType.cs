using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketAnalyse.Core
{
    public enum ProtocalType
    {
        /// <summary>
        /// IP协议
        /// </summary>
        IP,
        ICMP,
        IGMP,
        IPv6,
        TCP,
        UDP,
        DNS,
        DHCP,
        Http,
        Https,
        Ftp,
        Ssh,
        /// <summary>
        /// 邮件读取协议v3
        /// </summary>
        IMAP3,
        _,
        NoSuper
    }
}
