using Blazored.LocalStorage;
using D69soft.Client.Extensions;
using D69soft.Client.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Globalization;

namespace D69soft.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();

            //Service
            builder.Services.AddInfrastructure();

            //Auth
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

            //API
            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(builder.Configuration["BaseAddress"])
            });

            var host = builder.Build();
            await host.SetDefaultCulture();
            await host.RunAsync();
        }
    }
}