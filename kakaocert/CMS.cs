using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Barocert.Kakaocert
{
    [DataContract]
    public class CMS
    {
        [DataMember]
        public String receiverHP;
        [DataMember]
        public String receiverName;
        [DataMember]
        public String receiverBirthday;
        [DataMember]
        public String ci;
        [DataMember]
        public String reqTitle;
        [DataMember]
        public int? expireIn;
        [DataMember]
        public String returnURL;
        [DataMember]
        public String requestCorp;
        [DataMember]
        public String bankName;
        [DataMember]
        public String bankAccountNum;
        [DataMember]
        public String bankAccountName;
        [DataMember]
        public String bankAccountBirthday;
        [DataMember]
        public String bankServiceType;
        [DataMember]
        public bool appUseYN;

    }
}
