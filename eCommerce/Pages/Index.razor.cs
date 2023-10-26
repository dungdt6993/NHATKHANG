using D69soft.Client.Services.FIN;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static System.Net.WebRequestMethods;

namespace eCommerce.Pages
{
    partial class Index
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [Inject] HttpClient httpClient { get; set; }
        [Inject] eCommerceService eCommerceService { get; set; }

        //Filter
        FilterVM filterVM = new();

        //ItemsClass
        IEnumerable<ItemsClassVM> itemsClassVMs;

        //Items
        ItemsVM itemsVM = new();
        List<ItemsVM> itemsVMs;

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            await js.InvokeAsync<object>("fixJSFunctions");
        }

        protected override async Task OnInitializedAsync()
        {
            await GetItemsClassList();
            await GetItemsList();
        }

        private async Task GetItemsClassList()
        {
            itemsClassVMs = await eCommerceService.GetItemsClassList();
        }

        private async Task GetItemsList()
        {
            filterVM.IActive = true;
            filterVM.IsSale = true;
            itemsVMs = await eCommerceService.GetItemsList(filterVM);
        }

    }
}
