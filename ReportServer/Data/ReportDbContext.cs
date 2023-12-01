using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ReportServer.Data
{
    public class SqlDataConnectionDescription : DataConnection { }
    public class JsonDataConnectionDescription : DataConnection { }
    public abstract class DataConnection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string ConnectionString { get; set; }
    }

    public class ReportItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public byte[] LayoutData { get; set; }
    }

    public class ReportDbContext : DbContext
    {
        public DbSet<JsonDataConnectionDescription> JsonDataConnections { get; set; }
        public DbSet<SqlDataConnectionDescription> SqlDataConnections { get; set; }
        public DbSet<ReportItem> Reports { get; set; }
        public ReportDbContext(DbContextOptions<ReportDbContext> options) : base(options)
        {
        }
        public void InitializeDatabase()
        {
            Database.EnsureCreated();

            var nwindJsonDataConnectionName = "NWindProductsJson";
            if (!JsonDataConnections.Any(x => x.Name == nwindJsonDataConnectionName))
            {
                var newData = new JsonDataConnectionDescription
                {
                    Name = nwindJsonDataConnectionName,
                    DisplayName = "Northwind Products (JSON)",
                    ConnectionString = "Uri=Data/nwind.json"
                };
                JsonDataConnections.Add(newData);
            }


            var nwindSqlDataConnectionName = "NWindConnectionString";
            if (!SqlDataConnections.Any(x => x.Name == nwindSqlDataConnectionName))
            {
                var newData = new SqlDataConnectionDescription
                {
                    Name = nwindSqlDataConnectionName,
                    DisplayName = "Northwind Data Connection",
                    ConnectionString = "XpoProvider=SQLite;Data Source=|DataDirectory|/Data/nwind.db"
                };
                SqlDataConnections.Add(newData);
            }

            var reportsDataConnectionName = "ReportsDataSqlite";
            if (!SqlDataConnections.Any(x => x.Name == reportsDataConnectionName))
            {
                var newData = new SqlDataConnectionDescription
                {
                    Name = reportsDataConnectionName,
                    DisplayName = "Reports Data Sqlite",
                    ConnectionString = "XpoProvider=SQLite;Data Source=|DataDirectory|/Data/reportsData.db"
                };
                SqlDataConnections.Add(newData);
            }

            var SqlServerDataConnectionName = "GatewayConnectionString";
            if (!SqlDataConnections.Any(x => x.Name == SqlServerDataConnectionName))
            {
                var newData = new SqlDataConnectionDescription
                {
                    Name = SqlServerDataConnectionName,
                    DisplayName = "Gateway Sql Server Connection",
                    ConnectionString = "XpoProvider=MSSqlServer;Data Source=LAPTOP-F6PCCLT5\\SQL2022;User ID=sa;Password=@Test123;Initial Catalog=gateway_new;Persist Security Info=true;",

                };
                SqlDataConnections.Add(newData);
            }
            SaveChanges();
        }
    }
}