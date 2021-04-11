using MetWorkingGeo.Infra.Interfaces;
using MetWorkingGeo.Infra.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace MetWorkingGeo.Infra
{
    public static class DependencyInjection
    {
        public static void AddMongoRepository(this IServiceCollection services)
        {
            services.AddSingleton<IDbGeolocalizacaoMongodb, DbGeolocalizacaoMongodb>();
            services.AddSingleton<IDbTimelineMongodb, DBTimelineMongodb>();
        }
    }
}