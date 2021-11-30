using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.IO.Pipelines;
using System.Text;
using System.Text.Json;
using System.Buffers;
using Tiny.IFlySDk.Common;
using Tiny.IFlySDk.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace TestSdkApp // Note: actual namespace depends on the project name.
{
    public class Program
    {
        public static string AppId = "5c56f257";
        public static string ApiKey = "7b845bf729c3eeb97be6de4d29e0b446";
        public static string ApiSecret = "50c591a9cde3b1ce14d201db9d793b01";
        public static async Task Main(string[] args)
        {
            string text = @"正在为您查询合肥的天气情况。今天是2020年2月24日，合肥市今天多云，最低温度9摄氏度，最高温度15摄氏度，微风。";
            var provider = GetServiceProvider();
            IFlyTTSService flyTTSService = provider.GetService<IFlyTTSService>();
            var audioStream = await flyTTSService.ConvertTextToAudioAsync(text);
            var header = WavFileHelper.CreateWaveFileHeader((int)audioStream.Length, 1, 16000, 16);
            using (var fs = new FileStream($"{Guid.NewGuid().ToString()}.wav", FileMode.Create, FileAccess.ReadWrite))
            {
                await fs.WriteAsync(header);
                await audioStream.CopyToAsync(fs);
                await fs.FlushAsync();
                audioStream.Close();
                audioStream.Dispose();
            }
            Console.ReadLine();
            return;
        }
        static IServiceProvider GetServiceProvider()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsetting.json", false);
            var configuration = configurationBuilder.Build();
            IServiceCollection services = new ServiceCollection();
            services.AddIFlyTTSService(configuration.GetSection("IFlySdkOption"));
            services.AddSingleton(configuration);

            return services.BuildServiceProvider();
        }
    }

}