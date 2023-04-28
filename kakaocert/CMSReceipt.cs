using System;
using System.Text;
using System.Runtime.Serialization;


namespace Kakaocert
{
    [DataContract]
    public class CMSReceipt
    {
        [DataMember]
        public String receiptID;
        [DataMember]
        public String scheme;
    }

}
