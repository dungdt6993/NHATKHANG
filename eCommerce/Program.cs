using D69soft.Client.Services.FIN;
using eCommerce;
using eCommerce.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<eCommerceService>();

//API
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["ApiUrl"])
});


var host = builder.Build();
await host.SetDefaultCulture();
await host.RunAsync();
