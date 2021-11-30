using System.Text;
using System.Text.Json.Serialization;

namespace Tiny.IFlySDk.Models
{
    /// <summary>
    /// 语音合成请求
    /// </summary>
    public class IFlyTTSRequest
    {
        public IFlyTTSRequest(string appId, string str)
        {
            Common = new CommonParams()
            {
                App_Id = appId,
            };
            Business = new BusinessParams();
            var strBytes = Encoding.UTF8.GetBytes(str);
            if (strBytes.Length > 8000)
            {
                throw new Exception("不能发送大于8000字节的消息，请将消息分片！");
            }
            Data = new DataParams()
            {
                Text = Convert.ToBase64String(strBytes)
            };
        }
        /// <summary>
        /// 公共参数
        /// </summary>
        public CommonParams Common { get; set; }
        /// <summary>
        /// 业务参数
        /// </summary>
        public BusinessParams Business { get; set; }
        /// <summary>
        /// 业务数据量参数
        /// </summary>
        public DataParams Data { get; set; }
    }

}