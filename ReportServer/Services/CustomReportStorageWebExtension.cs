using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevExpress.XtraReports.UI;
using ReportServer.PredefinedReports;
using ReportServer.Data;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Web.Extensions;

namespace ReportServer.Services
{
    public class CustomReportStorageWebExtension : ReportStorageWebExtension
    {

        public Dictionary<string, XtraReport> Reports = new Dictionary<string, XtraReport>();
        protected ReportDbContext DbContext { get; set; }
        public CustomReportStorageWebExtension(ReportDbContext dbContext) {
            this.DbContext = dbContext;

            string reportDirectory = "C:\\Users\\bgcor\\OneDrive\\Desktop\\ReportDir\\";

            string[] files = Directory.GetFiles(reportDirectory);

            foreach(var file in files)
            {
                string url = Path.GetFileNameWithoutExtension(file);
                XtraReport report = XtraReport.FromFile(file, true);

                Reports.Add(url, report);
            }
        }

        

        public override bool CanSetData(string url) {
            // Determines whether a report with the specified URL can be saved.
            // Add custom logic that returns **false** for reports that should be read-only.
            // Return **true** if no valdation is required.
            // This method is called only for valid URLs (if the **IsValidUrl** method returns **true**).

            return true;
        }

        public override bool IsValidUrl(string url) {
            // Determines whether the URL passed to the current report storage is valid.
            // Implement your own logic to prohibit URLs that contain spaces or other specific characters.
            // Return **true** if no validation is required.

            return true;
        }

        public override byte[] GetData(string url)
        {
            //var parts = url.Split('?');
            //var reportName = parts[0];
            //
            //Debug.WriteLine("BusinessCode: " + url);

            if (Reports.TryGetValue(url, out XtraReport report))
            {
                using (MemoryStream stream = new MemoryStream())
                {

                    report.RequestParameters = false;
                    report.SaveLayoutToXml(stream);
                    return stream.ToArray();
                }
            }
            else
            {
                return null;
            }
        }

        public override Dictionary<string, string> GetUrls()
        {
            return Reports.ToDictionary(x => x.Key, y => y.Key);
        }

        public override void SetData(XtraReport report, string url)
        {
            if (Reports.ContainsKey(url))
            {
                Reports[url] = report;
            }
            else
            {
                Reports.Add(url, report);
            }

            using (FileStream stream = new FileStream("C:\\Users\\bgcor\\OneDrive\\Desktop\\ReportDir\\" + url + ".resx", FileMode.Create))
            {
                report.SaveLayoutToXml(stream);
            }
        }

        public override string SetNewData(XtraReport report, string defaultUrl)
        {
            SetData(report, defaultUrl);

            using (FileStream stream = new FileStream("C:\\Users\\bgcor\\OneDrive\\Desktop\\ReportDir\\" + defaultUrl + ".resx", FileMode.Create))
            {
                report.SaveLayoutToXml(stream);
            }

            return defaultUrl;
        }
    }
}