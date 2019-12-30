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

    public static class InternetDataTool
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
    }
}
