using D69soft.Client.Extension;
using D69soft.Server.Hubs.POS;
using Data.Infrastructure;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.XtraReports.Web.Extensions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.FileProviders;

namespace D69soft
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Dapper.SqlMapper.Settings.CommandTimeout = 180;

            var builder = WebApplication.CreateBuilder(args);

            //Sql database connection (name defined in appsettings.json)
            var SqlConnectionConfig = new SqlConnectionConfig(builder.Configuration.GetConnectionString("D69softDB"));
            builder.Services.AddSingleton(SqlConnectionConfig);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            builder.Services.AddControllers(options =>
            {
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
            });

            //Report
            builder.Services.AddDevExpressControls();
            builder.Services.AddScoped<ReportStorageWebExtension, CustomReportStorageWebExtension>();

            //SignalR
            builder.Services.AddSignalR();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseReporting(builder => {
                builder.UserDesignerOptions.DataBindingMode =
                    DevExpress.XtraReports.UI.DataBindingMode.ExpressionsAdvanced;
            });
            app.UseDevExpressControls();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(app.Environment.ContentRootPath, "DataProtection")),
                RequestPath = "/Data"
            });

            app.UseRouting();

            app.MapRazorPages();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                app.MapHub<CashierHub>("/cashierHub");
            });

            app.MapFallbackToFile("index.html");

            string contentPath = app.Environment.ContentRootPath;
            AppDomain.CurrentDomain.SetData("DXResourceDirectory", contentPath);

            app.Run();
        }
    }
}