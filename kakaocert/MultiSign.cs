using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Barocert.Kakaocert
{
    [DataContract]
    public class MultiSign
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
        public List<MultiSignTokens> tokens = new List<MultiSignTokens>();
        [DataMember]
        public String tokenType;
        [DataMember]
        public String returnURL;
        [DataMember]
        public bool appUseYN;

        public void addToken(MultiSignTokens token)
        {
            tokens.Add(token);
        }
    }
}
