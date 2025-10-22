// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 86013570cf894baf02edc7a5f21c033c4b6ed0f0a6b03682200f8bc9b5c45374
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Services.NotificationClients;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Logging.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.IO;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Tests.Tak
{

    [TestClass]
    public class TakFileLoaderTests
    {        
        TakClient _client;

        [TestInitialize]
        public void Initialize() 
        {
            _client = new TakClient(new AdminLogger(new ConsoleLogWriter()));
        }
        
        private byte[] GetFileBuffer()
        {
            Assert.IsTrue(System.IO.File.Exists("kevin.zip"), "should have data package file named kevin.zip in output path");
            return System.IO.File.ReadAllBytes("kevin.zip");
        }

        [TestMethod]
        public async Task ShouldInitTak()
        {
            using (var ms = new MemoryStream(GetFileBuffer()))
            {
                var result = await _client.InitAsync(ms);
                Assert.IsTrue(result.Successful, result.ErrorMessage);
            }
        }
    }
}
