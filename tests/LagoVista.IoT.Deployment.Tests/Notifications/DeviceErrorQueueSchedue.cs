using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Admin.Services;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Logging.Utils;
using LagoVista.UserAdmin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Tests.Notifications
{
    [TestClass]
    public class DeviceErrorQueueSchedue 
    {

        protected IAdminLogger AdminLogger = new AdminLogger(new ConsoleLogWriter());
        
        public const string ORGID = "BB204A8050E44149873B00202B2CC11";
        public const string USERID = "321603F7FC2247C5BD203977A53BDAC7";

        protected EntityHeader OrgEH = new EntityHeader() { Id = ORGID, Text = "ORG NAME" };
        protected EntityHeader UserEH = new EntityHeader() { Id = USERID, Text = "USEr NAME" };

        Mock<IDeviceErrorHandler> _errorHandler = new Mock<IDeviceErrorHandler>();

        IDeviceErrorScheduleCheckSender _deviceErrorScheduleCheckSender;
        IDeviceErrorScheduleCheckListener _listener;

        [TestInitialize]
        public void Init()
        {
            var settings = new ConnectionSettings()
            {
                AccountId = Environment.GetEnvironmentVariable("DEV_SB_NUVIOT_ERRS_ACCOUNT_ID"),
                UserName = Environment.GetEnvironmentVariable("DEV_SB_NUVIOT_ERRS_USER_NAME"),
                AccessKey = Environment.GetEnvironmentVariable("DEV_SB_NUVIOT_ERRS_ACCESS_KEY"),
                ResourceName = "deviceerrorescalationqueue"
            };

            if(String.IsNullOrEmpty(settings.AccountId)) throw new Exception("Environment Variable DEV_SB_NUVIOT_ERRS_ACCOUNT_ID must be set");
            if (String.IsNullOrEmpty(settings.UserName)) throw new Exception("Environment Variable DEV_SB_NUVIOT_ERRS_USER_NAME must be set");
            if (String.IsNullOrEmpty(settings.AccessKey)) throw new Exception("Environment Variable DEV_SB_NUVIOT_ERRS_ACCESS_KEY must be set");

            var errorSettings = new Settings()
            {
                DeviceErrorScheduleQueueSettings = settings
            };
            
            _deviceErrorScheduleCheckSender = new DeviceErrorScheduleCheckSender(errorSettings,AdminLogger); 
            _listener = new DeviceErrorScheduleCheckListener(_errorHandler.Object, errorSettings, AdminLogger);  
        }

        [TestMethod]
        public async Task SendNotificationAsync()
        {
            var result = await _deviceErrorScheduleCheckSender.ScheduleAsync(new Models.DeviceErrorScheduleCheck()
            {
                DeviceException = new DeviceManagement.Models.DeviceException() {  ErrorCode = "ABC1234", DeviceRepositoryId = "1234", DeviceUniqueId = "ADF"} ,
                Org = OrgEH,
                User = UserEH,
               
                DueTimeStamp = DateTime.UtcNow.AddSeconds(30).ToJSONString()

            });

            Assert.IsTrue(result.Successful);
        }


        [TestMethod]
        public async Task ReceiveMessageAsync()
        {
            await _listener.StartAsync();

            await Task.Delay(5000);

            await _listener.StopAsync();
        
        }
    }



    class Settings : IDeviceErrorScheduleCheckSettings
    {
        public IConnectionSettings DeviceErrorScheduleQueueSettings { get; set; }

    }
}
