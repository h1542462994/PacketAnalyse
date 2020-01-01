using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace PacketAnalyse.Core
{
    /// <summary>
    /// 网络监听器
    /// </summary>
    public class NetworkListener
    {
        private MagicSocket magicSocket;
        private readonly byte[] buffer = new byte[65536];
        private IPAddress _bindAddress;
        public IPAddress BindAddress
        {
            get => _bindAddress;
            set
            {
                _bindAddress = value;
            }
        }

        /// <summary>
        /// 通知一个或多个等待的线程已发生事件（即信号器）
        /// </summary>
        private readonly ManualResetEvent resetEvent = new ManualResetEvent(false);

        public event InternetDataReceivedEventHandler<IPDatagram> InternetDataReceived;

        private static Socket genSocket(IPAddress ipAddr)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP) { Blocking = true };
            socket.Bind(new IPEndPoint(ipAddr, 0));
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, 1);
            byte[] In = { 1, 0, 0, 0 };
            byte[] Out = { 0, 0, 0, 0 };
            socket.IOControl(IOControlCode.ReceiveAll, In, Out);
            return socket;
        }

        public NetworkListener(){ }

        ~NetworkListener()
        {
            Close();
        }

        private void Do(MagicSocket magicSocket)
        {
            while (true)
            {
                try
                {
                    if (!magicSocket.status)
                    {
                        Close();
                        return;
                    }
                    int length = magicSocket.socket.Receive(buffer);
                    ArraySegment<byte> data = new ArraySegment<byte>(buffer, 0, length);
                    IPDatagram iPDataGram = IPDatagram.Parse(data.ToArray());
                    InternetDataReceived?.Invoke(this, new InternetDataReceivedEventArgs<IPDatagram>(iPDataGram));

                } catch (Exception e){
                    Console.WriteLine(e);
                }

            }
        }

        private void Close()
        {
            if (magicSocket != null)
            {
                try
                {
                    magicSocket.socket.Shutdown(SocketShutdown.Both);
                    magicSocket.socket.Close();
                }
                catch (Exception)
                {
                    Console.WriteLine(@"关闭socket错误！");
                }
            }
        }

        public void Pause()
        {
            magicSocket.status = false;
        }

        public void Start()
        {
            magicSocket = new MagicSocket(genSocket(_bindAddress));
            Task.Run(() => Do(magicSocket));
        }
    }

    public class MagicSocket
    {
        public MagicSocket(Socket socket)
        {
            this.socket = socket;
        }

        public bool status = true;
        public Socket socket;
    }
}
