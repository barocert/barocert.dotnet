using System;
using System.Text;
using System.Runtime.Serialization;

namespace Barocert.Passcert
{
    [DataContract]
    public class IdentityReceipt
    {
        [DataMember]
        public String receiptID;
        [DataMember]
        public String scheme;
        [DataMember]
        public String marketUrl;
    }

}
