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
