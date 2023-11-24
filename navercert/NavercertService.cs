using System;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Net;
using Linkhub;
using Barocert;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;

using Linkhub.BouncyCastle.Crypto;
using Linkhub.BouncyCastle.Crypto.Modes;
using Linkhub.BouncyCastle.Crypto.Engines;
using Linkhub.BouncyCastle.Crypto.Parameters;
using Linkhub.BouncyCastle.Security;

namespace Barocert.Navercert
{
    public class NavercertService : BaseService
    {
        public NavercertService(string LinkID, string SecretKey)
            : base(LinkID, SecretKey)
        {
            this.AddScope("421");
            this.AddScope("422");
            this.AddScope("423");
        }
        public IdentityReceipt requestIdentity(string ClientCode, Identity identity)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (null == identity) throw new BarocertException(-99999999, "본인인증 요청정보가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(identity.receiverHP)) throw new BarocertException(-99999999, "수신자 휴대폰번호가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(identity.receiverName)) throw new BarocertException(-99999999, "수신자 성명이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(identity.receiverBirthday)) throw new BarocertException(-99999999, "생년월일이 입력되지 않았습니다.");
            if (null == identity.expireIn) throw new BarocertException(-99999999, "만료시간이 입력되지 않았습니다.");
            
            string PostData = toJsonString(identity);
            return httppost<IdentityReceipt>("/NAVER/Identity/" + ClientCode, PostData);
        }

        public IdentityStatus getIdentityStatus(string ClientCode, string ReceiptId)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (string.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httpget<IdentityStatus>("/NAVER/Identity/" + ClientCode + "/" + ReceiptId);
        }

        public IdentityResult verifyIdentity(string ClientCode, string ReceiptId)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (string.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");
            
            return httppost<IdentityResult>("/NAVER/Identity/" + ClientCode + "/" + ReceiptId);
        }

        public SignReceipt requestSign(string ClientCode, Sign sign)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (null == sign) throw new BarocertException(-99999999, "전자서명 요청정보가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(sign.receiverHP)) throw new BarocertException(-99999999, "수신자 휴대폰번호가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(sign.receiverName)) throw new BarocertException(-99999999, "수신자 성명이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(sign.receiverBirthday)) throw new BarocertException(-99999999, "생년월일이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(sign.reqTitle)) throw new BarocertException(-99999999, "인증요청 메시지 제목이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(sign.callCenterNum)) throw new BarocertException(-99999999, "고객센터 연락처가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(sign.reqMessage)) throw new BarocertException(-99999999, "인증요청 메시지가 입력되지 않았습니다.");
            if (null == sign.expireIn) throw new BarocertException(-99999999, "만료시간이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(sign.token)) throw new BarocertException(-99999999, "토큰 원문이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(sign.tokenType)) throw new BarocertException(-99999999, "원문 유형이 입력되지 않았습니다.");

            string PostData = toJsonString(sign);

            return httppost<SignReceipt>("/NAVER/Sign/" + ClientCode, PostData);
        }

        public SignStatus getSignStatus(string ClientCode, string ReceiptId)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (string.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httpget<SignStatus>("/NAVER/Sign/" + ClientCode + "/" + ReceiptId);
        }

        public SignResult verifySign(string ClientCode, string ReceiptId)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (string.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");
            
            return httppost<SignResult>("/NAVER/Sign/" + ClientCode + "/" + ReceiptId);
        }

        public MultiSignReceipt requestMultiSign(string ClientCode, MultiSign multiSign)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (null == multiSign) throw new BarocertException(-99999999, "전자서명 요청정보가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(multiSign.receiverHP)) throw new BarocertException(-99999999, "수신자 휴대폰번호가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(multiSign.receiverName)) throw new BarocertException(-99999999, "수신자 성명이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(multiSign.receiverBirthday)) throw new BarocertException(-99999999, "생년월일이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(multiSign.reqTitle)) throw new BarocertException(-99999999, "인증요청 메시지 제목이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(multiSign.callCenterNum)) throw new BarocertException(-99999999, "고객센터 연락처가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(multiSign.reqMessage)) throw new BarocertException(-99999999, "인증요청 메시지가 입력되지 않았습니다.");
            if (null == multiSign.expireIn) throw new BarocertException(-99999999, "만료시간이 입력되지 않았습니다.");
            if (isNullorEmptyToken(multiSign.tokens)) throw new BarocertException(-99999999, "토큰 원문이 입력되지 않았습니다.");
            if (isNullorEmptyTokenType(multiSign.tokens)) throw new BarocertException(-99999999, "원문 유형이 입력되지 않았습니다.");
            
            string PostData = toJsonString(multiSign);

            return httppost<MultiSignReceipt>("/NAVER/MultiSign/" + ClientCode, PostData);
        }


        public MultiSignStatus getMultiSignStatus(string ClientCode, string ReceiptId)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (string.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httpget<MultiSignStatus>("/NAVER/MultiSign/" + ClientCode + "/" + ReceiptId);
        }


        public MultiSignResult verifyMultiSign(string ClientCode, string ReceiptId)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (string.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httppost<MultiSignResult>("/NAVER/MultiSign/" + ClientCode + "/" + ReceiptId);
        }

    }
}
