using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Barocert.Navercert
{
    [DataContract]
    public class CMS
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
        public string requestCorp;
        [DataMember]
        public string bankName;
        [DataMember]
        public string bankAccountNum;
        [DataMember]
        public string bankAccountName;
        [DataMember]
        public string bankAccountBirthday;
        [DataMember]
        public string returnURL;
        [DataMember]
        public string deviceOSType;
        [DataMember]
        public bool appUseYN;
    }
}
