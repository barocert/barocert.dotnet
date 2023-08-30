using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Barocert.Passcert
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
        public String reqTitle;
        [DataMember]
        public String reqMessage;
        [DataMember]
        public String callCenterNum;
        [DataMember]
        public int? expireIn;
        [DataMember]
        public bool userAgreementYN;
        [DataMember]
        public bool receiverInfoYN;
        [DataMember]
        public String bankName;
        [DataMember]
        public String bankAccountNum;
        [DataMember]
        public String bankAccountName;
        [DataMember]
        public String bankWithdraw;
        [DataMember]
        public String bankServiceType;
        [DataMember]
        public String telcoType;
        [DataMember]
        public String deviceOSType;
        [DataMember]
        public bool appUseYN;
        [DataMember]
        public bool useTssYN;
    }
}
