// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 95d6a00b9775091bf65ca4b0de3bcd6bcb57884f936f2decd211ce39410fa353
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models.Resources;

namespace LagoVista.IoT.Deployment.Models
{

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.WatchdogMessage_Title,
        DeploymentAdminResources.Names.WatchdogMessage_Help,
        DeploymentAdminResources.Names.WatchdogMessage_Help,
      EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeploymentAdminResources), Icon: "icon-fo-message-info",
      GetListUrl: "/api/deployment/instance/{id}/message/watchdog/monitored")]
    public class WatchdogMessageStatus
    {
        public string Id { get; set; }

        /// <summary>
        /// In theory a message could have multiple mssages associated with it.
        /// </summary>
        public string WatchdogId { get; set; }

        /// <summary>
        /// Name of the watch dog for this message.
        /// </summary>
        public string WatchdogName { get; set; }

        /// <summary>
        /// The time this message was last received from a device.
        /// </summary>
        public string LastReceived { get; set; }

        /// <summary>
        /// If we are currently excluding this from a watch dog, when does that exclusion end.
        /// </summary>
        public string ExclusionEnd { get; set; }

        /// <summary>
        /// Number of seconds overdue since the last message was expected.
        /// </summary>
        public int OverdueSeconds { get; set; }

        /// <summary>
        /// Number of seconds since either a last message was received or the end of an exclusion period.
        /// </summary>
        public int SecondsSinceLastEvent { get; set; }

        /// <summary>
        /// Time and date when the watch dog shold be expired and the message should be considered overdue.
        /// </summary>
        public string Expired { get; set; }

        /// <summary>
        /// Last time an error was generated or a notification was sent to a usre.
        /// </summary>
        public string LastNotified { get; set; }

        /// <summary>
        /// Set to true if the message is in an exclusion period.
        /// </summary>
        public bool Excluded { get; set; }

        /// <summary>
        /// Name of the exclusion.
        /// </summary>
        public string ExclusionName { get; set; }

        /// <summary>
        /// The description of the exclusion.
        /// </summary>
        public string ExclusionDescription { get; set; }


        public string MessageName { get; set; }
        public string MessageId { get; set; }


        /// <summary>
        /// Field that is set to identify if the watch dog is currently disabled.
        /// </summary>
        public bool WatchdogDisabled { get; set; }

        public string GeneratedErrorCode { get; set; }
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

    }
}
