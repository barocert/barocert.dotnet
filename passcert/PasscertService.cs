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

namespace Barocert.Passcert
{
    public class PasscertService : BaseService
    {
        public PasscertService(string LinkID, string SecretKey)
            : base(LinkID, SecretKey)
        {
            this.AddScope("441");
            this.AddScope("442");
            this.AddScope("443");
            this.AddScope("444");
        }
        public IdentityReceipt requestIdentity(string ClientCode, Identity identity)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (null == identity) throw new BarocertException(-99999999, "본인인증 요청정보가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(identity.receiverHP)) throw new BarocertException(-99999999, "수신자 휴대폰번호가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(identity.receiverName)) throw new BarocertException(-99999999, "수신자 성명이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(identity.reqTitle)) throw new BarocertException(-99999999, "인증요청 메시지 제목이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(identity.callCenterNum)) throw new BarocertException(-99999999, "고객센터 연락처가 입력되지 않았습니다.");
            if (null == identity.expireIn) throw new BarocertException(-99999999, "만료시간이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(identity.token)) throw new BarocertException(-99999999, "토큰 원문이 입력되지 않았습니다.");

            string PostData = toJsonString(identity);

            return httppost<IdentityReceipt>("/PASS/Identity/" + ClientCode, PostData);
        }

        public IdentityStatus getIdentityStatus(string ClientCode, string ReceiptId)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (string.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httpget<IdentityStatus>("/PASS/Identity/" + ClientCode + "/" + ReceiptId);
        }

        public IdentityResult verifyIdentity(string ClientCode, string ReceiptId, IdentityVerify identityVerify)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (string.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");
            if (null == identityVerify) throw new BarocertException(-99999999, "본인인증 검증 요청 정보가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(identityVerify.receiverHP)) throw new BarocertException(-99999999, "수신자 휴대폰번호가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(identityVerify.receiverName)) throw new BarocertException(-99999999, "수신자 성명이 입력되지 않았습니다.");

            string PostData = toJsonString(identityVerify);

            return httppost<IdentityResult>("/PASS/Identity/" + ClientCode + "/" + ReceiptId, PostData);
        }

        public SignReceipt requestSign(string ClientCode, Sign sign)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (null == sign) throw new BarocertException(-99999999, "전자서명 요청정보가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(sign.receiverHP)) throw new BarocertException(-99999999, "수신자 휴대폰번호가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(sign.receiverName)) throw new BarocertException(-99999999, "수신자 성명이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(sign.reqTitle)) throw new BarocertException(-99999999, "인증요청 메시지 제목이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(sign.callCenterNum)) throw new BarocertException(-99999999, "고객센터 연락처가 입력되지 않았습니다.");
            if (null == sign.expireIn) throw new BarocertException(-99999999, "만료시간이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(sign.token)) throw new BarocertException(-99999999, "토큰 원문이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(sign.tokenType)) throw new BarocertException(-99999999, "원문 유형이 입력되지 않았습니다.");

            string PostData = toJsonString(sign);

            return httppost<SignReceipt>("/PASS/Sign/" + ClientCode, PostData);
        }

        public SignStatus getSignStatus(string ClientCode, string ReceiptId)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (string.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httpget<SignStatus>("/PASS/Sign/" + ClientCode + "/" + ReceiptId);
        }

        public SignResult verifySign(string ClientCode, string ReceiptId, SignVerify signVerify)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (string.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");
            if (null == signVerify) throw new BarocertException(-99999999, "전자서명 검증 요청 정보가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(signVerify.receiverHP)) throw new BarocertException(-99999999, "수신자 휴대폰번호가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(signVerify.receiverName)) throw new BarocertException(-99999999, "수신자 성명이 입력되지 않았습니다.");

            string PostData = toJsonString(signVerify);

            return httppost<SignResult>("/PASS/Sign/" + ClientCode + "/" + ReceiptId, PostData);
        }

        public CMSReceipt requestCMS(string ClientCode, CMS cms)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (null == cms) throw new BarocertException(-99999999, "자동이체 출금동의 요청정보가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(cms.receiverHP)) throw new BarocertException(-99999999, "수신자 휴대폰번호가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(cms.receiverName)) throw new BarocertException(-99999999, "수신자 성명이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(cms.reqTitle)) throw new BarocertException(-99999999, "인증요청 메시지 제목이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(cms.callCenterNum)) throw new BarocertException(-99999999, "고객센터 연락처가 입력되지 않았습니다.");
            if (null == cms.expireIn) throw new BarocertException(-99999999, "만료시간이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(cms.bankName)) throw new BarocertException(-99999999, "출금은행명이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(cms.bankAccountNum)) throw new BarocertException(-99999999, "출금계좌번호가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(cms.bankAccountName)) throw new BarocertException(-99999999, "출금계좌 예금주명이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(cms.bankServiceType)) throw new BarocertException(-99999999, "출금 유형이 입력되지 않았습니다.");

            string PostData = toJsonString(cms);

            return httppost<CMSReceipt>("/PASS/CMS/" + ClientCode, PostData);
        }

        public CMSStatus getCMSStatus(string ClientCode, string ReceiptId)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (string.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httpget<CMSStatus>("/PASS/CMS/" + ClientCode + "/" + ReceiptId);
        }

        public CMSResult verifyCMS(string ClientCode, string ReceiptId, CMSVerify cmsVerify)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (string.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");
            if (null == cmsVerify) throw new BarocertException(-99999999, "출금동의 검증 요청 정보가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(cmsVerify.receiverHP)) throw new BarocertException(-99999999, "수신자 휴대폰번호가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(cmsVerify.receiverName)) throw new BarocertException(-99999999, "수신자 성명이 입력되지 않았습니다.");

            string PostData = toJsonString(cmsVerify);

            return httppost<CMSResult>("/PASS/CMS/" + ClientCode + "/" + ReceiptId, PostData);
        }

        public LoginReceipt requestLogin(string ClientCode, Login login)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (null == login) throw new BarocertException(-99999999, "간편로그인 요청정보가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(login.receiverHP)) throw new BarocertException(-99999999, "수신자 휴대폰번호가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(login.receiverName)) throw new BarocertException(-99999999, "수신자 성명이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(login.reqTitle)) throw new BarocertException(-99999999, "인증요청 메시지 제목이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(login.callCenterNum)) throw new BarocertException(-99999999, "고객센터 연락처가 입력되지 않았습니다.");
            if (null == login.expireIn) throw new BarocertException(-99999999, "만료시간이 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(login.token)) throw new BarocertException(-99999999, "토큰 원문이 입력되지 않았습니다.");

            string PostData = toJsonString(login);

            return httppost<LoginReceipt>("/PASS/Login/" + ClientCode, PostData);
        }

        public LoginStatus getLoginStatus(string ClientCode, string ReceiptId)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (string.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httpget<LoginStatus>("/PASS/Login/" + ClientCode + "/" + ReceiptId);
        }

        public LoginResult verifyLogin(string ClientCode, string ReceiptId, LoginVerify loginVerify)
        {
            if (string.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (string.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");
            if (null == loginVerify) throw new BarocertException(-99999999, "본인인증 검증 요청 정보가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(loginVerify.receiverHP)) throw new BarocertException(-99999999, "수신자 휴대폰번호가 입력되지 않았습니다.");
            if (string.IsNullOrEmpty(loginVerify.receiverName)) throw new BarocertException(-99999999, "수신자 성명이 입력되지 않았습니다.");

            string PostData = toJsonString(loginVerify);

            return httppost<LoginResult>("/PASS/Login/" + ClientCode + "/" + ReceiptId, PostData);
        }

    }
}
