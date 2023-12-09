using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Barocert.Passcert
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
        public string reqMessage;
        [DataMember]
        public string requestDT;
        [DataMember]
        public string completeDT;
        [DataMember]
        public string expireDT;
        [DataMember]
        public string rejectDT;
        [Obsolete]
        public string tokenType;
        [Obsolete]
        public bool userAgreementYN;
        [Obsolete]
        public bool receiverInfoYN;
        [Obsolete]
        public string telcoType;
        [Obsolete]
        public string deviceOSType;
        [Obsolete]
        public string originalTypeCode;
        [Obsolete]
        public string originalURL;
        [Obsolete]
        public string originalFormatCode;
        [Obsolete]
        public string scheme;
        [Obsolete]
        public bool appUseYN;
    }
}
