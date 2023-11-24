using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Barocert.Passcert
{
    [DataContract]
    public class Identity
    {
        [DataMember]
        public string receiverHP;
        [DataMember]
        public string receiverName;
        [DataMember]
        public string receiverBirthday;
        [DataMember]
        public string reqTitle;
        [DataMember]
        public string reqMessage;
        [DataMember]
        public string callCenterNum;
        [DataMember]
        public int? expireIn;
        [DataMember]
        public string token;
        [DataMember]
        public bool userAgreementYN;
        [DataMember]
        public bool receiverInfoYN;
        [DataMember]
        public string telcoType;
        [DataMember]
        public string deviceOSType;
        [DataMember]
        public bool appUseYN;
        [DataMember]
        public bool useTssYN;
    }
}
