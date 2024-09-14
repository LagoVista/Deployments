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
