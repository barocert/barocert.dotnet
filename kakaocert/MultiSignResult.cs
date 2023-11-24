
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
    }
}
