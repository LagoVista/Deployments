// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 0b6517d027ec09cd06ec47ea71c7fda497886b5951c56ea20ef74b1ab17ac84e
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public class RouteConnection 
    {
        public RouteConnection()
        {
            Mappings = new List<KeyValuePair<string, object>>();
        }

        public List<KeyValuePair<string, object>> Mappings { get; set; }

        public string Id { get; set; }
        public string Name { get; set;  }

        public static RouteConnection Create(string id, string name)
        {
            return new RouteConnection() { Id = id, Name = name };
        }
    }
}
