﻿using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Models;
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

        public TagReplacementService(IAppUserRepo appUserRepo,ISecureLinkManager secureLinkManager, IAppConfig appConfig)
        {
            _appUserRepo = appUserRepo ?? throw new ArgumentNullException(nameof(appUserRepo));
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
            _secureLinkManager = secureLinkManager ?? throw new ArgumentNullException(nameof(secureLinkManager));
        }

        public async Task<string> ReplaceTagsAsync(string template, bool isHtmlContent, Device device, OrgLocation location)
        {
            //The following tags will be replaced in the generated content [DeviceName] [DeviceId] [DeviceLocation] [DeviceSummary] [NotificationTimeStamp]

            return template;

            template = template.Replace("[DeviceName]", device.Name);
            template = template.Replace("[DeviceId]", device.DeviceId);

            return template;
            if (location != null)
            {
                var geoAddress = String.Empty;
                if(!String.IsNullOrEmpty(location.Addr1))
                    geoAddress += $"{location.Addr1} {location.City}, {location.StateProvince} {location.PostalCode}";
                else  if(!String.IsNullOrEmpty(location.City))
                    geoAddress = $"{location.City}, {location.StateProvince} {location.PostalCode}";
                else
                    geoAddress = $"{location.StateProvince} {location.PostalCode}";

                template = template.Replace("[DeviceStreetAddress]", geoAddress);
                template = template.Replace("[PhoneNumber]", location.PhoneNumber);
                template = template.Replace("[DeviceLocationName]", location.Name);
                    
                var locationHtml = location.ToHTML(_appConfig.WebAddress);
                if (location.DiagramReferences.Any())
                {
                    locationHtml += $"<h3>Diagrams</h3>";
                    foreach (var diagram in location.DiagramReferences)
                    {
                        var link = $"{_appConfig.WebAddress}/public/diagram/{diagram.LocationDiagram.Id}/{diagram.LocationDiagramLayer.Id}/{diagram.LocationDiagramShape.Id}/{device.OwnerOrganization.Id}/{device.DeviceRepository.Id}";

                        if (!EntityHeader.IsNullOrEmpty(device.Customer))
                            link += $"/{device.Customer.Id}";

                        var secureLink = await _secureLinkManager.GenerateSecureLinkAsync(link, location.CreatedBy, TimeSpan.FromHours(2), location.Organization, location.CreatedBy);
                        var url = secureLink.Result;
                        locationHtml += $"<div><a href='{url}'> {diagram.LocationDiagram.Text}/{diagram.LocationDiagramShape.Text} </a></div>";
                    }
                }

                template = template.Replace("[DeviceLocation]", locationHtml);


                if (template.Contains("[Location_Admin_Contact]") && !EntityHeader.IsNullOrEmpty(location.AdminContact))
                {
                    var adminContact = await _appUserRepo.GetCachedAppUserAsync(location.AdminContact.Id);
                    if (adminContact != null)
                    {
                        var phoneHtml = String.IsNullOrEmpty(adminContact.PhoneNumber) ? String.Empty : $"<a href='tel:{adminContact.PhoneNumber}> ({adminContact.PhoneNumber})</a>";
                        template = template.Replace("[Location_Admin_Contact]", $"<div>{adminContact.Name} {phoneHtml}</div>");
                    }
                    else
                        template = template.Replace("[Location_Admin_Contact]", String.Empty);
                }
                else
                    template = template.Replace("[Location_Admin_Contact]", String.Empty);

                if (template.Contains("[Location_Technical_Contact]") && !EntityHeader.IsNullOrEmpty(location.TechnicalContact))
                {
                    var technicalContact = await _appUserRepo.GetCachedAppUserAsync(location.TechnicalContact.Id);
                    if (technicalContact != null)
                    {
                        var phoneHtml = String.IsNullOrEmpty(technicalContact.PhoneNumber) ? String.Empty : $"<a href='tel:{technicalContact.PhoneNumber}> ({technicalContact.PhoneNumber})</a>";
                        template = template.Replace("[Location_Technical_Contact]", $"<div>{technicalContact.Name} {phoneHtml}</div>");
                    }
                    else
                        template = template.Replace("[Location_Technical_Contact]", String.Empty);
                }
                else
                    template = template.Replace("[Location_Technical_Contact]", String.Empty);
            }
            else
            {
                template = template.Replace("[DeviceStreetAddress]", String.Empty);
                template = template.Replace("[Location_Admin_Contact]", String.Empty);
                template = template.Replace("[Location_Technical_Contact]", String.Empty);
                template = template.Replace("[DeviceLocation]", string.Empty);
                template = template.Replace("[DeviceLocationName]", "No Location Provided");
            }

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
