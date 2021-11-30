using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tiny.IFlySDk.Common;
using Tiny.IFlySDk.Service;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceInjectionExtensions
    {
        public static IServiceCollection AddIFlyTTSService(this IServiceCollection services,IFlySdkOption option)
        {
            services.AddSingleton(option);
            services.AddTransient<IFlyTTSService, FlyTTSService>();
            return services;
        }
        public static IServiceCollection AddIFlyTTSService(this IServiceCollection services, Action<IFlySdkOption> option)
        {
            services.Configure(option);
            services.AddTransient<IFlyTTSService, FlyTTSService>();
            return services;
        }
        public static IServiceCollection AddIFlyTTSService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<IFlySdkOption>(configuration);
            services.AddTransient<IFlyTTSService, FlyTTSService>();
            return services;
        }
    }
}
