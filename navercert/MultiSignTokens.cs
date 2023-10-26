using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Barocert.Navercert
{
    [DataContract]
    public class MultiSignTokens
    {
        [DataMember]
        public String token;
        [DataMember]
        public String tokenType;
    }
}
