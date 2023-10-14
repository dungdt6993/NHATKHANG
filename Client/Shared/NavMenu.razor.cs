using D69soft.Shared.Models.ViewModels.SYSTEM;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using D69soft.Client.Services;
using D69soft.Client.Extensions;

namespace D69soft.Client.Shared
{
    partial class NavMenu
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }
        [Inject] SysService sysService { get; set; }

        bool isLoadingScreen = true;

        //Filter
        FilterVM filterVM = new();

        UserVM userVM = new();

        IEnumerable<FuncVM> modules;

        IEnumerable<FuncVM> funcMenuGrps;

        IEnumerable<FuncVM> funcMenus;

        bool ckViewFuncMenuRpt = false;

        protected override async Task OnInitializedAsync()
        {
            filterVM.UserID = (await authenticationStateTask).User.GetUserId();

            userVM = await sysService.GetInfoUser(filterVM.UserID);

            if (userVM == null)
            {
                navigationManager.NavigateTo("/Auth/Login");
            }

            modules = await sysService.GetModuleMenu(filterVM.UserID);

            funcMenuGrps = await sysService.GetFuncMenuGroup(filterVM.UserID);

            funcMenus = await sysService.GetFuncMenu(filterVM.UserID);

            ckViewFuncMenuRpt = await sysService.CheckViewFuncMenuRpt(filterVM.UserID);

            isLoadingScreen = false;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeAsync<object>("menu_treeview");
            }
        }

        private void ClickMenuFunc(string _urlFunc)
        {
            navigationManager.NavigateTo(_urlFunc, false);
        }
    }
}
