
using System;
using System.Text;
using System.Runtime.Serialization;

namespace Barocert.Navercert
{
    [DataContract]
    public class SignResult
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
        public string receiverDay;
        [DataMember]
        public string receiverYear;
        [DataMember]
        public string receiverHP;
        [DataMember]
        public string receiverGender;
        [DataMember]
        public string receiverEmail;
        [DataMember]
        public string receiverForeign;
    }
}
