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

        public static Socket CreateSocket(this IPAddress iPAddress)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP) { Blocking = true };
            socket.Bind(new IPEndPoint(iPAddress, 0));
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, 1);
            byte[] inOption = { 1, 0, 0, 0 };
            byte[] outOption = { 0, 0, 0, 0 };
            socket.IOControl(IOControlCode.ReceiveAll, inOption, outOption);
            return socket;
        }
    }
}
