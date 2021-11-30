using System.Text.Json.Serialization;

namespace Tiny.IFlySDk.Models
{
    public class CommonParams
    {
        /// <summary>
        /// 在平台申请的APPID信息
        /// </summary>
        public string App_Id { get; set; } = "";

        /// <summary>
        /// 请求用户服务返回的uid，用户及设备级别个性化功能依赖此参数
        /// </summary>
        public string UId { get; set; } = "";
    }

}