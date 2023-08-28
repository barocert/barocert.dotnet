using System;
using System.Text;
using System.Runtime.Serialization;


namespace Passcert
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
