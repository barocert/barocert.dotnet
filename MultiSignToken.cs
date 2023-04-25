using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Kakaocert
{
    [DataContract]
    public class MultiSignToken
    {
        [DataMember]
        public String reqTitle;
        [DataMember]
        public String token;
    }
}
