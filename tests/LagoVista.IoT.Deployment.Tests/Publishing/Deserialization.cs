// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 5e8923fa3e76021c96042d53a3aeddfd588b85dc9fb235aa7d49bafb3ebcc293
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.IoT.Deployment.Admin.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Tests.Publishing
{
    [TestClass]
    public class Deserialization
    {
        [TestMethod]
        public void Should_Successfully_Deserialization_Solution()
        {
            /*var jsonFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Publishing", "solution.json");
            if (System.IO.File.Exists(jsonFile))
            {
                var json = System.IO.File.ReadAllText(jsonFile);
                var solution = JsonConvert.DeserializeObject<Solution>(json);

            }
            else
            {
                Console.WriteLine("Not fond");
            }*/
        }

    }
}
