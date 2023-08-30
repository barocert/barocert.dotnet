using System;
using System.Text;
using System.Runtime.Serialization;

namespace Barocert.Passcert
{
    [DataContract]
    public class SignReceipt
    {
        [DataMember]
        public String receiptId;
        [DataMember]
        public String scheme;
        [DataMember]
        public String marketUrl;
    }

}
