using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Barocert.Navercert
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
        public string callCenterNum;
        [DataMember]
        public int? expireIn;
        [DataMember]
        public string returnURL;
        [DataMember]
        public string deviceOSType;
        [DataMember]
        public bool appUseYN;
    }
}
