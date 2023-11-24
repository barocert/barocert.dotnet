using System;
using System.Text;
using System.Runtime.Serialization;

namespace Barocert.Passcert
{
    [DataContract]
    public class CMSResult
    {
        [DataMember]
        public string receiptID;
        [DataMember]
        public int state;
        [DataMember]
        public string receiverHP;
        [DataMember]
        public string receiverName;
        [DataMember]
        public string receiverDay;
        [DataMember]
        public string receiverYear;
        [DataMember]
        public string receiverGender;
        [DataMember]
        public string receiverForeign;
        [DataMember]
        public string receiverTelcoType;
        [DataMember]
        public string signedData;
        [DataMember]
        public string ci;
    }
}
