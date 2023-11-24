using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Barocert.Passcert
{

    [DataContract]
    public class SignVerify
    {
        [DataMember]
        public string receiverHP;
        [DataMember]
        public string receiverName;

    }
}
