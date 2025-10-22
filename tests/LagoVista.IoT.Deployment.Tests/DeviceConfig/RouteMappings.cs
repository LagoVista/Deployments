// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 68a8c6d52ab52d65002f542d4bf8e6023cf18bd57c74ca2866f6f2fd521a78a4
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.IoT.Deployment.Admin.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Tests.DeviceConfig
{
    [TestClass]
    public class RouteMappings
    {

        [TestMethod]
        public void Route_Mapping_Primary()
        {
            var input = new RouteModuleConfig()
            {
                Id = "4737E4F455AD40A4AB4CBDC6436E4CC0}"
            };

            var output = new RouteModuleConfig() {
                Id = "D902D1F79D414A38B4816D7AF8402DDE",
                PrimaryOutput = new RouteConnection(),
                SecondaryOutputs = new List<RouteConnection>()                 
            };

            output.SecondaryOutputs.Add(new RouteConnection()
            {
                 Id = input.Id,
                 Mappings = new List<KeyValuePair<string, object>>()
                 {
                     new KeyValuePair<string, object>("KeyOne","mappingvalue1"),
                     new KeyValuePair<string, object>("keyTwo","mappingvalue2"),
                 }
            });

            var route = new Route();
            route.PipelineModules = new List<RouteModuleConfig>();
            route.PipelineModules.Add(output);
            route.PipelineModules.Add(input);

            route.BackPopulateMappings(input);

            Assert.AreEqual(2, input.IncomingMappings.Count);
        }

    }
}
