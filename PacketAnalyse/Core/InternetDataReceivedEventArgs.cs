using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketAnalyse.Core
{
    public class InternetDataReceivedEventArgs<T>: EventArgs where T:IInternetData
    {
        public InternetDataReceivedEventArgs(T data)
        {
            Data = data;
        }
        public T Data { get; private set; }
    }

    public delegate void InternetDataReceivedEventHandler<T>(object sender, InternetDataReceivedEventArgs<T> e) where T: IInternetData;


}
