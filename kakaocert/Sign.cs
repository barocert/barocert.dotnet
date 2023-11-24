using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Barocert.Kakaocert
{
    [DataContract]
    public class Sign
    {
        [DataMember]
        public string receiverHP;
        [DataMember]
        public string receiverName;
        [DataMember]
        public string receiverBirthday;
        [DataMember]
        public string ci;
        [DataMember]
        public string reqTitle;
        [DataMember]
        public string signTitle;
        [DataMember]
        public string extraMessage;
        [DataMember]
        public int? expireIn;
        [DataMember]
        public string token;
        [DataMember]
        public string tokenType;
        [DataMember]
        public string returnURL;
        [DataMember]
        public bool appUseYN;
    }
}
