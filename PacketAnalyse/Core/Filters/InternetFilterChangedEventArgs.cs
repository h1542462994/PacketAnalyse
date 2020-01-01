using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketAnalyse.Core.Filters
{
    public class InternetFilterChangedEventArgs
    {
        public InternetFilterChangedEventArgs(IInternetFilter filter)
        {
            Filter = filter;
        }

        public IInternetFilter Filter { get; private set; }
    }

    public delegate void InternetFilterChangedEventHandler(object sender, InternetFilterChangedEventArgs e); 
}
