using System.Globalization;
using System.Reflection;

//Resources:DeploymentAdminResources:Common_Description
namespace LagoVista.IoT.Deployment.Admin.Resources
{
	public class DeploymentAdminResources
	{
        private static global::System.Resources.ResourceManager _resourceManager;
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static global::System.Resources.ResourceManager ResourceManager 
		{
            get 
			{
                if (object.ReferenceEquals(_resourceManager, null)) 
				{
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LagoVista.IoT.Deployment.Admin.Resources.DeploymentAdminResources", typeof(DeploymentAdminResources).GetTypeInfo().Assembly);
                    _resourceManager = temp;
                }
                return _resourceManager;
            }
        }
        
        /// <summary>
        ///   Returns the formatted resource string.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static string GetResourceString(string key, params string[] tokens)
		{
			var culture = CultureInfo.CurrentCulture;;
            var str = ResourceManager.GetString(key, culture);

			for(int i = 0; i < tokens.Length; i += 2)
				str = str.Replace(tokens[i], tokens[i+1]);
										
            return str;
        }
        
        /// <summary>
        ///   Returns the formatted resource string.
        /// </summary>
		/*
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static HtmlString GetResourceHtmlString(string key, params string[] tokens)
		{
			var str = GetResourceString(key, tokens);
							
			if(str.StartsWith("HTML:"))
				str = str.Substring(5);

			return new HtmlString(str);
        }*/
		
		public static string Common_Description { get { return GetResourceString("Common_Description"); } }
//Resources:DeploymentAdminResources:Common_Id

		public static string Common_Id { get { return GetResourceString("Common_Id"); } }
//Resources:DeploymentAdminResources:Common_IsPublic

		public static string Common_IsPublic { get { return GetResourceString("Common_IsPublic"); } }
//Resources:DeploymentAdminResources:Common_Key

		public static string Common_Key { get { return GetResourceString("Common_Key"); } }
//Resources:DeploymentAdminResources:Common_Key_Help

		public static string Common_Key_Help { get { return GetResourceString("Common_Key_Help"); } }
//Resources:DeploymentAdminResources:Common_Key_Validation

		public static string Common_Key_Validation { get { return GetResourceString("Common_Key_Validation"); } }
//Resources:DeploymentAdminResources:Common_Name

		public static string Common_Name { get { return GetResourceString("Common_Name"); } }
//Resources:DeploymentAdminResources:Common_Notes

		public static string Common_Notes { get { return GetResourceString("Common_Notes"); } }
//Resources:DeploymentAdminResources:Deployment_Listeners

		public static string Deployment_Listeners { get { return GetResourceString("Deployment_Listeners"); } }
//Resources:DeploymentAdminResources:Deployment_Listeners_Help

		public static string Deployment_Listeners_Help { get { return GetResourceString("Deployment_Listeners_Help"); } }
//Resources:DeploymentAdminResources:Deployment_Planner

		public static string Deployment_Planner { get { return GetResourceString("Deployment_Planner"); } }
//Resources:DeploymentAdminResources:Deployment_Planner_Help

		public static string Deployment_Planner_Help { get { return GetResourceString("Deployment_Planner_Help"); } }
//Resources:DeploymentAdminResources:Deployment_Planner_Select

		public static string Deployment_Planner_Select { get { return GetResourceString("Deployment_Planner_Select"); } }
//Resources:DeploymentAdminResources:Deployment_Title

		public static string Deployment_Title { get { return GetResourceString("Deployment_Title"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_Description

		public static string DeviceConfiguration_Description { get { return GetResourceString("DeviceConfiguration_Description"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_Help

		public static string DeviceConfiguration_Help { get { return GetResourceString("DeviceConfiguration_Help"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_Routes

		public static string DeviceConfiguration_Routes { get { return GetResourceString("DeviceConfiguration_Routes"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_Sentinel

		public static string DeviceConfiguration_Sentinel { get { return GetResourceString("DeviceConfiguration_Sentinel"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_Title

		public static string DeviceConfiguration_Title { get { return GetResourceString("DeviceConfiguration_Title"); } }
//Resources:DeploymentAdminResources:DeviceMessageDefinition_Description

		public static string DeviceMessageDefinition_Description { get { return GetResourceString("DeviceMessageDefinition_Description"); } }
//Resources:DeploymentAdminResources:DeviceMessageDefinition_Help

		public static string DeviceMessageDefinition_Help { get { return GetResourceString("DeviceMessageDefinition_Help"); } }
//Resources:DeploymentAdminResources:DeviceMessageDefinition_MessageId

		public static string DeviceMessageDefinition_MessageId { get { return GetResourceString("DeviceMessageDefinition_MessageId"); } }
//Resources:DeploymentAdminResources:DeviceMessageDefinition_MessageId_Help

		public static string DeviceMessageDefinition_MessageId_Help { get { return GetResourceString("DeviceMessageDefinition_MessageId_Help"); } }
//Resources:DeploymentAdminResources:DeviceMessageDefinition_Title

		public static string DeviceMessageDefinition_Title { get { return GetResourceString("DeviceMessageDefinition_Title"); } }
//Resources:DeploymentAdminResources:Err_ErrorCommunicatingWithHost

		public static string Err_ErrorCommunicatingWithHost { get { return GetResourceString("Err_ErrorCommunicatingWithHost"); } }
//Resources:DeploymentAdminResources:Err_InstanceAlreadyRunning

		public static string Err_InstanceAlreadyRunning { get { return GetResourceString("Err_InstanceAlreadyRunning"); } }
//Resources:DeploymentAdminResources:Err_InstanceNotRunning

		public static string Err_InstanceNotRunning { get { return GetResourceString("Err_InstanceNotRunning"); } }
//Resources:DeploymentAdminResources:Errs_AlreadyDeployed

		public static string Errs_AlreadyDeployed { get { return GetResourceString("Errs_AlreadyDeployed"); } }
//Resources:DeploymentAdminResources:Errs_InstanceBusy

		public static string Errs_InstanceBusy { get { return GetResourceString("Errs_InstanceBusy"); } }
//Resources:DeploymentAdminResources:Errs_MustBeStoppedBeforeRemoving

		public static string Errs_MustBeStoppedBeforeRemoving { get { return GetResourceString("Errs_MustBeStoppedBeforeRemoving"); } }
//Resources:DeploymentAdminResources:Errs_NotDeployed

		public static string Errs_NotDeployed { get { return GetResourceString("Errs_NotDeployed"); } }
//Resources:DeploymentAdminResources:Host_AdminAPIUri

		public static string Host_AdminAPIUri { get { return GetResourceString("Host_AdminAPIUri"); } }
//Resources:DeploymentAdminResources:Host_AdminAPIUri_Help

		public static string Host_AdminAPIUri_Help { get { return GetResourceString("Host_AdminAPIUri_Help"); } }
//Resources:DeploymentAdminResources:Host_AdminEndpoint

		public static string Host_AdminEndpoint { get { return GetResourceString("Host_AdminEndpoint"); } }
//Resources:DeploymentAdminResources:Host_AdminEndpoint_Help

		public static string Host_AdminEndpoint_Help { get { return GetResourceString("Host_AdminEndpoint_Help"); } }
//Resources:DeploymentAdminResources:Host_AverageCPU_30_Minutes

		public static string Host_AverageCPU_30_Minutes { get { return GetResourceString("Host_AverageCPU_30_Minutes"); } }
//Resources:DeploymentAdminResources:Host_AverageMemory_30_Minutes

		public static string Host_AverageMemory_30_Minutes { get { return GetResourceString("Host_AverageMemory_30_Minutes"); } }
//Resources:DeploymentAdminResources:Host_AverageNetwork_30_Minutes

		public static string Host_AverageNetwork_30_Minutes { get { return GetResourceString("Host_AverageNetwork_30_Minutes"); } }
//Resources:DeploymentAdminResources:Host_CapacityStatus

		public static string Host_CapacityStatus { get { return GetResourceString("Host_CapacityStatus"); } }
//Resources:DeploymentAdminResources:Host_DateStampOnline

		public static string Host_DateStampOnline { get { return GetResourceString("Host_DateStampOnline"); } }
//Resources:DeploymentAdminResources:Host_Description

		public static string Host_Description { get { return GetResourceString("Host_Description"); } }
//Resources:DeploymentAdminResources:Host_Help

		public static string Host_Help { get { return GetResourceString("Host_Help"); } }
//Resources:DeploymentAdminResources:Host_Instance_Uri

		public static string Host_Instance_Uri { get { return GetResourceString("Host_Instance_Uri"); } }
//Resources:DeploymentAdminResources:Host_Instance_Uri_Help

		public static string Host_Instance_Uri_Help { get { return GetResourceString("Host_Instance_Uri_Help"); } }
//Resources:DeploymentAdminResources:Host_InstanceId

		public static string Host_InstanceId { get { return GetResourceString("Host_InstanceId"); } }
//Resources:DeploymentAdminResources:Host_InstanceId_Help

		public static string Host_InstanceId_Help { get { return GetResourceString("Host_InstanceId_Help"); } }
//Resources:DeploymentAdminResources:Host_Status

		public static string Host_Status { get { return GetResourceString("Host_Status"); } }
//Resources:DeploymentAdminResources:Host_Subscription

		public static string Host_Subscription { get { return GetResourceString("Host_Subscription"); } }
//Resources:DeploymentAdminResources:Host_SubscriptionSelect

		public static string Host_SubscriptionSelect { get { return GetResourceString("Host_SubscriptionSelect"); } }
//Resources:DeploymentAdminResources:Host_Title

		public static string Host_Title { get { return GetResourceString("Host_Title"); } }
//Resources:DeploymentAdminResources:Host_Type

		public static string Host_Type { get { return GetResourceString("Host_Type"); } }
//Resources:DeploymentAdminResources:Host_Type_Clustered

		public static string Host_Type_Clustered { get { return GetResourceString("Host_Type_Clustered"); } }
//Resources:DeploymentAdminResources:Host_Type_Community

		public static string Host_Type_Community { get { return GetResourceString("Host_Type_Community"); } }
//Resources:DeploymentAdminResources:Host_Type_Dedicated

		public static string Host_Type_Dedicated { get { return GetResourceString("Host_Type_Dedicated"); } }
//Resources:DeploymentAdminResources:Host_Type_Free

		public static string Host_Type_Free { get { return GetResourceString("Host_Type_Free"); } }
//Resources:DeploymentAdminResources:Host_Type_Select

		public static string Host_Type_Select { get { return GetResourceString("Host_Type_Select"); } }
//Resources:DeploymentAdminResources:Host_Type_Shared

		public static string Host_Type_Shared { get { return GetResourceString("Host_Type_Shared"); } }
//Resources:DeploymentAdminResources:Host_Type_SharedHighPerformance

		public static string Host_Type_SharedHighPerformance { get { return GetResourceString("Host_Type_SharedHighPerformance"); } }
//Resources:DeploymentAdminResources:HostCapacity_75Percent

		public static string HostCapacity_75Percent { get { return GetResourceString("HostCapacity_75Percent"); } }
//Resources:DeploymentAdminResources:HostCapacity_90Percent

		public static string HostCapacity_90Percent { get { return GetResourceString("HostCapacity_90Percent"); } }
//Resources:DeploymentAdminResources:HostCapacity_AtCapacity

		public static string HostCapacity_AtCapacity { get { return GetResourceString("HostCapacity_AtCapacity"); } }
//Resources:DeploymentAdminResources:HostCapacity_FailureImminent

		public static string HostCapacity_FailureImminent { get { return GetResourceString("HostCapacity_FailureImminent"); } }
//Resources:DeploymentAdminResources:HostCapacity_Ok

		public static string HostCapacity_Ok { get { return GetResourceString("HostCapacity_Ok"); } }
//Resources:DeploymentAdminResources:HostCapacity_OverCapacity

		public static string HostCapacity_OverCapacity { get { return GetResourceString("HostCapacity_OverCapacity"); } }
//Resources:DeploymentAdminResources:HostCapacity_Underutlized

		public static string HostCapacity_Underutlized { get { return GetResourceString("HostCapacity_Underutlized"); } }
//Resources:DeploymentAdminResources:HostStatus_Degraded

		public static string HostStatus_Degraded { get { return GetResourceString("HostStatus_Degraded"); } }
//Resources:DeploymentAdminResources:HostStatus_Failed

		public static string HostStatus_Failed { get { return GetResourceString("HostStatus_Failed"); } }
//Resources:DeploymentAdminResources:HostStatus_Offline

		public static string HostStatus_Offline { get { return GetResourceString("HostStatus_Offline"); } }
//Resources:DeploymentAdminResources:HostStatus_Running

		public static string HostStatus_Running { get { return GetResourceString("HostStatus_Running"); } }
//Resources:DeploymentAdminResources:HostStatus_Starting

		public static string HostStatus_Starting { get { return GetResourceString("HostStatus_Starting"); } }
//Resources:DeploymentAdminResources:HostStatus_Stopped

		public static string HostStatus_Stopped { get { return GetResourceString("HostStatus_Stopped"); } }
//Resources:DeploymentAdminResources:HostStatus_Stopping

		public static string HostStatus_Stopping { get { return GetResourceString("HostStatus_Stopping"); } }
//Resources:DeploymentAdminResources:Instance_Description

		public static string Instance_Description { get { return GetResourceString("Instance_Description"); } }
//Resources:DeploymentAdminResources:Instance_Help

		public static string Instance_Help { get { return GetResourceString("Instance_Help"); } }
//Resources:DeploymentAdminResources:Instance_Host

		public static string Instance_Host { get { return GetResourceString("Instance_Host"); } }
//Resources:DeploymentAdminResources:Instance_Host_Help

		public static string Instance_Host_Help { get { return GetResourceString("Instance_Host_Help"); } }
//Resources:DeploymentAdminResources:Instance_Host_Watermark

		public static string Instance_Host_Watermark { get { return GetResourceString("Instance_Host_Watermark"); } }
//Resources:DeploymentAdminResources:Instance_IsDeployed

		public static string Instance_IsDeployed { get { return GetResourceString("Instance_IsDeployed"); } }
//Resources:DeploymentAdminResources:Instance_IsDeployed_Help

		public static string Instance_IsDeployed_Help { get { return GetResourceString("Instance_IsDeployed_Help"); } }
//Resources:DeploymentAdminResources:Instance_Solution

		public static string Instance_Solution { get { return GetResourceString("Instance_Solution"); } }
//Resources:DeploymentAdminResources:Instance_Solution_Select

		public static string Instance_Solution_Select { get { return GetResourceString("Instance_Solution_Select"); } }
//Resources:DeploymentAdminResources:Instance_Status

		public static string Instance_Status { get { return GetResourceString("Instance_Status"); } }
//Resources:DeploymentAdminResources:Instance_Title

		public static string Instance_Title { get { return GetResourceString("Instance_Title"); } }
//Resources:DeploymentAdminResources:Route_Description

		public static string Route_Description { get { return GetResourceString("Route_Description"); } }
//Resources:DeploymentAdminResources:Route_Help

		public static string Route_Help { get { return GetResourceString("Route_Help"); } }
//Resources:DeploymentAdminResources:Route_InputTranslator

		public static string Route_InputTranslator { get { return GetResourceString("Route_InputTranslator"); } }
//Resources:DeploymentAdminResources:Route_IsDefault

		public static string Route_IsDefault { get { return GetResourceString("Route_IsDefault"); } }
//Resources:DeploymentAdminResources:Route_IsDefault_Help

		public static string Route_IsDefault_Help { get { return GetResourceString("Route_IsDefault_Help"); } }
//Resources:DeploymentAdminResources:Route_Messages

		public static string Route_Messages { get { return GetResourceString("Route_Messages"); } }
//Resources:DeploymentAdminResources:Route_Messages_Help

		public static string Route_Messages_Help { get { return GetResourceString("Route_Messages_Help"); } }
//Resources:DeploymentAdminResources:Route_OutputTranslator

		public static string Route_OutputTranslator { get { return GetResourceString("Route_OutputTranslator"); } }
//Resources:DeploymentAdminResources:Route_SelectInputTranslator

		public static string Route_SelectInputTranslator { get { return GetResourceString("Route_SelectInputTranslator"); } }
//Resources:DeploymentAdminResources:Route_SelectOutputTranslator

		public static string Route_SelectOutputTranslator { get { return GetResourceString("Route_SelectOutputTranslator"); } }
//Resources:DeploymentAdminResources:Route_SelectSentinel

		public static string Route_SelectSentinel { get { return GetResourceString("Route_SelectSentinel"); } }
//Resources:DeploymentAdminResources:Route_SelectTransmitter

		public static string Route_SelectTransmitter { get { return GetResourceString("Route_SelectTransmitter"); } }
//Resources:DeploymentAdminResources:Route_SelectWorkflow

		public static string Route_SelectWorkflow { get { return GetResourceString("Route_SelectWorkflow"); } }
//Resources:DeploymentAdminResources:Route_Title

		public static string Route_Title { get { return GetResourceString("Route_Title"); } }
//Resources:DeploymentAdminResources:Route_Transmitter

		public static string Route_Transmitter { get { return GetResourceString("Route_Transmitter"); } }
//Resources:DeploymentAdminResources:Route_Workflow

		public static string Route_Workflow { get { return GetResourceString("Route_Workflow"); } }
//Resources:DeploymentAdminResources:Solution_Description

		public static string Solution_Description { get { return GetResourceString("Solution_Description"); } }
//Resources:DeploymentAdminResources:Solution_DeviceConfigurations

		public static string Solution_DeviceConfigurations { get { return GetResourceString("Solution_DeviceConfigurations"); } }
//Resources:DeploymentAdminResources:Solution_DeviceConfigurations_Help

		public static string Solution_DeviceConfigurations_Help { get { return GetResourceString("Solution_DeviceConfigurations_Help"); } }
//Resources:DeploymentAdminResources:Solution_Environment

		public static string Solution_Environment { get { return GetResourceString("Solution_Environment"); } }
//Resources:DeploymentAdminResources:Solution_Help

		public static string Solution_Help { get { return GetResourceString("Solution_Help"); } }
//Resources:DeploymentAdminResources:Solution_Title

		public static string Solution_Title { get { return GetResourceString("Solution_Title"); } }

		public static class Names
		{
			public const string Common_Description = "Common_Description";
			public const string Common_Id = "Common_Id";
			public const string Common_IsPublic = "Common_IsPublic";
			public const string Common_Key = "Common_Key";
			public const string Common_Key_Help = "Common_Key_Help";
			public const string Common_Key_Validation = "Common_Key_Validation";
			public const string Common_Name = "Common_Name";
			public const string Common_Notes = "Common_Notes";
			public const string Deployment_Listeners = "Deployment_Listeners";
			public const string Deployment_Listeners_Help = "Deployment_Listeners_Help";
			public const string Deployment_Planner = "Deployment_Planner";
			public const string Deployment_Planner_Help = "Deployment_Planner_Help";
			public const string Deployment_Planner_Select = "Deployment_Planner_Select";
			public const string Deployment_Title = "Deployment_Title";
			public const string DeviceConfiguration_Description = "DeviceConfiguration_Description";
			public const string DeviceConfiguration_Help = "DeviceConfiguration_Help";
			public const string DeviceConfiguration_Routes = "DeviceConfiguration_Routes";
			public const string DeviceConfiguration_Sentinel = "DeviceConfiguration_Sentinel";
			public const string DeviceConfiguration_Title = "DeviceConfiguration_Title";
			public const string DeviceMessageDefinition_Description = "DeviceMessageDefinition_Description";
			public const string DeviceMessageDefinition_Help = "DeviceMessageDefinition_Help";
			public const string DeviceMessageDefinition_MessageId = "DeviceMessageDefinition_MessageId";
			public const string DeviceMessageDefinition_MessageId_Help = "DeviceMessageDefinition_MessageId_Help";
			public const string DeviceMessageDefinition_Title = "DeviceMessageDefinition_Title";
			public const string Err_ErrorCommunicatingWithHost = "Err_ErrorCommunicatingWithHost";
			public const string Err_InstanceAlreadyRunning = "Err_InstanceAlreadyRunning";
			public const string Err_InstanceNotRunning = "Err_InstanceNotRunning";
			public const string Errs_AlreadyDeployed = "Errs_AlreadyDeployed";
			public const string Errs_InstanceBusy = "Errs_InstanceBusy";
			public const string Errs_MustBeStoppedBeforeRemoving = "Errs_MustBeStoppedBeforeRemoving";
			public const string Errs_NotDeployed = "Errs_NotDeployed";
			public const string Host_AdminAPIUri = "Host_AdminAPIUri";
			public const string Host_AdminAPIUri_Help = "Host_AdminAPIUri_Help";
			public const string Host_AdminEndpoint = "Host_AdminEndpoint";
			public const string Host_AdminEndpoint_Help = "Host_AdminEndpoint_Help";
			public const string Host_AverageCPU_30_Minutes = "Host_AverageCPU_30_Minutes";
			public const string Host_AverageMemory_30_Minutes = "Host_AverageMemory_30_Minutes";
			public const string Host_AverageNetwork_30_Minutes = "Host_AverageNetwork_30_Minutes";
			public const string Host_CapacityStatus = "Host_CapacityStatus";
			public const string Host_DateStampOnline = "Host_DateStampOnline";
			public const string Host_Description = "Host_Description";
			public const string Host_Help = "Host_Help";
			public const string Host_Instance_Uri = "Host_Instance_Uri";
			public const string Host_Instance_Uri_Help = "Host_Instance_Uri_Help";
			public const string Host_InstanceId = "Host_InstanceId";
			public const string Host_InstanceId_Help = "Host_InstanceId_Help";
			public const string Host_Status = "Host_Status";
			public const string Host_Subscription = "Host_Subscription";
			public const string Host_SubscriptionSelect = "Host_SubscriptionSelect";
			public const string Host_Title = "Host_Title";
			public const string Host_Type = "Host_Type";
			public const string Host_Type_Clustered = "Host_Type_Clustered";
			public const string Host_Type_Community = "Host_Type_Community";
			public const string Host_Type_Dedicated = "Host_Type_Dedicated";
			public const string Host_Type_Free = "Host_Type_Free";
			public const string Host_Type_Select = "Host_Type_Select";
			public const string Host_Type_Shared = "Host_Type_Shared";
			public const string Host_Type_SharedHighPerformance = "Host_Type_SharedHighPerformance";
			public const string HostCapacity_75Percent = "HostCapacity_75Percent";
			public const string HostCapacity_90Percent = "HostCapacity_90Percent";
			public const string HostCapacity_AtCapacity = "HostCapacity_AtCapacity";
			public const string HostCapacity_FailureImminent = "HostCapacity_FailureImminent";
			public const string HostCapacity_Ok = "HostCapacity_Ok";
			public const string HostCapacity_OverCapacity = "HostCapacity_OverCapacity";
			public const string HostCapacity_Underutlized = "HostCapacity_Underutlized";
			public const string HostStatus_Degraded = "HostStatus_Degraded";
			public const string HostStatus_Failed = "HostStatus_Failed";
			public const string HostStatus_Offline = "HostStatus_Offline";
			public const string HostStatus_Running = "HostStatus_Running";
			public const string HostStatus_Starting = "HostStatus_Starting";
			public const string HostStatus_Stopped = "HostStatus_Stopped";
			public const string HostStatus_Stopping = "HostStatus_Stopping";
			public const string Instance_Description = "Instance_Description";
			public const string Instance_Help = "Instance_Help";
			public const string Instance_Host = "Instance_Host";
			public const string Instance_Host_Help = "Instance_Host_Help";
			public const string Instance_Host_Watermark = "Instance_Host_Watermark";
			public const string Instance_IsDeployed = "Instance_IsDeployed";
			public const string Instance_IsDeployed_Help = "Instance_IsDeployed_Help";
			public const string Instance_Solution = "Instance_Solution";
			public const string Instance_Solution_Select = "Instance_Solution_Select";
			public const string Instance_Status = "Instance_Status";
			public const string Instance_Title = "Instance_Title";
			public const string Route_Description = "Route_Description";
			public const string Route_Help = "Route_Help";
			public const string Route_InputTranslator = "Route_InputTranslator";
			public const string Route_IsDefault = "Route_IsDefault";
			public const string Route_IsDefault_Help = "Route_IsDefault_Help";
			public const string Route_Messages = "Route_Messages";
			public const string Route_Messages_Help = "Route_Messages_Help";
			public const string Route_OutputTranslator = "Route_OutputTranslator";
			public const string Route_SelectInputTranslator = "Route_SelectInputTranslator";
			public const string Route_SelectOutputTranslator = "Route_SelectOutputTranslator";
			public const string Route_SelectSentinel = "Route_SelectSentinel";
			public const string Route_SelectTransmitter = "Route_SelectTransmitter";
			public const string Route_SelectWorkflow = "Route_SelectWorkflow";
			public const string Route_Title = "Route_Title";
			public const string Route_Transmitter = "Route_Transmitter";
			public const string Route_Workflow = "Route_Workflow";
			public const string Solution_Description = "Solution_Description";
			public const string Solution_DeviceConfigurations = "Solution_DeviceConfigurations";
			public const string Solution_DeviceConfigurations_Help = "Solution_DeviceConfigurations_Help";
			public const string Solution_Environment = "Solution_Environment";
			public const string Solution_Help = "Solution_Help";
			public const string Solution_Title = "Solution_Title";
		}
	}
}
