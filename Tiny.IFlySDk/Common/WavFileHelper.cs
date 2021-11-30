using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiny.IFlySDk.Common
{
    public class WavFileHelper
    {
        /// <summary>
        /// 创建WAV音频文件头信息
        /// </summary>
        /// <param name="data_Len">音频数据长度</param>
        /// <param name="data_SoundCH">音频声道数</param>
        /// <param name="data_Sample">采样率，常见有：11025、22050、44100等</param>
        /// <param name="data_SamplingBits">采样位数，常见有：4、8、12、16、24、32</param>
        /// <returns></returns>
        public static byte[] CreateWaveFileHeader(int data_Len, int data_SoundCH, int data_Sample, int data_SamplingBits)
        {
            // WAV音频文件头信息
            List<byte> WAV_HeaderInfo = new List<byte>();  // 长度应该是44个字节
            WAV_HeaderInfo.AddRange(Encoding.ASCII.GetBytes("RIFF"));           // 4个字节：固定格式，“RIFF”对应的ASCII码，表明这个文件是有效的 "资源互换文件格式（Resources lnterchange File Format）"
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(data_Len + 44 - 8));  // 4个字节：总长度-8字节，表明从此后面所有的数据长度，小端模式存储数据
            WAV_HeaderInfo.AddRange(Encoding.ASCII.GetBytes("WAVE"));           // 4个字节：固定格式，“WAVE”对应的ASCII码，表明这个文件的格式是WAV
            WAV_HeaderInfo.AddRange(Encoding.ASCII.GetBytes("fmt "));           // 4个字节：固定格式，“fmt ”(有一个空格)对应的ASCII码，它是一个格式块标识
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(16));                 // 4个字节：fmt的数据块的长度（如果没有其他附加信息，通常为16），小端模式存储数据
            var fmt_Struct = new
            {
                PCM_Code = (short)1,                  // 4B，编码格式代码：常见WAV文件采用PCM脉冲编码调制格式，通常为1。
                SoundChannel = (short)data_SoundCH,   // 2B，声道数
                SampleRate = (int)data_Sample,        // 4B，没个通道的采样率：常见有：11025、22050、44100等
                BytesPerSec = (int)(data_SamplingBits * data_Sample * data_SoundCH / 8),  // 4B，数据传输速率 = 声道数×采样频率×每样本的数据位数/8。播放软件利用此值可以估计缓冲区的大小。
                BlockAlign = (short)(data_SamplingBits * data_SoundCH / 8),               // 2B，采样帧大小 = 声道数×每样本的数据位数/8。
                SamplingBits = (short)data_SamplingBits,     // 4B，每个采样值（采样本）的位数，常见有：4、8、12、16、24、32
            };
            // 依次写入fmt数据块的数据（默认长度为16）
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.PCM_Code));
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.SoundChannel));
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.SampleRate));
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.BytesPerSec));
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.BlockAlign));
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.SamplingBits));
            /* 还 可以继续写入其他的扩展信息，那么fmt的长度计算要增加。*/

            WAV_HeaderInfo.AddRange(Encoding.ASCII.GetBytes("data"));             // 4个字节：固定格式，“data”对应的ASCII码
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(data_Len));             // 4个字节：正式音频数据的长度。数据使用小端模式存放，如果是多声道，则声道数据交替存放。
            /* 到这里文件头信息填写完成，通常情况下共44个字节*/
            return WAV_HeaderInfo.ToArray();
        }
    }
}
