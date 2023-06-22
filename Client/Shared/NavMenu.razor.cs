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

        protected string UserID;

        UserVM userVM = new();

        IEnumerable<FuncVM> modules;

        IEnumerable<FuncVM> funcMenuGrps;

        IEnumerable<FuncVM> funcMenus;

        bool ckViewFuncMenuRpt = false;

        protected override async Task OnInitializedAsync()
        {
            UserID = (await authenticationStateTask).User.GetUserId();

            userVM = await sysService.GetInfoUser(UserID);

            if (userVM == null) {
                navigationManager.NavigateTo("/Auth/Login");
            }

            modules = await sysService.GetModuleMenu(UserID);

            funcMenuGrps = await sysService.GetFuncMenuGroup(UserID);

            funcMenus = await sysService.GetFuncMenu(UserID);

            ckViewFuncMenuRpt = await sysService.CheckViewFuncMenuRpt(UserID);

            isLoadingScreen = false;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await js.InvokeAsync<object>("menu_treeview");
        }

        private void ClickMenuFunc(string _urlFunc)
        {
            navigationManager.NavigateTo(_urlFunc, true);
        }
    }
}
