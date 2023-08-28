using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Passcert
{

    [DataContract]
    public class SignStatus
    {
        [DataMember]
        public String receiptID;
        [DataMember]
        public String clientCode;
        [DataMember]
        public int state;
        [DataMember]
        public int expireIn;
        [DataMember]
        public String callCenterName;
        [DataMember]
        public String callCenterNum;
        [DataMember]
        public String reqTitle;
        [DataMember]
        public String reqMessage;
        [DataMember]
        public String requestDT;
        [DataMember]
        public String completeDT;
        [DataMember]
        public String expireDT;
        [DataMember]
        public String rejectDT;
        [DataMember]
        public String tokenType;
        [DataMember]
        public bool userAgreementYN;
        [DataMember]
        public bool receiverInfoYN;
        [DataMember]
        public String telcoType;
        [DataMember]
        public String deviceOSType;
        [DataMember]
        public String originalTypeCode;
        [DataMember]
        public String originalURL;
        [DataMember]
        public String originalFormatCode;
        [DataMember]
        public String scheme;
        [DataMember]
        public bool appUseYN;
    }
}
