using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Barocert.Kakaocert
{
    [DataContract]
    public class CMSStatus
    {
        [DataMember]
        public string receiptID;
        [DataMember]
        public string clientCode;
        [DataMember]
        public int state;
        [DataMember]
        public int expireIn;
        [DataMember]
        public string callCenterName;
        [DataMember]
        public string callCenterNum;
        [DataMember]
        public string reqTitle;
        [DataMember]
        public string authCategory;
        [DataMember]
        public string returnURL;
        [DataMember]
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
        [DataMember]
        public string scheme;
        [DataMember]
        public bool appUseYN;
    }
}
