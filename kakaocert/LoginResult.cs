﻿
using System;
using System.Text;
using System.Runtime.Serialization;

namespace Barocert.Kakaocert
{
    [DataContract]
    public class LoginResult
    {
        [DataMember]
        public string txID;
        [DataMember]
        public int state;
        [DataMember]
        public string signedData;
        [DataMember]
        public string ci;
        [DataMember]
        public string receiverName;
        [DataMember]
        public string receiverYear;
        [DataMember]
        public string receiverDay;
        [DataMember]
        public string receiverHP;
    }
}
