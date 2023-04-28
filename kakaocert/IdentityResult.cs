
using System;
using System.Text;
using System.Runtime.Serialization;

namespace Kakaocert
{
    [DataContract]
    public class IdentityResult
    {
        [DataMember]
        public String receiptID;
        [DataMember]
        public String state;
        [DataMember]
        public String signedData;
        [DataMember]
        public String ci;
    }
}
