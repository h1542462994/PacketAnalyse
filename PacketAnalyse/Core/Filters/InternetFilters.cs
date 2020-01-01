using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketAnalyse.Core.Filters
{
    public class InternetFilters
    {
        public InternetFilters()
        {
        }

        private InternetTypeFilter typeFilter = new InternetTypeFilter(InternetType.All);
        private InternetProtocalFilter protocalFilter = new InternetProtocalFilter(ProtocalFilterOption.All);
        private LocalIPFilter localIPFilter = new LocalIPFilter();
        public InternetTypeFilter TypeFilter { get=> typeFilter; set
            {
                typeFilter = value;
                this.InternetFilterChanged?.Invoke(this, new InternetFilterChangedEventArgs(value));
            } 
        }
        public InternetProtocalFilter ProtocalFilter { get => protocalFilter; set
            {
                protocalFilter = value;
                this.InternetFilterChanged?.Invoke(this, new InternetFilterChangedEventArgs(value));
            }
        }
        public LocalIPFilter LocalIPFilter { get => localIPFilter; set
            {
                localIPFilter = value;
                this.InternetFilterChanged?.Invoke(this, new InternetFilterChangedEventArgs(value));
            } 
        }

        public event InternetFilterChangedEventHandler InternetFilterChanged;
    }
}
