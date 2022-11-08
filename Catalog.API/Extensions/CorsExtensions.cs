using Catalog.API.Models;

namespace Catalog.API.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCORSPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            var origins = new List<string>();
            configuration.Bind(AppSettings.CORS_MAIN, origins);

            services.AddCors(options =>
            {
                options.AddPolicy(name: Policies.CORS_MAIN,
                    builder => builder.WithOrigins(origins.ToArray())
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            return services;
        }
    }
}