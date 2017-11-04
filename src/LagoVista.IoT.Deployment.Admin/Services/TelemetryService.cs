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


        private async Task<ListResponse<TelemetryReportData>> GetReportData(String appId, String apiKey, String filter)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-api-key", apiKey);

                var query = System.Net.WebUtility.UrlEncode(filter);

                var uri = $"https://api.applicationinsights.io/beta/apps/{appId}/query?query={query}";

                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var str = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<RootObject>(str);
                    var resultsTable = results.Tables.Where(tbl => tbl.TableName == "Table_0").FirstOrDefault();
                    if (resultsTable == null)
                    {
                        return  ListResponse<TelemetryReportData>.Create(new List<TelemetryReportData>());
                    }

                    var customDataColumn = resultsTable.Columns.Where(col => col.ColumnName == "customDimensions").FirstOrDefault();
                    var customDataIdx = customDataColumn == null ? null : (int?)resultsTable.Columns.IndexOf(customDataColumn);

                    var itemIdColumn = resultsTable.Columns.Where(col => col.ColumnName == "itemId").FirstOrDefault();
                    var itemIdColumnIndex = itemIdColumn == null ? null : (int?)resultsTable.Columns.IndexOf(itemIdColumn);

                    var itemType = resultsTable.Columns.Where(col => col.ColumnName == "itemType").FirstOrDefault();
                    int? itemTypeIndex = itemType == null ? null : (int?)resultsTable.Columns.IndexOf(itemType);

                    var nameColumn = resultsTable.Columns.Where(col => col.ColumnName == "name").FirstOrDefault();
                    int? nameIdx = nameColumn == null ? null : (int?)resultsTable.Columns.IndexOf(nameColumn);

                    var appInsightReport = new List<TelemetryReportData>();

                    customDataColumn = resultsTable.Columns.Where(col => col.ColumnName == "timestamp").First();
                    var timeStampIdx = resultsTable.Columns.IndexOf(customDataColumn);
                    foreach (var row in resultsTable.Rows)
                    {
                        TelemetryReportData reportData;
                        if(customDataIdx.HasValue)
                        {
                            reportData = JsonConvert.DeserializeObject<TelemetryReportData>(row[customDataIdx.Value].ToString());
                        }
                        else
                        {
                            reportData = new TelemetryReportData();
                        }

                        if (itemIdColumnIndex.HasValue) reportData.ItemId = row[itemIdColumnIndex.Value].ToString();
                        if (itemTypeIndex.HasValue) reportData.ItemType = row[itemTypeIndex.Value].ToString();
                        if (nameIdx.HasValue) reportData.Name = row[nameIdx.Value].ToString();
                        reportData.TimeStamp = row[timeStampIdx].ToString();
                        appInsightReport.Add(reportData);
                    }

                    return ListResponse<TelemetryReportData>.Create(appInsightReport);
                }
                else
                {
                    _adminLogger.AddError("telemetryservice", response.ReasonPhrase, new KeyValuePair<string, string>("filter", filter));
                    throw new Exception(Resources.DeploymentAdminResources.Telemetry_ErrorQueryServer);
                }
            }
        }

        private string BuildQuery(string recordType, string type, string id, ListRequest request, string additionalFilter = "")
        {
            var query = $"{recordType} | where customDimensions.{type} == '{id}'";

            if (!string.IsNullOrEmpty(additionalFilter))
            {
                query += $" {additionalFilter}";
            }

            if (!String.IsNullOrEmpty(request.EndDate))
            {
                query += $" and timestamp < datetime('{request.EndDate}')";
            }


            query += $" | top {request.PageSize} by timestamp desc";
            return query;
        }

        public Task<string> GetItemDetailsAsync(string itemId, string type)
        {
            var query = $"{type} | where itemId == '{itemId}'";
            return GetItemDetailsAsync(_keys.InstanceAppId, _keys.InstanceAPIKey, query);
        }

        private async Task<string> GetItemDetailsAsync(String appId, String apiKey, String filter)
        {
            var query = System.Net.WebUtility.UrlEncode(filter);

            var uri = $"https://api.applicationinsights.io/beta/apps/{appId}/query?query={query}";
           
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-api-key", apiKey);
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    _adminLogger.AddError("telemetryservice", response.ReasonPhrase, new KeyValuePair<string, string>("filter", query));
                    throw new Exception(Resources.DeploymentAdminResources.Telemetry_ErrorQueryServer);
                }
            }
        }

        public Task<ListResponse<TelemetryReportData>> GetForHostAsync(String hostId, string recordType, ListRequest request)
        {
            var query = BuildQuery(recordType, "hostId", hostId, request);
            return GetReportData(_keys.InstanceAppId, _keys.InstanceAPIKey, query);
        }

        public Task<ListResponse<TelemetryReportData>> GetForInstanceAsync(String instanceId, string recordType, ListRequest request)
        {
            var query = BuildQuery(recordType, "instanceId", instanceId, request);
            return GetReportData(_keys.InstanceAppId, _keys.InstanceAPIKey, query);
        }

        public Task<ListResponse<TelemetryReportData>> GetForPipelineModuleAsync(string pipelineModuleId, string recordType, ListRequest request)
        {
            var query = BuildQuery(recordType, "piplineModuleId", pipelineModuleId, request);
            return GetReportData(_keys.InstanceAppId, _keys.InstanceAPIKey, query);
        }

        public Task<ListResponse<TelemetryReportData>> GetForPipelineQueueAsync(string pipelineModuleId, string recordType, ListRequest request)
        {
            var query = BuildQuery(recordType, "pipelineModuleId", pipelineModuleId, request, "and customDimensions.tag = 'queue'");
            return GetReportData(_keys.InstanceAppId, _keys.InstanceAPIKey, query);
        }

        public Task<ListResponse<TelemetryReportData>> GetForDeviceAsync(string deviceId, string recordType, ListRequest request)
        {
            var query = BuildQuery(recordType, "deviceId", deviceId, request);
            return GetReportData(_keys.InstanceAppId, _keys.InstanceAPIKey, query);
        }

        public Task<ListResponse<TelemetryReportData>> GetForDeviceTypeAsync(string deviceTypeId, string recordType, ListRequest request)
        {
            var query = BuildQuery(recordType, "deviceTypeId", deviceTypeId, request);
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
