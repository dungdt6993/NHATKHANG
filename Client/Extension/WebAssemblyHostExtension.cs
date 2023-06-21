using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Globalization;

namespace D69soft.Client.Extension
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
                culture = new CultureInfo("en-US");

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}
