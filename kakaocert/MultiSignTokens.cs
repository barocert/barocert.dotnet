using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Barocert.Kakaocert
{
    [DataContract]
    public class MultiSignTokens
    {
        [DataMember]
        public String reqTitle;
        [DataMember]
        public String token;
    }
}
