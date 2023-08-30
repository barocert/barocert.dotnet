using System;
using System.Text;
using System.Runtime.Serialization;

namespace Barocert.Kakaocert
{
    [DataContract]
    public class CMSResult
    {
        [DataMember]
        public String receiptID;
        [DataMember]
        public int state;
        [DataMember]
        public String signedData;
        [DataMember]
        public String ci;
    }
}
