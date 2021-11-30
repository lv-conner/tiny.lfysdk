namespace Tiny.IFlySDk.Common 
{
    public class IFlySdkOption
    {
        /// <summary>
        /// AppID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// ApiSecret
        /// </summary>
        public string ApiSecret { get; set; }

        /// <summary>
        /// ApiKey
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// ASR接口地址
        /// </summary>
        public Uri ASRUrl { get; set; } = new Uri("wss://iat-api.xfyun.cn/v2/iat");

        /// <summary>
        /// TTS接口地址
        /// </summary>
        public Uri TTSUrl { get; set; } = new Uri("wss://tts-api.xfyun.cn/v2/tts");

        public ApiType ApiType { get; set; }
    }
}