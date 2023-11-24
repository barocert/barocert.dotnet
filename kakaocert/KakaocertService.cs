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

namespace Barocert.Kakaocert
{
    public class KakaocertService : BaseService
    {

        public KakaocertService(string LinkID, string SecretKey)
            : base(LinkID, SecretKey)
        {
            this.AddScope("401");
            this.AddScope("402");
            this.AddScope("403");
            this.AddScope("404");
            this.AddScope("405");
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
            if (string.IsNullOrEmpty(identity.reqTitle)) throw new BarocertException(-99999999, "인증요청 메시지 제목이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(identity.token)) throw new BarocertException(-99999999, "토큰 원문이 입력되지 않았습니다.");

            string PostData = toJsonString(identity);

            return httppost<IdentityReceipt>("/KAKAO/Identity/" + ClientCode, PostData);
        }

        public IdentityStatus getIdentityStatus(string ClientCode, string ReceiptId)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (string.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httpget<IdentityStatus>("/KAKAO/Identity/" + ClientCode + "/" + ReceiptId);
        }

        public IdentityResult verifyIdentity(string ClientCode, string ReceiptId)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (string.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httppost<IdentityResult>("/KAKAO/Identity/" + ClientCode + "/" + ReceiptId);
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
            if (null == sign.expireIn) throw new BarocertException(-99999999, "만료시간이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(sign.signTitle))
                if (string.IsNullOrEmpty(sign.reqTitle)) 
                    throw new BarocertException(-99999999, "서명 요청 제목이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(sign.token)) throw new BarocertException(-99999999, "토큰 원문이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(sign.tokenType)) throw new BarocertException(-99999999, "원문 유형이 입력되지 않았습니다.");

            string PostData = toJsonString(sign);

            return httppost<SignReceipt>("/KAKAO/Sign/" + ClientCode, PostData);
        }

        public SignStatus getSignStatus(string ClientCode, string ReceiptId)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (string.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httpget<SignStatus>("/KAKAO/Sign/" + ClientCode + "/" + ReceiptId);
        }

        public SignResult verifySign(string ClientCode, string ReceiptId)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (string.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httppost<SignResult>("/KAKAO/Sign/" + ClientCode + "/" + ReceiptId);
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
            if (null == multiSign.expireIn) throw new BarocertException(-99999999, "만료시간이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(multiSign.reqTitle)) throw new BarocertException(-99999999, "인증요청 메시지 제목이 입력되지 않았습니다.");
            if (isNullorEmptyTitle(multiSign.tokens)) throw new BarocertException(-99999999, "서명 요청 제목이 입력되지 않았습니다.");
            if (isNullorEmptyToken(multiSign.tokens)) throw new BarocertException(-99999999, "토큰 원문이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(multiSign.tokenType)) throw new BarocertException(-99999999, "원문 유형이 입력되지 않았습니다.");

            string PostData = toJsonString(multiSign);

            return httppost<MultiSignReceipt>("/KAKAO/MultiSign/" + ClientCode, PostData);
        }


        public MultiSignStatus getMultiSignStatus(string ClientCode, string ReceiptId)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (string.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httpget<MultiSignStatus>("/KAKAO/MultiSign/" + ClientCode + "/" + ReceiptId);
        }


        public MultiSignResult verifyMultiSign(string ClientCode, string ReceiptId)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (string.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httppost<MultiSignResult>("/KAKAO/MultiSign/" + ClientCode + "/" + ReceiptId);
        }

        public CMSReceipt requestCMS(string ClientCode, CMS cms)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (null == cms) throw new BarocertException(-99999999, "자동이체 출금동의 요청정보가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(cms.receiverHP)) throw new BarocertException(-99999999, "수신자 휴대폰번호가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(cms.receiverName)) throw new BarocertException(-99999999, "수신자 성명이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(cms.receiverBirthday)) throw new BarocertException(-99999999, "생년월일이 입력되지 않았습니다.");
            if (null == cms.expireIn) throw new BarocertException(-99999999, "만료시간이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(cms.reqTitle)) throw new BarocertException(-99999999, "인증요청 메시지 제목이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(cms.requestCorp)) throw new BarocertException(-99999999, "청구기관명이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(cms.bankName)) throw new BarocertException(-99999999, "은행명이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(cms.bankAccountNum)) throw new BarocertException(-99999999, "계좌번호가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(cms.bankAccountName)) throw new BarocertException(-99999999, "예금주명이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(cms.bankAccountBirthday)) throw new BarocertException(-99999999, "예금주 생년월일이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(cms.bankServiceType)) throw new BarocertException(-99999999, "출금 유형이 입력되지 않았습니다.");

            string PostData = toJsonString(cms);

            return httppost<CMSReceipt>("/KAKAO/CMS/" + ClientCode, PostData);
        }

        public CMSStatus getCMSStatus(string ClientCode, string ReceiptId)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (string.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httpget<CMSStatus>("/KAKAO/CMS/" + ClientCode + "/" + ReceiptId);
        }

        public CMSResult verifyCMS(string ClientCode, string ReceiptId)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (string.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httppost<CMSResult>("/KAKAO/CMS/" + ClientCode + "/" + ReceiptId);
        }

        public LoginResult verifyLogin(string ClientCode, string txID)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (string.IsNullOrEmpty(txID)) throw new BarocertException(-99999999, "트랜잭션 아이디가 입력되지 않았습니다.");

            return httppost<LoginResult>("/KAKAO/Login/" + ClientCode + "/" + txID);
        }

    }
}
