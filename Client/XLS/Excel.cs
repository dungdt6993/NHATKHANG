using D69soft.Shared.Models.ViewModels.FIN;
using Microsoft.JSInterop;
namespace D69soft.Client.XLS;

public class Excel
{
    public async Task TemplateWeatherForecastAsync(IJSRuntime js, 
                                                   Stream streamTemplate,
                                                   List<InvoiceVM> data, 
                                                   string filename)
    {
        var templateXLS = new UseTemplateXLS();
		var XLSStream = templateXLS.Edition(streamTemplate, data);

        await js.InvokeVoidAsync("BlazorDownloadFile", filename, XLSStream);
    }

}
