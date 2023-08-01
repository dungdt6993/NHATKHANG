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

            //HR
            services.AddTransient<ProfileService>();
            services.AddTransient<OrganizationalChartService>();
            services.AddTransient<DutyRosterService>();
            services.AddTransient<DayOffService>();
            services.AddTransient<AgreementTextService>();
            services.AddTransient<PayrollService>();

            //OP
            services.AddTransient<OPService>();

            //DOC
            services.AddTransient<DocumentService>();

            //KPI
            services.AddTransient<KPIService>();

            //FIN
            services.AddTransient<PurchasingService>();
            services.AddTransient<InventoryService>();
            services.AddTransient<VoucherService>();

            //EA
            services.AddTransient<RequestService>();

            //CRUISES
            services.AddTransient<OccupancyService>();

            //POS
            services.AddTransient<CashierService>();
            services.AddTransient<CustomerService>();

            return services;
        }
    }
}