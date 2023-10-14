using ClosedXML.Report;
using D69soft.Shared.Models.ViewModels.FIN;

namespace D69soft.Client.XLS;

public class UseTemplateXLS
{
    
    public byte[] Edition(Stream streamTemplate, List<InvoiceVM> data)
    {
        var template = new XLTemplate(streamTemplate);
        
		template.AddVariable("WeatherForecasts", data);
        template.Generate();

        MemoryStream XLSStream = new();
        template.SaveAs(XLSStream);
        

        return XLSStream.ToArray();
    }
}
