using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Barocert.Navercert
{
    [DataContract]
    public class IdentityStatus
    {
        [DataMember]
        public string receiptID;
        [DataMember]
        public string clientCode;
        [DataMember]
        public int state;
        [Obsolete]
        public int expireIn;
        [Obsolete]
        public string callCenterName;
        [Obsolete]
        public string callCenterNum;
        [Obsolete]
        public string returnURL;
        [DataMember]
        public string expireDT;
        [Obsolete]
        public string scheme;
        [Obsolete]
        public string deviceOSType;
        [Obsolete]
        public bool appUseYN;
    }
}
