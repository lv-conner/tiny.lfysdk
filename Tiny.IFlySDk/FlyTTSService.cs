using System.Net.WebSockets;
using System.IO.Pipelines;
using System.Text;
using System.Text.Json;
using System.Buffers;
using Tiny.IFlySDk.Common;
using Tiny.IFlySDk.Models;
using Microsoft.Extensions.Options;

namespace Tiny.IFlySDk.Service
{
    public class FlyTTSService : IFlyTTSService
    {
        public static byte[] crlf = new byte[2] { (byte)'\r', (byte)'\n' };
        public static byte[] inputDelimter = new byte[3] { (byte)'\r', (byte)'m', (byte)'\n' };
        public JsonSerializerOptions _jsonSerializerOptions;
        private readonly IFlySdkOption _iflySdkOption;
        public FlyTTSService(IOptionsMonitor<IFlySdkOption> iflySdkOption):this(iflySdkOption.CurrentValue)
        {

        }
        public FlyTTSService(IFlySdkOption iflySdkOption)
        {
            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = IFlyTTSJsonNamingPolicy.Instance
            };
            _iflySdkOption = iflySdkOption;
        }
        public async Task<Stream> ConvertTextToAudioAsync(string text)
        {
            var url = IFlySdkHelper.BuildIFlyUrl(_iflySdkOption.TTSUrl, _iflySdkOption.ApiSecret, _iflySdkOption.ApiKey);
            var ws = new ClientWebSocket();
            var frame = new IFlyTTSRequest(_iflySdkOption.AppId, text);
            await ws.ConnectAsync(new Uri(url), CancellationToken.None);
            if (ws.State == WebSocketState.Open)
            {
                await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(frame, _jsonSerializerOptions))), WebSocketMessageType.Text, true, CancellationToken.None);
                var pipe = new Pipe();
                var readTask = ReadMessageAsync(pipe.Reader);
                var writeTask = WriteToPipeAsync(ws, pipe);
                var stream = await readTask;
                if(ws.State == WebSocketState.Open)
                {
                    await ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "NormalClosure", CancellationToken.None);
                }
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "NormalClosure", CancellationToken.None);
                return stream;
            }
            else
            {
                throw new Exception("连接到讯飞服务器失败，请检查appKey相关是否正确！");
            }
        }
        public async Task WriteToPipeAsync(ClientWebSocket ws,Pipe pipe)
        {
            while (true)
            {
                try
                {
                    var buffer = pipe.Writer.GetMemory(40960);
                    var recevicedRst = await ws.ReceiveAsync(buffer, CancellationToken.None);
                    pipe.Writer.Advance(recevicedRst.Count);
                    if (recevicedRst.EndOfMessage)
                    {
                        await pipe.Writer.WriteAsync(crlf);
                    }
                    if (recevicedRst.MessageType == WebSocketMessageType.Close && ws.State == WebSocketState.Open)
                    {
                        await ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "NormalClosure", CancellationToken.None);
                    }
                    if (recevicedRst.Count == 0)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
            pipe.Writer.Complete();
        }
        public async Task<Stream> ReadMessageAsync(PipeReader reader)
        {
            List<IFlyTTSResponse> msgs = new List<IFlyTTSResponse>();
            while (true)
            {
                var readRst = await reader.ReadAsync();
                var buffer = readRst.Buffer;
                var (readPosition, rst) = ReadData(buffer);
                reader.AdvanceTo(readPosition);
                if(rst.Count > 0)
                {
                    foreach (var item in rst)
                    {
                        msgs.Add(item);
                        if (item.Data.Status == 2)
                        {
                            Console.WriteLine("Receive msg complete!");
                            break;
                        }
                    }
                    if (msgs.Any(p => p.Data.Status == 2))
                    {
                        break;
                    }
                }
                if (readRst.IsCompleted)
                {
                    break;
                }
            }
            reader.Complete();
            var ms = new MemoryStream();
            foreach (var item in msgs)
            {
                await ms.WriteAsync(Convert.FromBase64String(item.Data.Audio));
            }
            ms.Position = 0;
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
        public void SaveMSg(List<IFlyTTSResponse> msgs)
        {
            using (var ms = new MemoryStream())
            {
                foreach (var item in msgs)
                {
                    var audioData = Convert.FromBase64String(item.Data.Audio);
                    ms.Write(audioData);
                }
                ms.Flush();
                ms.Position = 0;
                ms.Seek(0, SeekOrigin.Begin);
            }
        }
        public (SequencePosition, List<IFlyTTSResponse>) ReadData(ReadOnlySequence<byte> data)
        {
            List<IFlyTTSResponse> rst = new List<IFlyTTSResponse>();
            SequenceReader<byte> reader = new SequenceReader<byte>(data);
            if (reader.IsNext(crlf, advancePast: true))
            {
                reader.Advance(2);
            }
            //尝试读取
            if (reader.TryReadToAny(out ReadOnlySpan<byte> line, crlf, advancePastDelimiter: false))
            {
                var msg = Encoding.UTF8.GetString(line);
                IFlyTTSResponse result = JsonSerializer.Deserialize<IFlyTTSResponse>(msg, _jsonSerializerOptions);
                Console.WriteLine($"{result.Code}-{result.Message}-{result.Data.Status}-{result.Data.Ced}");
                reader.Advance(2);
                rst.Add(result);
            }
            return (reader.Position, rst);
        }
    }
}