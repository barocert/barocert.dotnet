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

using Barocert.Kakaocert;
using Barocert.Navercert;

namespace Barocert
{

    public abstract class BaseService
    {
        private const string ServiceID = "BAROCERT";
        private const string ServiceURL_Default = "https://barocert.linkhub.co.kr";
        private const string ServiceURL_Static = "https://static-barocert.linkhub.co.kr";

        private const string APIVERSION = "2.1";
        private const string CRLF = "\r\n";

        private Dictionary<string, Token> _tokenTable = new Dictionary<string, Token>();
        private bool _IPRestrictOnOff;
        private bool _UseStaticIP;
        private bool _UseLocalTimeYN;
        private string _LinkID;
        private string _SecretKey;
        private Authority _LinkhubAuth;
        private string _ServiceURL;
        private string _AuthURL;
        private List<string> _Scopes = new List<string>();

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

        public string ServiceURL
        {
            set { _ServiceURL = value; }
            get { return _ServiceURL; }
        }

        public string AuthURL
        {
            set { _LinkhubAuth.AuthURL = value; }
            get { return _LinkhubAuth.AuthURL; }
        }

        public BaseService(string LinkID, string SecretKey)
        {
            _LinkhubAuth = new Authority(LinkID, SecretKey);
            _Scopes.Add("partner");
            _LinkID = LinkID;
            _SecretKey = SecretKey;
            _IPRestrictOnOff = true;
            _UseLocalTimeYN = true;
        }

        public void AddScope(string scope)
        {
            _Scopes.Add(scope);
        }

        protected string toJsonString(Object graph)
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
                string t = reader.ReadToEnd();

                JavaScriptSerializer jss = new JavaScriptSerializer();

                return jss.Deserialize<T>(t);
            }
        }

        protected string getURL
        {
            get
            {
                if (_ServiceURL != null)
                {
                    return _ServiceURL;
                }

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

        private string getSession_Token()
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
                    Console.WriteLine("test");
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

        protected T httpget<T>(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getURL + url);

            if (string.IsNullOrEmpty(_LinkID) == false)
            {
                string bearerToken = getSession_Token();
                request.Headers.Add("Authorization", "Bearer" + " " + bearerToken);
            }

            request.Headers.Add("x-bc-version", APIVERSION);

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

        protected T httppost<T>(string url)
        {
            return httppost<T>(url, null);
        }

        protected T httppost<T>(string url, string PostData)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getURL + url);
            Console.Write(getURL);

            request.ContentType = "application/json;";


            string bearerToken = getSession_Token();

            string xDate = _LinkhubAuth.getTime(UseStaticIP, false, false);


            request.Headers.Add("Authorization", "Bearer" + " " + bearerToken);

            request.Headers.Add("x-bc-date", xDate);


            request.Headers.Add("Accept-Encoding", "gzip, deflate");

            request.AutomaticDecompression = DecompressionMethods.GZip;

            request.Method = "POST";

            if (string.IsNullOrEmpty(PostData)) PostData = "";

            byte[] btPostDAta = Encoding.UTF8.GetBytes(PostData);

            string HMAC_target = "POST\n";
            if (false == string.IsNullOrEmpty(PostData))
            {
                HMAC_target += Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(PostData))) + "\n";
            }
            HMAC_target += xDate + "\n";
            HMAC_target += url + "\n";

            HMACSHA256 hmac = new HMACSHA256(Convert.FromBase64String(_SecretKey));
            string hmac_str = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(HMAC_target)));

            request.Headers.Add("x-bc-version", APIVERSION);
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

        public bool isNullorEmptyTitle(List<Barocert.Kakaocert.MultiSignTokens> multiSignTokens)
        {
            if (multiSignTokens == null) return true;
            foreach (Barocert.Kakaocert.MultiSignTokens signTokens in multiSignTokens)
            {
                if (signTokens == null) return true;
                if (string.IsNullOrEmpty(signTokens.signTitle) && (string.IsNullOrEmpty(signTokens.reqTitle)))
                {
                    return true;
                }
            }
            return false;
        }

        public bool isNullorEmptyToken(List<Barocert.Kakaocert.MultiSignTokens> multiSignTokens)
        {
            if (multiSignTokens == null) return true;
            foreach (Barocert.Kakaocert.MultiSignTokens signTokens in multiSignTokens)
            {
                if (signTokens == null) return true;
                if (string.IsNullOrEmpty(signTokens.token)) return true;
            }
            return false;
        }

        public bool isNullorEmptyTokenType(List<Barocert.Navercert.MultiSignTokens> multiSignTokens)
        {
            if (multiSignTokens == null) return true;
            foreach (Barocert.Navercert.MultiSignTokens signTokens in multiSignTokens)
            {
                if (signTokens == null) return true;
                if (string.IsNullOrEmpty(signTokens.tokenType)) return true;
            }
            return false;
        }

        public bool isNullorEmptyToken(List<Barocert.Navercert.MultiSignTokens> multiSignTokens)
        {
            if (multiSignTokens == null) return true;
            foreach (Barocert.Navercert.MultiSignTokens signTokens in multiSignTokens)
            {
                if (signTokens == null) return true;
                if (string.IsNullOrEmpty(signTokens.token)) return true;
            }
            return false;
        }

        public string encrypt(string plainText)
        {
            return encGCM(plainText);
        }

        private string encGCM(string plainText)
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

        private string encCBC(string plainText)
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

    }
}
