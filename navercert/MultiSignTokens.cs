using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Barocert.Navercert
{
    [DataContract]
    public class MultiSignTokens
    {
        [DataMember]
        public string token;
        [DataMember]
        public string tokenType;
    }
}
