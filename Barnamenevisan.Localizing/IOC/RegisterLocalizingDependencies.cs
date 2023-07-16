using Barnamenevisan.Localizing.Repository;
using Barnamenevisan.Localizing.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Barnamenevisan.Localizing.IOC
{
    public class RegisterLocalizingDependencies
    {
        public static void Register(IServiceCollection services, Type DbContextType)
        {
            services.AddScoped<ILocalizedPropertyRepository, LocalizedPropertyRepository>(option =>
            {
                return new LocalizedPropertyRepository(option.GetRequiredService(DbContextType) as DbContext);
            });
            services.AddScoped<ILocalizingService, LocalizinService>();
        }
    }
}
