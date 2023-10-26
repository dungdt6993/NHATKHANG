using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Globalization;

namespace eCommerce.Extensions
{
    public static class WebAssemblyHostExtension
    {
        public async static Task SetDefaultCulture(this WebAssemblyHost host)
        {
            CultureInfo culture = new CultureInfo("vi-VN");

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}
