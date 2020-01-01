using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketAnalyse.Core
{
    class NoAnalyseDatagram : IInternetData
    {
        public NoAnalyseDatagram(byte[] rawData, ProtocalType protocalType = ProtocalType._)
        {
            this.rawData = rawData;
            this.protocalType = protocalType;
        }

        private ProtocalType protocalType;

        private byte[] rawData;

        public ProtocalType ProtocalType => protocalType;

        public IInternetData Super => null;

        public bool HasSuper => false;

        public byte[] RawData => rawData;
    }
}
