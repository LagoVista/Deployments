// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 0c7cc0c74bda344b2665c35d665d29e7d2d8c38da55fd435d40e236c28b16731
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.UserAdmin.Interfaces.Managers;
using LagoVista.UserAdmin.Interfaces.Repos.Users;
using LagoVista.UserAdmin.Models.Orgs;
using RingCentral;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Services.NotificationClients
{
    public class TagReplacementService : ITagReplacementService
    {
        private readonly IAppUserRepo _appUserRepo;
        private readonly IAppConfig _appConfig;
        private readonly ISecureLinkManager _secureLinkManager;
        private readonly IAdminLogger _adminLogger;
        private readonly ICustomerTagReplacementService _customerTagReplacementService;

        public TagReplacementService(IAppUserRepo appUserRepo, ISecureLinkManager secureLinkManager, ICustomerTagReplacementService customerTagReplacementService, IAppConfig appConfig, IAdminLogger adminLogger)
        {
            _appUserRepo = appUserRepo ?? throw new ArgumentNullException(nameof(appUserRepo));
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
            _secureLinkManager = secureLinkManager ?? throw new ArgumentNullException(nameof(secureLinkManager));
            _adminLogger = adminLogger ?? throw new ArgumentNullException(nameof(adminLogger));
            _customerTagReplacementService = customerTagReplacementService ?? throw new ArgumentNullException(nameof(customerTagReplacementService));
        }

        public async Task<string> ReplaceTagsAsync(string template, bool isHtmlContent, Device device, OrgLocation location)
        {
            var checkPoint = "Starting";

            if (String.IsNullOrEmpty(template))
            {
                _adminLogger.AddError("[TagReplacementService__ReplaceTagsAsync]", $"Null Template", device.DeviceId.ToKVP("deviceId"), device.OwnerOrganization.Text.ToKVP("orgId"));
                return string.Empty;
            }

            if(!EntityHeader.IsNullOrEmpty(device.Customer))
                template = await _customerTagReplacementService.ReplaceTagsAsync(template, isHtmlContent, device);

            try
            {
                //The following tags will be replaced in the generated content [DeviceName] [DeviceId] [DeviceLocation] [DeviceSummary] [NotificationTimeStamp]
                if (device == null)
                    return template;

                template = template.Replace("[DeviceName]", device.Name);
                template = template.Replace("[DeviceId]", device.DeviceId);
                template = template.Replace("[DeviceSummary]", device.Summary);

                checkPoint = "Basic Device Info";

                if (location != null)
                {
                    checkPoint = "Location Not Null";

                    var geoAddress = String.Empty;
                    if (!String.IsNullOrEmpty(location.Addr1))
                        geoAddress += $"{location.Addr1} {location.City}, {location.StateProvince} {location.PostalCode}";
                    else if (!String.IsNullOrEmpty(location.City))
                        geoAddress = $"{location.City}, {location.StateProvince} {location.PostalCode}";
                    else
                        geoAddress = $"{location.StateProvince} {location.PostalCode}";

                    checkPoint = "Location Info";

                    template = template.Replace("[DeviceStreetAddress]", geoAddress);
                    template = template.Replace("[PhoneNumber]", location.PhoneNumber);
                    template = template.Replace("[DeviceLocationName]", location.Name);

                    checkPoint = "Phone and Name";

                    var locationHtml = location.ToHTML(_appConfig.WebAddress);
                    if (location.DiagramReferences != null && location.DiagramReferences.Any())
                    {
                        checkPoint = "Starting Diagram Reference";

                        locationHtml += $"<h3>Diagrams</h3>";
                        foreach (var diagram in location.DiagramReferences)
                        {
                            checkPoint = $"Diagram Reference Start: {diagram.Id} ";

                            var link = $"{_appConfig.WebAddress}/public/diagram/{diagram.LocationDiagram.Id}/{diagram.LocationDiagramLayer.Id}/{diagram.LocationDiagramShape.Id}/{device.OwnerOrganization.Id}/{device.DeviceRepository.Id}";

                            if (!EntityHeader.IsNullOrEmpty(device.Customer))
                                link += $"/{device.Customer.Id}";

                            var secureLink = await _secureLinkManager.GenerateSecureLinkAsync(link, location.CreatedBy, TimeSpan.FromHours(2), location.Organization, location.CreatedBy);
                            var url = secureLink.Result;
                            locationHtml += $"<div><a href='{url}'> {diagram.LocationDiagram.Text}/{diagram.LocationDiagramShape.Text} </a></div>";


                            checkPoint = $"Diagram Reference Ending: {diagram.Id} ";
                        }

                        checkPoint = "Ending";
                    }

                    template = template.Replace("[DeviceLocation]", locationHtml);

                    checkPoint = "Location Completed";

                    if (template.Contains("[Admin_Contact]") && !EntityHeader.IsNullOrEmpty(location.AdminContact))
                    {
                        var adminContact = await _appUserRepo.GetCachedAppUserAsync(location.AdminContact.Id);
                        if (adminContact != null)
                        {
                            var phoneHtml = String.IsNullOrEmpty(adminContact.PhoneNumber) ? String.Empty : $"<a href='tel:{adminContact.PhoneNumber}> ({adminContact.PhoneNumber})</a>";
                            template = template.Replace("[Admin_Contact]", $"<div>{adminContact.Name} {phoneHtml}</div>");
                        }
                        else
                            template = template.Replace("[Admin_Contact]", String.Empty);

                        checkPoint = "Location Admin Contact Not Null";
                    }
                    else
                        template = template.Replace("[Admin_Contact]", String.Empty);

                    checkPoint = "Location Admin Contact Complete";

                    if (template.Contains("[Technical_Contact]") && !EntityHeader.IsNullOrEmpty(location.TechnicalContact))
                    {
                        var technicalContact = await _appUserRepo.GetCachedAppUserAsync(location.TechnicalContact.Id);
                        if (technicalContact != null)
                        {
                            var phoneHtml = String.IsNullOrEmpty(technicalContact.PhoneNumber) ? String.Empty : $"<a href='tel:{technicalContact.PhoneNumber}> ({technicalContact.PhoneNumber})</a>";
                            template = template.Replace("[Technical_Contact]", $"<div>{technicalContact.Name} {phoneHtml}</div>");
                        }
                        else
                            template = template.Replace("[Technical_Contact]", String.Empty);

                        checkPoint = "Location Technical Contact Not Null";
                    }
                    else
                        template = template.Replace("[Technical_Contact]", String.Empty);

                    checkPoint = "Technical Contact Complete";
                }
                else
                {
                    template = template.Replace("[DeviceStreetAddress]", String.Empty);
                    template = template.Replace("[Admin_Contact]", String.Empty);
                    template = template.Replace("[Technical_Contact]", String.Empty);
                    template = template.Replace("[DeviceLocation]", string.Empty);
                    template = template.Replace("[DeviceLocationName]", "No Location Provided");

                    checkPoint = "No location";
                }

                checkPoint = "Location Complete";

                var sensorHtml = new StringBuilder("<h4>Sensors</h4>");

                if (device.SensorCollection != null)
                {
                    checkPoint = "Sensors Not Null - Start";

                    foreach (var sensor in device.SensorCollection)
                    {
                        sensorHtml.AppendLine($"<div>{sensor.Name}: {sensor.Value}</div>");
                    }
                    template = template.Replace("[DeviceSensors]", sensorHtml.ToString());

                    checkPoint = "Sensors Not Null - Complete";
                }
                else
                    template = template.Replace("[DeviceSensors]", string.Empty);

                checkPoint = "Sensors Complete";

                template = template.Replace("[NotificationTimeStamp]", DateTime.Now.ToString());

                checkPoint = "Tag Replace Complete";
            }
            catch (Exception ex)
            {
                _adminLogger.AddError("[TagReplacementService__ReplaeTagsAsync]", $"Error replacing tags in template {ex.Message} - Check Point {checkPoint}");
            }

            return template;
        }

    }
}
