using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Barocert.Kakaocert
{
    [DataContract]
    public class SignStatus
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
        public string reqTitle;
        [Obsolete]
        public string authCategory;
        [Obsolete]
        public string returnURL;
        [Obsolete]
        public string tokenType;
        [DataMember]
        public string requestDT;
        [DataMember]
        public string viewDT;
        [DataMember]
        public string completeDT;
        [DataMember]
        public string expireDT;
        [DataMember]
        public string verifyDT;
        [Obsolete]
        public string scheme;
        [Obsolete]
        public bool appUseYN;
    }
}
