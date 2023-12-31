using System.Web;
using D69soft.Client.PredefinedReports;
using DevExpress.XtraReports.UI;

namespace D69soft.Client.Extensions
{
    public class CustomReportStorageWebExtension : DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension
    {
        readonly string ReportDirectory;
        const string FileExtension = ".repx";

        public CustomReportStorageWebExtension(IWebHostEnvironment env)
        {
            ReportDirectory = Path.Combine(env.ContentRootPath, "Files/Reports");
            if (!Directory.Exists(ReportDirectory))
            {
                Directory.CreateDirectory(ReportDirectory);
            }
        }

        private bool IsWithinReportsFolder(string reportName, string folder)
        {
            var rootDirectory = new DirectoryInfo(folder);
            var fileInfo = new FileInfo(Path.Combine(folder, reportName));
            return fileInfo.Directory.FullName.ToLower().StartsWith(rootDirectory.FullName.ToLower());
        }

        public override bool CanSetData(string reportName)
        {
            // Determines whether or not it is possible to store a report by a given URL. 
            // For instance, make the CanSetData method return false for reports that should be read-only in your storage. 
            // This method is called only for valid URLs (i.e., if the IsValidUrl method returned true) before the SetData method is called.

            return true;
        }

        public override bool IsValidUrl(string reportName)
        {
            // Determines whether or not the URL passed to the current Report Storage is valid. 
            // For instance, implement your own logic to prohibit URLs that contain white spaces or some other special characters. 
            // This method is called before the CanSetData and GetData methods.

            return Path.GetFileName(reportName) == reportName;
        }

        public override byte[] GetData(string url)
        {
            try
            {
                string[] parts = url.Split("?");
                string reportName = parts[0];
                string parametersString = parts.Length > 1 ? parts[1] : String.Empty;
                XtraReport report = null;

                if (Directory.EnumerateFiles(ReportDirectory).
                    Select(Path.GetFileNameWithoutExtension).Contains(reportName))
                {
                    byte[] reportBytes = File.ReadAllBytes(
                        Path.Combine(ReportDirectory, reportName + FileExtension));
                    using (MemoryStream ms = new MemoryStream(reportBytes))
                        report = XtraReport.FromStream(ms);
                }
                if (ReportFactory.Reports.ContainsKey(reportName))
                {
                    report = ReportFactory.Reports[reportName]();
                }

                if (report != null)
                {
                    // Assign parameters here
                    var parameters = HttpUtility.ParseQueryString(parametersString);

                    foreach (string parameterName in parameters.AllKeys)
                    {
                        report.Parameters[parameterName].Value = Convert.ChangeType(
                        parameters.Get(parameterName), report.Parameters[parameterName].Type);
                    }

                    report.RequestParameters = false;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        report.SaveLayoutToXml(ms);
                        return ms.ToArray();
                    }

                }
            }
            catch (Exception ex)
            {
                throw new DevExpress.XtraReports.Web.ClientControls.FaultException(
                    "Could not get report data.", ex);
            }
            throw new DevExpress.XtraReports.Web.ClientControls.FaultException(
                string.Format("Could not find report '{0}'.", url));
        }

        public override Dictionary<string, string> GetUrls()
        {
            // Returns a dictionary of the existing report URLs and display names. 
            // This method is called when running the Report Designer, 
            // before the Open Report and Save Report dialogs are shown and after a new report is saved to a storage.

            return Directory.GetFiles(ReportDirectory, "*" + FileExtension)
                                     .Select(Path.GetFileNameWithoutExtension)
                                     .Union(ReportFactory.Reports.Select(x => x.Key).Where(x => x != "CustomNewReport"))
                                     .ToDictionary<string, string>(x => x);
        }

        public override void SetData(XtraReport report, string reportName)
        {
            // Stores the specified report to a Report Storage using the specified URL. 
            // This method is called only after the IsValidUrl and CanSetData methods are called.
            if (!IsWithinReportsFolder(reportName, ReportDirectory))
                throw new DevExpress.XtraReports.Web.ClientControls.FaultException("Invalid report name.");
            report.SaveLayoutToXml(Path.Combine(ReportDirectory, reportName + FileExtension));
        }

        public override string SetNewData(XtraReport report, string defaultReportName)
        {
            // Stores the specified report using a new URL. 
            // The IsValidUrl and CanSetData methods are never called before this method. 
            // You can validate and correct the specified URL directly in the SetNewData method implementation 
            // and return the resulting URL used to save a report in your storage.
            SetData(report, defaultReportName);
            return defaultReportName;
        }
    }
}
