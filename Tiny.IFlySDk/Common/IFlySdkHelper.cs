using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Tiny.IFlySDk.Common
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
        public static string BuildIFlyUrl(Uri url, string apiSecret,string appKey)
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
    }
}