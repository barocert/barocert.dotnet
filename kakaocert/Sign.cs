using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Barocert.Kakaocert
{
    [DataContract]
    public class Sign
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
        public String token;
        [DataMember]
        public String tokenType;
        [DataMember]
        public String returnURL;
        [DataMember]
        public bool appUseYN;
    }
}
