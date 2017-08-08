using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Logging.Loggers;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LagoVista.Core.Models.UIMetaData;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public class TelemetryService : ITelemetryService
    {
        IInstrumentationKeyProvider _keys;
        IAdminLogger _adminLogger;

        public TelemetryService(IAdminLogger adminLogger, IInstrumentationKeyProvider keys)
        {
            _keys = keys;
            _adminLogger = adminLogger;
        }


        private async Task<IEnumerable<TelemetryReportData>> GetReportData(String appId, String apiKey, String filter)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-api-key", apiKey);

            _adminLogger.AddCustomEvent(Core.PlatformSupport.LogLevel.Verbose, "telemetryservice", "requesttelemetry", new KeyValuePair<string, string>("filter", filter));

            var query = System.Net.WebUtility.UrlEncode(filter);

            var uri = $"https://api.applicationinsights.io/beta/apps/{appId}/query?query={query}";

            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var str = await response.Content.ReadAsStringAsync();
                var results = JsonConvert.DeserializeObject<RootObject>(str);
                var resultsTable = results.Tables.Where(tbl => tbl.TableName == "Table_0").First();
                var column = resultsTable.Columns.Where(col => col.ColumnName == "customDimensions").First();
                var customDataIdx = resultsTable.Columns.IndexOf(column);

                column = resultsTable.Columns.Where(col => col.ColumnName == "name").First();
                var nameIdx = resultsTable.Columns.IndexOf(column);

                var appInsightReport = new List<TelemetryReportData>();

                column = resultsTable.Columns.Where(col => col.ColumnName == "timestamp").First();
                var timeStampIdx = resultsTable.Columns.IndexOf(column);
                foreach (var row in resultsTable.Rows)
                {
                    var customData = JsonConvert.DeserializeObject<TelemetryReportData>(row[customDataIdx].ToString());
                    customData.Name = row[nameIdx].ToString();
                    customData.TimeStamp = row[timeStampIdx].ToString();
                    appInsightReport.Add(customData);
                }

                return appInsightReport;
            }
            else
            {
                _adminLogger.AddError("telemetryservice", response.ReasonPhrase, new KeyValuePair<string, string>("filter", filter));
                throw new Exception(Resources.DeploymentAdminResources.Telemetry_ErrorQueryServer);
            }
        }

        private string BuildQuery(string type, string id, ListRequest request, string additionalFilter = "")
        {
            var query = $"customEvents | where customDimensions.{type} == '{id}'";

            if(string.IsNullOrEmpty(additionalFilter))
            {
                query += $" {additionalFilter}";
            }

            if (String.IsNullOrEmpty(request.EndDate))
            {
                query += $" and timestamp < datetime('{request.EndDate}')";
            }


            query += $" | top {request.PageSize} by timestamp desc";

            return query;
        }

        public Task<IEnumerable<TelemetryReportData>> GetForHostAsync(String hostId, ListRequest request)
        {
            var query = BuildQuery("hostId", hostId, request);
            return GetReportData(_keys.InstanceAppId, _keys.InstanceAPIKey, query);
        }

        public Task<IEnumerable<TelemetryReportData>> GetForInstanceAsync(String instanceId, ListRequest request)
        {
            var query = BuildQuery("instanceId", instanceId, request);
            return GetReportData(_keys.InstanceAppId, _keys.InstanceAPIKey, query);
        }

        public Task<IEnumerable<TelemetryReportData>> GetForPipelineModuleAsync(string pipelineModuleId, ListRequest request)
        {
            var query = BuildQuery("piplineModuleId", pipelineModuleId, request);
            return GetReportData(_keys.InstanceAppId, _keys.InstanceAPIKey, query);
        }

        public Task<IEnumerable<TelemetryReportData>> GetForPipelineQueueAsync(string pipelineModuleId, ListRequest request)
        {
            var query = BuildQuery("pipelineModuleId", pipelineModuleId, request, "and customDimensions.tag = 'queue'");
            return GetReportData(_keys.InstanceAppId, _keys.InstanceAPIKey, query);
        }

        public Task<IEnumerable<TelemetryReportData>> GetForDeviceAsync(string deviceId, ListRequest request)
        {
            var query = BuildQuery("deviceId", deviceId, request);
            return GetReportData(_keys.InstanceAppId, _keys.InstanceAPIKey, query);
        }

        public Task<IEnumerable<TelemetryReportData>> GetForDeviceTypeAsync(string deviceTypeId, ListRequest request)
        {
            var query = BuildQuery("deviceTypeId", deviceTypeId, request);
            return GetReportData(_keys.InstanceAppId, _keys.InstanceAPIKey, query);
        }

        class Column
        {
            public string ColumnName { get; set; }
            public string DataType { get; set; }
            public string ColumnType { get; set; }
        }

        class Table
        {
            public string TableName { get; set; }
            public List<Column> Columns { get; set; }
            public List<List<object>> Rows { get; set; }
        }

        class RootObject
        {
            public List<Table> Tables { get; set; }
        }
    }
}
