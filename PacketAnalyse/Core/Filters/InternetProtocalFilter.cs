using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketAnalyse.Core.Filters
{
    public class InternetProtocalFilter: IInternetFilter
    {
        public InternetProtocalFilter(ProtocalFilterOption type)
        {
            Type = type;
        }

        public ProtocalFilterOption Type { get; private set; }
    }

    public enum ProtocalFilterOption
    {
        ICMP = 1,
        IGMP = 2,
        DNS = 4,
        Http = 8,
        Https = 16,
        Others = 32,
        All = 63
    }
}
