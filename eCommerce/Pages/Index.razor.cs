using D69soft.Client.Services.FIN;
using D69soft.Shared.Models.ViewModels.FIN;
using Microsoft.AspNetCore.Components;

namespace eCommerce.Pages
{
    partial class Index
    {
        [Inject] eCommerceService eCommerceService { get; set; }

        //ItemsClass
        IEnumerable<ItemsClassVM> itemsClassVMs;

        protected override async Task OnInitializedAsync()
        {
            await GetItemsClassList();
        }

        private async Task GetItemsClassList()
        {
            itemsClassVMs = await eCommerceService.GetItemsClassList();
        }
    }
}
