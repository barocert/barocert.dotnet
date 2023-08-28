using System;
using System.Text;
using System.Runtime.Serialization;

namespace Passcert
{
    [DataContract]
    public class CMSResult
    {
        [DataMember]
        public String receiptID;
        [DataMember]
        public int state;
        [DataMember]
        public String receiverHP;
        [DataMember]
        public String receiverName;
        [DataMember]
        public String receiverDay;
        [DataMember]
        public String receiverYear;
        [DataMember]
        public String receiverGender;
        [DataMember]
        public String receiverForeign;
        [DataMember]
        public String receiverTelcoType;
        [DataMember]
        public String signedData;
        [DataMember]
        public String ci;
    }
}
