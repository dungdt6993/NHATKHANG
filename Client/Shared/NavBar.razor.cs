using D69soft.Client.Extensions;
using D69soft.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace D69soft.Client.Shared
{
    partial class NavBar
    {
        [Inject] AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject] AuthService authService { get; set; }

        protected string UserID;

        //List<LogNotificationVM> notifications;

        //protected override async Task OnInitializedAsync()
        //{
        //    UserID = (await authenticationStateTask).User.GetUserId();

        //    notifications = await sysService.GetNotifications(UserID);
        //}

        private async Task Logout()
        {
            await authService.Logout();
        }

        //private async Task SeenNotificationsALL()
        //{
        //    await sysService.SeenNotificationsALL(UserID);
        //    notifications = await sysService.GetNotifications(UserID);
        //}
    }
}
