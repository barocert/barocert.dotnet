
using System;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Kakaocert
{
    [DataContract]
    public class MultiSignResult
    {
        [DataMember]
        public String receiptId;
        [DataMember]
        public String state;
        [DataMember]
        public List<String> multiSignedData;
        [DataMember]
        public String ci;
    }
}
