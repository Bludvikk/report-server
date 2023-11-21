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

        public static SelectQuery CreateSelectQuery(string tableName)
        {
            return SelectQueryFluentBuilder.AddTable(tableName).SelectAllColumns().Build(tableName);
        }

        public static Dictionary<string, object> GetAvailableDataSources() {
            var dataSources = new Dictionary<string, object>();
            // Create a SQL data source with the specified connection string.
            SqlDataSource ds = new SqlDataSource("Gateway");
            

            ds.Queries.Add(CreateSelectQuery("RefSupplierClass"));
            ds.Queries.Add(CreateSelectQuery("Business"));
            ds.Queries.Add(CreateSelectQuery("MasterContact"));
            ds.Queries.Add(CreateSelectQuery("Location"));
            ds.Queries.Add(CreateSelectQuery("MasterContactEmail"));
            ds.Queries.Add(CreateSelectQuery("RegistrationStatus"));
            ds.Queries.Add(CreateSelectQuery("MasterContactPhone"));
            ds.Queries.Add(CreateSelectQuery("RefSalutation"));
            ds.Queries.Add(CreateSelectQuery("Registration"));
            ds.Queries.Add(CreateSelectQuery("Inventory"));
            ds.Queries.Add(CreateSelectQuery("RefDesignation"));
            ds.Queries.Add(CreateSelectQuery("MasterItem"));
            ds.Queries.Add(CreateSelectQuery("SalesDetails"));
            ds.Queries.Add(CreateSelectQuery("RefGender"));
            ds.Queries.Add(CreateSelectQuery("ModTransferHeader"));
            ds.Queries.Add(CreateSelectQuery("ModStockDetails"));
            ds.Queries.Add(CreateSelectQuery("MasterAddress"));
            ds.Queries.Add(CreateSelectQuery("ModPosting"));
            ds.Queries.Add(CreateSelectQuery("sysdiagrams"));
            ds.Queries.Add(CreateSelectQuery("RefBrand"));
            ds.Queries.Add(CreateSelectQuery("RefAddressType"));
            ds.Queries.Add(CreateSelectQuery("RefDivision"));
            ds.Queries.Add(CreateSelectQuery("RefCountry"));
            ds.Queries.Add(CreateSelectQuery("RefFamily"));
            ds.Queries.Add(CreateSelectQuery("RefGenericType"));
            ds.Queries.Add(CreateSelectQuery("RefUOM"));
            ds.Queries.Add(CreateSelectQuery("RefBank"));
            ds.Queries.Add(CreateSelectQuery("RefCardType"));
            ds.Queries.Add(CreateSelectQuery("RefShippingMethod"));
            ds.Queries.Add(CreateSelectQuery("RefCategory"));
            ds.Queries.Add(CreateSelectQuery("RefPayment"));
            ds.Queries.Add(CreateSelectQuery("RefDiscount"));
            ds.Queries.Add(CreateSelectQuery("RefTerms"));
            ds.Queries.Add(CreateSelectQuery("MasterSupplier"));
            ds.Queries.Add(CreateSelectQuery("MasterUser"));
            ds.Queries.Add(CreateSelectQuery("RefSupplierGroup"));
            ds.Queries.Add(CreateSelectQuery("PortalUser"));
            ds.Queries.Add(CreateSelectQuery("Organization"));


            ds.RebuildResultSchema();
            dataSources.Add("Gateway", ds);
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