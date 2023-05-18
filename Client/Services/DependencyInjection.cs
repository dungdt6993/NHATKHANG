using D69soft.Server.Services;
using D69soft.Server.Services.HR;

namespace D69soft.Client.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            //Auth
            services.AddTransient<AuthService>();

            //SYSTEM
            services.AddTransient<SysService>();

            //HR
            services.AddTransient<ProfileService>();

            return services;
        }
    }
}