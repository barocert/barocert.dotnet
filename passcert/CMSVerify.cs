﻿using System;
using System.Collections.Generic;
using System.Linq;
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
