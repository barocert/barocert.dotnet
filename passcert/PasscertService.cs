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


namespace Passcert
{

    public class PasscertService : BaseService
    {
        public PasscertService(String LinkID, String SecretKey)
            : base(LinkID, SecretKey)
        {
            this.AddScope("441");
            this.AddScope("442");
            this.AddScope("443");
            this.AddScope("444");
        }
        public IdentityReceipt requestIdentity(String ClientCode, Identity identity)
        {
            if (String.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (null == identity) throw new BarocertException(-99999999, "본인인증 요청정보가 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(identity.receiverHP)) throw new BarocertException(-99999999, "수신자 휴대폰번호가 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(identity.receiverName)) throw new BarocertException(-99999999, "수신자 성명이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(identity.reqTitle)) throw new BarocertException(-99999999, "인증요청 메시지 제목이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(identity.callCenterNum)) throw new BarocertException(-99999999, "고객센터 연락처가 입력되지 않았습니다.");
            if (null == identity.expireIn) throw new BarocertException(-99999999, "만료시간이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(identity.token)) throw new BarocertException(-99999999, "토큰 원문이 입력되지 않았습니다.");

            String PostData = toJsonString(identity);

            return httppost<IdentityReceipt>("/PASS/Identity/" + ClientCode, PostData);
        }

        public IdentityStatus getIdentityStatus(String ClientCode, String ReceiptId)
        {
            if (String.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (String.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httpget<IdentityStatus>("/PASS/Identity/" + ClientCode + "/" + ReceiptId);
        }

        public IdentityResult verifyIdentity(String ClientCode, String ReceiptId, IdentityVerify identityVerify)
        {
            if (String.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (String.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");
            if (null == identityVerify) throw new BarocertException(-99999999, "본인인증 검증 요청 정보가 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(identityVerify.receiverHP)) throw new BarocertException(-99999999, "수신자 휴대폰번호가 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(identityVerify.receiverName)) throw new BarocertException(-99999999, "수신자 성명이 입력되지 않았습니다.");

            String PostData = toJsonString(identityVerify);

            return httppost<IdentityResult>("/PASS/Identity/" + ClientCode + "/" + ReceiptId, PostData);
        }

        public SignReceipt requestSign(String ClientCode, Sign sign)
        {
            if (String.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (null == sign) throw new BarocertException(-99999999, "전자서명 요청정보가 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(sign.receiverHP)) throw new BarocertException(-99999999, "수신자 휴대폰번호가 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(sign.receiverName)) throw new BarocertException(-99999999, "수신자 성명이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(sign.reqTitle)) throw new BarocertException(-99999999, "인증요청 메시지 제목이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(sign.callCenterNum)) throw new BarocertException(-99999999, "고객센터 연락처가 입력되지 않았습니다.");
            if (null == sign.expireIn) throw new BarocertException(-99999999, "만료시간이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(sign.token)) throw new BarocertException(-99999999, "토큰 원문이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(sign.tokenType)) throw new BarocertException(-99999999, "원문 유형이 입력되지 않았습니다.");

            String PostData = toJsonString(sign);

            return httppost<SignReceipt>("/PASS/Sign/" + ClientCode, PostData);
        }

        public SignStatus getSignStatus(String ClientCode, String ReceiptId)
        {
            if (String.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (String.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httpget<SignStatus>("/PASS/Sign/" + ClientCode + "/" + ReceiptId);
        }

        public SignResult verifySign(String ClientCode, String ReceiptId, SignVerify signVerify)
        {
            if (String.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (String.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");
            if (null == signVerify) throw new BarocertException(-99999999, "전자서명 검증 요청 정보가 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(signVerify.receiverHP)) throw new BarocertException(-99999999, "수신자 휴대폰번호가 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(signVerify.receiverName)) throw new BarocertException(-99999999, "수신자 성명이 입력되지 않았습니다.");

            String PostData = toJsonString(signVerify);

            return httppost<SignResult>("/PASS/Sign/" + ClientCode + "/" + ReceiptId, PostData);
        }

        public CMSReceipt requestCMS(String ClientCode, CMS cms)
        {
            if (String.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (null == cms) throw new BarocertException(-99999999, "자동이체 출금동의 요청정보가 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(cms.receiverHP)) throw new BarocertException(-99999999, "수신자 휴대폰번호가 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(cms.receiverName)) throw new BarocertException(-99999999, "수신자 성명이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(cms.reqTitle)) throw new BarocertException(-99999999, "인증요청 메시지 제목이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(cms.callCenterNum)) throw new BarocertException(-99999999, "고객센터 연락처가 입력되지 않았습니다.");
            if (null == cms.expireIn) throw new BarocertException(-99999999, "만료시간이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(cms.bankName)) throw new BarocertException(-99999999, "출금은행명이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(cms.bankAccountNum)) throw new BarocertException(-99999999, "출금계좌번호가 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(cms.bankAccountName)) throw new BarocertException(-99999999, "출금계좌 예금주명이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(cms.bankServiceType)) throw new BarocertException(-99999999, "출금 유형이 입력되지 않았습니다.");

            String PostData = toJsonString(cms);

            return httppost<CMSReceipt>("/PASS/CMS/" + ClientCode, PostData);
        }

        public CMSStatus getCMSStatus(String ClientCode, String ReceiptId)
        {
            if (String.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (String.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httpget<CMSStatus>("/PASS/CMS/" + ClientCode + "/" + ReceiptId);
        }

        public CMSResult verifyCMS(String ClientCode, String ReceiptId, CMSVerify cmsVerify)
        {
            if (String.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (String.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");
            if (null == cmsVerify) throw new BarocertException(-99999999, "출금동의 검증 요청 정보가 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(cmsVerify.receiverHP)) throw new BarocertException(-99999999, "수신자 휴대폰번호가 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(cmsVerify.receiverName)) throw new BarocertException(-99999999, "수신자 성명이 입력되지 않았습니다.");

            String PostData = toJsonString(cmsVerify);

            return httppost<CMSResult>("/PASS/CMS/" + ClientCode + "/" + ReceiptId, PostData);
        }

        public LoginReceipt requestLogin(String ClientCode, Login login)
        {
            if (String.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (null == login) throw new BarocertException(-99999999, "간편로그인 요청정보가 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(login.receiverHP)) throw new BarocertException(-99999999, "수신자 휴대폰번호가 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(login.receiverName)) throw new BarocertException(-99999999, "수신자 성명이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(login.reqTitle)) throw new BarocertException(-99999999, "인증요청 메시지 제목이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(login.callCenterNum)) throw new BarocertException(-99999999, "고객센터 연락처가 입력되지 않았습니다.");
            if (null == login.expireIn) throw new BarocertException(-99999999, "만료시간이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(login.token)) throw new BarocertException(-99999999, "토큰 원문이 입력되지 않았습니다.");

            String PostData = toJsonString(login);

            return httppost<LoginReceipt>("/PASS/Login/" + ClientCode, PostData);
        }

        public LoginStatus getLoginStatus(String ClientCode, String ReceiptId)
        {
            if (String.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (String.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httpget<LoginStatus>("/PASS/Login/" + ClientCode + "/" + ReceiptId);
        }

        public LoginResult verifyLogin(String ClientCode, String ReceiptId, LoginVerify loginVerify)
        {
            if (String.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (String.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");
            if (null == loginVerify) throw new BarocertException(-99999999, "본인인증 검증 요청 정보가 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(loginVerify.receiverHP)) throw new BarocertException(-99999999, "수신자 휴대폰번호가 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(loginVerify.receiverName)) throw new BarocertException(-99999999, "수신자 성명이 입력되지 않았습니다.");

            String PostData = toJsonString(loginVerify);

            return httppost<LoginResult>("/PASS/Login/" + ClientCode + "/" + ReceiptId, PostData);
        }

    }
}
