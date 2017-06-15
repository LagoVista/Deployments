using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Logging.Loggers;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public class TelemetryService : ITelemetryService
    {
        IInstrumentationKeyProvider _keys;

        public TelemetryService(IInstrumentationKeyProvider keys)
        {
            _keys = keys;
        }


        private async Task<IEnumerable<TelemetryReportData>> GetReportData(String appId, String apiKey, String filter)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-api-key", apiKey);

            var query = System.Net.WebUtility.UrlEncode(filter);

            var uri = $"https://api.applicationinsights.io/beta/apps/{appId}/query?query={query}";

            var str = await client.GetStringAsync(uri);
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

        public Task<IEnumerable<TelemetryReportData>> GetForHostAsync(String hostId)
        {
            return GetReportData(_keys.InstanceAppId, _keys.InstanceAPIKey, $"customEvents | where customDimensions.hostId == '{hostId}' | take 100");
        }

        public Task<IEnumerable<TelemetryReportData>> GetForInstanceAsync(String instanceId)
        {
            return GetReportData(_keys.InstanceAppId, _keys.InstanceAPIKey, $"customEvents | where customDimensions.instanceId == '{instanceId}' | take 100");
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
