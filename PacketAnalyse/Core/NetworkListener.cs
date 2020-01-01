using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PacketAnalyse.Core
{
    public class NetworkListener : IDisposable
    {
        public NetworkListener()
        {

        }
        public NetworkListener(IPAddress iPAddress)
        {
            this.iPAddress = iPAddress;
        }

        private byte[] buffer = new byte[65536];

        public event InternetDataReceivedEventHandler<IPDatagram> OnInternetDataReceived;
        private Socket Socket { get; set; }
        public bool IsRunning { get; private set; } = false;
        private IPAddress iPAddress;
        public IPAddress IPAddress { get => iPAddress;
            set
            {
                if (IsRunning)
                {
                    throw new InvalidOperationException("监听器正在执行时不可更改Ip地址");
                }
                iPAddress = value;
            }
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            IsRunning = false;
            if (Socket!= null)
            {
                try
                {
                    Socket.Shutdown(SocketShutdown.Both);
                    Socket.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public void Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                Socket = IPAddress.CreateSocket();
                Task.Run(() => DoTask());
            } 
            else
            {
                throw new InvalidOperationException("监听器已开始运行");
            }
               
        }

        private void DoTask()
        {
            while (true)
            {
                try
                {
                    int length = Socket.Receive(buffer);
                    ArraySegment<byte> data = new ArraySegment<byte>(buffer, 0, length);
                    IPDatagram datagram = IPDatagram.Parse(data.ToArray());
                    OnInternetDataReceived?.Invoke(this, new InternetDataReceivedEventArgs<IPDatagram>(datagram));
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex);
                    IsRunning = false;
                    break;
                }
            }
        }
    }
    public class NetworkListenerGroup
    {
        public Dictionary<IPAddress,NetworkListener> current = new Dictionary<IPAddress, NetworkListener>();
        public IEnumerable<IPAddress> Current => current.Keys;


        public event InternetDataReceivedEventHandler<IPDatagram> OnInternetDataReceived;
    
        public void Start()
        {
            Stop();
            foreach (var item in NetworkHelper.GetIpv4s())
            {
                NetworkListener listener = new NetworkListener(item);
                listener.OnInternetDataReceived += HandleInternetData;
                listener.Start();
                current.Add(item, listener);
            }
        }

        public void Stop()
        {
            foreach (var item in current)
            {
                item.Value.OnInternetDataReceived -= HandleInternetData;
                item.Value.Close();
            }
            current.Clear();
        }

        private void HandleInternetData(object sender, InternetDataReceivedEventArgs<IPDatagram> e)
        {
            OnInternetDataReceived?.Invoke(this, e);
        }
    }
}
