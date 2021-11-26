using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace TestSdkApp // Note: actual namespace depends on the project name.
{
    public class IFlySdkHelper
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="apiSecretIsKey"></param>
        /// <param name="buider"></param>
        /// <returns></returns>
        private static string HMACSha256(string apiSecretIsKey, string buider)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(apiSecretIsKey);

            using (HMACSHA256 hMACSHA256 = new HMACSHA256(bytes))
            {
                byte[] date = Encoding.UTF8.GetBytes(buider);
                date = hMACSHA256.ComputeHash(date);
                hMACSHA256.Clear();
                return Convert.ToBase64String(date);
            }
        }
        public static string BuildAuthUrlCore(Uri url, string apiSecret,string appKey)
        {
            string date = DateTime.UtcNow.ToString("r");
            //build signature string
            string signatureOrigin = $"host: {url.Host}\ndate: {date}\nGET {url.LocalPath} HTTP/1.1";
            string signature = HMACSha256(apiSecret, signatureOrigin);
            string authorization = $"api_key=\"{appKey}\", algorithm=\"hmac-sha256\", headers=\"host date request-line\", signature=\"{signature}\"";
            //Build url
            StringBuilder urlBuilder = new StringBuilder();
            urlBuilder.Append(url.ToString());
            urlBuilder.Append("?");
            urlBuilder.Append("authorization=");
            urlBuilder.Append(Convert.ToBase64String(Encoding.UTF8.GetBytes(authorization)));
            urlBuilder.Append("&");
            urlBuilder.Append("date=");
            urlBuilder.Append(HttpUtility.UrlEncode(date).Replace("+", "%20"));  //默认会将空格编码为+号
            urlBuilder.Append("&");
            urlBuilder.Append("host=");
            urlBuilder.Append(url.Host);
            return urlBuilder.ToString();
        }
        //生成URL
        public static string BuildAuthUrl(IFlySdkOption _settings)
        {
            string date = DateTime.UtcNow.ToString("r");
            Uri uri = null;

            //var uri = _settings.ApiType switch
            //{
            //    Enum.ApiType.ASR => new Uri(_settings.ASRUrl),
            //    Enum.ApiType.TTS => new Uri(_settings.TTSUrl),
            //    _ => throw new Exception("Unknow Api type."),
            //};

            if (_settings.ApiType == ApiType.ASR)
            {
                uri = new Uri(_settings.ASRUrl);
            }
            else if (_settings.ApiType == ApiType.TTS)
            {
                uri = new Uri(_settings.TTSUrl);
            }
            else
            {
                throw new Exception("Unknow Api type.");
            }


            //build signature string
            string signatureOrigin = $"host: {uri.Host}\ndate: {date}\nGET {uri.LocalPath} HTTP/1.1";
            string signature = HMACSha256(_settings.ApiSecret, signatureOrigin);
            string authorization = $"api_key=\"{_settings.ApiKey}\", algorithm=\"hmac-sha256\", headers=\"host date request-line\", signature=\"{signature}\"";
            //Build url
            StringBuilder urlBuilder = new StringBuilder();
            urlBuilder.Append(uri.ToString());
            urlBuilder.Append("?");
            urlBuilder.Append("authorization=");
            urlBuilder.Append(Convert.ToBase64String(Encoding.UTF8.GetBytes(authorization)));
            urlBuilder.Append("&");
            urlBuilder.Append("date=");
            urlBuilder.Append(HttpUtility.UrlEncode(date).Replace("+", "%20"));  //默认会将空格编码为+号
            urlBuilder.Append("&");
            urlBuilder.Append("host=");
            urlBuilder.Append(uri.Host);
            return urlBuilder.ToString();
        }
    }
}