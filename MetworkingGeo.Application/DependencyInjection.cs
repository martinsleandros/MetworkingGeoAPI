using System;
using System.Reflection;
using MetworkingGeoAPI.Application.Interfaces;
using MetworkingGeoAPI.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace MetworkingGeoAPI.Application
{
    public static class DependencyInjection
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IGeoLocalizacaoService, GeoLocalizacaoService>();
            services.AddSingleton<ITimelineService, TimelineService>();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddHttpClient<ITimelineService, TimelineService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(message => HttpPolicyExtensions.HandleTransientHttpError()
                    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                    .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
        }
    }
}