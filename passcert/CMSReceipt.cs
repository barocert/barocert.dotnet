﻿using System;
using System.Text;
using System.Runtime.Serialization;


namespace Passcert
{
    [DataContract]
    public class CMSReceipt
    {
        [DataMember]
        public String receiptId;
        [DataMember]
        public String scheme;
        [DataMember]
        public String marketUrl;
    }

}