using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Models
{
    public class InstanceAccount
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AccessKey { get; set; }
        public string AccessKeyHash { get; set; }
    }
}
