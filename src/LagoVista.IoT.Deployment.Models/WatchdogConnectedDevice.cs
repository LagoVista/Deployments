using LagoVista.Core.Attributes;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models.Resources;

namespace LagoVista.IoT.Deployment.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.WatchdogConnectedDevice_Title, 
        DeploymentAdminResources.Names.WatchdogConnectedDevice_Help, 
        DeploymentAdminResources.Names.WatchdogConnectedDevice_Help,
      EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeploymentAdminResources), Icon: "icon-ae-core-2",
      GetListUrl: "/api/deployment/instance/{id}/connected/monitored")]
    public class WatchdogConnectedDevice
    {
        public string Id { get; set; }
        public string LastContact { get; set; }
        public int TimeoutSeconds { get; set; }
        public int OverdueSeconds { get; set; }
        public string Expired { get; set; }
        public string DeviceName { get; set; }
        
        /// <summary>
        /// This is the actual devices id such as [dev0001]
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// This is the unique/guid of the device.
        /// </summary>
        public string DeviceUniqueId { get; set; }

        public string DeviceConfigurationId { get; set; }
        public string DeviceConfiguration { get; set; }
        public string DeviceTypeId { get; set; }
        public string DeviceType { get; set; }
        public string LastNotified { get; set; }
        public bool WatchdogDisabled { get; set; }
    }
}
