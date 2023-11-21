using System.Collections.Generic;
using System.Threading.Tasks;
using DevExpress.DataAccess.Sql;
using Microsoft.AspNetCore.Mvc;
using ReportServer.Models;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.ReportDesigner;
using DevExpress.AspNetCore.Reporting.QueryBuilder;
using DevExpress.AspNetCore.Reporting.ReportDesigner;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using DevExpress.DataAccess.ConnectionParameters;

namespace ReportServer.Controllers {
    public class HomeController : Controller {
        public IActionResult Index() {
            return View();
        }
        public IActionResult Error() {
            Models.ErrorModel model = new Models.ErrorModel();
            return View(model);
        }

        public async Task<IActionResult> Designer([FromServices] IReportDesignerClientSideModelGenerator clientSideModelGenerator, [FromQuery] string reportName, ReportDesignerCustomModel model) {
            model.ReportDesignerModel = await CreateDefaultReportDesignerModel(clientSideModelGenerator, reportName, null);
            return View(model);
        }

        public static Dictionary<string, object> GetAvailableDataSources()
        {
            var dataSources = new Dictionary<string, object>();
            return dataSources;
        }

        public static Task<ReportDesignerModel> CreateDefaultReportDesignerModel(IReportDesignerClientSideModelGenerator clientSideModelGenerator, string reportName, XtraReport report) {
            reportName = string.IsNullOrEmpty(reportName) ? "TestReport" : reportName;
            var dataSources = GetAvailableDataSources();
            if(report != null) {
                return clientSideModelGenerator.GetModelAsync(report, dataSources, ReportDesignerController.DefaultUri, WebDocumentViewerController.DefaultUri, QueryBuilderController.DefaultUri);
            }
            return clientSideModelGenerator.GetModelAsync(reportName, dataSources, ReportDesignerController.DefaultUri, WebDocumentViewerController.DefaultUri, QueryBuilderController.DefaultUri);
        }
        public async Task<IActionResult> Viewer(
            [FromServices] IWebDocumentViewerClientSideModelGenerator clientSideModelGenerator,
            [FromQuery] string reportName) {

            var reportToOpen = string.IsNullOrEmpty(reportName) ? "TestReport" : reportName;
            var model = new Models.ViewerModel {
                ViewerModelToBind = await clientSideModelGenerator.GetModelAsync(reportToOpen, WebDocumentViewerController.DefaultUri)
            };
            return View(model);
        }
    }
}