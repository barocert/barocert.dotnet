﻿using System;
using System.Text;
using System.Runtime.Serialization;

namespace Barocert.Navercert
{
    [DataContract]
    public class SignReceipt
    {
        [DataMember]
        public String receiptID;
        [DataMember]
        public String scheme;
        [DataMember]
        public String marketUrl;
    }

}