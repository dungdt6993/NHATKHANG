using D69soft.Client.Services.FIN;
using D69soft.Client.Services.HR;
using D69soft.Client.Services.OP;
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

            //SYS
            services.AddTransient<SysService>();

            //FIN
            services.AddTransient<MoneyService>();
            services.AddTransient<PurchasingService>();
            services.AddTransient<InventoryService>();
            services.AddTransient<VoucherService>();
            services.AddTransient<CustomerService>();

            //HR
            services.AddTransient<ProfileService>();
            services.AddTransient<OrganizationalChartService>();
            services.AddTransient<DutyRosterService>();
            services.AddTransient<DayOffService>();
            services.AddTransient<AgreementTextService>();
            services.AddTransient<PayrollService>();
            services.AddTransient<DocumentService>();
            services.AddTransient<KPIService>();

            //OP
            services.AddTransient<OPService>();
            services.AddTransient<RequestService>();

            return services;
        }
    }
}