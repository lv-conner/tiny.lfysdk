namespace TestSdkApp // Note: actual namespace depends on the project name.
{
    public class IFlySdkOption
    {
        /// <summary>
        /// AppID
        /// </summary>
        public string AppID { get; set; }

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
        public string ASRUrl { get; set; } = "wss://iat-api.xfyun.cn/v2/iat";

        /// <summary>
        /// TTS接口地址
        /// </summary>
        public string TTSUrl { get; set; } = "wss://tts-api.xfyun.cn/v2/tts";

        public ApiType ApiType { get; set; }
    }
}