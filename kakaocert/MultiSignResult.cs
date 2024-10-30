
using System;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Barocert.Kakaocert
{
    [DataContract]
    public class MultiSignResult
    {
        [DataMember]
        public string receiptID;
        [DataMember]
        public int state;
        [DataMember]
        public List<string> multiSignedData;
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
