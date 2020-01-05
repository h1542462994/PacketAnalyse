using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PacketAnalyse.Annotations;

namespace PacketAnalyse.Core
{
    [Obsolete]
    public class IPSelector: INotifyPropertyChanged
    {
        private int _selectedIndex = 0;
        private IPAddress _selectedIPAddr;
        public IPAddress[] Addresses { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public IPAddress SelectedIPAddr { 
            get {
                return _selectedIPAddr;
            }
            set {
                _selectedIPAddr = value;
            } 
        }
        public IPSelector(int selectedIndex, IPAddress[] addresses)
        {
            this.Addresses = addresses;
            this.SelectedIndex = selectedIndex;
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {  
                _selectedIndex = value;
                _selectedIPAddr = Addresses[_selectedIndex];
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIndex)));
            }
        }

    }

    public static class NetworkHelper
    {

        /// <summary>
        /// 获取本机所有网卡的IPv4地址
        /// </summary>
        public static IEnumerable<IPAddress> GetIpv4s()
        {
            try
            {
                string hostName = Dns.GetHostName(); //得到主机名
                IPHostEntry ipEntry = Dns.GetHostEntry(hostName);
                var list = from item in ipEntry.AddressList
                    where item.AddressFamily == AddressFamily.InterNetwork
                    select item;
                return list;
            } 
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 创建一个绑定到iPAddress的Socket并设置为接收所有IPv4数据包
        /// </summary>
        public static Socket CreateSocket(this IPAddress iPAddress)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP) { Blocking = true };
            socket.Bind(new IPEndPoint(iPAddress, 0));
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, 1);
            byte[] inOption = { 1, 0, 0, 0 };
            byte[] outOption = { 0, 0, 0, 0 };
            //启用在网络上的所有 IPv4 数据包的都接收。 套接字必须具有地址族 System.Net.Sockets.AddressFamily.InterNetwork,
            //，套接字类型必须为 System.Net.Sockets.SocketType.Raw, ，并且协议类型必须为 System.Net.Sockets.ProtocolType.IP。
            //当前用户必须属于本地计算机上 Administrators 组和套接字必须绑定到特定端口。 在 Windows 2000 和更高版本操作系统上支持此控制代码。
            //此值等于 Winsock 2 SIO_RCVALL 常量。
            socket.IOControl(IOControlCode.ReceiveAll, inOption, outOption);
            return socket;
        }

        /// <summary>
        /// 判断是否为局域网IP
        /// </summary>
        /// <param name="iPAddress">要判断的IP地址</param>
        /// <returns>返回判断结果</returns>
        public static bool IsInnerIP(this IPAddress iPAddress)
        {
            byte[] b = iPAddress.GetAddressBytes();
            if (b[0] == 192 && b[1] == 168 )
            {
                return true;
            }
            else if (b[0] == 10)
            {
                return true;
            }
            else if (b[0] == 172 && (b[1] >= 16 && b[1] < 32))
            {
                return true;
            }
            return false;
        }
    }
}
