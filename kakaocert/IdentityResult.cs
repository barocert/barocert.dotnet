
using System;
using System.Text;
using System.Runtime.Serialization;

namespace Barocert.Kakaocert
{
    [DataContract]
    public class IdentityResult
    {
        [DataMember]
        public string receiptID;
        [DataMember]
        public int state;
        [DataMember]
        public string signedData;
        [DataMember]
        public string ci;
        [DataMember]
        public string receiverName;
        [DataMember]
        public string receiverYear;
        [DataMember]
        public string receiverDay;
        [DataMember]
        public string receiverHP;
        [DataMember]
        public string receiverGender;
    }
}
