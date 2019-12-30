using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketAnalyse.Core
{
    public static class NormalExtension
    {
        public static string Scope(this byte[] data)
        {
            StringBuilder builder = new StringBuilder();
            char[] charss = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
            foreach (byte item in data)
            {
                builder.Append($"{charss[(item & 0xf0) >> 4]}{charss[item & 0x0f]} ");
            }
            return builder.ToString();
        }
    }
}
