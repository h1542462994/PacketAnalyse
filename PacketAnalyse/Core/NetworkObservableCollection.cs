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
        public Filters.InternetFilters Filters { get; private set; } = new Filters.InternetFilters();
        public ObservableCollection<IPDatagramScope> Scopes { get; private set; } = new ObservableCollection<IPDatagramScope>();

        public NetworkObservableCollection(NetworkListenerGroup group, Dispatcher dispatcher)
        {
            this.group = group;
            this.dispatcher = dispatcher;
            group.OnInternetDataReceived += Group_OnInternetDataReceived;
            this.Filters.InternetFilterChanged += Filters_InternetFilterChanged;
        }

        private void Filters_InternetFilterChanged(object sender, Filters.InternetFilterChangedEventArgs e)
        {
            this.Scopes.Clear();
            foreach (var item in this)
            {
                if (CheckItem(item))
                {
                    this.Scopes.Add(item.Scope);
                }
            }
        }

        protected override void ClearItems()
        {
            Scopes.Clear();
            base.ClearItems();
        }

        private void AddItem(IPDatagram datagram)
        {
            dispatcher.Invoke(() =>
            {
                this.Add(datagram);
                if (CheckItem(datagram))
                {
                    this.Scopes.Add(datagram.Scope);
                }
            });
        }

        private bool CheckItem(IPDatagram datagram)
        {
            if (Filters.TypeFilter.InternetType == Core.Filters.InternetType.Inner && !datagram.Header.IsInnerDatagram())
            {
                return false;
            }
            else if (Filters.TypeFilter.InternetType == Core.Filters.InternetType.Outer && datagram.Header.IsInnerDatagram())
            {
                return false;
            }
            else if (!datagram.IsType(Filters.ProtocalFilter.Type))
            {
                return false;
            }
            else if (Filters.LocalIPFilter.BanedAddresses != null)
            {
                foreach (var item in Filters.LocalIPFilter.BanedAddresses)
                {
                    if (datagram.Header.IsOneAddressOf(item))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void Group_OnInternetDataReceived(object sender, InternetDataReceivedEventArgs<IPDatagram> e)
        {
            AddItem(e.Data);
        }
    }
}
