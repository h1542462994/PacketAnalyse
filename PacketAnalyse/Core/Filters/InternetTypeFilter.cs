using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketAnalyse.Core.Filters
{
    public class InternetTypeFilter: IInternetFilter
    {
        public InternetTypeFilter(InternetType internetType)
        {
            InternetType = internetType;
        }

        public InternetType InternetType { get; private set; }
    }
    
    public enum InternetType
    {
        All,
        Inner,
        Outer
    }
    
}
