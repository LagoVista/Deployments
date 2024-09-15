using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.UserAdmin.Interfaces.Repos.Users;
using LagoVista.UserAdmin.Models.Orgs;
using RingCentral;
using System;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Services.NotificationClients
{
    public class TagReplacementService : ITagReplacementService
    {
        private readonly IAppUserRepo _appUserRepo;
        private readonly IAppConfig _appConfig;

        public TagReplacementService(IAppUserRepo appUserRepo, IAppConfig appConfig)
        {
            _appUserRepo = appUserRepo ?? throw new ArgumentNullException(nameof(appUserRepo));
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
        }

        public async Task<string> ReplaceTagsAsync(string template, bool isHtmlContent, Device device, OrgLocation location)
        {
            //The following tags will be replaced in the generated content [DeviceName] [DeviceId] [DeviceLocation] [DeviceSummary] [NotificationTimeStamp]

            template = template.Replace("[DeviceName]", device.Name);
            template = template.Replace("[DeviceId]", device.DeviceId);
            if (location != null)
            {
                template = template.Replace("[DeviceLocation]", location.ToHTML(_appConfig.WebAddress));
                if (template.Contains("[Location_Admin_Contact]") && !EntityHeader.IsNullOrEmpty(location.AdminContact))
                {
                    var adminContact = await _appUserRepo.FindByIdAsync(location.AdminContact.Id);
                    if (adminContact != null)
                    {
                        var phoneHtml = String.IsNullOrEmpty(adminContact.PhoneNumber) ? String.Empty : $"<a href='tel:{adminContact.PhoneNumber}> ({adminContact.PhoneNumber})</a>";
                        template.Replace("[Location_Admin_Contact]", $"<div>{adminContact.Name} {phoneHtml}</div>");
                    }
                }

                if (template.Contains("[Location_Technical_Contact]") && !EntityHeader.IsNullOrEmpty(location.TechnicalContact))
                {
                    var technicalContact = await _appUserRepo.FindByIdAsync(location.TechnicalContact.Id);
                    if (technicalContact != null)
                    {
                        var phoneHtml = String.IsNullOrEmpty(technicalContact.PhoneNumber) ? String.Empty : $"<a href='tel:{technicalContact.PhoneNumber}> ({technicalContact.PhoneNumber})</a>";
                        template.Replace("[Location_Technical_Contact]", $"<div>{technicalContact.Name} {phoneHtml}</div>");
                    }
                }
            }
            else
                template = template.Replace("[DeviceLocation]", string.Empty);

            var sensorHtml = new StringBuilder("<h4>Sensors</h4>");

            if (device.SensorCollection != null)
            {
                foreach (var sensor in device.SensorCollection)
                {
                    sensorHtml.AppendLine($"<div>{sensor.Name}: {sensor.Value}</div>");
                }
            }
            template = template.Replace("[DeviceSensors]", sensorHtml.ToString());
            template = template.Replace("[DeviceSummary]", device.Summary);
            template = template.Replace("[NotificationTimeStamp]", DateTime.Now.ToString());

            return template;
        }
    }
}
