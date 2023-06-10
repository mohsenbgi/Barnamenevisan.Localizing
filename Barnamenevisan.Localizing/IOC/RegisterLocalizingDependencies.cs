using Barnamenevisan.Localizing.Repository;
using Barnamenevisan.Localizing.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Barnamenevisan.Localizing.IOC
{
    public class RegisterLocalizingDependencies
    {
        public static void Register(IServiceCollection services)
        {
            services.AddScoped<ILocalizedPropertyRepository, LocalizedPropertyRepository>();
            services.AddScoped<ILocalizingService, LocalizinService>();
        }
    }
}
