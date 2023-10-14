using D69soft.Shared.Models.ViewModels.HR;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using D69soft.Client.Services;

namespace D69soft.Client.Pages.SYS
{
    partial class Log
    {
        [Inject] SysService sysService { get; set; }

        LogVM logVM = new();

        bool isLoadingScreen;

        List<LogVM> logVMs;

        FilterVM FilterVM = new();

        protected override async Task OnInitializedAsync()
        {
            logVMs = await sysService.GetLog();

            isLoadingScreen = false;
        }
    }
}
