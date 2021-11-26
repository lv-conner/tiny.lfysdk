using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.IO.Pipelines;
using System.Text;
using System.Text.Json;
using System.Buffers;

namespace TestSdkApp // Note: actual namespace depends on the project name.
{

    public interface IFlyTTSService
    {
        Task<Stream> ConvertTextToAudioAsync(string text);
    }
    public class FlyTTSService : IFlyTTSService
    {
        public string AppId = "5c56f257";
        public string ApiKey = "7b845bf729c3eeb97be6de4d29e0b446";
        public string ApiSecret = "50c591a9cde3b1ce14d201db9d793b01";
        public static byte[] crlf = new byte[2] { (byte)'\r', (byte)'\n' };

        public Task<Stream> ConvertTextToAudioAsync(string text)
        {
            throw new NotImplementedException();
        }
    }
    public class IFlyWebSocket
    {

    }
    public class Program
    {
        public static string AppId = "5c56f257";
        public static string ApiKey = "7b845bf729c3eeb97be6de4d29e0b446";
        public static string ApiSecret = "50c591a9cde3b1ce14d201db9d793b01";
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var iflySetting = new IFlySdkOption()
            {
                ApiKey = ApiKey,
                ApiSecret = ApiSecret,
                AppID = AppId,
                ApiType = ApiType.TTS
            };
            var url = IFlySdkHelper.BuildAuthUrl(iflySetting);
            var url1 = IFlySdkHelper.BuildAuthUrlCore(new Uri("wss://tts-api.xfyun.cn/v2/tts"), ApiSecret, ApiKey);

            string text = "正在为您查询合肥的天气情况。今天是2020年2月24日，合肥市今天多云，最低温度9摄氏度，最高温度15摄氏度，微风。";
            var ws = new ClientWebSocket();
            var frame = new TTSFrameData(AppId, text);

            await ws.ConnectAsync(new Uri(url1), CancellationToken.None);
            if(ws.State == WebSocketState.Open)
            {
                await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(frame))), WebSocketMessageType.Text, true, CancellationToken.None);
                var pipe = new Pipe();
                var _ = ReadMessageAsync(pipe.Reader);
                while (true)
                {
                    try
                    {
                        var buffer = pipe.Writer.GetMemory(4096);
                        Console.WriteLine("Reading");
                        var recevicedRst = await ws.ReceiveAsync(buffer, CancellationToken.None);
                        Console.WriteLine($"Count:{recevicedRst.Count},MessageType:${recevicedRst.MessageType},bool End of Message ${recevicedRst.EndOfMessage}");
                        pipe.Writer.Advance(recevicedRst.Count);
                        Console.WriteLine($"Advance{recevicedRst.Count}");
                        await pipe.Writer.FlushAsync();
                        Console.WriteLine($"FlushAsync{recevicedRst.Count}");
                        if(recevicedRst.EndOfMessage)
                        {
                            await pipe.Writer.WriteAsync(crlf);
                        }
                        if (recevicedRst.MessageType == WebSocketMessageType.Close)
                        {
                            Console.WriteLine("Close Socket!");
                            await ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "NormalClosure", CancellationToken.None);
                        }
                        if (recevicedRst.Count == 0)
                        {
                            break;
                        }
                        Console.WriteLine("Continue");
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        break;
                    }
                }
                pipe.Writer.Complete();
                Console.WriteLine("Start Read");
                Console.ReadLine();
            }
            Console.WriteLine(url);
            Console.ReadLine();
        }
        public static async Task ReadMessageAsync(PipeReader reader)
        {
            List<TTSResult> msgs = new List<TTSResult>();
            while (true)
            {
                var readRst = await reader.ReadAsync();
                var buffer = readRst.Buffer;

                var (readPosition, rst) = ReadData(buffer);
                reader.AdvanceTo(readPosition);
                foreach (var item in rst)
                {
                    msgs.Add(item);
                    if (item.Data.Status == 2)
                    {
                        Console.WriteLine("Receive msg complete!");
                        SaveMSg(msgs);
                        msgs = new List<TTSResult>();
                    }
                }
                if (readRst.IsCompleted)
                {
                    break;
                }
            }
            reader.Complete();
        }
        /// <summary>
        /// 创建WAV音频文件头信息
        /// </summary>
        /// <param name="data_Len">音频数据长度</param>
        /// <param name="data_SoundCH">音频声道数</param>
        /// <param name="data_Sample">采样率，常见有：11025、22050、44100等</param>
        /// <param name="data_SamplingBits">采样位数，常见有：4、8、12、16、24、32</param>
        /// <returns></returns>
        private static byte[] CreateWaveFileHeader(int data_Len, int data_SoundCH, int data_Sample, int data_SamplingBits)
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
        /// <summary>
        /// 将用户输入写入到Pipe中。
        /// </summary>
        /// <param name="pipeWriter"></param>
        /// <returns></returns>
        public static async Task ReadUserInputWriterToPipe(PipeWriter pipeWriter)
        {
            while(true)
            {
                Console.WriteLine("请输入待转文本！");
                await Task.Yield();
                var str = Console.ReadLine().Trim();
                var frame = new TTSFrameData(AppId, str);
                var ss = JsonSerializer.Serialize(frame);
                Encoding.UTF8.GetBytes(ss, pipeWriter);
                await pipeWriter.FlushAsync();
            }
        }
        /// <summary>
        /// 将数据从pipe读取出来，然后写入到websocket中
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="pipeReader"></param>
        /// <returns></returns>
        public static async Task WriterDataToWebSocket(ClientWebSocket ws,PipeReader pipeReader)
        {
            while(true)
            {
                var readRst = await pipeReader.ReadAsync();
                var data = readRst.Buffer;
                //如何判断用户已经写入完毕
                if(readRst.Buffer.IsSingleSegment)
                {
                    await ws.SendAsync(readRst.Buffer.First, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    foreach (var item in readRst.Buffer)
                    {
                        await ws.SendAsync(readRst.Buffer.First, WebSocketMessageType.Text, false, CancellationToken.None);
                    }
                    await ws.SendAsync(new ArraySegment<byte>(), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                pipeReader.AdvanceTo(readRst.Buffer.End);
                if (readRst.IsCompleted)
                {
                    break;
                }
            }
            pipeReader.Complete();
        }
        public static byte[] crlf = new byte[2] { (byte)'\r',(byte)'\n' };
        /// <summary>
        /// 将数据从ws中读取出来，写入到pipe中
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="pipeWriter"></param>
        /// <returns></returns>
        public static async Task ReceviceIlfyDataAsync(ClientWebSocket ws, PipeWriter pipeWriter)
        {

            while (true)
            {
                var buffer = pipeWriter.GetMemory(8000);
                var receiveResult = await ws.ReceiveAsync(buffer, CancellationToken.None);
                if (receiveResult.Count == 0)
                {
                    break;
                }
                else
                {
                    pipeWriter.Advance(receiveResult.Count);
                }
                if (receiveResult.EndOfMessage)
                {
                    await pipeWriter.WriteAsync(new ReadOnlyMemory<byte>(crlf));
                    pipeWriter.Advance(2);
                }
                await pipeWriter.FlushAsync();
            }
            pipeWriter.Complete();
        }
        /// <summary>
        /// 将websocket写入到pipe中的数据读取出来。
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static async Task ReadWebSocketDataAsync(PipeReader reader)
        {
            List<TTSResult> msgs = new List<TTSResult>();
            while(true)
            {
                var readRst = await reader.ReadAsync();
                var buffer = readRst.Buffer;

                var (readPosition,rst) = ReadData(buffer);
                reader.AdvanceTo(readPosition);
                foreach (var item in rst)
                {
                    msgs.Add(item);
                    if(item.Data.Status == 2)
                    {
                        Console.WriteLine("Receive msg complete!");
                        SaveMSg(msgs);
                        msgs = new List<TTSResult>();
                    }
                }
                if (readRst.IsCompleted)
                {
                    break;
                }
            }
            reader.Complete();
        }
        public static void SaveMSg(List<TTSResult> msgs)
        {
            using(var ms = new MemoryStream())
            {
                foreach (var item in msgs)
                {
                    var audioData = Convert.FromBase64String(item.Data.Audio);
                    ms.Write(audioData);
                }
                ms.Flush();
                ms.Position = 0;
                ms.Seek(0, SeekOrigin.Begin);
                using (var file = new FileStream($"{Guid.NewGuid().ToString()}.wav", FileMode.Create, FileAccess.Write))
                {
                    var header = CreateWaveFileHeader((int)ms.Length,1,16000,16);
                    file.Write(header);
                    ms.CopyTo(file);
                    file.Flush();
                    file.Close();
                }
            }
        }
        public static (SequencePosition, List<TTSResult>) ReadData(ReadOnlySequence<byte> data)
        {
            List<TTSResult> rst = new List<TTSResult>();
            SequenceReader<byte> reader = new SequenceReader<byte>(data);
            if (reader.IsNext(crlf, advancePast: true))
            {
                reader.Advance(2);
            }
            //尝试读取
            if (reader.TryReadToAny(out ReadOnlySpan<byte> line, crlf, advancePastDelimiter: false))
            {
                var msg = Encoding.UTF8.GetString(line);
                Console.WriteLine("Read an message");
                //Console.WriteLine($"Receviced Msg {msg}");
                TTSResult result = JsonSerializer.Deserialize<TTSResult>(msg, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
                Console.WriteLine(result.Data.Status);
                reader.Advance(2);
                rst.Add(result);
            }
            return (reader.Position,rst);
        }
    }

}