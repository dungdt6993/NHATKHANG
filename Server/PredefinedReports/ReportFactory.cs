using D69soft.Server.PredefinedReports;
using DevExpress.XtraReports.UI;

namespace D69soft.Client.PredefinedReports
{
    public static class ReportFactory
    {
        public static Dictionary<string, Func<XtraReport>> Reports = new Dictionary<string, Func<XtraReport>>()
        {
            ["CustomNewReport"] = () => new CustomNewReport()
        };
    }
}
