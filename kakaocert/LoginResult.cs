
using System;
using System.Text;
using System.Runtime.Serialization;

namespace Barocert.Kakaocert
{
    [DataContract]
    public class LoginResult
    {
        [DataMember]
        public String txID;
        [DataMember]
        public int state;
        [DataMember]
        public String signedData;
        [DataMember]
        public String ci;
    }
}
