﻿using System;
using System.Text;
using System.Runtime.Serialization;

namespace Barocert.Navercert
{
    [DataContract]
    public class CMSReceipt
    {
        [DataMember]
        public string receiptID;
        [DataMember]
        public string scheme;
        [DataMember]
        public string marketUrl;
    }

}
