using System.Text.Json.Serialization;

namespace Tiny.IFlySDk.Models
{
    public class BusinessParams
    {
        #region 官方参数
        /// <summary>
        /// 引擎类型
        /// aisound（普通效果）
        /// intp65（中文）
        /// intp65_en（英文）
        /// xtts（优化效果）
        /// 默认为intp65
        /// </summary>
        public string Ent { get; set; } = "intp65";
        /// <summary>
        /// 音频编码
        /// speex：压缩格式
        /// speex-wb;7：压缩格式，压缩等级1 ~10，默认为7
        /// </summary>
        public string Aue { get; set; } = "raw";
        /// <summary>
        /// 音频采样率
        /// audio/L16;rate=16000
        /// audio/L16;rate=8000
        /// (目前官网"x_"系列发音人中仅讯飞虫虫，讯飞春春，讯飞飞飞，讯飞刚刚，讯飞宋宝宝，讯飞小包，讯飞小东，讯飞小肥，讯飞小乔，讯飞小瑞，讯飞小师，讯飞小王，讯飞颖儿支持8k)
        /// </summary>
        public string Auf { get; set; } = "audio/L16;rate=16000";
        /// <summary>
        /// 发音人，可选值详见控制台-我的应用-在线语音合成服务管理-发音人授权管理，使用方法参考官网
        /// </summary>
        public string Vcn { get; set; } = "xiaoyan";
        /// <summary>
        /// 语速，可选值：[0-100]，默认为50
        /// </summary>
        public int Speed { get; set; } = 50;
        /// <summary>
        /// 音量，可选值：[0-100]，默认为50
        /// </summary>
        public int Volume { get; set; } = 50;
        /// <summary>
        /// 音高
        /// </summary>
        public int Pitch { get; set; } = 50;
        /// <summary>
        /// 背景音乐0：无(默认)，1：有
        /// </summary>
        public int Bgs { get; set; } = 0;

        /// <summary>
        /// 文本编码格式
        /// GB2312、GBK、BIG5、UNICODE、GB18030、UTF8
        /// </summary>
        public string TTE { get; set; } = "UTF8";
        /// <summary>
        /// 设置英文发音方式：
        /// 0：自动判断处理，如果不确定将按照英文词语拼写处理（缺省）
        /// 1：所有英文按字母发音
        /// 2：自动判断处理，如果不确定将按照字母朗读
        /// 默认按英文单词发音
        /// </summary>
        public string Reg { get; set; } = "2";

        //public string ram { get; set; } = "0";
        /// <summary>
        /// 合成音频数字发音方式
        /// 0：自动判断（默认值）
        /// 1：完全数值
        /// 2：完全字符串
        /// 3：字符串优先
        /// </summary>
        public string Rdn { get; set; } = "0";

        #endregion
    }

}