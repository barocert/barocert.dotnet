using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Kakaocert
{

    [DataContract]
    public class Identity
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
        public String returnURL;
        [DataMember]
        public bool appUseYN;
    }
}
