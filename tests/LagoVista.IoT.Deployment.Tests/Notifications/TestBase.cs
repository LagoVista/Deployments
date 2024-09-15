using LagoVista.Core.Interfaces;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Logging.Utils;
using LagoVista.MediaServices.Interfaces;
using LagoVista.UserAdmin.Interfaces.Repos.Orgs;
using LagoVista.UserAdmin.Interfaces.Repos.Users;
using Moq;

namespace LagoVista.IoT.Deployment.Tests.Notifications
{
    public class TestBase
    {

        protected IAdminLogger AdminLogger = new AdminLogger(new ConsoleLogWriter());
        protected Mock<ILinkShortener> LinkShortener = new Mock<ILinkShortener>();
        protected Mock<UserAdmin.Interfaces.Managers.IEmailSender> EmailMessageSender = new Mock<UserAdmin.Interfaces.Managers.IEmailSender>();
        protected Mock<IDistributionListRepo> DistroLibRepo = new Mock<IDistributionListRepo>();
        protected Mock<IDeviceNotificationRepo> NotificationRepo = new Mock<IDeviceNotificationRepo>();
        protected Mock<IOrgLocationRepo> OrgLocationRepo = new Mock<IOrgLocationRepo>();
        protected Mock<IEmailSender> EmailSender = new Mock<IEmailSender>();
        protected Mock<ISecureStorage> SecureStorege = new Mock<ISecureStorage>();
        protected Mock<UserAdmin.Interfaces.Managers.ISmsSender> SMSMessageSender = new Mock<UserAdmin.Interfaces.Managers.ISmsSender>();
        protected Mock<ISMSSender> SMSSender = new Mock<ISMSSender>();
        protected Mock<IRestSender> RestSender = new Mock<IRestSender>();
        protected Mock<IMqttSender> MqttSender = new Mock<IMqttSender>();
        protected Mock<IDeploymentInstanceRepo> DeploymentInstanceRepo = new Mock<IDeploymentInstanceRepo>();
        protected Mock<ICOTSender> COTSender = new Mock<ICOTSender>();
        protected Mock<IAppUserRepo> AppUserRepo = new Mock<IAppUserRepo>();
        protected Mock<IAppConfig> AppConfig = new Mock<IAppConfig>();
        protected Mock<IDeviceRepositoryManager> RepoManager = new Mock<IDeviceRepositoryManager>();
        protected Mock<INotificationLandingPage> LandingPageBuilder = new Mock<INotificationLandingPage>();
        protected Mock<IDeviceManager> DeviceManager = new Mock<IDeviceManager>();
        protected Mock<IDeviceNotificationTracking> NotificationTacker = new Mock<IDeviceNotificationTracking>();
        protected Mock<ITagReplacementService> TagReplacer = new Mock<ITagReplacementService>();
        protected Mock<IStaticPageStorage> StaticPageService = new Mock<IStaticPageStorage>();
        protected Mock<IMediaServicesManager> MediaServicesManager = new Mock<IMediaServicesManager>();
    }
}
