using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PacketAnalyse.Core
{
    /// <summary>
    /// 网络监听器
    /// </summary>
    public class NetworkListener
    {
        private readonly Socket socket;
        private readonly byte[] buffer;
        private Task task;
        private CancellationTokenSource tokenSource;
        /// <summary>
        /// 取消任务执行的token
        /// </summary>
        private CancellationToken token;
        /// <summary>
        /// 通知一个或多个等待的线程已发生事件（即信号器）
        /// </summary>
        private ManualResetEvent resetEvent = new ManualResetEvent(false);

        public event InternetDataReceivedEventHander<IPDatagram> InternetDataReceived;


        public NetworkListener(IPAddress ip)
        {
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
            task = new Task(() => Do(), token);
            buffer = new byte[65536];
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP) { Blocking = true };
            socket.Bind(new IPEndPoint(ip, 0));
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, 1);
            byte[] In = { 1, 0, 0, 0 };
            byte[] Out = { 0, 0, 0, 0 };
            socket.IOControl(IOControlCode.ReceiveAll, In, Out);
            task.Start();
        }

        ~NetworkListener()
        {
            Close();
        }

        private void Do()
        {
            while (true)
            {
                //取消的代码
                if (token.IsCancellationRequested)
                {
                    return;
                }
                //阻塞等待的关键代码。
                resetEvent.WaitOne();

                int length = socket.Receive(buffer);
                ArraySegment<byte> data = new ArraySegment<byte>(buffer, 0, length);
                IPDatagram iPDatagram = IPDatagram.Parse(data.ToArray());
                InternetDataReceived?.Invoke(this, new InternetDataReceivedEventArgs<IPDatagram>(iPDatagram));

                //Console.WriteLine("Hello world");

                //await Task.Delay(100);
            }
        }

        private void Close()
        {
            if (socket != null)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch (Exception)
                {
                    Console.WriteLine("关闭socket错误！");
                }
            }
        }

        public void Pause()
        {
            resetEvent.Reset();
        }

        public void Contiune()
        {
            resetEvent.Set();
        }
    }
}
