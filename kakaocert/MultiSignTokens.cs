using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Barocert.Kakaocert
{
    [DataContract]
    public class MultiSignTokens
    {
        [DataMember]
        public string reqTitle;
        [DataMember]
        public string signTitle;
        [DataMember]
        public string token;
    }
}
