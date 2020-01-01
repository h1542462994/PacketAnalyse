using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketAnalyse.Core
{
    public interface IInternetData
    {
        /// <summary>
        /// 当前的协议类型
        /// </summary>
        ProtocalType ProtocalType { get; }
        /// <summary>
        /// 上层协议的数据
        /// </summary>
        IInternetData Super { get; }
        /// <summary>
        /// 是否有上层协议
        /// </summary>
        bool HasSuper { get; }
        /// <summary>
        /// 协议的原始数据
        /// </summary>
        byte[] RawData { get;  }
    }

    public static class InternetDataExtension
    {
        public static string Scope(this IInternetData data)
        {
            string result = "";
            IInternetData d = data;
            while (d != null)
            {
                result += d.ProtocalType;
                if (d is UDPDatagram u)
                {
                    result += $"({u.Header.SourcePort} => {u.Header.DestPort})";
                }
                else if (d is TCPDatagram t)
                {
                    result += $"({t.Header.SourcePort} => {t.Header.DestPort})";
                }
                d = d.Super;
                if (d != null)
                {
                    result += "=>";
                }
            }

            return result;
        }


        public static bool IsType(this IInternetData data, ProtocalType type)
        {
            IInternetData d = data;
            while (d != null)
            {
                if (d.ProtocalType == type)
                {
                    return true;
                }
                d = d.Super;
            }
            return false;
        }

        public static bool IsType(this IInternetData data, Filters.ProtocalFilterOption type)
        {
            if (data.IsType(ProtocalType.ICMP))
            {
                if ((type & Filters.ProtocalFilterOption.ICMP) != Filters.ProtocalFilterOption.ICMP)
                {
                    return false;
                }
            }
            else if (data.IsType(ProtocalType.IGMP))
            {
                if ((type & Filters.ProtocalFilterOption.IGMP) != Filters.ProtocalFilterOption.IGMP)
                {
                    return false;
                }
            }
            else if (data.IsType(ProtocalType.DNS))
            {
                if ((type & Filters.ProtocalFilterOption.DNS) != Filters.ProtocalFilterOption.DNS)
                {
                    return false;
                }
            }
            else if (data.IsType(ProtocalType.Http))
            {
                if ((type & Filters.ProtocalFilterOption.Http) != Filters.ProtocalFilterOption.Http)
                {
                    return false;
                }
            }
            else if (data.IsType(ProtocalType.Https))
            {
                if ((type & Filters.ProtocalFilterOption.Https) != Filters.ProtocalFilterOption.Https)
                {
                    return false;
                }
            }
            else 
            {
                if ((type & Filters.ProtocalFilterOption.Others) != Filters.ProtocalFilterOption.Others)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
