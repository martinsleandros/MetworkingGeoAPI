using MetworkingGeoAPI.Application.Interfaces;
using MetworkingGeoAPI.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MetworkingGeoAPI.Application
{
    public static class DependencyInjection
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IGeoLocalizacaoService, GeoLocalizacaoService>();
        }
    }
}