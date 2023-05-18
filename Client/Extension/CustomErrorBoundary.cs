using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace D69soft.Client.Extension
{
    public class CustomErrorBoundary : ErrorBoundary
    {
        [Inject]
        private IWebAssemblyHostEnvironment webAssemblyHostEnvironment { get; set; }

        protected override Task OnErrorAsync(Exception exception)
        {
            if (webAssemblyHostEnvironment.IsDevelopment())
                return base.OnErrorAsync(exception);
            else
                return Task.CompletedTask;
        }
    }
}
