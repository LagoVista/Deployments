// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 6242fcc3f7f30d65634fedf97a6d8438fa04ef16be1a9230190f4c71d9423f5c
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models.UIMetaData;
using System.Linq;
using LagoVista.IoT.Deployment.Admin.Resources;
using LagoVista.IoT.Deployment.Models.Resources;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    internal class LocalizationService : ILocalizationService
    {
        public List<EntityHeader> GetSupportedCultures()
        {
            return new List<EntityHeader>()
            {
                EntityHeader.Create("en-us", "en-us", "English (US)"),
                EntityHeader.Create("fr-fr", "fr-fr", "French (France)"),
                EntityHeader.Create("fr-ca", "fr-ca", "French (Canada)"),
                EntityHeader.Create("it-it", "it-it", "Italian"),
                EntityHeader.Create("de-de", "de-de", "German"),
                EntityHeader.Create("es-es", "es-mx", "Spanish (Spain)"),
                EntityHeader.Create("es-mx", "es-mx", "Spanish (Mexico)"),
            };       
        }

        public CultureInfo GetCulture(string key)
        {
            throw new NotImplementedException();
        }

        public List<EnumDescription> GetCultureEnumOptions()
        {
            var options = GetSupportedCultures().Select(tz => new EnumDescription() { Id = tz.Id, Key = tz.Id, Label = tz.Text, Name = tz.Text }).ToList();
            options.Insert(0, new EnumDescription()
            {
                Id = "-1",
                Key = "-1",
                Label = DeploymentAdminResources.Common_SelectCulture,
                Name = DeploymentAdminResources.Common_SelectCulture,
            });

            return options;
        }
    }
}
