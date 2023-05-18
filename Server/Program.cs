using D69soft.Server.Services;
using Data.Infrastructure;
using DevExpress.XtraCharts;
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

            builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

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

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "DataProtection")),
                RequestPath = "/Data"
            });

            app.UseRouting();


            app.MapRazorPages();
            app.MapControllers();
            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}