using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Deployment.Admin.Services.NotificationClients;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Logging.Utils;
using LagoVista.MediaServices.Interfaces;
using LagoVista.UserAdmin.Interfaces.Managers;
using LagoVista.UserAdmin.Interfaces.Repos.Orgs;
using LagoVista.UserAdmin.Interfaces.Repos.Users;
using LagoVista.UserAdmin.Models.Orgs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Runtime.CompilerServices;


namespace LagoVista.IoT.Deployment.Tests.Notifications
{
    public class TestBase
    {
        public const string ORGID = "BB204A8050E44149873B00202B2CC11";
        public const string USERID = "321603F7FC2247C5BD203977A53BDAC7";

        protected EntityHeader OrgEH = new EntityHeader() { Id = ORGID, Text = "ORG NAME" };
        protected EntityHeader UserEH = new EntityHeader() { Id = USERID, Text = "USEr NAME" };

        protected IAdminLogger AdminLogger = new AdminLogger(new ConsoleLogWriter());
        protected Mock<ILinkShortener> LinkShortener = new Mock<ILinkShortener>();
        protected Mock<UserAdmin.Interfaces.Managers.IEmailSender> EmailMessageSender = new Mock<UserAdmin.Interfaces.Managers.IEmailSender>();
        protected Mock<IDistributionListRepo> DistroLibRepo = new Mock<IDistributionListRepo>();
        protected Mock<IDeviceNotificationRepo> NotificationRepo = new Mock<IDeviceNotificationRepo>();
        protected Mock<IOrgLocationRepo> OrgLocationRepo = new Mock<IOrgLocationRepo>();
        protected Mock<Admin.Interfaces.IEmailSender> EmailSender = new Mock<Admin.Interfaces.IEmailSender>();
        protected Mock<ISecureStorage> SecureStorege = new Mock<ISecureStorage>();
        protected Mock<UserAdmin.Interfaces.Managers.ISmsSender> SMSMessageSender = new Mock<UserAdmin.Interfaces.Managers.ISmsSender>();
        protected Mock<ISMSSender> SMSSender = new Mock<ISMSSender>();
        protected Mock<IRestSender> RestSender = new Mock<IRestSender>();
        protected Mock<IMqttSender> MqttSender = new Mock<IMqttSender>();
        protected Mock<IDeploymentInstanceRepo> DeploymentInstanceRepo = new Mock<IDeploymentInstanceRepo>();
        protected Mock<ICOTSender> COTSender = new Mock<ICOTSender>();
        protected Mock<IAppUserRepo> AppUserRepo = new Mock<IAppUserRepo>();
        protected Mock<IDeviceConfigHelper> DeviceConfigHelper = new Mock<IDeviceConfigHelper>();
        protected Mock<IAppConfig> AppConfig = new Mock<IAppConfig>();
        protected Mock<IDeviceRepositoryManager> RepoManager = new Mock<IDeviceRepositoryManager>();
        protected Mock<IDeviceRepositoryRepo> RepoRepo = new Mock<IDeviceRepositoryRepo>();
        protected Mock<INotificationLandingPage> LandingPageBuilder = new Mock<INotificationLandingPage>();
        protected Mock<IDeviceManager> DeviceManager = new Mock<IDeviceManager>();
        protected Mock<IDeviceNotificationTracking> NotificationTacker = new Mock<IDeviceNotificationTracking>();
        protected Mock<ITagReplacementService> TagReplacer = new Mock<ITagReplacementService>();
        protected Mock<IStaticPageStorage> StaticPageService = new Mock<IStaticPageStorage>();
        protected Mock<IMediaServicesManager> MediaServicesManager = new Mock<IMediaServicesManager>();
        protected Mock<IOrganizationRepo> OrgRepo = new Mock<IOrganizationRepo>();
        protected Mock<IOrganizationManager> OrgMananager = new Mock<IOrganizationManager>();
        protected Mock<ITimeZoneServices> TimeZoneService = new Mock<ITimeZoneServices>();
        protected Mock<IRaisedNotificationHistoryRepo> RasiedDeviceNotificationRepo = new Mock<IRaisedNotificationHistoryRepo>();
        protected Mock<IBackgroundServiceTaskQueue> BackgroundTaskQueue = new Mock<IBackgroundServiceTaskQueue>();
        protected Mock<IDeviceCommandSender> DeviceCommandSender = new Mock<IDeviceCommandSender>();
        protected Mock<ISecureLinkManager> SecureLinkManager = new Mock<ISecureLinkManager>();

        protected const string ROOT_CERT_ID = "ROOTABC1235";
        protected const string CERT_SECRET_ID = "CERTSECRETID";
        protected const string USER_FILE_ID = "USErFILEDEF9876543";

        [TestInitialize]
        public void InitBase()
        {
            Assert.IsTrue(System.IO.File.Exists("kevin.zip"), "should have data package file kevin.zip");

            var buffer = System.IO.File.ReadAllBytes("kevin.zip");

            SecureStorege.Setup(ss => ss.GetSecretAsync(It.IsAny<EntityHeader>(), It.Is<string>(str => str == CERT_SECRET_ID), It.IsAny<EntityHeader>())).ReturnsAsync(new InvokeResult<string>()
            {

                Result = "atakatak"
            });

            

            MediaServicesManager.Setup(msm => msm.GetResourceMediaAsync(It.Is<string>(str => str == USER_FILE_ID), It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>())).ReturnsAsync(new MediaServices.Models.MediaItemResponse()
            {
                 ImageBytes = buffer,
            });


            var cert = System.IO.File.ReadAllBytes("slroot.crt");
            MediaServicesManager.Setup(msm => msm.GetResourceMediaAsync(It.Is<string>(str => str == ROOT_CERT_ID), It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>())).ReturnsAsync(new MediaServices.Models.MediaItemResponse()
            {
                ImageBytes = cert,
            });

            TagReplacer.Setup(trs => trs.ReplaceTagsAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Device>(), It.IsAny<OrgLocation>())).ReturnsAsync("JUST SOME CONTENT");

            LinkShortener.Setup(lnks => lnks.ShortenLinkAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(InvokeResult<string>.Create("https://alert.com/abcd"));

            LandingPageBuilder.Setup(lpg => lpg.PreparePage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DeviceNotification>(), It.IsAny<bool>(), It.IsAny<Device>(), It.IsAny<OrgLocation>(), It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>())).
                ReturnsAsync(Core.Validation.InvokeResult<NotificationLinks>.Create(GetLinks()));

            EmailMessageSender.Setup(ems => ems.SendAsync(It.IsAny<Email>(), It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>())).ReturnsAsync(InvokeResult<string>.Create("{A12ADAA2-5D66-421D-A9A0-D1C8B05B1D6F}"));

            EmailMessageSender.Setup(ems => ems.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(InvokeResult.Success);
            SMSMessageSender.Setup(sms => sms.SendAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(InvokeResult.Success);
            SMSSender.Setup(sms => sms.SendAsync(It.IsAny<string>(), It.IsAny<NotificationRecipient>(), It.IsAny<NotificationLinks>(), It.IsAny<bool>(), It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>())).ReturnsAsync(InvokeResult.Success);
            TimeZoneService.Setup(tms => tms.GetTimeZoneById(It.IsAny<string>())).Returns(TimeZoneInfo.Local);
       
        }


        protected RaisedDeviceNotification GetRaisedNotification()
        {
            return new RaisedDeviceNotification()
            {
                NotificationKey = "abc123",
                DeviceId = "abc12",
                DeviceUniqueId = "def455",
                DeviceRepositoryId = "hij555"
            };
        }

    
        protected DeviceNotification GetNotification()
        {
            return new DeviceNotification()
            {
                SendEmail = true,
                SendSMS = true,
                EmailSubject = "TEST CONTENT [DeviceId]",
                EmailContent = "Testing Message [DeviceId]",
                SmsContent = "SEND SMS MESSAGE [DeviceId]",
            };
        }

        protected Device GetDevice()
        {
            return new Device()
            {

            };
        }

        protected OrgLocation GetLocation()
        {
            return new OrgLocation()
            {

            };
        }

        public NotificationRecipient GetRecipient()
        {
            return new NotificationRecipient()
            {
                Id = "D054A5C208564CF9A0B8BED754DD6FCD",
                SendEmail = true,
                SendSMS = true,
                Email = "kevinw@foo.com",
                Phone = "6125551212",
                FirstName = "Kevin",
                LastName = "Wolf"
            };
        }

        public NotificationLinks GetLinks()
        {
            return new NotificationLinks()
            {
                FullLandingPageLink = "https://www.nuviot.com/mypage/mysite/{recipientid}"
            };
        }

        public bool GetTestMode()
        {
            return false;
        }
    }
}
