using D69soft.Client.Services.FIN;
using D69soft.Shared.Models.ViewModels.FIN;
using D69soft.Shared.Models.ViewModels.SYSTEM;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace eCommerce.Pages
{
    partial class Items
    {
        [Inject] IJSRuntime js { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [Inject] HttpClient httpClient { get; set; }
        [Inject] eCommerceService eCommerceService { get; set; }

        //Filter
        FilterVM filterVM = new();

        //ItemsClass
        IEnumerable<ItemsClassVM> itemsClassVMs;

        //ItemsGroup
        IEnumerable<ItemsGroupVM> itemsGroupVMs;

        //Items
        ItemsVM itemsVM = new();
        List<ItemsVM> itemsVMs;

        protected override async Task OnInitializedAsync()
        {
            await GetItemsList();
            await GetItemsGroupList();
        }

        private async Task GetItemsGroupList()
        {
            itemsGroupVMs = await eCommerceService.GetItemsGroupList();
        }

        private async Task GetItemsList()
        {
            filterVM.IActive = true;
            filterVM.IsSale = true;
            itemsVMs = await eCommerceService.GetItemsList(filterVM);
        }
    }
}
