using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PacketAnalyse.Core
{
    public static class LocalIPHelper
    {
        public static IPAddress Get()
        {
            try
            {
                string hostName = Dns.GetHostName(); //得到主机名
                IPHostEntry ipEntry = Dns.GetHostEntry(hostName);
                foreach (IPAddress item in ipEntry.AddressList)
                {
                    if (item.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return item;
                    }
                }
                return null;
            } 
            catch (Exception)
            {
                return null;
            }
        }
    }
}
