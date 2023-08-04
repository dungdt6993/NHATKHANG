using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Globalization;

namespace D69soft.Client.Extensions
{
    public static class WebAssemblyHostExtension
    {
        public async static Task SetDefaultCulture(this WebAssemblyHost host)
        {
            var localStorege = host.Services.GetRequiredService<ILocalStorageService>();
            var cultureFromLS = await localStorege.GetItemAsStringAsync("culture");

            CultureInfo culture;

            if (cultureFromLS != null)
                culture = new CultureInfo(cultureFromLS);
            else
                culture = new CultureInfo("vi-VN");

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}
