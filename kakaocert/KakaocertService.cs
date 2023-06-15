using System;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Net;
using Linkhub;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;

using Linkhub.BouncyCastle.Crypto;
using Linkhub.BouncyCastle.Crypto.Modes;
using Linkhub.BouncyCastle.Crypto.Engines;
using Linkhub.BouncyCastle.Crypto.Parameters;
using Linkhub.BouncyCastle.Security;


namespace Kakaocert
{

    public class KakaocertService
    {
        private const string ServiceID = "BAROCERT";
        private const String ServiceURL_Default = "https://barocert.linkhub.co.kr";
        private const String ServiceURL_Static = "https://static-barocert.linkhub.co.kr";

        private const String APIVersion = "2.0";
        private const String CRLF = "\r\n";

        private Dictionary<String, Token> _tokenTable = new Dictionary<String, Token>();
        private bool _IPRestrictOnOff;
        private bool _UseStaticIP;
        private bool _UseLocalTimeYN;
        private String _LinkID;
        private String _SecretKey;
        private Authority _LinkhubAuth;
        private List<String> _Scopes = new List<string>();

        private const int CBC_IV_LENGTH = 16;
        private const int GCM_IV_LENGTH = 12;
        private const int GCM_TAG_LENGTH = 128;
        private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

        public bool IPRestrictOnOff
        {
            set { _IPRestrictOnOff = value; }
            get { return _IPRestrictOnOff; }
        }

        public bool UseStaticIP
        {
            set { _UseStaticIP = value; }
            get { return _UseStaticIP; }
        }

        public bool UseLocalTimeYN
        {
            set { _UseLocalTimeYN = value; }
            get { return _UseLocalTimeYN; }
        }

        public KakaocertService(String LinkID, String SecretKey)
        {
            _LinkhubAuth = new Authority(LinkID, SecretKey);
            _Scopes.Add("partner");
            _Scopes.Add("401");
            _Scopes.Add("402");
            _Scopes.Add("403");
            _Scopes.Add("404");
            _LinkID = LinkID;
            _SecretKey = SecretKey;
            _IPRestrictOnOff = true;
            _UseLocalTimeYN = true;
        }

        protected String toJsonString(Object graph)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(graph.GetType());
                ser.WriteObject(ms, graph);
                ms.Seek(0, SeekOrigin.Begin);
                return new StreamReader(ms).ReadToEnd();
            }
        }
        protected T fromJson<T>(Stream jsonStream)
        {
            using (StreamReader reader = new StreamReader(jsonStream, Encoding.UTF8, true))
            {
                String t = reader.ReadToEnd();

                JavaScriptSerializer jss = new JavaScriptSerializer();

                return jss.Deserialize<T>(t);
            }
        }

        protected String ServiceURL
        {
            get
            {
                if (UseStaticIP)
                {
                    return ServiceURL_Static;
                }
                else
                {
                    return ServiceURL_Default;
                }
            }
        }

        private String getSession_Token()
        {
            Token _token = null;

            if (_tokenTable.ContainsKey(_LinkID))
            {
                _token = _tokenTable[_LinkID];
            }

            bool expired = true;
            if (_token != null)
            {
                DateTime now = DateTime.Parse(_LinkhubAuth.getTime(UseStaticIP, UseLocalTimeYN, false));

                DateTime expiration = DateTime.Parse(_token.expiration);

                expired = expiration < now;

            }

            if (expired)
            {
                try
                {
                    if (_IPRestrictOnOff) // IPRestrictOnOff 사용시
                    {
                        _token = _LinkhubAuth.getToken(ServiceID, "", _Scopes, null, UseStaticIP, UseLocalTimeYN, false);
                    }
                    else
                    {
                        _token = _LinkhubAuth.getToken(ServiceID, "", _Scopes, "*", UseStaticIP, UseLocalTimeYN, false);
                    }


                    if (_tokenTable.ContainsKey(_LinkID))
                    {
                        _tokenTable.Remove(_LinkID);
                    }
                    _tokenTable.Add(_LinkID, _token);
                }
                catch (LinkhubException le)
                {
                    throw new BarocertException(le);
                }
            }

            return _token.session_token;
        }

        protected T httpget<T>(String url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ServiceURL + url);

            if (String.IsNullOrEmpty(_LinkID) == false)
            {
                String bearerToken = getSession_Token();
                request.Headers.Add("Authorization", "Bearer" + " " + bearerToken);
            }

            request.Headers.Add("x-bc-version", "2.0");

            request.Headers.Add("Accept-Encoding", "gzip, deflate");

            request.AutomaticDecompression = DecompressionMethods.GZip;

            request.Method = "GET";

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream stReadData = response.GetResponseStream())
                    {
                        return fromJson<T>(stReadData);
                    }
                }
            }
            catch (Exception we)
            {
                if (we is WebException && ((WebException)we).Response != null)
                {
                    using (Stream stReadData = ((WebException)we).Response.GetResponseStream())
                    {
                        Response t = fromJson<Response>(stReadData);
                        throw new BarocertException(t.code, t.message);
                    }
                }
                throw new BarocertException(-99999999, we.Message);
            }

        }

        protected T httppost<T>(String url)
        {
            return httppost<T>(url, null);
        }

        protected T httppost<T>(String url, String PostData)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ServiceURL + url);

            request.ContentType = "application/json;";


            String bearerToken = getSession_Token();

            String xDate = _LinkhubAuth.getTime(UseStaticIP, false, false);


            request.Headers.Add("Authorization", "Bearer" + " " + bearerToken);

            request.Headers.Add("x-bc-date", xDate);


            request.Headers.Add("Accept-Encoding", "gzip, deflate");

            request.AutomaticDecompression = DecompressionMethods.GZip;

            request.Method = "POST";

            if (String.IsNullOrEmpty(PostData)) PostData = "";

            byte[] btPostDAta = Encoding.UTF8.GetBytes(PostData);

            String HMAC_target = "POST\n";
            HMAC_target += url + "\n";
            if (false == String.IsNullOrEmpty(PostData))
            {
                HMAC_target += Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(PostData))) + "\n";
            }
            HMAC_target += xDate + "\n";

            HMACSHA256 hmac = new HMACSHA256(Convert.FromBase64String(_SecretKey));
            String hmac_str = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(HMAC_target)));

            request.Headers.Add("x-bc-version", "2.0");
            request.Headers.Add("x-bc-auth", hmac_str);
            request.Headers.Add("x-bc-encryptionmode", "GCM");

            request.ContentLength = btPostDAta.Length;

            request.GetRequestStream().Write(btPostDAta, 0, btPostDAta.Length);

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream stReadData = response.GetResponseStream())
                    {
                        return fromJson<T>(stReadData);
                    }
                }
            }
            catch (Exception we)
            {
                if (we is WebException && ((WebException)we).Response != null)
                {
                    using (Stream stReadData = ((WebException)we).Response.GetResponseStream())
                    {
                        Response t = fromJson<Response>(stReadData);
                        throw new BarocertException(t.code, t.message);
                    }
                }
                throw new BarocertException(-99999999, we.Message);
            }
        }

        public String encrypt(String plainText)
        {
            return encGCM(plainText);
        }

        private String encGCM(String plainText)
        {
            var cipher = new GcmBlockCipher(new AesEngine());
            byte[] iv = newGCMbyte();
            byte[] key = Convert.FromBase64String(_SecretKey);
            var parameters = new AeadParameters(new KeyParameter(key), GCM_TAG_LENGTH, iv, null);
            cipher.Init(true, parameters);
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] ciphertextBytes = new byte[cipher.GetOutputSize(utf8.GetByteCount(plainText))];
            int len = cipher.ProcessBytes(utf8.GetBytes(plainText), 0, utf8.GetByteCount(plainText), ciphertextBytes, 0);
            cipher.DoFinal(ciphertextBytes, len);

            byte[] concatted = new byte[ciphertextBytes.Length + iv.Length];
            iv.CopyTo(concatted, 0);
            ciphertextBytes.CopyTo(concatted, 12);
            return Convert.ToBase64String(concatted);
        }

        private String encCBC(String plainText)
        {
            byte[] iv = newCBCbyte();
            byte[] concatted = null;

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.KeySize = 256;
                aes.Key = Convert.FromBase64String(_SecretKey);
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform enc = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, enc, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }

                        byte[] encrypted = ms.ToArray();

                        concatted = new byte[encrypted.Length + aes.IV.Length];
                        aes.IV.CopyTo(concatted, 0);
                        encrypted.CopyTo(concatted, 16);
                    }
                }
            }

            return Convert.ToBase64String(concatted);
        }

        private static byte[] newGCMbyte()
        {
            byte[] nonce = new byte[GCM_IV_LENGTH];
            new RNGCryptoServiceProvider().GetBytes(nonce);
            return nonce;
        }
        private static byte[] newCBCbyte()
        {
            byte[] nonce = new byte[CBC_IV_LENGTH];
            new RNGCryptoServiceProvider().GetBytes(nonce);
            return nonce;
        }


        public IdentityReceipt requestIdentity(String ClientCode, Identity identity)
        {
            if (String.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (null == identity) throw new BarocertException(-99999999, "본인인증 요청정보가 입력되지 않았습니다.");

            if (String.IsNullOrEmpty(identity.receiverHP)) throw new BarocertException(-99999999, "수신자 휴대폰번호가 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(identity.receiverName)) throw new BarocertException(-99999999, "수신자 성명이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(identity.receiverBirthday)) throw new BarocertException(-99999999, "생년월일이 입력되지 않았습니다.");

            if (null == identity.expireIn) throw new BarocertException(-99999999, "만료시간이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(identity.reqTitle)) throw new BarocertException(-99999999, "인증요청 메시지 제목이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(identity.token)) throw new BarocertException(-99999999, "토큰 원문이 입력되지 않았습니다.");

            String PostData = toJsonString(identity);

            return httppost<IdentityReceipt>("/KAKAO/Identity/" + ClientCode, PostData);
        }

        public IdentityStatus getIdentityStatus(String ClientCode, String ReceiptId)
        {
            if (String.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (String.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httpget<IdentityStatus>("/KAKAO/Identity/" + ClientCode + "/" + ReceiptId);
        }

        public IdentityResult verifyIdentity(String ClientCode, String ReceiptId)
        {
            if (String.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (String.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httppost<IdentityResult>("/KAKAO/Identity/" + ClientCode + "/" + ReceiptId);
        }

        public SignReceipt requestSign(String ClientCode, Sign sign)
        {
            if (String.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (null == sign) throw new BarocertException(-99999999, "전자서명 요청정보가 입력되지 않았습니다.");

            if (String.IsNullOrEmpty(sign.receiverHP)) throw new BarocertException(-99999999,"수신자 휴대폰번호가 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(sign.receiverName)) throw new BarocertException(-99999999,"수신자 성명이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(sign.receiverBirthday)) throw new BarocertException(-99999999,"생년월일이 입력되지 않았습니다.");
            if (null == sign.expireIn) throw new BarocertException(-99999999,"만료시간이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(sign.reqTitle)) throw new BarocertException(-99999999,"인증요청 메시지 제목이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(sign.token)) throw new BarocertException(-99999999,"토큰 원문이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(sign.tokenType)) throw new BarocertException(-99999999,"원문 유형이 입력되지 않았습니다.");

            String PostData = toJsonString(sign);

            return httppost<SignReceipt>("/KAKAO/Sign/" + ClientCode, PostData);
        }

        public SignStatus getSignStatus(String ClientCode, String ReceiptId)
        {
            if (String.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (String.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httpget<SignStatus>("/KAKAO/Sign/" + ClientCode + "/" + ReceiptId);
        }

        public SignResult verifySign(String ClientCode, String ReceiptId)
        {
            if (String.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (String.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httppost<SignResult>("/KAKAO/Sign/" + ClientCode + "/" + ReceiptId);
        }

        public MultiSignReceipt requestMultiSign(String ClientCode, MultiSign multiSign)
        {
            if (String.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (null == multiSign) throw new BarocertException(-99999999, "전자서명 요청정보가 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(multiSign.receiverHP)) throw new BarocertException(-99999999,"수신자 휴대폰번호가 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(multiSign.receiverName)) throw new BarocertException(-99999999,"수신자 성명이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(multiSign.receiverBirthday)) throw new BarocertException(-99999999,"생년월일이 입력되지 않았습니다.");
            if (null == multiSign.expireIn) throw new BarocertException(-99999999,"만료시간이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(multiSign.reqTitle)) throw new BarocertException(-99999999,"인증요청 메시지 제목이 입력되지 않았습니다.");
            if (isNullorEmptyTitle(multiSign.tokens)) throw new BarocertException(-99999999,"인증요청 메시지 제목이 입력되지 않았습니다.");
            if (isNullorEmptyToken(multiSign.tokens)) throw new BarocertException(-99999999,"토큰 원문이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(multiSign.tokenType)) throw new BarocertException(-99999999,"원문 유형이 입력되지 않았습니다.");

            String PostData = toJsonString(multiSign);

            return httppost<MultiSignReceipt>("/KAKAO/MultiSign/" + ClientCode, PostData);
        }


        public MultiSignStatus getMultiSignStatus(String ClientCode, String ReceiptId)
        {
            if (String.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (String.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httpget<MultiSignStatus>("/KAKAO/MultiSign/" + ClientCode + "/" + ReceiptId);
        }


        public MultiSignResult verifyMultiSign(String ClientCode, String ReceiptId)
        {
            if (String.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (String.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httppost<MultiSignResult>("/KAKAO/MultiSign/" + ClientCode + "/" + ReceiptId);
        }

        public CMSReceipt requestCMS(String ClientCode, CMS cms)
        {
            if (String.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (null == cms) throw new BarocertException(-99999999, "자동이체 출금동의 요청정보가 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(cms.receiverHP)) throw new BarocertException(-99999999,"수신자 휴대폰번호가 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(cms.receiverName)) throw new BarocertException(-99999999,"수신자 성명이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(cms.receiverBirthday)) throw new BarocertException(-99999999,"생년월일이 입력되지 않았습니다.");
            if (null == cms.expireIn) throw new BarocertException(-99999999,"만료시간이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(cms.reqTitle)) throw new BarocertException(-99999999,"인증요청 메시지 제목이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(cms.requestCorp)) throw new BarocertException(-99999999,"청구기관명이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(cms.bankName)) throw new BarocertException(-99999999,"은행명이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(cms.bankAccountNum)) throw new BarocertException(-99999999,"계좌번호가 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(cms.bankAccountName)) throw new BarocertException(-99999999,"예금주명이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(cms.bankAccountBirthday)) throw new BarocertException(-99999999,"예금주 생년월일이 입력되지 않았습니다.");
            if (String.IsNullOrEmpty(cms.bankServiceType)) throw new BarocertException(-99999999, "출금 유형이 입력되지 않았습니다.");

            String PostData = toJsonString(cms);

            return httppost<CMSReceipt>("/KAKAO/CMS/" + ClientCode, PostData);
        }

        public CMSStatus getCMSStatus(String ClientCode, String ReceiptId)
        {
            if (String.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (String.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httpget<CMSStatus>("/KAKAO/CMS/" + ClientCode + "/" + ReceiptId);
        }

        public CMSResult verifyCMS(String ClientCode, String ReceiptId)
        {
            if (String.IsNullOrEmpty(ClientCode)) throw new BarocertException(-99999999, "이용기관코드가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ClientCode, @"^\d+$")) throw new BarocertException(-99999999, "이용기관코드는 숫자만 입력할 수 있습니다.");
            if (ClientCode.Length != 12) throw new BarocertException(-99999999, "이용기관코드는 12자 입니다.");
            if (String.IsNullOrEmpty(ReceiptId)) throw new BarocertException(-99999999, "접수아이디가 입력되지 않았습니다.");
            if (false == Regex.IsMatch(ReceiptId, @"^\d+$")) throw new BarocertException(-99999999, "접수아이디는 숫자만 입력할 수 있습니다.");
            if (ReceiptId.Length != 32) throw new BarocertException(-99999999, "접수아이디는 32자 입니다.");

            return httppost<CMSResult>("/KAKAO/CMS/" + ClientCode + "/" + ReceiptId);
        }

        public bool isNullorEmptyTitle(List<MultiSignTokens> multiSignTokens){
            if(multiSignTokens == null) return true;
            foreach(MultiSignTokens signTokens in multiSignTokens ){
                if(signTokens == null) return true;
                if(String.IsNullOrEmpty(signTokens.reqTitle)) return true;
            }
            return false;
        }

        public bool isNullorEmptyToken(List<MultiSignTokens> multiSignTokens){
            if(multiSignTokens == null) return true;
            foreach(MultiSignTokens signTokens in multiSignTokens ){
                if(signTokens == null) return true;
                if(String.IsNullOrEmpty(signTokens.token)) return true;
            }
        return false;
    }

    }
}
