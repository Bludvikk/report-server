using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevExpress.XtraReports.UI;
using ReportServer.PredefinedReports;
using ReportServer.Data;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Web.Extensions;
using System.Web;
using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;
using Microsoft.Extensions.Configuration;

namespace ReportServer.Services
{
    public class CustomReportStorageWebExtension : ReportStorageWebExtension
    {
        public string reportDirectory = "C:\\Users\\bgcor\\OneDrive\\Desktop\\ReportDir\\";

        private readonly IConfiguration _configuration;
        private readonly ReportDbContext _dbContext;

        public Dictionary<string, XtraReport> Reports = new Dictionary<string, XtraReport>();
        protected ReportDbContext DbContext { get; set; }
        public CustomReportStorageWebExtension(ReportDbContext dbContext, IConfiguration configuration)
        {
            this.DbContext = dbContext;
            this._configuration = configuration;


            string[] files = Directory.GetFiles(reportDirectory);

            foreach (var file in files)
            {
                string url = Path.GetFileNameWithoutExtension(file);
                XtraReport report = XtraReport.FromFile(file, true);

                Reports.Add(url, report);
            }
        }



        public override bool CanSetData(string url)
        {
            // Determines whether a report with the specified URL can be saved.
            // Add custom logic that returns **false** for reports that should be read-only.
            // Return **true** if no valdation is required.
            // This method is called only for valid URLs (if the **IsValidUrl** method returns **true**).

            return true;
        }

        public override bool IsValidUrl(string url)
        {
            // Determines whether the URL passed to the current report storage is valid.
            // Implement your own logic to prohibit URLs that contain spaces or other specific characters.
            // Return **true** if no validation is required.

            return true;
        }

        public override byte[] GetData(string url)
        {
            var parts = url.Split('?');
            var reportName = parts[0];
            var parameter = parts.Length > 1 ? parts[1] : null;

            Debug.WriteLine("params", parameter);

            if (Reports.TryGetValue(reportName, out XtraReport report))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    if (parameter != null)
                    {
                        var parameters = HttpUtility.ParseQueryString(parameter);
                        var paramValue = parameters.Get("Business");
                        report.RequestParameters = false;
                        report.Parameters["Business"].Value = paramValue;
                    }
                    report.SaveLayoutToXml(stream);
                    return stream.ToArray();
                }
            }
            else
            {
                return null; // or return new byte[0];
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

            using (FileStream stream = new FileStream(reportDirectory + url + ".resx", FileMode.Create))
            {
                report.SaveLayoutToXml(stream);
            }
            SaveReportNamesToDatabase(url);

        }

        public override string SetNewData(XtraReport report, string defaultUrl)
        {
            SetData(report, defaultUrl);

            using (FileStream stream = new FileStream(reportDirectory + defaultUrl + ".resx", FileMode.Create))
            {
                report.SaveLayoutToXml(stream);
            }

            

            return defaultUrl;
        }

        public void SaveReportNamesToDatabase(string reportName)
        {
            var newFormat = reportName.Replace("-", " ");

            var optionsBuilder = new DbContextOptionsBuilder<ReportDbContext>();
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("GatewayConnectionString"));

            using (var context = new ReportDbContext(optionsBuilder.Options))
            {
                var newReport = new Report
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = newFormat,
                    ReportName = reportName,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                context.Report.Add(newReport);
                context.SaveChanges();
            }
        }
    }
}