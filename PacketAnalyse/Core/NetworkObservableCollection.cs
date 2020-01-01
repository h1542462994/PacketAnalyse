using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace PacketAnalyse.Core
{
    public class NetworkObservableCollection : ObservableCollection<IPDatagram>
    {
        private NetworkListenerGroup group;
        private Dispatcher dispatcher;
        public ObservableCollection<IPDatagramScope> Scopes { get; private set; } = new ObservableCollection<IPDatagramScope>();

        public NetworkObservableCollection(NetworkListenerGroup group, Dispatcher dispatcher)
        {
            this.group = group;
            this.dispatcher = dispatcher;
            group.OnInternetDataReceived += Group_OnInternetDataReceived;
        }

        private void Group_OnInternetDataReceived(object sender, InternetDataReceivedEventArgs<IPDatagram> e)
        {
            dispatcher.Invoke(() =>
            { 
                this.Add(e.Data);
                this.Scopes.Add(e.Data.Scope);
            });
        }
    }
}
