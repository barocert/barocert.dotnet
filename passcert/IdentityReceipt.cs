using System;
using System.Text;
using System.Runtime.Serialization;

namespace Barocert.Passcert
{
    [DataContract]
    public class IdentityReceipt
    {
        [DataMember]
        public string receiptID;
        [DataMember]
        public string scheme;
        [DataMember]
        public string marketUrl;
    }

}
