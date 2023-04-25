﻿using System;
using System.Text;
using System.Runtime.Serialization;

namespace Kakaocert
{
    [DataContract]
    public class CMSResult
    {
        [DataMember]
        public String receiptId;
        [DataMember]
        public String state;
        [DataMember]
        public String signedData;
        [DataMember]
        public String ci;
    }
}
