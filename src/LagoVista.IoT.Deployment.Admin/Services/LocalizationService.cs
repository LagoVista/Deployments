using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using LagoVista.Core.Interfaces;

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

    }
}
