﻿using System;
using System.Text;
using System.Runtime.Serialization;

namespace Barocert.Kakaocert
{
    [DataContract]
    public class MultiSignReceipt
    {
        [DataMember]
        public string receiptID;
        [DataMember]
        public string scheme;
    }

}
