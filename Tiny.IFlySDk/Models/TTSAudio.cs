namespace Tiny.IFlySDk.Models
{
    public class TTSAudio
    {
        /// <summary>
        /// 合成后的音频片段，采用base64编码
        /// </summary>
        public string Audio { get; set; }

        /// <summary>
        /// 当前音频流状态，1表示合成中，2表示合成结束
        /// </summary>
        public string Ced { get; set; }

        /// <summary>
        /// 合成进度，指当前合成文本的字节数
        /// 注：请注意合成是以句为单位切割的，若文本只有一句话，则每次返回结果的ced是相同的。
        /// </summary>
        public int Status { get; set; }
    }

}