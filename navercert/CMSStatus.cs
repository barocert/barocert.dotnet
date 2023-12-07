using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Barocert.Navercert
{
    [DataContract]
    public class CMSStatus
    {
        [DataMember]
        public string receiptID;
        [DataMember]
        public string clientCode;
        [DataMember]
        public int state;
        [DataMember]
        public string expireDT;
    }
}
