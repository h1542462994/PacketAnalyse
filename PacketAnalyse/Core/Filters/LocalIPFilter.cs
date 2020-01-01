using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PacketAnalyse.Core.Filters
{
    public class LocalIPFilter
    {
        public LocalIPFilter(IPAddress[] banedAddresses)
        {
            BanedAddresses = banedAddresses;
        }

        public IPAddress[] BanedAddresses { get; private set; }
    }
}
