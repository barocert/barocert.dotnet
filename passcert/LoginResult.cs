﻿
using System;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Passcert
{
    [DataContract]
    public class LoginResult
    {
        [DataMember]
        public String receiptID;
        [DataMember]
        public int state;
        [DataMember]
        public String receiverName;
        [DataMember]
        public String receiverDay;
        [DataMember]
        public String receiverYear;
        [DataMember]
        public String receiverGender;
        [DataMember]
        public String receiverForeign;
        [DataMember]
        public String receiverTelcoType;
        [DataMember]
        public String signedData;
        [DataMember]
        public String ci;
    }
}
