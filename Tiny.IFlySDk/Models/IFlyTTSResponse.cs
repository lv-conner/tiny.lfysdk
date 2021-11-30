namespace Tiny.IFlySDk.Models
{
    /// <summary>
    /// 语音合成响应数据
    /// </summary>
    public class IFlyTTSResponse
    {
        /// <summary>
        /// 返回码，0表示成功，其它表示异常，详情请参考错误码。
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 本次会话的id，只在第一帧请求时返回
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// 语音数据信息
        /// </summary>
        public TTSAudio Data { get; set; }
    }

}