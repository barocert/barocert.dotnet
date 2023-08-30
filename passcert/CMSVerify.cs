using System;
using System.Text;
using System.Runtime.Serialization;

namespace Barocert.Passcert
{
        [DataContract]
        public class CMSVerify
        {
            [DataMember]
            public String receiverHP;
            [DataMember]
            public String receiverName;
        }
}
