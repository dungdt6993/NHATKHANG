using D69soft.Client.Extensions;
using D69soft.Server.Hubs.POS;
using Data.Infrastructure;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.Security.Resources;
using DevExpress.XtraReports.Web.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;

namespace D69soft
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Dapper.SqlMapper.Settings.CommandTimeout = 0;

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

            builder.Services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            //JWT
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JwtIssuer"],
                        ValidAudience = builder.Configuration["JwtAudience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSecurityKey"]))
                    };
                });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .SetIsOriginAllowed((host) => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            var app = builder.Build();

            var supportedCultures = new[] { new CultureInfo("vi-VN") };
            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("vi-VN"),
                SupportedCultures = supportedCultures,
            };
            app.UseRequestLocalization(localizationOptions);

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

            app.UseReporting(builder =>
            {
                builder.UserDesignerOptions.DataBindingMode =
                    DevExpress.XtraReports.UI.DataBindingMode.ExpressionsAdvanced;
            });
            app.UseDevExpressControls();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(app.Environment.ContentRootPath, "Files")),
                RequestPath = "/Data"
            });

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.MapRazorPages();

            app.MapControllers();
            app.MapHub<CashierHub>("/cashierHub");

            app.MapFallbackToFile("index.html");

<<<<<<< HEAD
=======
            string contentPath = app.Environment.ContentRootPath;
            AccessSettings.StaticResources.TrySetRules(DirectoryAccessRule.Allow(contentPath));

>>>>>>> 39a1826812327dfc264af91bfb3d7789be57deb5
            app.Run();
        }
    }
}