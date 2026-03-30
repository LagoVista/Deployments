// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: d26de2e24172fa349f813f8882ae2b078b27c106bfd0267c59ef1af7af7a62d0
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Admin.Services;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Logging.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Tests.Notifications
{
    [TestClass]
    public class DeviceErrorQueueSchedule
    {
        protected IAdminLogger AdminLogger = new AdminLogger(new ConsoleLogWriter());

        public const string ORGID = "BB204A8050E44149873B00202B2CC11";
        public const string USERID = "321603F7FC2247C5BD203977A53BDAC7";

        protected EntityHeader OrgEH = new EntityHeader() { Id = ORGID, Text = "ORG NAME" };
        protected EntityHeader UserEH = new EntityHeader() { Id = USERID, Text = "USER NAME" };

        private Mock<IDeviceErrorHandler> _errorHandler;
        private IDeviceErrorScheduleCheckSender _deviceErrorScheduleCheckSender;
        private IDeviceErrorScheduleCheckListener _listener;
        private ServiceProvider _provider;

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

            if (String.IsNullOrEmpty(settings.AccountId)) throw new Exception("Environment Variable DEV_SB_NUVIOT_ERRS_ACCOUNT_ID must be set");
            if (String.IsNullOrEmpty(settings.UserName)) throw new Exception("Environment Variable DEV_SB_NUVIOT_ERRS_USER_NAME must be set");
            if (String.IsNullOrEmpty(settings.AccessKey)) throw new Exception("Environment Variable DEV_SB_NUVIOT_ERRS_ACCESS_KEY must be set");

            var errorSettings = new Settings()
            {
                DeviceErrorScheduleQueueSettings = settings
            };

            _errorHandler = new Mock<IDeviceErrorHandler>();

            var services = new ServiceCollection();
            services.AddScoped<IDeviceErrorHandler>(_ => _errorHandler.Object);

            _provider = services.BuildServiceProvider(new ServiceProviderOptions()
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });

            var scopeFactory = _provider.GetRequiredService<IServiceScopeFactory>();

            _deviceErrorScheduleCheckSender = new DeviceErrorScheduleCheckSender(errorSettings, AdminLogger);
            _listener = new DeviceErrorScheduleCheckListener(scopeFactory, errorSettings, AdminLogger);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (_listener != null)
            {
                await _listener.StopAsync();
            }

            _provider?.Dispose();
        }

        [TestMethod]
        public async Task SendAndReceiveMessageAsync()
        {
            var handled = new TaskCompletionSource<DeviceManagement.Models.DeviceException>(TaskCreationOptions.RunContinuationsAsynchronously);

            _errorHandler
                .Setup(x => x.HandleDeviceExceptionAsync(It.IsAny<DeviceManagement.Models.DeviceException>(), It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>()))
                .Callback<DeviceManagement.Models.DeviceException, EntityHeader, EntityHeader>((deviceException, org, user) =>
                {
                    handled.TrySetResult(deviceException);
                })
                .ReturnsAsync(InvokeResult.Success);

            await _listener.StartAsync();

            var result = await _deviceErrorScheduleCheckSender.ScheduleAsync(new Models.DeviceErrorScheduleCheck()
            {
                DeviceException = new DeviceManagement.Models.DeviceException()
                {
                    ErrorCode = "ABC1234",
                    DeviceRepositoryId = "1234",
                    DeviceUniqueId = "ADF"
                },
                Org = OrgEH,
                User = UserEH,
                DueTimeStamp = DateTime.UtcNow.AddSeconds(5).ToJSONString()
            });

            Assert.IsTrue(result.Successful);

            var completed = await Task.WhenAny(handled.Task, Task.Delay(TimeSpan.FromSeconds(30)));
            Assert.AreSame(handled.Task, completed, "Timed out waiting for the scheduled message to be processed.");

            var receivedException = await handled.Task;
            Assert.AreEqual("ABC1234", receivedException.ErrorCode);

            _errorHandler.Verify(x => x.HandleDeviceExceptionAsync(
                It.Is<DeviceManagement.Models.DeviceException>(ex => ex.ErrorCode == "ABC1234"),
                It.Is<EntityHeader>(eh => eh.Id == ORGID),
                It.Is<EntityHeader>(eh => eh.Id == USERID)),
                Times.Once);
        }
    }

    class Settings : IDeviceErrorScheduleCheckSettings
    {
        public IConnectionSettings DeviceErrorScheduleQueueSettings { get; set; }

    }
}
