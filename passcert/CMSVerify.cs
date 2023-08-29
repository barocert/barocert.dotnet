using System;
using System.Text;
using System.Runtime.Serialization;

namespace Passcert
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
