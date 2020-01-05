namespace PacketAnalyse.Core
{
    class ICMPDatagram : IInternetData
    {
        byte Type; //ICMP报文类型
        byte Code; //ICMP报文代码

        public ICMPDatagram(byte type, byte code)
        {
            Type = type;
            Code = code;
        }

        public ProtocalType ProtocalType => ProtocalType.ICMP;

        public IInternetData Super { get; private set; }

        public bool HasSuper { get; private set; }

        public byte[] RawData { get; private set; }

        /// <summary>
        /// 根据ICMP报文的Type和Code，将常见的错误类型显示出来
        /// </summary>
        public override string ToString()
        {
            string info = "";
            if(Type == 3)
            {
                info += "目的不可达-";
                if (Code == 0)
                {
                    info += "网络不可达";
                } 
                else if (Code == 1)
                {
                    info += "主机不可达";
                }
                else if (Code == 2)
                {
                    info += "协议不可达";
                }
                else if (Code == 3)
                {
                    info += "端口不可达";
                } 
                else if (Code == 4)
                {
                    info += "需要进行分片但设置不分片比特";
                }
                else if (Code == 5)
                {
                    info += "源站选路失败";
                }
                else if (Code == 6)
                {
                    info += "目的网络未知";
                }
                else if (Code == 7)
                {
                    info += "目的主机未知";
                }
                else
                {
                    info += "其他原因";
                }

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
            } 
            else if (Type <= 18)
            {
                info += "其他类型";
            }
            else
            {
                info += "*";
            }

            if(Type <= 18)
            {
                info += $" [TYPE {Type}, Code {Code}]";
            }
            
            return info;
        }

        /// <summary>
        /// 对ICMP raw byte[]进行解析获取Type, Code
        /// </summary>
        /// <param name="data">ICMP报文</param>
        /// <returns>返回解析好的ICMPDatagram</returns>
        internal static IInternetData Parse(byte[] data)
        {
            return new ICMPDatagram(data[0], data[1])
            {
                Super = null,
                HasSuper = false,
                RawData = data
            };
        }
    }
}
