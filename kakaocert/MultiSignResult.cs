
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
        public String receiptID;
        [DataMember]
        public int state;
        [DataMember]
        public List<String> multiSignedData;
        [DataMember]
        public String ci;
    }
}
