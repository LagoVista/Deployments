/*12/27/2024 5:28:19 AM*/
using System.Globalization;
using System.Reflection;

//Resources:DeploymentAdminResources:ClientApp_AppAuthKey1
namespace LagoVista.IoT.Deployment.Models.Resources
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LagoVista.IoT.Deployment.Models.Resources.DeploymentAdminResources", typeof(DeploymentAdminResources).GetTypeInfo().Assembly);
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
		
		public static string ClientApp_AppAuthKey1 { get { return GetResourceString("ClientApp_AppAuthKey1"); } }
//Resources:DeploymentAdminResources:ClientApp_AppAuthKey2

		public static string ClientApp_AppAuthKey2 { get { return GetResourceString("ClientApp_AppAuthKey2"); } }
//Resources:DeploymentAdminResources:ClientApp_Description

		public static string ClientApp_Description { get { return GetResourceString("ClientApp_Description"); } }
//Resources:DeploymentAdminResources:ClientApp_DeviceConfigs

		public static string ClientApp_DeviceConfigs { get { return GetResourceString("ClientApp_DeviceConfigs"); } }
//Resources:DeploymentAdminResources:ClientApp_DeviceTypes

		public static string ClientApp_DeviceTypes { get { return GetResourceString("ClientApp_DeviceTypes"); } }
//Resources:DeploymentAdminResources:ClientApp_Help

		public static string ClientApp_Help { get { return GetResourceString("ClientApp_Help"); } }
//Resources:DeploymentAdminResources:ClientApp_Instance

		public static string ClientApp_Instance { get { return GetResourceString("ClientApp_Instance"); } }
//Resources:DeploymentAdminResources:ClientApp_Kiosk

		public static string ClientApp_Kiosk { get { return GetResourceString("ClientApp_Kiosk"); } }
//Resources:DeploymentAdminResources:ClientApp_Kiosk_Select

		public static string ClientApp_Kiosk_Select { get { return GetResourceString("ClientApp_Kiosk_Select"); } }
//Resources:DeploymentAdminResources:ClientApp_SelectInstance

		public static string ClientApp_SelectInstance { get { return GetResourceString("ClientApp_SelectInstance"); } }
//Resources:DeploymentAdminResources:ClientApp_Title

		public static string ClientApp_Title { get { return GetResourceString("ClientApp_Title"); } }
//Resources:DeploymentAdminResources:ClientApplications_Title

		public static string ClientApplications_Title { get { return GetResourceString("ClientApplications_Title"); } }
//Resources:DeploymentAdminResources:ClientApps_Title

		public static string ClientApps_Title { get { return GetResourceString("ClientApps_Title"); } }
//Resources:DeploymentAdminResources:Common_Category

		public static string Common_Category { get { return GetResourceString("Common_Category"); } }
//Resources:DeploymentAdminResources:Common_Category_Select

		public static string Common_Category_Select { get { return GetResourceString("Common_Category_Select"); } }
//Resources:DeploymentAdminResources:Common_Customer

		public static string Common_Customer { get { return GetResourceString("Common_Customer"); } }
//Resources:DeploymentAdminResources:Common_Description

		public static string Common_Description { get { return GetResourceString("Common_Description"); } }
//Resources:DeploymentAdminResources:Common_Details

		public static string Common_Details { get { return GetResourceString("Common_Details"); } }
//Resources:DeploymentAdminResources:Common_Icon

		public static string Common_Icon { get { return GetResourceString("Common_Icon"); } }
//Resources:DeploymentAdminResources:Common_Id

		public static string Common_Id { get { return GetResourceString("Common_Id"); } }
//Resources:DeploymentAdminResources:Common_IsPublic

		public static string Common_IsPublic { get { return GetResourceString("Common_IsPublic"); } }
//Resources:DeploymentAdminResources:Common_IsPublic_Help

		public static string Common_IsPublic_Help { get { return GetResourceString("Common_IsPublic_Help"); } }
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
//Resources:DeploymentAdminResources:Common_Resources

		public static string Common_Resources { get { return GetResourceString("Common_Resources"); } }
//Resources:DeploymentAdminResources:Common_Summary

		public static string Common_Summary { get { return GetResourceString("Common_Summary"); } }
//Resources:DeploymentAdminResources:ContainerRepositories_Title

		public static string ContainerRepositories_Title { get { return GetResourceString("ContainerRepositories_Title"); } }
//Resources:DeploymentAdminResources:ContainerRepository_Description

		public static string ContainerRepository_Description { get { return GetResourceString("ContainerRepository_Description"); } }
//Resources:DeploymentAdminResources:ContainerRepository_Help

		public static string ContainerRepository_Help { get { return GetResourceString("ContainerRepository_Help"); } }
//Resources:DeploymentAdminResources:ContainerRepository_IsDefautForRuntime

		public static string ContainerRepository_IsDefautForRuntime { get { return GetResourceString("ContainerRepository_IsDefautForRuntime"); } }
//Resources:DeploymentAdminResources:ContainerRepository_IsDefautForRuntime_Help

		public static string ContainerRepository_IsDefautForRuntime_Help { get { return GetResourceString("ContainerRepository_IsDefautForRuntime_Help"); } }
//Resources:DeploymentAdminResources:ContainerRepository_Namespace

		public static string ContainerRepository_Namespace { get { return GetResourceString("ContainerRepository_Namespace"); } }
//Resources:DeploymentAdminResources:ContainerRepository_Namespace_Help

		public static string ContainerRepository_Namespace_Help { get { return GetResourceString("ContainerRepository_Namespace_Help"); } }
//Resources:DeploymentAdminResources:ContainerRepository_Password

		public static string ContainerRepository_Password { get { return GetResourceString("ContainerRepository_Password"); } }
//Resources:DeploymentAdminResources:ContainerRepository_Password_Help

		public static string ContainerRepository_Password_Help { get { return GetResourceString("ContainerRepository_Password_Help"); } }
//Resources:DeploymentAdminResources:ContainerRepository_Preferred

		public static string ContainerRepository_Preferred { get { return GetResourceString("ContainerRepository_Preferred"); } }
//Resources:DeploymentAdminResources:ContainerRepository_Preferred_Help

		public static string ContainerRepository_Preferred_Help { get { return GetResourceString("ContainerRepository_Preferred_Help"); } }
//Resources:DeploymentAdminResources:ContainerRepository_Preferred_Select

		public static string ContainerRepository_Preferred_Select { get { return GetResourceString("ContainerRepository_Preferred_Select"); } }
//Resources:DeploymentAdminResources:ContainerRepository_Registry

		public static string ContainerRepository_Registry { get { return GetResourceString("ContainerRepository_Registry"); } }
//Resources:DeploymentAdminResources:ContainerRepository_Registry_Help

		public static string ContainerRepository_Registry_Help { get { return GetResourceString("ContainerRepository_Registry_Help"); } }
//Resources:DeploymentAdminResources:ContainerRepository_RepositoryName

		public static string ContainerRepository_RepositoryName { get { return GetResourceString("ContainerRepository_RepositoryName"); } }
//Resources:DeploymentAdminResources:ContainerRepository_RepositoryName_Help

		public static string ContainerRepository_RepositoryName_Help { get { return GetResourceString("ContainerRepository_RepositoryName_Help"); } }
//Resources:DeploymentAdminResources:ContainerRepository_Tags

		public static string ContainerRepository_Tags { get { return GetResourceString("ContainerRepository_Tags"); } }
//Resources:DeploymentAdminResources:ContainerRepository_Title

		public static string ContainerRepository_Title { get { return GetResourceString("ContainerRepository_Title"); } }
//Resources:DeploymentAdminResources:ContainerRepository_UserName

		public static string ContainerRepository_UserName { get { return GetResourceString("ContainerRepository_UserName"); } }
//Resources:DeploymentAdminResources:ContainerRepository_UserName_Help

		public static string ContainerRepository_UserName_Help { get { return GetResourceString("ContainerRepository_UserName_Help"); } }
//Resources:DeploymentAdminResources:CotNotification_CustomCertExpires

		public static string CotNotification_CustomCertExpires { get { return GetResourceString("CotNotification_CustomCertExpires"); } }
//Resources:DeploymentAdminResources:CotNotification_DataPackageFile

		public static string CotNotification_DataPackageFile { get { return GetResourceString("CotNotification_DataPackageFile"); } }
//Resources:DeploymentAdminResources:CotNotification_DataPackageFile_Help

		public static string CotNotification_DataPackageFile_Help { get { return GetResourceString("CotNotification_DataPackageFile_Help"); } }
//Resources:DeploymentAdminResources:CotNotification_Description

		public static string CotNotification_Description { get { return GetResourceString("CotNotification_Description"); } }
//Resources:DeploymentAdminResources:CotNotification_FillColor

		public static string CotNotification_FillColor { get { return GetResourceString("CotNotification_FillColor"); } }
//Resources:DeploymentAdminResources:CotNotification_Help

		public static string CotNotification_Help { get { return GetResourceString("CotNotification_Help"); } }
//Resources:DeploymentAdminResources:CotNotification_IgnoreCertificateErrors

		public static string CotNotification_IgnoreCertificateErrors { get { return GetResourceString("CotNotification_IgnoreCertificateErrors"); } }
//Resources:DeploymentAdminResources:CotNotification_IgnoreCertificateErrors_Help

		public static string CotNotification_IgnoreCertificateErrors_Help { get { return GetResourceString("CotNotification_IgnoreCertificateErrors_Help"); } }
//Resources:DeploymentAdminResources:CotNotification_IncludeDeviceLocation


		///<summary>
		///Include Device Location
		///</summary>
		public static string CotNotification_IncludeDeviceLocation { get { return GetResourceString("CotNotification_IncludeDeviceLocation"); } }
//Resources:DeploymentAdminResources:CotNotification_IncludeLocationPolygon

		public static string CotNotification_IncludeLocationPolygon { get { return GetResourceString("CotNotification_IncludeLocationPolygon"); } }
//Resources:DeploymentAdminResources:CotNotification_IncludeLocationPolygon_Help

		public static string CotNotification_IncludeLocationPolygon_Help { get { return GetResourceString("CotNotification_IncludeLocationPolygon_Help"); } }
//Resources:DeploymentAdminResources:CotNotification_PublicRootCert

		public static string CotNotification_PublicRootCert { get { return GetResourceString("CotNotification_PublicRootCert"); } }
//Resources:DeploymentAdminResources:CotNotification_PublicRootCert_Help

		public static string CotNotification_PublicRootCert_Help { get { return GetResourceString("CotNotification_PublicRootCert_Help"); } }
//Resources:DeploymentAdminResources:CotNotification_Remarks

		public static string CotNotification_Remarks { get { return GetResourceString("CotNotification_Remarks"); } }
//Resources:DeploymentAdminResources:CotNotification_StaleSeconds

		public static string CotNotification_StaleSeconds { get { return GetResourceString("CotNotification_StaleSeconds"); } }
//Resources:DeploymentAdminResources:CotNotification_StaleSeconds_Help

		public static string CotNotification_StaleSeconds_Help { get { return GetResourceString("CotNotification_StaleSeconds_Help"); } }
//Resources:DeploymentAdminResources:CotNotification_Title

		public static string CotNotification_Title { get { return GetResourceString("CotNotification_Title"); } }
//Resources:DeploymentAdminResources:CotNotification_Type_Help

		public static string CotNotification_Type_Help { get { return GetResourceString("CotNotification_Type_Help"); } }
//Resources:DeploymentAdminResources:CotNotification_UseCustomRoot

		public static string CotNotification_UseCustomRoot { get { return GetResourceString("CotNotification_UseCustomRoot"); } }
//Resources:DeploymentAdminResources:CotNotification_UseCustomRoot_Help

		public static string CotNotification_UseCustomRoot_Help { get { return GetResourceString("CotNotification_UseCustomRoot_Help"); } }
//Resources:DeploymentAdminResources:CotNotifications_Title

		public static string CotNotifications_Title { get { return GetResourceString("CotNotifications_Title"); } }
//Resources:DeploymentAdminResources:CotNotifiication_Type

		public static string CotNotifiication_Type { get { return GetResourceString("CotNotifiication_Type"); } }
//Resources:DeploymentAdminResources:Deployment_Listeners

		public static string Deployment_Listeners { get { return GetResourceString("Deployment_Listeners"); } }
//Resources:DeploymentAdminResources:Deployment_Listeners_Help

		public static string Deployment_Listeners_Help { get { return GetResourceString("Deployment_Listeners_Help"); } }
//Resources:DeploymentAdminResources:Deployment_Logging

		public static string Deployment_Logging { get { return GetResourceString("Deployment_Logging"); } }
//Resources:DeploymentAdminResources:Deployment_Logging_Cloud

		public static string Deployment_Logging_Cloud { get { return GetResourceString("Deployment_Logging_Cloud"); } }
//Resources:DeploymentAdminResources:Deployment_Logging_Help

		public static string Deployment_Logging_Help { get { return GetResourceString("Deployment_Logging_Help"); } }
//Resources:DeploymentAdminResources:Deployment_Logging_Local

		public static string Deployment_Logging_Local { get { return GetResourceString("Deployment_Logging_Local"); } }
//Resources:DeploymentAdminResources:Deployment_Logging_Select

		public static string Deployment_Logging_Select { get { return GetResourceString("Deployment_Logging_Select"); } }
//Resources:DeploymentAdminResources:Deployment_Planner

		public static string Deployment_Planner { get { return GetResourceString("Deployment_Planner"); } }
//Resources:DeploymentAdminResources:Deployment_Planner_Help

		public static string Deployment_Planner_Help { get { return GetResourceString("Deployment_Planner_Help"); } }
//Resources:DeploymentAdminResources:Deployment_Planner_Select

		public static string Deployment_Planner_Select { get { return GetResourceString("Deployment_Planner_Select"); } }
//Resources:DeploymentAdminResources:Deployment_Title

		public static string Deployment_Title { get { return GetResourceString("Deployment_Title"); } }
//Resources:DeploymentAdminResources:DeploymentActivities_Help

		public static string DeploymentActivities_Help { get { return GetResourceString("DeploymentActivities_Help"); } }
//Resources:DeploymentAdminResources:DeploymentActivities_TItle

		public static string DeploymentActivities_TItle { get { return GetResourceString("DeploymentActivities_TItle"); } }
//Resources:DeploymentAdminResources:DeploymentActivity_ActivityType

		public static string DeploymentActivity_ActivityType { get { return GetResourceString("DeploymentActivity_ActivityType"); } }
//Resources:DeploymentAdminResources:DeploymentActivity_Duration

		public static string DeploymentActivity_Duration { get { return GetResourceString("DeploymentActivity_Duration"); } }
//Resources:DeploymentAdminResources:DeploymentActivity_ErrorMessage

		public static string DeploymentActivity_ErrorMessage { get { return GetResourceString("DeploymentActivity_ErrorMessage"); } }
//Resources:DeploymentAdminResources:DeploymentActivity_ResourceType

		public static string DeploymentActivity_ResourceType { get { return GetResourceString("DeploymentActivity_ResourceType"); } }
//Resources:DeploymentAdminResources:DeploymentActivity_Start

		public static string DeploymentActivity_Start { get { return GetResourceString("DeploymentActivity_Start"); } }
//Resources:DeploymentAdminResources:DeploymentActivity_Status

		public static string DeploymentActivity_Status { get { return GetResourceString("DeploymentActivity_Status"); } }
//Resources:DeploymentAdminResources:DeploymentConfiguration_DockerSwarm

		public static string DeploymentConfiguration_DockerSwarm { get { return GetResourceString("DeploymentConfiguration_DockerSwarm"); } }
//Resources:DeploymentAdminResources:DeploymentConfiguration_Kubernetes

		public static string DeploymentConfiguration_Kubernetes { get { return GetResourceString("DeploymentConfiguration_Kubernetes"); } }
//Resources:DeploymentAdminResources:DeploymentConfiguration_SingleInstance

		public static string DeploymentConfiguration_SingleInstance { get { return GetResourceString("DeploymentConfiguration_SingleInstance"); } }
//Resources:DeploymentAdminResources:DeploymentConfiguration_UWPApp

		public static string DeploymentConfiguration_UWPApp { get { return GetResourceString("DeploymentConfiguration_UWPApp"); } }
//Resources:DeploymentAdminResources:DeploymentInstance_HealthCheckEnabled

		public static string DeploymentInstance_HealthCheckEnabled { get { return GetResourceString("DeploymentInstance_HealthCheckEnabled"); } }
//Resources:DeploymentAdminResources:DeploymentInstance_HealthCheckEnabled_Help

		public static string DeploymentInstance_HealthCheckEnabled_Help { get { return GetResourceString("DeploymentInstance_HealthCheckEnabled_Help"); } }
//Resources:DeploymentAdminResources:DeploymentInstance_Integrations

		public static string DeploymentInstance_Integrations { get { return GetResourceString("DeploymentInstance_Integrations"); } }
//Resources:DeploymentAdminResources:DeploymentInstance_SharedAccessKey_Help

		public static string DeploymentInstance_SharedAccessKey_Help { get { return GetResourceString("DeploymentInstance_SharedAccessKey_Help"); } }
//Resources:DeploymentAdminResources:DeploymentInstance_SharedAccessKey1

		public static string DeploymentInstance_SharedAccessKey1 { get { return GetResourceString("DeploymentInstance_SharedAccessKey1"); } }
//Resources:DeploymentAdminResources:DeploymentInstance_SharedAccessKey2

		public static string DeploymentInstance_SharedAccessKey2 { get { return GetResourceString("DeploymentInstance_SharedAccessKey2"); } }
//Resources:DeploymentAdminResources:DeploymentInstance_TimeZone

		public static string DeploymentInstance_TimeZone { get { return GetResourceString("DeploymentInstance_TimeZone"); } }
//Resources:DeploymentAdminResources:DeploymentInstance_TimeZone_RegEx

		public static string DeploymentInstance_TimeZone_RegEx { get { return GetResourceString("DeploymentInstance_TimeZone_RegEx"); } }
//Resources:DeploymentAdminResources:DeploymentInstance_Version

		public static string DeploymentInstance_Version { get { return GetResourceString("DeploymentInstance_Version"); } }
//Resources:DeploymentAdminResources:DeploymentInstance_Version_Help

		public static string DeploymentInstance_Version_Help { get { return GetResourceString("DeploymentInstance_Version_Help"); } }
//Resources:DeploymentAdminResources:DeploymentInstance_Version_Select

		public static string DeploymentInstance_Version_Select { get { return GetResourceString("DeploymentInstance_Version_Select"); } }
//Resources:DeploymentAdminResources:DeploymentInstance_WiFiConnectionProfiles

		public static string DeploymentInstance_WiFiConnectionProfiles { get { return GetResourceString("DeploymentInstance_WiFiConnectionProfiles"); } }
//Resources:DeploymentAdminResources:DeploymentInstanceCredentials__Expires

		public static string DeploymentInstanceCredentials__Expires { get { return GetResourceString("DeploymentInstanceCredentials__Expires"); } }
//Resources:DeploymentAdminResources:DeploymentInstanceCredentials__Expires_Help

		public static string DeploymentInstanceCredentials__Expires_Help { get { return GetResourceString("DeploymentInstanceCredentials__Expires_Help"); } }
//Resources:DeploymentAdminResources:DeploymentInstanceCredentials__Password

		public static string DeploymentInstanceCredentials__Password { get { return GetResourceString("DeploymentInstanceCredentials__Password"); } }
//Resources:DeploymentAdminResources:DeploymentInstanceCredentials__UserId

		public static string DeploymentInstanceCredentials__UserId { get { return GetResourceString("DeploymentInstanceCredentials__UserId"); } }
//Resources:DeploymentAdminResources:DeploymentInstanceCredentials_Description

		public static string DeploymentInstanceCredentials_Description { get { return GetResourceString("DeploymentInstanceCredentials_Description"); } }
//Resources:DeploymentAdminResources:DeploymentInstanceCredentials_Help

		public static string DeploymentInstanceCredentials_Help { get { return GetResourceString("DeploymentInstanceCredentials_Help"); } }
//Resources:DeploymentAdminResources:DeploymentInstanceCredentials_Title

		public static string DeploymentInstanceCredentials_Title { get { return GetResourceString("DeploymentInstanceCredentials_Title"); } }
//Resources:DeploymentAdminResources:DeploymentInstanceStatus_Description

		public static string DeploymentInstanceStatus_Description { get { return GetResourceString("DeploymentInstanceStatus_Description"); } }
//Resources:DeploymentAdminResources:DeploymentInstanceStatus_Title

		public static string DeploymentInstanceStatus_Title { get { return GetResourceString("DeploymentInstanceStatus_Title"); } }
//Resources:DeploymentAdminResources:DeploymentQueueType_InMemory

		public static string DeploymentQueueType_InMemory { get { return GetResourceString("DeploymentQueueType_InMemory"); } }
//Resources:DeploymentAdminResources:DeploymentQueueType_Kafka

		public static string DeploymentQueueType_Kafka { get { return GetResourceString("DeploymentQueueType_Kafka"); } }
//Resources:DeploymentAdminResources:DeploymentQueueType_QueueTechnology

		public static string DeploymentQueueType_QueueTechnology { get { return GetResourceString("DeploymentQueueType_QueueTechnology"); } }
//Resources:DeploymentAdminResources:DeploymentQueueType_QueueTechnology_Help

		public static string DeploymentQueueType_QueueTechnology_Help { get { return GetResourceString("DeploymentQueueType_QueueTechnology_Help"); } }
//Resources:DeploymentAdminResources:DeploymentQueueType_QueueTechnology_Select

		public static string DeploymentQueueType_QueueTechnology_Select { get { return GetResourceString("DeploymentQueueType_QueueTechnology_Select"); } }
//Resources:DeploymentAdminResources:DeploymentQueueType_RabbitMQ

		public static string DeploymentQueueType_RabbitMQ { get { return GetResourceString("DeploymentQueueType_RabbitMQ"); } }
//Resources:DeploymentAdminResources:DeploymentQueueType_ServiceBus

		public static string DeploymentQueueType_ServiceBus { get { return GetResourceString("DeploymentQueueType_ServiceBus"); } }
//Resources:DeploymentAdminResources:DeploymentType_Cloud

		public static string DeploymentType_Cloud { get { return GetResourceString("DeploymentType_Cloud"); } }
//Resources:DeploymentAdminResources:DeploymentType_Managed

		public static string DeploymentType_Managed { get { return GetResourceString("DeploymentType_Managed"); } }
//Resources:DeploymentAdminResources:DeploymentType_OnPremise

		public static string DeploymentType_OnPremise { get { return GetResourceString("DeploymentType_OnPremise"); } }
//Resources:DeploymentAdminResources:DeploymentType_Shared

		public static string DeploymentType_Shared { get { return GetResourceString("DeploymentType_Shared"); } }
//Resources:DeploymentAdminResources:DeviceCommand_Description

		public static string DeviceCommand_Description { get { return GetResourceString("DeviceCommand_Description"); } }
//Resources:DeploymentAdminResources:DeviceCommand_TItle

		public static string DeviceCommand_TItle { get { return GetResourceString("DeviceCommand_TItle"); } }
//Resources:DeploymentAdminResources:DeviceConfig_Commands

		public static string DeviceConfig_Commands { get { return GetResourceString("DeviceConfig_Commands"); } }
//Resources:DeploymentAdminResources:DeviceConfig_Commands_Help

		public static string DeviceConfig_Commands_Help { get { return GetResourceString("DeviceConfig_Commands_Help"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_CustomStatusType

		public static string DeviceConfiguration_CustomStatusType { get { return GetResourceString("DeviceConfiguration_CustomStatusType"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_CustomStatusType_Help

		public static string DeviceConfiguration_CustomStatusType_Help { get { return GetResourceString("DeviceConfiguration_CustomStatusType_Help"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_CustomStatusType_Watermark

		public static string DeviceConfiguration_CustomStatusType_Watermark { get { return GetResourceString("DeviceConfiguration_CustomStatusType_Watermark"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_Description

		public static string DeviceConfiguration_Description { get { return GetResourceString("DeviceConfiguration_Description"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_DeviceErrorCodes

		public static string DeviceConfiguration_DeviceErrorCodes { get { return GetResourceString("DeviceConfiguration_DeviceErrorCodes"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_DeviceIdLabel

		public static string DeviceConfiguration_DeviceIdLabel { get { return GetResourceString("DeviceConfiguration_DeviceIdLabel"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_DeviceIdLabel_Default

		public static string DeviceConfiguration_DeviceIdLabel_Default { get { return GetResourceString("DeviceConfiguration_DeviceIdLabel_Default"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_DeviceIdLabel_Help

		public static string DeviceConfiguration_DeviceIdLabel_Help { get { return GetResourceString("DeviceConfiguration_DeviceIdLabel_Help"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_DeviceLabel

		public static string DeviceConfiguration_DeviceLabel { get { return GetResourceString("DeviceConfiguration_DeviceLabel"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_DeviceLabel_Default

		public static string DeviceConfiguration_DeviceLabel_Default { get { return GetResourceString("DeviceConfiguration_DeviceLabel_Default"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_DeviceLabel_Help

		public static string DeviceConfiguration_DeviceLabel_Help { get { return GetResourceString("DeviceConfiguration_DeviceLabel_Help"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_DeviceNameLabel

		public static string DeviceConfiguration_DeviceNameLabel { get { return GetResourceString("DeviceConfiguration_DeviceNameLabel"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_DeviceNameLabel_Default

		public static string DeviceConfiguration_DeviceNameLabel_Default { get { return GetResourceString("DeviceConfiguration_DeviceNameLabel_Default"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_DeviceNameLabel_Help

		public static string DeviceConfiguration_DeviceNameLabel_Help { get { return GetResourceString("DeviceConfiguration_DeviceNameLabel_Help"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_DeviceTypeLabel

		public static string DeviceConfiguration_DeviceTypeLabel { get { return GetResourceString("DeviceConfiguration_DeviceTypeLabel"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_DeviceTypeLabel_Default

		public static string DeviceConfiguration_DeviceTypeLabel_Default { get { return GetResourceString("DeviceConfiguration_DeviceTypeLabel_Default"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_DeviceTypeLabel_Help

		public static string DeviceConfiguration_DeviceTypeLabel_Help { get { return GetResourceString("DeviceConfiguration_DeviceTypeLabel_Help"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_Help

		public static string DeviceConfiguration_Help { get { return GetResourceString("DeviceConfiguration_Help"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_Icon

		public static string DeviceConfiguration_Icon { get { return GetResourceString("DeviceConfiguration_Icon"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_MessageWatchDogs

		public static string DeviceConfiguration_MessageWatchDogs { get { return GetResourceString("DeviceConfiguration_MessageWatchDogs"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_Properties

		public static string DeviceConfiguration_Properties { get { return GetResourceString("DeviceConfiguration_Properties"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_Properties_Help

		public static string DeviceConfiguration_Properties_Help { get { return GetResourceString("DeviceConfiguration_Properties_Help"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_Routes

		public static string DeviceConfiguration_Routes { get { return GetResourceString("DeviceConfiguration_Routes"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_SensorDefintions

		public static string DeviceConfiguration_SensorDefintions { get { return GetResourceString("DeviceConfiguration_SensorDefintions"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_Sentinel

		public static string DeviceConfiguration_Sentinel { get { return GetResourceString("DeviceConfiguration_Sentinel"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_Title

		public static string DeviceConfiguration_Title { get { return GetResourceString("DeviceConfiguration_Title"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_WatchDogEnabled_Default

		public static string DeviceConfiguration_WatchDogEnabled_Default { get { return GetResourceString("DeviceConfiguration_WatchDogEnabled_Default"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_WatchDogEnabled_Default_Help

		public static string DeviceConfiguration_WatchDogEnabled_Default_Help { get { return GetResourceString("DeviceConfiguration_WatchDogEnabled_Default_Help"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_WatchDogTimeout

		public static string DeviceConfiguration_WatchDogTimeout { get { return GetResourceString("DeviceConfiguration_WatchDogTimeout"); } }
//Resources:DeploymentAdminResources:DeviceConfiguration_WatchDogTimeout_Help

		public static string DeviceConfiguration_WatchDogTimeout_Help { get { return GetResourceString("DeviceConfiguration_WatchDogTimeout_Help"); } }
//Resources:DeploymentAdminResources:DeviceConfigurations_Title

		public static string DeviceConfigurations_Title { get { return GetResourceString("DeviceConfigurations_Title"); } }
//Resources:DeploymentAdminResources:DeviceConfiguratoin_DeviceTypeLabel_Help

		public static string DeviceConfiguratoin_DeviceTypeLabel_Help { get { return GetResourceString("DeviceConfiguratoin_DeviceTypeLabel_Help"); } }
//Resources:DeploymentAdminResources:DeviceConfiugration_CustomMobilePage

		public static string DeviceConfiugration_CustomMobilePage { get { return GetResourceString("DeviceConfiugration_CustomMobilePage"); } }
//Resources:DeploymentAdminResources:DeviceConfiugration_CustomMobilePage_Help

		public static string DeviceConfiugration_CustomMobilePage_Help { get { return GetResourceString("DeviceConfiugration_CustomMobilePage_Help"); } }
//Resources:DeploymentAdminResources:DeviceConfiugration_CustomPage

		public static string DeviceConfiugration_CustomPage { get { return GetResourceString("DeviceConfiugration_CustomPage"); } }
//Resources:DeploymentAdminResources:DeviceConfiugration_CustomPage_Help

		public static string DeviceConfiugration_CustomPage_Help { get { return GetResourceString("DeviceConfiugration_CustomPage_Help"); } }
//Resources:DeploymentAdminResources:DeviceConfiugration_CustomPage_QuickLink

		public static string DeviceConfiugration_CustomPage_QuickLink { get { return GetResourceString("DeviceConfiugration_CustomPage_QuickLink"); } }
//Resources:DeploymentAdminResources:DeviceConfiugration_CustomPage_QuickLink_Help

		public static string DeviceConfiugration_CustomPage_QuickLink_Help { get { return GetResourceString("DeviceConfiugration_CustomPage_QuickLink_Help"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_AutoExpiresTimespan

		public static string DeviceErrorCode_AutoExpiresTimespan { get { return GetResourceString("DeviceErrorCode_AutoExpiresTimespan"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_AutoExpiresTimespan_Help

		public static string DeviceErrorCode_AutoExpiresTimespan_Help { get { return GetResourceString("DeviceErrorCode_AutoExpiresTimespan_Help"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_AutoExpiresTimespanQuantity

		public static string DeviceErrorCode_AutoExpiresTimespanQuantity { get { return GetResourceString("DeviceErrorCode_AutoExpiresTimespanQuantity"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_Days

		public static string DeviceErrorCode_Days { get { return GetResourceString("DeviceErrorCode_Days"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_Description

		public static string DeviceErrorCode_Description { get { return GetResourceString("DeviceErrorCode_Description"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_DeviceNotification

		public static string DeviceErrorCode_DeviceNotification { get { return GetResourceString("DeviceErrorCode_DeviceNotification"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_DeviceNotification_Help

		public static string DeviceErrorCode_DeviceNotification_Help { get { return GetResourceString("DeviceErrorCode_DeviceNotification_Help"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_DeviceNotification_Select

		public static string DeviceErrorCode_DeviceNotification_Select { get { return GetResourceString("DeviceErrorCode_DeviceNotification_Select"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_DistributionList

		public static string DeviceErrorCode_DistributionList { get { return GetResourceString("DeviceErrorCode_DistributionList"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_DistributionList_Help

		public static string DeviceErrorCode_DistributionList_Help { get { return GetResourceString("DeviceErrorCode_DistributionList_Help"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_DistributionList_Select

		public static string DeviceErrorCode_DistributionList_Select { get { return GetResourceString("DeviceErrorCode_DistributionList_Select"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_EmailSubject

		public static string DeviceErrorCode_EmailSubject { get { return GetResourceString("DeviceErrorCode_EmailSubject"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_EmailSubject_Help

		public static string DeviceErrorCode_EmailSubject_Help { get { return GetResourceString("DeviceErrorCode_EmailSubject_Help"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_ErrorCode

		public static string DeviceErrorCode_ErrorCode { get { return GetResourceString("DeviceErrorCode_ErrorCode"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_EscalateAfterAttemptCount

		public static string DeviceErrorCode_EscalateAfterAttemptCount { get { return GetResourceString("DeviceErrorCode_EscalateAfterAttemptCount"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_EscalateAfterAttemptCount_Help

		public static string DeviceErrorCode_EscalateAfterAttemptCount_Help { get { return GetResourceString("DeviceErrorCode_EscalateAfterAttemptCount_Help"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_Help

		public static string DeviceErrorCode_Help { get { return GetResourceString("DeviceErrorCode_Help"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_Hours

		public static string DeviceErrorCode_Hours { get { return GetResourceString("DeviceErrorCode_Hours"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_IncidentProtocol

		public static string DeviceErrorCode_IncidentProtocol { get { return GetResourceString("DeviceErrorCode_IncidentProtocol"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_IncidentProtocol_Help

		public static string DeviceErrorCode_IncidentProtocol_Help { get { return GetResourceString("DeviceErrorCode_IncidentProtocol_Help"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_IncidentProtocol_Select

		public static string DeviceErrorCode_IncidentProtocol_Select { get { return GetResourceString("DeviceErrorCode_IncidentProtocol_Select"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_Minutes

		public static string DeviceErrorCode_Minutes { get { return GetResourceString("DeviceErrorCode_Minutes"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_NotApplicable

		public static string DeviceErrorCode_NotApplicable { get { return GetResourceString("DeviceErrorCode_NotApplicable"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_NotificationInterval

		public static string DeviceErrorCode_NotificationInterval { get { return GetResourceString("DeviceErrorCode_NotificationInterval"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_NotificationInterval_Help

		public static string DeviceErrorCode_NotificationInterval_Help { get { return GetResourceString("DeviceErrorCode_NotificationInterval_Help"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_NotificationIntervalQuantity

		public static string DeviceErrorCode_NotificationIntervalQuantity { get { return GetResourceString("DeviceErrorCode_NotificationIntervalQuantity"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_NotifyOnClear

		public static string DeviceErrorCode_NotifyOnClear { get { return GetResourceString("DeviceErrorCode_NotifyOnClear"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_NotifyOnRaise

		public static string DeviceErrorCode_NotifyOnRaise { get { return GetResourceString("DeviceErrorCode_NotifyOnRaise"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_SelectTimespan

		public static string DeviceErrorCode_SelectTimespan { get { return GetResourceString("DeviceErrorCode_SelectTimespan"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_SendEmail

		public static string DeviceErrorCode_SendEmail { get { return GetResourceString("DeviceErrorCode_SendEmail"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_SendSMS

		public static string DeviceErrorCode_SendSMS { get { return GetResourceString("DeviceErrorCode_SendSMS"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_TicketTemplate

		public static string DeviceErrorCode_TicketTemplate { get { return GetResourceString("DeviceErrorCode_TicketTemplate"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_TicketTemplate_Help

		public static string DeviceErrorCode_TicketTemplate_Help { get { return GetResourceString("DeviceErrorCode_TicketTemplate_Help"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_TicketTemplate_Select

		public static string DeviceErrorCode_TicketTemplate_Select { get { return GetResourceString("DeviceErrorCode_TicketTemplate_Select"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_Title

		public static string DeviceErrorCode_Title { get { return GetResourceString("DeviceErrorCode_Title"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_TriggerOnEachOccurrence

		public static string DeviceErrorCode_TriggerOnEachOccurrence { get { return GetResourceString("DeviceErrorCode_TriggerOnEachOccurrence"); } }
//Resources:DeploymentAdminResources:DeviceErrorCode_TriggerOnEachOccurrence_Help

		public static string DeviceErrorCode_TriggerOnEachOccurrence_Help { get { return GetResourceString("DeviceErrorCode_TriggerOnEachOccurrence_Help"); } }
//Resources:DeploymentAdminResources:DeviceErrorCodes_Title

		public static string DeviceErrorCodes_Title { get { return GetResourceString("DeviceErrorCodes_Title"); } }
//Resources:DeploymentAdminResources:DeviceNotification_EmailSubject

		public static string DeviceNotification_EmailSubject { get { return GetResourceString("DeviceNotification_EmailSubject"); } }
//Resources:DeploymentAdminResources:DeviceNotification_EmailSubject_Help

		public static string DeviceNotification_EmailSubject_Help { get { return GetResourceString("DeviceNotification_EmailSubject_Help"); } }
//Resources:DeploymentAdminResources:DeviceNotification_SMS_Help

		public static string DeviceNotification_SMS_Help { get { return GetResourceString("DeviceNotification_SMS_Help"); } }
//Resources:DeploymentAdminResources:DeviceNotification_TagHelp

		public static string DeviceNotification_TagHelp { get { return GetResourceString("DeviceNotification_TagHelp"); } }
//Resources:DeploymentAdminResources:DeviceNotification_Title

		public static string DeviceNotification_Title { get { return GetResourceString("DeviceNotification_Title"); } }
//Resources:DeploymentAdminResources:DeviceNotificationHistory_Description

		public static string DeviceNotificationHistory_Description { get { return GetResourceString("DeviceNotificationHistory_Description"); } }
//Resources:DeploymentAdminResources:DeviceNotificationHistory_Title

		public static string DeviceNotificationHistory_Title { get { return GetResourceString("DeviceNotificationHistory_Title"); } }
//Resources:DeploymentAdminResources:DeviceNotifications_CotNotifications

		public static string DeviceNotifications_CotNotifications { get { return GetResourceString("DeviceNotifications_CotNotifications"); } }
//Resources:DeploymentAdminResources:DeviceNotifications_Description

		public static string DeviceNotifications_Description { get { return GetResourceString("DeviceNotifications_Description"); } }
//Resources:DeploymentAdminResources:DeviceNotifications_EmailContent

		public static string DeviceNotifications_EmailContent { get { return GetResourceString("DeviceNotifications_EmailContent"); } }
//Resources:DeploymentAdminResources:DeviceNotifications_EmailSubject

		public static string DeviceNotifications_EmailSubject { get { return GetResourceString("DeviceNotifications_EmailSubject"); } }
//Resources:DeploymentAdminResources:DeviceNotifications_Escalation_Notification

		public static string DeviceNotifications_Escalation_Notification { get { return GetResourceString("DeviceNotifications_Escalation_Notification"); } }
//Resources:DeploymentAdminResources:DeviceNotifications_Escalation_Notification_Help

		public static string DeviceNotifications_Escalation_Notification_Help { get { return GetResourceString("DeviceNotifications_Escalation_Notification_Help"); } }
//Resources:DeploymentAdminResources:DeviceNotifications_ForwardToParent

		public static string DeviceNotifications_ForwardToParent { get { return GetResourceString("DeviceNotifications_ForwardToParent"); } }
//Resources:DeploymentAdminResources:DeviceNotifications_ForwardToParentBody

		public static string DeviceNotifications_ForwardToParentBody { get { return GetResourceString("DeviceNotifications_ForwardToParentBody"); } }
//Resources:DeploymentAdminResources:DeviceNotifications_IncludeLandingPageContent

		public static string DeviceNotifications_IncludeLandingPageContent { get { return GetResourceString("DeviceNotifications_IncludeLandingPageContent"); } }
//Resources:DeploymentAdminResources:DeviceNotifications_LandingPageContent

		public static string DeviceNotifications_LandingPageContent { get { return GetResourceString("DeviceNotifications_LandingPageContent"); } }
//Resources:DeploymentAdminResources:DeviceNotifications_MqttNotifications

		public static string DeviceNotifications_MqttNotifications { get { return GetResourceString("DeviceNotifications_MqttNotifications"); } }
//Resources:DeploymentAdminResources:DeviceNotifications_RestNotifications

		public static string DeviceNotifications_RestNotifications { get { return GetResourceString("DeviceNotifications_RestNotifications"); } }
//Resources:DeploymentAdminResources:DeviceNotifications_SendEmail

		public static string DeviceNotifications_SendEmail { get { return GetResourceString("DeviceNotifications_SendEmail"); } }
//Resources:DeploymentAdminResources:DeviceNotifications_SendSMS

		public static string DeviceNotifications_SendSMS { get { return GetResourceString("DeviceNotifications_SendSMS"); } }
//Resources:DeploymentAdminResources:DeviceNotifications_SharedTemplate

		public static string DeviceNotifications_SharedTemplate { get { return GetResourceString("DeviceNotifications_SharedTemplate"); } }
//Resources:DeploymentAdminResources:DeviceNotifications_SMSContent

		public static string DeviceNotifications_SMSContent { get { return GetResourceString("DeviceNotifications_SMSContent"); } }
//Resources:DeploymentAdminResources:DeviceNotifications_Title

		public static string DeviceNotifications_Title { get { return GetResourceString("DeviceNotifications_Title"); } }
//Resources:DeploymentAdminResources:Err_CantPublishNotRunning

		public static string Err_CantPublishNotRunning { get { return GetResourceString("Err_CantPublishNotRunning"); } }
//Resources:DeploymentAdminResources:Err_CouldNotFindDestinationModule

		public static string Err_CouldNotFindDestinationModule { get { return GetResourceString("Err_CouldNotFindDestinationModule"); } }
//Resources:DeploymentAdminResources:Err_CouldNotLoadDeviceConfiguration

		public static string Err_CouldNotLoadDeviceConfiguration { get { return GetResourceString("Err_CouldNotLoadDeviceConfiguration"); } }
//Resources:DeploymentAdminResources:Err_CouldNotLoadInstance

		public static string Err_CouldNotLoadInstance { get { return GetResourceString("Err_CouldNotLoadInstance"); } }
//Resources:DeploymentAdminResources:Err_CouldNotLoadListener

		public static string Err_CouldNotLoadListener { get { return GetResourceString("Err_CouldNotLoadListener"); } }
//Resources:DeploymentAdminResources:Err_CouldNotLoadPlanner

		public static string Err_CouldNotLoadPlanner { get { return GetResourceString("Err_CouldNotLoadPlanner"); } }
//Resources:DeploymentAdminResources:Err_CouldNotLoadSolution

		public static string Err_CouldNotLoadSolution { get { return GetResourceString("Err_CouldNotLoadSolution"); } }
//Resources:DeploymentAdminResources:Err_CouldntStart_NotOffline

		public static string Err_CouldntStart_NotOffline { get { return GetResourceString("Err_CouldntStart_NotOffline"); } }
//Resources:DeploymentAdminResources:Err_CouldntStop_NotRunning

		public static string Err_CouldntStop_NotRunning { get { return GetResourceString("Err_CouldntStop_NotRunning"); } }
//Resources:DeploymentAdminResources:Err_EmptyRoute

		public static string Err_EmptyRoute { get { return GetResourceString("Err_EmptyRoute"); } }
//Resources:DeploymentAdminResources:Err_ErrorCommunicatingWithHost

		public static string Err_ErrorCommunicatingWithHost { get { return GetResourceString("Err_ErrorCommunicatingWithHost"); } }
//Resources:DeploymentAdminResources:Err_InstanceAlreadyRunning

		public static string Err_InstanceAlreadyRunning { get { return GetResourceString("Err_InstanceAlreadyRunning"); } }
//Resources:DeploymentAdminResources:Err_InstanceNotRunning

		public static string Err_InstanceNotRunning { get { return GetResourceString("Err_InstanceNotRunning"); } }
//Resources:DeploymentAdminResources:Err_InstanceWithoutHost

		public static string Err_InstanceWithoutHost { get { return GetResourceString("Err_InstanceWithoutHost"); } }
//Resources:DeploymentAdminResources:Err_InstanceWithoutSolution

		public static string Err_InstanceWithoutSolution { get { return GetResourceString("Err_InstanceWithoutSolution"); } }
//Resources:DeploymentAdminResources:Err_MCPServerExists

		public static string Err_MCPServerExists { get { return GetResourceString("Err_MCPServerExists"); } }
//Resources:DeploymentAdminResources:Err_MultipleMCPServersFound

		public static string Err_MultipleMCPServersFound { get { return GetResourceString("Err_MultipleMCPServersFound"); } }
//Resources:DeploymentAdminResources:Err_MultipleNotificationServersFound

		public static string Err_MultipleNotificationServersFound { get { return GetResourceString("Err_MultipleNotificationServersFound"); } }
//Resources:DeploymentAdminResources:Err_NoMCPServerExists

		public static string Err_NoMCPServerExists { get { return GetResourceString("Err_NoMCPServerExists"); } }
//Resources:DeploymentAdminResources:Err_NoMessageDefinitionOnRoute

		public static string Err_NoMessageDefinitionOnRoute { get { return GetResourceString("Err_NoMessageDefinitionOnRoute"); } }
//Resources:DeploymentAdminResources:Err_NoNotificationsServerExists

		public static string Err_NoNotificationsServerExists { get { return GetResourceString("Err_NoNotificationsServerExists"); } }
//Resources:DeploymentAdminResources:Err_NoPlannerHasBeenSpecified

		public static string Err_NoPlannerHasBeenSpecified { get { return GetResourceString("Err_NoPlannerHasBeenSpecified"); } }
//Resources:DeploymentAdminResources:Err_NotificationServerExists

		public static string Err_NotificationServerExists { get { return GetResourceString("Err_NotificationServerExists"); } }
//Resources:DeploymentAdminResources:Err_RouteModule_ModuleIsRequired

		public static string Err_RouteModule_ModuleIsRequired { get { return GetResourceString("Err_RouteModule_ModuleIsRequired"); } }
//Resources:DeploymentAdminResources:Err_RouteModule_ModuleTypeNotDefined

		public static string Err_RouteModule_ModuleTypeNotDefined { get { return GetResourceString("Err_RouteModule_ModuleTypeNotDefined"); } }
//Resources:DeploymentAdminResources:Err_RouteModule_NameNotDefined

		public static string Err_RouteModule_NameNotDefined { get { return GetResourceString("Err_RouteModule_NameNotDefined"); } }
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
//Resources:DeploymentAdminResources:Host_AverageCPU_1_Minute

		public static string Host_AverageCPU_1_Minute { get { return GetResourceString("Host_AverageCPU_1_Minute"); } }
//Resources:DeploymentAdminResources:Host_AverageMemory_1_Minute

		public static string Host_AverageMemory_1_Minute { get { return GetResourceString("Host_AverageMemory_1_Minute"); } }
//Resources:DeploymentAdminResources:Host_CapacityStatus

		public static string Host_CapacityStatus { get { return GetResourceString("Host_CapacityStatus"); } }
//Resources:DeploymentAdminResources:Host_CloudProvider

		public static string Host_CloudProvider { get { return GetResourceString("Host_CloudProvider"); } }
//Resources:DeploymentAdminResources:Host_CloudProvider_Help

		public static string Host_CloudProvider_Help { get { return GetResourceString("Host_CloudProvider_Help"); } }
//Resources:DeploymentAdminResources:Host_CloudProviders

		public static string Host_CloudProviders { get { return GetResourceString("Host_CloudProviders"); } }
//Resources:DeploymentAdminResources:Host_ComputeResource_Uri

		public static string Host_ComputeResource_Uri { get { return GetResourceString("Host_ComputeResource_Uri"); } }
//Resources:DeploymentAdminResources:Host_ComputeResource_Uri_Help

		public static string Host_ComputeResource_Uri_Help { get { return GetResourceString("Host_ComputeResource_Uri_Help"); } }
//Resources:DeploymentAdminResources:Host_ComputeResourceId

		public static string Host_ComputeResourceId { get { return GetResourceString("Host_ComputeResourceId"); } }
//Resources:DeploymentAdminResources:Host_ComputeResourceId_Help

		public static string Host_ComputeResourceId_Help { get { return GetResourceString("Host_ComputeResourceId_Help"); } }
//Resources:DeploymentAdminResources:Host_ContainerRepository

		public static string Host_ContainerRepository { get { return GetResourceString("Host_ContainerRepository"); } }
//Resources:DeploymentAdminResources:Host_ContainerRepository_Select

		public static string Host_ContainerRepository_Select { get { return GetResourceString("Host_ContainerRepository_Select"); } }
//Resources:DeploymentAdminResources:Host_ContainerTag

		public static string Host_ContainerTag { get { return GetResourceString("Host_ContainerTag"); } }
//Resources:DeploymentAdminResources:Host_ContainerTag_Select

		public static string Host_ContainerTag_Select { get { return GetResourceString("Host_ContainerTag_Select"); } }
//Resources:DeploymentAdminResources:Host_DateStampOnline

		public static string Host_DateStampOnline { get { return GetResourceString("Host_DateStampOnline"); } }
//Resources:DeploymentAdminResources:Host_DebugMode

		public static string Host_DebugMode { get { return GetResourceString("Host_DebugMode"); } }
//Resources:DeploymentAdminResources:Host_DebugMode_Help

		public static string Host_DebugMode_Help { get { return GetResourceString("Host_DebugMode_Help"); } }
//Resources:DeploymentAdminResources:Host_DedicatedInstance

		public static string Host_DedicatedInstance { get { return GetResourceString("Host_DedicatedInstance"); } }
//Resources:DeploymentAdminResources:Host_Description

		public static string Host_Description { get { return GetResourceString("Host_Description"); } }
//Resources:DeploymentAdminResources:Host_DNSName

		public static string Host_DNSName { get { return GetResourceString("Host_DNSName"); } }
//Resources:DeploymentAdminResources:Host_HasSSLCert

		public static string Host_HasSSLCert { get { return GetResourceString("Host_HasSSLCert"); } }
//Resources:DeploymentAdminResources:Host_Help

		public static string Host_Help { get { return GetResourceString("Host_Help"); } }
//Resources:DeploymentAdminResources:Host_InternalServiceName

		public static string Host_InternalServiceName { get { return GetResourceString("Host_InternalServiceName"); } }
//Resources:DeploymentAdminResources:Host_IPv4_Address

		public static string Host_IPv4_Address { get { return GetResourceString("Host_IPv4_Address"); } }
//Resources:DeploymentAdminResources:Host_LastPing

		public static string Host_LastPing { get { return GetResourceString("Host_LastPing"); } }
//Resources:DeploymentAdminResources:Host_MonitoringProvider

		public static string Host_MonitoringProvider { get { return GetResourceString("Host_MonitoringProvider"); } }
//Resources:DeploymentAdminResources:Host_MonitoringURI

		public static string Host_MonitoringURI { get { return GetResourceString("Host_MonitoringURI"); } }
//Resources:DeploymentAdminResources:Host_SelectSize

		public static string Host_SelectSize { get { return GetResourceString("Host_SelectSize"); } }
//Resources:DeploymentAdminResources:Host_ShowSiteDetails

		public static string Host_ShowSiteDetails { get { return GetResourceString("Host_ShowSiteDetails"); } }
//Resources:DeploymentAdminResources:Host_ShowSiteDetails_Help

		public static string Host_ShowSiteDetails_Help { get { return GetResourceString("Host_ShowSiteDetails_Help"); } }
//Resources:DeploymentAdminResources:Host_Size

		public static string Host_Size { get { return GetResourceString("Host_Size"); } }
//Resources:DeploymentAdminResources:Host_SSLExpires

		public static string Host_SSLExpires { get { return GetResourceString("Host_SSLExpires"); } }
//Resources:DeploymentAdminResources:Host_Status

		public static string Host_Status { get { return GetResourceString("Host_Status"); } }
//Resources:DeploymentAdminResources:Host_StatusDetails

		public static string Host_StatusDetails { get { return GetResourceString("Host_StatusDetails"); } }
//Resources:DeploymentAdminResources:Host_StatusTimeStamp

		public static string Host_StatusTimeStamp { get { return GetResourceString("Host_StatusTimeStamp"); } }
//Resources:DeploymentAdminResources:Host_Subscription

		public static string Host_Subscription { get { return GetResourceString("Host_Subscription"); } }
//Resources:DeploymentAdminResources:Host_SubscriptionSelect

		public static string Host_SubscriptionSelect { get { return GetResourceString("Host_SubscriptionSelect"); } }
//Resources:DeploymentAdminResources:Host_Title

		public static string Host_Title { get { return GetResourceString("Host_Title"); } }
//Resources:DeploymentAdminResources:Host_Type

		public static string Host_Type { get { return GetResourceString("Host_Type"); } }
//Resources:DeploymentAdminResources:Host_Type_BackupMCP

		public static string Host_Type_BackupMCP { get { return GetResourceString("Host_Type_BackupMCP"); } }
//Resources:DeploymentAdminResources:Host_Type_Clustered

		public static string Host_Type_Clustered { get { return GetResourceString("Host_Type_Clustered"); } }
//Resources:DeploymentAdminResources:Host_Type_Community

		public static string Host_Type_Community { get { return GetResourceString("Host_Type_Community"); } }
//Resources:DeploymentAdminResources:Host_Type_Dedicated

		public static string Host_Type_Dedicated { get { return GetResourceString("Host_Type_Dedicated"); } }
//Resources:DeploymentAdminResources:Host_Type_Free

		public static string Host_Type_Free { get { return GetResourceString("Host_Type_Free"); } }
//Resources:DeploymentAdminResources:Host_Type_MCP

		public static string Host_Type_MCP { get { return GetResourceString("Host_Type_MCP"); } }
//Resources:DeploymentAdminResources:Host_Type_Notifications

		public static string Host_Type_Notifications { get { return GetResourceString("Host_Type_Notifications"); } }
//Resources:DeploymentAdminResources:Host_Type_RemoteBackupMCP

		public static string Host_Type_RemoteBackupMCP { get { return GetResourceString("Host_Type_RemoteBackupMCP"); } }
//Resources:DeploymentAdminResources:Host_Type_RemoteMCP

		public static string Host_Type_RemoteMCP { get { return GetResourceString("Host_Type_RemoteMCP"); } }
//Resources:DeploymentAdminResources:Host_Type_Select

		public static string Host_Type_Select { get { return GetResourceString("Host_Type_Select"); } }
//Resources:DeploymentAdminResources:Host_Type_Shared

		public static string Host_Type_Shared { get { return GetResourceString("Host_Type_Shared"); } }
//Resources:DeploymentAdminResources:Host_Type_SharedHighPerformance

		public static string Host_Type_SharedHighPerformance { get { return GetResourceString("Host_Type_SharedHighPerformance"); } }
//Resources:DeploymentAdminResources:Host_UpSince

		public static string Host_UpSince { get { return GetResourceString("Host_UpSince"); } }
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
//Resources:DeploymentAdminResources:Hosts_Title

		public static string Hosts_Title { get { return GetResourceString("Hosts_Title"); } }
//Resources:DeploymentAdminResources:HostSize_ExtraLarge

		public static string HostSize_ExtraLarge { get { return GetResourceString("HostSize_ExtraLarge"); } }
//Resources:DeploymentAdminResources:HostSize_ExtraSmall

		public static string HostSize_ExtraSmall { get { return GetResourceString("HostSize_ExtraSmall"); } }
//Resources:DeploymentAdminResources:HostSize_Large

		public static string HostSize_Large { get { return GetResourceString("HostSize_Large"); } }
//Resources:DeploymentAdminResources:HostSize_Medium

		public static string HostSize_Medium { get { return GetResourceString("HostSize_Medium"); } }
//Resources:DeploymentAdminResources:HostSize_Small

		public static string HostSize_Small { get { return GetResourceString("HostSize_Small"); } }
//Resources:DeploymentAdminResources:HostStatus_ConfiguringDNS

		public static string HostStatus_ConfiguringDNS { get { return GetResourceString("HostStatus_ConfiguringDNS"); } }
//Resources:DeploymentAdminResources:HostStatus_Deploying

		public static string HostStatus_Deploying { get { return GetResourceString("HostStatus_Deploying"); } }
//Resources:DeploymentAdminResources:HostStatus_DeployingContainer

		public static string HostStatus_DeployingContainer { get { return GetResourceString("HostStatus_DeployingContainer"); } }
//Resources:DeploymentAdminResources:HostStatus_Destroying

		public static string HostStatus_Destroying { get { return GetResourceString("HostStatus_Destroying"); } }
//Resources:DeploymentAdminResources:HostStatus_FailedDeployment

		public static string HostStatus_FailedDeployment { get { return GetResourceString("HostStatus_FailedDeployment"); } }
//Resources:DeploymentAdminResources:HostStatus_HealthCheckFailed

		public static string HostStatus_HealthCheckFailed { get { return GetResourceString("HostStatus_HealthCheckFailed"); } }
//Resources:DeploymentAdminResources:HostStatus_Offline

		public static string HostStatus_Offline { get { return GetResourceString("HostStatus_Offline"); } }
//Resources:DeploymentAdminResources:HostStatus_QueuedForDeployment

		public static string HostStatus_QueuedForDeployment { get { return GetResourceString("HostStatus_QueuedForDeployment"); } }
//Resources:DeploymentAdminResources:HostStatus_RestartingContainer

		public static string HostStatus_RestartingContainer { get { return GetResourceString("HostStatus_RestartingContainer"); } }
//Resources:DeploymentAdminResources:HostStatus_RestartingHost

		public static string HostStatus_RestartingHost { get { return GetResourceString("HostStatus_RestartingHost"); } }
//Resources:DeploymentAdminResources:HostStatus_Running

		public static string HostStatus_Running { get { return GetResourceString("HostStatus_Running"); } }
//Resources:DeploymentAdminResources:HostStatus_Starting

		public static string HostStatus_Starting { get { return GetResourceString("HostStatus_Starting"); } }
//Resources:DeploymentAdminResources:HostStatus_StartingContainer

		public static string HostStatus_StartingContainer { get { return GetResourceString("HostStatus_StartingContainer"); } }
//Resources:DeploymentAdminResources:HostStatus_Stopped

		public static string HostStatus_Stopped { get { return GetResourceString("HostStatus_Stopped"); } }
//Resources:DeploymentAdminResources:HostStatus_Stopping

		public static string HostStatus_Stopping { get { return GetResourceString("HostStatus_Stopping"); } }
//Resources:DeploymentAdminResources:HostStatus_UpdatingRuntime

		public static string HostStatus_UpdatingRuntime { get { return GetResourceString("HostStatus_UpdatingRuntime"); } }
//Resources:DeploymentAdminResources:HostStatus_WaitingForServer

		public static string HostStatus_WaitingForServer { get { return GetResourceString("HostStatus_WaitingForServer"); } }
//Resources:DeploymentAdminResources:HostType_Development

		public static string HostType_Development { get { return GetResourceString("HostType_Development"); } }
//Resources:DeploymentAdminResources:HostType_MultiTenantMQTT

		public static string HostType_MultiTenantMQTT { get { return GetResourceString("HostType_MultiTenantMQTT"); } }
//Resources:DeploymentAdminResources:Incident_Description

		public static string Incident_Description { get { return GetResourceString("Incident_Description"); } }
//Resources:DeploymentAdminResources:Incident_Title

		public static string Incident_Title { get { return GetResourceString("Incident_Title"); } }
//Resources:DeploymentAdminResources:IncidentProtocol_Description

		public static string IncidentProtocol_Description { get { return GetResourceString("IncidentProtocol_Description"); } }
//Resources:DeploymentAdminResources:IncidentProtocol_Steps

		public static string IncidentProtocol_Steps { get { return GetResourceString("IncidentProtocol_Steps"); } }
//Resources:DeploymentAdminResources:IncidentProtocol_Title

		public static string IncidentProtocol_Title { get { return GetResourceString("IncidentProtocol_Title"); } }
//Resources:DeploymentAdminResources:IncidentProtocols_Title

		public static string IncidentProtocols_Title { get { return GetResourceString("IncidentProtocols_Title"); } }
//Resources:DeploymentAdminResources:IncidentProtocolStep_AssignedTo

		public static string IncidentProtocolStep_AssignedTo { get { return GetResourceString("IncidentProtocolStep_AssignedTo"); } }
//Resources:DeploymentAdminResources:IncidentProtocolStep_Description

		public static string IncidentProtocolStep_Description { get { return GetResourceString("IncidentProtocolStep_Description"); } }
//Resources:DeploymentAdminResources:IncidentProtocolStep_Title

		public static string IncidentProtocolStep_Title { get { return GetResourceString("IncidentProtocolStep_Title"); } }
//Resources:DeploymentAdminResources:Incidents_Title

		public static string Incidents_Title { get { return GetResourceString("Incidents_Title"); } }
//Resources:DeploymentAdminResources:Instance_Caches

		public static string Instance_Caches { get { return GetResourceString("Instance_Caches"); } }
//Resources:DeploymentAdminResources:Instance_Credentials

		public static string Instance_Credentials { get { return GetResourceString("Instance_Credentials"); } }
//Resources:DeploymentAdminResources:Instance_Credentials_Help

		public static string Instance_Credentials_Help { get { return GetResourceString("Instance_Credentials_Help"); } }
//Resources:DeploymentAdminResources:Instance_DataStreams

		public static string Instance_DataStreams { get { return GetResourceString("Instance_DataStreams"); } }
//Resources:DeploymentAdminResources:Instance_DebugMode

		public static string Instance_DebugMode { get { return GetResourceString("Instance_DebugMode"); } }
//Resources:DeploymentAdminResources:Instance_DebugMode_Help

		public static string Instance_DebugMode_Help { get { return GetResourceString("Instance_DebugMode_Help"); } }
//Resources:DeploymentAdminResources:Instance_DeploymentConfiguration

		public static string Instance_DeploymentConfiguration { get { return GetResourceString("Instance_DeploymentConfiguration"); } }
//Resources:DeploymentAdminResources:Instance_DeploymentConfiguration_Select

		public static string Instance_DeploymentConfiguration_Select { get { return GetResourceString("Instance_DeploymentConfiguration_Select"); } }
//Resources:DeploymentAdminResources:Instance_DeploymentType

		public static string Instance_DeploymentType { get { return GetResourceString("Instance_DeploymentType"); } }
//Resources:DeploymentAdminResources:Instance_DeploymentType_Select

		public static string Instance_DeploymentType_Select { get { return GetResourceString("Instance_DeploymentType_Select"); } }
//Resources:DeploymentAdminResources:Instance_Description

		public static string Instance_Description { get { return GetResourceString("Instance_Description"); } }
//Resources:DeploymentAdminResources:Instance_DeviceRepo

		public static string Instance_DeviceRepo { get { return GetResourceString("Instance_DeviceRepo"); } }
//Resources:DeploymentAdminResources:Instance_DeviceRepo_Help

		public static string Instance_DeviceRepo_Help { get { return GetResourceString("Instance_DeviceRepo_Help"); } }
//Resources:DeploymentAdminResources:Instance_DeviceRepo_Select

		public static string Instance_DeviceRepo_Select { get { return GetResourceString("Instance_DeviceRepo_Select"); } }
//Resources:DeploymentAdminResources:Instance_Help

		public static string Instance_Help { get { return GetResourceString("Instance_Help"); } }
//Resources:DeploymentAdminResources:Instance_Host

		public static string Instance_Host { get { return GetResourceString("Instance_Host"); } }
//Resources:DeploymentAdminResources:Instance_Host_Help

		public static string Instance_Host_Help { get { return GetResourceString("Instance_Host_Help"); } }
//Resources:DeploymentAdminResources:Instance_Host_Watermark

		public static string Instance_Host_Watermark { get { return GetResourceString("Instance_Host_Watermark"); } }
//Resources:DeploymentAdminResources:Instance_InputCommandAnonymous

		public static string Instance_InputCommandAnonymous { get { return GetResourceString("Instance_InputCommandAnonymous"); } }
//Resources:DeploymentAdminResources:Instance_InputCommandAnonymous_Help

		public static string Instance_InputCommandAnonymous_Help { get { return GetResourceString("Instance_InputCommandAnonymous_Help"); } }
//Resources:DeploymentAdminResources:Instance_InputCommandBasicAuthPassword

		public static string Instance_InputCommandBasicAuthPassword { get { return GetResourceString("Instance_InputCommandBasicAuthPassword"); } }
//Resources:DeploymentAdminResources:Instance_InputCommandBasicAuthUserName

		public static string Instance_InputCommandBasicAuthUserName { get { return GetResourceString("Instance_InputCommandBasicAuthUserName"); } }
//Resources:DeploymentAdminResources:Instance_InputCommandPort

		public static string Instance_InputCommandPort { get { return GetResourceString("Instance_InputCommandPort"); } }
//Resources:DeploymentAdminResources:Instance_InputCommandPort_Help

		public static string Instance_InputCommandPort_Help { get { return GetResourceString("Instance_InputCommandPort_Help"); } }
//Resources:DeploymentAdminResources:Instance_InputCommandSSL

		public static string Instance_InputCommandSSL { get { return GetResourceString("Instance_InputCommandSSL"); } }
//Resources:DeploymentAdminResources:Instance_InputCommandSSL_Help

		public static string Instance_InputCommandSSL_Help { get { return GetResourceString("Instance_InputCommandSSL_Help"); } }
//Resources:DeploymentAdminResources:Instance_IsDeployed

		public static string Instance_IsDeployed { get { return GetResourceString("Instance_IsDeployed"); } }
//Resources:DeploymentAdminResources:Instance_IsDeployed_Help

		public static string Instance_IsDeployed_Help { get { return GetResourceString("Instance_IsDeployed_Help"); } }
//Resources:DeploymentAdminResources:Instance_LastPing

		public static string Instance_LastPing { get { return GetResourceString("Instance_LastPing"); } }
//Resources:DeploymentAdminResources:Instance_LocalLogs

		public static string Instance_LocalLogs { get { return GetResourceString("Instance_LocalLogs"); } }
//Resources:DeploymentAdminResources:Instance_LocalMessageStorage

		public static string Instance_LocalMessageStorage { get { return GetResourceString("Instance_LocalMessageStorage"); } }
//Resources:DeploymentAdminResources:Instance_LocalUsageStatistics

		public static string Instance_LocalUsageStatistics { get { return GetResourceString("Instance_LocalUsageStatistics"); } }
//Resources:DeploymentAdminResources:Instance_PrimaryCache

		public static string Instance_PrimaryCache { get { return GetResourceString("Instance_PrimaryCache"); } }
//Resources:DeploymentAdminResources:Instance_PrimaryCache_Select

		public static string Instance_PrimaryCache_Select { get { return GetResourceString("Instance_PrimaryCache_Select"); } }
//Resources:DeploymentAdminResources:Instance_PrimaryCacheType

		public static string Instance_PrimaryCacheType { get { return GetResourceString("Instance_PrimaryCacheType"); } }
//Resources:DeploymentAdminResources:Instance_PrimaryCacheType_Select

		public static string Instance_PrimaryCacheType_Select { get { return GetResourceString("Instance_PrimaryCacheType_Select"); } }
//Resources:DeploymentAdminResources:Instance_QueueConnection

		public static string Instance_QueueConnection { get { return GetResourceString("Instance_QueueConnection"); } }
//Resources:DeploymentAdminResources:Instance_SettingsValues

		public static string Instance_SettingsValues { get { return GetResourceString("Instance_SettingsValues"); } }
//Resources:DeploymentAdminResources:Instance_Solution

		public static string Instance_Solution { get { return GetResourceString("Instance_Solution"); } }
//Resources:DeploymentAdminResources:Instance_Solution_Help

		public static string Instance_Solution_Help { get { return GetResourceString("Instance_Solution_Help"); } }
//Resources:DeploymentAdminResources:Instance_Solution_Select

		public static string Instance_Solution_Select { get { return GetResourceString("Instance_Solution_Select"); } }
//Resources:DeploymentAdminResources:Instance_Status

		public static string Instance_Status { get { return GetResourceString("Instance_Status"); } }
//Resources:DeploymentAdminResources:Instance_StatusDetails

		public static string Instance_StatusDetails { get { return GetResourceString("Instance_StatusDetails"); } }
//Resources:DeploymentAdminResources:Instance_StatusTimeStamp

		public static string Instance_StatusTimeStamp { get { return GetResourceString("Instance_StatusTimeStamp"); } }
//Resources:DeploymentAdminResources:Instance_Subscription_Help

		public static string Instance_Subscription_Help { get { return GetResourceString("Instance_Subscription_Help"); } }
//Resources:DeploymentAdminResources:Instance_TestMode

		public static string Instance_TestMode { get { return GetResourceString("Instance_TestMode"); } }
//Resources:DeploymentAdminResources:Instance_Title

		public static string Instance_Title { get { return GetResourceString("Instance_Title"); } }
//Resources:DeploymentAdminResources:Instance_UpSince

		public static string Instance_UpSince { get { return GetResourceString("Instance_UpSince"); } }
//Resources:DeploymentAdminResources:Instances_Title

		public static string Instances_Title { get { return GetResourceString("Instances_Title"); } }
//Resources:DeploymentAdminResources:InstanceStates_CreatingRuntime

		public static string InstanceStates_CreatingRuntime { get { return GetResourceString("InstanceStates_CreatingRuntime"); } }
//Resources:DeploymentAdminResources:InstanceStates_Degraded

		public static string InstanceStates_Degraded { get { return GetResourceString("InstanceStates_Degraded"); } }
//Resources:DeploymentAdminResources:InstanceStates_DeployingContainer

		public static string InstanceStates_DeployingContainer { get { return GetResourceString("InstanceStates_DeployingContainer"); } }
//Resources:DeploymentAdminResources:InstanceStates_DeployingRuntime

		public static string InstanceStates_DeployingRuntime { get { return GetResourceString("InstanceStates_DeployingRuntime"); } }
//Resources:DeploymentAdminResources:InstanceStates_FailedToDeploy

		public static string InstanceStates_FailedToDeploy { get { return GetResourceString("InstanceStates_FailedToDeploy"); } }
//Resources:DeploymentAdminResources:InstanceStates_FailedToInitialize

		public static string InstanceStates_FailedToInitialize { get { return GetResourceString("InstanceStates_FailedToInitialize"); } }
//Resources:DeploymentAdminResources:InstanceStates_FailedToStart

		public static string InstanceStates_FailedToStart { get { return GetResourceString("InstanceStates_FailedToStart"); } }
//Resources:DeploymentAdminResources:InstanceStates_FatalError

		public static string InstanceStates_FatalError { get { return GetResourceString("InstanceStates_FatalError"); } }
//Resources:DeploymentAdminResources:InstanceStates_HostFailedHealthCheck

		public static string InstanceStates_HostFailedHealthCheck { get { return GetResourceString("InstanceStates_HostFailedHealthCheck"); } }
//Resources:DeploymentAdminResources:InstanceStates_HostRestarting

		public static string InstanceStates_HostRestarting { get { return GetResourceString("InstanceStates_HostRestarting"); } }
//Resources:DeploymentAdminResources:InstanceStates_Initializing

		public static string InstanceStates_Initializing { get { return GetResourceString("InstanceStates_Initializing"); } }
//Resources:DeploymentAdminResources:InstanceStates_NotDeployed

		public static string InstanceStates_NotDeployed { get { return GetResourceString("InstanceStates_NotDeployed"); } }
//Resources:DeploymentAdminResources:InstanceStates_Offline

		public static string InstanceStates_Offline { get { return GetResourceString("InstanceStates_Offline"); } }
//Resources:DeploymentAdminResources:InstanceStates_Paused

		public static string InstanceStates_Paused { get { return GetResourceString("InstanceStates_Paused"); } }
//Resources:DeploymentAdminResources:InstanceStates_Pausing

		public static string InstanceStates_Pausing { get { return GetResourceString("InstanceStates_Pausing"); } }
//Resources:DeploymentAdminResources:InstanceStates_Ready

		public static string InstanceStates_Ready { get { return GetResourceString("InstanceStates_Ready"); } }
//Resources:DeploymentAdminResources:InstanceStates_RestartingRuntime

		public static string InstanceStates_RestartingRuntime { get { return GetResourceString("InstanceStates_RestartingRuntime"); } }
//Resources:DeploymentAdminResources:InstanceStates_Running

		public static string InstanceStates_Running { get { return GetResourceString("InstanceStates_Running"); } }
//Resources:DeploymentAdminResources:InstanceStates_Starting

		public static string InstanceStates_Starting { get { return GetResourceString("InstanceStates_Starting"); } }
//Resources:DeploymentAdminResources:InstanceStates_StartingRuntime

		public static string InstanceStates_StartingRuntime { get { return GetResourceString("InstanceStates_StartingRuntime"); } }
//Resources:DeploymentAdminResources:InstanceStates_Stopped

		public static string InstanceStates_Stopped { get { return GetResourceString("InstanceStates_Stopped"); } }
//Resources:DeploymentAdminResources:InstanceStates_Stopping

		public static string InstanceStates_Stopping { get { return GetResourceString("InstanceStates_Stopping"); } }
//Resources:DeploymentAdminResources:InstanceStates_Undeploying

		public static string InstanceStates_Undeploying { get { return GetResourceString("InstanceStates_Undeploying"); } }
//Resources:DeploymentAdminResources:InstanceStates_UpdatingRuntime

		public static string InstanceStates_UpdatingRuntime { get { return GetResourceString("InstanceStates_UpdatingRuntime"); } }
//Resources:DeploymentAdminResources:InstanceStates_UpdatingSolution

		public static string InstanceStates_UpdatingSolution { get { return GetResourceString("InstanceStates_UpdatingSolution"); } }
//Resources:DeploymentAdminResources:Integeration_APIKey

		public static string Integeration_APIKey { get { return GetResourceString("Integeration_APIKey"); } }
//Resources:DeploymentAdminResources:Integration_AccountId

		public static string Integration_AccountId { get { return GetResourceString("Integration_AccountId"); } }
//Resources:DeploymentAdminResources:Integration_Description

		public static string Integration_Description { get { return GetResourceString("Integration_Description"); } }
//Resources:DeploymentAdminResources:Integration_FromAddress

		public static string Integration_FromAddress { get { return GetResourceString("Integration_FromAddress"); } }
//Resources:DeploymentAdminResources:Integration_Help

		public static string Integration_Help { get { return GetResourceString("Integration_Help"); } }
//Resources:DeploymentAdminResources:Integration_RoutingKey

		public static string Integration_RoutingKey { get { return GetResourceString("Integration_RoutingKey"); } }
//Resources:DeploymentAdminResources:Integration_SMS

		public static string Integration_SMS { get { return GetResourceString("Integration_SMS"); } }
//Resources:DeploymentAdminResources:Integration_SMTP

		public static string Integration_SMTP { get { return GetResourceString("Integration_SMTP"); } }
//Resources:DeploymentAdminResources:Integration_Title

		public static string Integration_Title { get { return GetResourceString("Integration_Title"); } }
//Resources:DeploymentAdminResources:Integration_Uri

		public static string Integration_Uri { get { return GetResourceString("Integration_Uri"); } }
//Resources:DeploymentAdminResources:IntegrationType

		public static string IntegrationType { get { return GetResourceString("IntegrationType"); } }
//Resources:DeploymentAdminResources:IntegrationType_PagerDuty

		public static string IntegrationType_PagerDuty { get { return GetResourceString("IntegrationType_PagerDuty"); } }
//Resources:DeploymentAdminResources:IntegrationType_Select_Watermark

		public static string IntegrationType_Select_Watermark { get { return GetResourceString("IntegrationType_Select_Watermark"); } }
//Resources:DeploymentAdminResources:IntegrationType_SendGrid

		public static string IntegrationType_SendGrid { get { return GetResourceString("IntegrationType_SendGrid"); } }
//Resources:DeploymentAdminResources:IntegrationType_Twillio

		public static string IntegrationType_Twillio { get { return GetResourceString("IntegrationType_Twillio"); } }
//Resources:DeploymentAdminResources:MessageWatchDog_Description

		public static string MessageWatchDog_Description { get { return GetResourceString("MessageWatchDog_Description"); } }
//Resources:DeploymentAdminResources:MessageWatchDog_DeviceMessage_Select

		public static string MessageWatchDog_DeviceMessage_Select { get { return GetResourceString("MessageWatchDog_DeviceMessage_Select"); } }
//Resources:DeploymentAdminResources:MessageWatchDog_ErrorCode

		public static string MessageWatchDog_ErrorCode { get { return GetResourceString("MessageWatchDog_ErrorCode"); } }
//Resources:DeploymentAdminResources:MessageWatchDog_ErrorCode_Help

		public static string MessageWatchDog_ErrorCode_Help { get { return GetResourceString("MessageWatchDog_ErrorCode_Help"); } }
//Resources:DeploymentAdminResources:MessageWatchDog_ErrorCode_Select

		public static string MessageWatchDog_ErrorCode_Select { get { return GetResourceString("MessageWatchDog_ErrorCode_Select"); } }
//Resources:DeploymentAdminResources:MessageWatchDog_ExcludeHolidays

		public static string MessageWatchDog_ExcludeHolidays { get { return GetResourceString("MessageWatchDog_ExcludeHolidays"); } }
//Resources:DeploymentAdminResources:MessageWatchDog_Exclusion_Description

		public static string MessageWatchDog_Exclusion_Description { get { return GetResourceString("MessageWatchDog_Exclusion_Description"); } }
//Resources:DeploymentAdminResources:MessageWatchDog_Exclusion_End

		public static string MessageWatchDog_Exclusion_End { get { return GetResourceString("MessageWatchDog_Exclusion_End"); } }
//Resources:DeploymentAdminResources:MessageWatchDog_Exclusion_End_Help

		public static string MessageWatchDog_Exclusion_End_Help { get { return GetResourceString("MessageWatchDog_Exclusion_End_Help"); } }
//Resources:DeploymentAdminResources:MessageWatchDog_Exclusion_Help

		public static string MessageWatchDog_Exclusion_Help { get { return GetResourceString("MessageWatchDog_Exclusion_Help"); } }
//Resources:DeploymentAdminResources:MessageWatchDog_Exclusion_Start

		public static string MessageWatchDog_Exclusion_Start { get { return GetResourceString("MessageWatchDog_Exclusion_Start"); } }
//Resources:DeploymentAdminResources:MessageWatchDog_Exclusion_Start_Help

		public static string MessageWatchDog_Exclusion_Start_Help { get { return GetResourceString("MessageWatchDog_Exclusion_Start_Help"); } }
//Resources:DeploymentAdminResources:MessageWatchDog_Exclusion_Title

		public static string MessageWatchDog_Exclusion_Title { get { return GetResourceString("MessageWatchDog_Exclusion_Title"); } }
//Resources:DeploymentAdminResources:MessageWatchDog_Help

		public static string MessageWatchDog_Help { get { return GetResourceString("MessageWatchDog_Help"); } }
//Resources:DeploymentAdminResources:MessageWatchDog_Message

		public static string MessageWatchDog_Message { get { return GetResourceString("MessageWatchDog_Message"); } }
//Resources:DeploymentAdminResources:MessageWatchDog_Message_Help

		public static string MessageWatchDog_Message_Help { get { return GetResourceString("MessageWatchDog_Message_Help"); } }
//Resources:DeploymentAdminResources:MessageWatchDog_SaturdayExclusions

		public static string MessageWatchDog_SaturdayExclusions { get { return GetResourceString("MessageWatchDog_SaturdayExclusions"); } }
//Resources:DeploymentAdminResources:MessageWatchDog_StartupBuffer

		public static string MessageWatchDog_StartupBuffer { get { return GetResourceString("MessageWatchDog_StartupBuffer"); } }
//Resources:DeploymentAdminResources:MessageWatchDog_StartupBuffer_Help

		public static string MessageWatchDog_StartupBuffer_Help { get { return GetResourceString("MessageWatchDog_StartupBuffer_Help"); } }
//Resources:DeploymentAdminResources:MessageWatchDog_SundayExclusions

		public static string MessageWatchDog_SundayExclusions { get { return GetResourceString("MessageWatchDog_SundayExclusions"); } }
//Resources:DeploymentAdminResources:MessageWatchdog_Timeout

		public static string MessageWatchdog_Timeout { get { return GetResourceString("MessageWatchdog_Timeout"); } }
//Resources:DeploymentAdminResources:MessageWatchdog_Timeout_Help

		public static string MessageWatchdog_Timeout_Help { get { return GetResourceString("MessageWatchdog_Timeout_Help"); } }
//Resources:DeploymentAdminResources:MessageWatchdog_Timeout_Interval

		public static string MessageWatchdog_Timeout_Interval { get { return GetResourceString("MessageWatchdog_Timeout_Interval"); } }
//Resources:DeploymentAdminResources:MessageWatchdog_Timeout_Interval_Help

		public static string MessageWatchdog_Timeout_Interval_Help { get { return GetResourceString("MessageWatchdog_Timeout_Interval_Help"); } }
//Resources:DeploymentAdminResources:MessageWatchdog_Timeout_Interval_Select

		public static string MessageWatchdog_Timeout_Interval_Select { get { return GetResourceString("MessageWatchdog_Timeout_Interval_Select"); } }
//Resources:DeploymentAdminResources:MessageWatchDog_Title

		public static string MessageWatchDog_Title { get { return GetResourceString("MessageWatchDog_Title"); } }
//Resources:DeploymentAdminResources:MessageWatchDog_WeekdayExclusions

		public static string MessageWatchDog_WeekdayExclusions { get { return GetResourceString("MessageWatchDog_WeekdayExclusions"); } }
//Resources:DeploymentAdminResources:MqttNotification_Address

		public static string MqttNotification_Address { get { return GetResourceString("MqttNotification_Address"); } }
//Resources:DeploymentAdminResources:MqttNotification_Anonymous

		public static string MqttNotification_Anonymous { get { return GetResourceString("MqttNotification_Anonymous"); } }
//Resources:DeploymentAdminResources:MqttNotification_Certificate

		public static string MqttNotification_Certificate { get { return GetResourceString("MqttNotification_Certificate"); } }
//Resources:DeploymentAdminResources:MqttNotification_CertificatePassword

		public static string MqttNotification_CertificatePassword { get { return GetResourceString("MqttNotification_CertificatePassword"); } }
//Resources:DeploymentAdminResources:MqttNotification_ClientId

		public static string MqttNotification_ClientId { get { return GetResourceString("MqttNotification_ClientId"); } }
//Resources:DeploymentAdminResources:MqttNotification_ClientId_Help

		public static string MqttNotification_ClientId_Help { get { return GetResourceString("MqttNotification_ClientId_Help"); } }
//Resources:DeploymentAdminResources:MqttNotification_Password

		public static string MqttNotification_Password { get { return GetResourceString("MqttNotification_Password"); } }
//Resources:DeploymentAdminResources:MqttNotification_Payload

		public static string MqttNotification_Payload { get { return GetResourceString("MqttNotification_Payload"); } }
//Resources:DeploymentAdminResources:MqttNotification_Payload_Help

		public static string MqttNotification_Payload_Help { get { return GetResourceString("MqttNotification_Payload_Help"); } }
//Resources:DeploymentAdminResources:MqttNotification_Port

		public static string MqttNotification_Port { get { return GetResourceString("MqttNotification_Port"); } }
//Resources:DeploymentAdminResources:MqttNotification_SecureConnection

		public static string MqttNotification_SecureConnection { get { return GetResourceString("MqttNotification_SecureConnection"); } }
//Resources:DeploymentAdminResources:MqttNotification_Title

		public static string MqttNotification_Title { get { return GetResourceString("MqttNotification_Title"); } }
//Resources:DeploymentAdminResources:MqttNotification_Topic

		public static string MqttNotification_Topic { get { return GetResourceString("MqttNotification_Topic"); } }
//Resources:DeploymentAdminResources:MqttNotification_Topic_Help

		public static string MqttNotification_Topic_Help( string deviceid) { return GetResourceString("MqttNotification_Topic_Help", "{deviceid}", deviceid); }
//Resources:DeploymentAdminResources:MqttNotification_UserName

		public static string MqttNotification_UserName { get { return GetResourceString("MqttNotification_UserName"); } }
//Resources:DeploymentAdminResources:MqttNotifications_Help

		public static string MqttNotifications_Help { get { return GetResourceString("MqttNotifications_Help"); } }
//Resources:DeploymentAdminResources:MqttNotifications_Title

		public static string MqttNotifications_Title { get { return GetResourceString("MqttNotifications_Title"); } }
//Resources:DeploymentAdminResources:NuvIoT_Edition

		public static string NuvIoT_Edition { get { return GetResourceString("NuvIoT_Edition"); } }
//Resources:DeploymentAdminResources:NuvIoTEdition_App

		public static string NuvIoTEdition_App { get { return GetResourceString("NuvIoTEdition_App"); } }
//Resources:DeploymentAdminResources:NuvIoTEdition_Cluster

		public static string NuvIoTEdition_Cluster { get { return GetResourceString("NuvIoTEdition_Cluster"); } }
//Resources:DeploymentAdminResources:NuvIoTEdition_Container

		public static string NuvIoTEdition_Container { get { return GetResourceString("NuvIoTEdition_Container"); } }
//Resources:DeploymentAdminResources:NuvIoTEdition_Select

		public static string NuvIoTEdition_Select { get { return GetResourceString("NuvIoTEdition_Select"); } }
//Resources:DeploymentAdminResources:NuvIoTEdition_Shared

		public static string NuvIoTEdition_Shared { get { return GetResourceString("NuvIoTEdition_Shared"); } }
//Resources:DeploymentAdminResources:RemoteDeployment_Description

		public static string RemoteDeployment_Description { get { return GetResourceString("RemoteDeployment_Description"); } }
//Resources:DeploymentAdminResources:RemoteDeployment_Help

		public static string RemoteDeployment_Help { get { return GetResourceString("RemoteDeployment_Help"); } }
//Resources:DeploymentAdminResources:RemoteDeployment_Instances

		public static string RemoteDeployment_Instances { get { return GetResourceString("RemoteDeployment_Instances"); } }
//Resources:DeploymentAdminResources:RemoteDeployment_PrimaryMCP

		public static string RemoteDeployment_PrimaryMCP { get { return GetResourceString("RemoteDeployment_PrimaryMCP"); } }
//Resources:DeploymentAdminResources:RemoteDeployment_SecondaryMCP

		public static string RemoteDeployment_SecondaryMCP { get { return GetResourceString("RemoteDeployment_SecondaryMCP"); } }
//Resources:DeploymentAdminResources:RemoteDeployment_Title

		public static string RemoteDeployment_Title { get { return GetResourceString("RemoteDeployment_Title"); } }
//Resources:DeploymentAdminResources:RestNotificaiton_AdditionalHeader_Help

		public static string RestNotificaiton_AdditionalHeader_Help { get { return GetResourceString("RestNotificaiton_AdditionalHeader_Help"); } }
//Resources:DeploymentAdminResources:RestNotificaiton_AdditionalHeader_Title

		public static string RestNotificaiton_AdditionalHeader_Title { get { return GetResourceString("RestNotificaiton_AdditionalHeader_Title"); } }
//Resources:DeploymentAdminResources:RestNotification_AdditionalHeaders

		public static string RestNotification_AdditionalHeaders { get { return GetResourceString("RestNotification_AdditionalHeaders"); } }
//Resources:DeploymentAdminResources:RestNotification_Address

		public static string RestNotification_Address { get { return GetResourceString("RestNotification_Address"); } }
//Resources:DeploymentAdminResources:RestNotification_Address_Help

		public static string RestNotification_Address_Help( string deviceid) { return GetResourceString("RestNotification_Address_Help", "{deviceid}", deviceid); }
//Resources:DeploymentAdminResources:RestNotification_Anonymous

		public static string RestNotification_Anonymous { get { return GetResourceString("RestNotification_Anonymous"); } }
//Resources:DeploymentAdminResources:RestNotification_ContentType

		public static string RestNotification_ContentType { get { return GetResourceString("RestNotification_ContentType"); } }
//Resources:DeploymentAdminResources:RestNotification_HeaderName

		public static string RestNotification_HeaderName { get { return GetResourceString("RestNotification_HeaderName"); } }
//Resources:DeploymentAdminResources:RestNotification_HeaderValue

		public static string RestNotification_HeaderValue { get { return GetResourceString("RestNotification_HeaderValue"); } }
//Resources:DeploymentAdminResources:RestNotification_Help

		public static string RestNotification_Help { get { return GetResourceString("RestNotification_Help"); } }
//Resources:DeploymentAdminResources:RestNotification_Method

		public static string RestNotification_Method { get { return GetResourceString("RestNotification_Method"); } }
//Resources:DeploymentAdminResources:RestNotification_Method_DELETE

		public static string RestNotification_Method_DELETE { get { return GetResourceString("RestNotification_Method_DELETE"); } }
//Resources:DeploymentAdminResources:RestNotification_Method_GET

		public static string RestNotification_Method_GET { get { return GetResourceString("RestNotification_Method_GET"); } }
//Resources:DeploymentAdminResources:RestNotification_Method_PATCH

		public static string RestNotification_Method_PATCH { get { return GetResourceString("RestNotification_Method_PATCH"); } }
//Resources:DeploymentAdminResources:RestNotification_Method_POST

		public static string RestNotification_Method_POST { get { return GetResourceString("RestNotification_Method_POST"); } }
//Resources:DeploymentAdminResources:RestNotification_Method_PUT

		public static string RestNotification_Method_PUT { get { return GetResourceString("RestNotification_Method_PUT"); } }
//Resources:DeploymentAdminResources:RestNotification_Method_Select

		public static string RestNotification_Method_Select { get { return GetResourceString("RestNotification_Method_Select"); } }
//Resources:DeploymentAdminResources:RestNotification_Password

		public static string RestNotification_Password { get { return GetResourceString("RestNotification_Password"); } }
//Resources:DeploymentAdminResources:RestNotification_Payload

		public static string RestNotification_Payload { get { return GetResourceString("RestNotification_Payload"); } }
//Resources:DeploymentAdminResources:RestNotification_Title

		public static string RestNotification_Title { get { return GetResourceString("RestNotification_Title"); } }
//Resources:DeploymentAdminResources:RestNotification_UserName

		public static string RestNotification_UserName { get { return GetResourceString("RestNotification_UserName"); } }
//Resources:DeploymentAdminResources:RestNotifications_Title

		public static string RestNotifications_Title { get { return GetResourceString("RestNotifications_Title"); } }
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
//Resources:DeploymentAdminResources:Route_Message_Help

		public static string Route_Message_Help { get { return GetResourceString("Route_Message_Help"); } }
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
//Resources:DeploymentAdminResources:RouteModuleConfig_Help

		public static string RouteModuleConfig_Help { get { return GetResourceString("RouteModuleConfig_Help"); } }
//Resources:DeploymentAdminResources:RouteModuleConfig_Title

		public static string RouteModuleConfig_Title { get { return GetResourceString("RouteModuleConfig_Title"); } }
//Resources:DeploymentAdminResources:RouteModuleConfig_Unassigned

		public static string RouteModuleConfig_Unassigned { get { return GetResourceString("RouteModuleConfig_Unassigned"); } }
//Resources:DeploymentAdminResources:Solution_DefaultListener

		public static string Solution_DefaultListener { get { return GetResourceString("Solution_DefaultListener"); } }
//Resources:DeploymentAdminResources:Solution_DefaultListener_Help

		public static string Solution_DefaultListener_Help { get { return GetResourceString("Solution_DefaultListener_Help"); } }
//Resources:DeploymentAdminResources:Solution_DefaultListener_Select

		public static string Solution_DefaultListener_Select { get { return GetResourceString("Solution_DefaultListener_Select"); } }
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
//Resources:DeploymentAdminResources:Solution_Icon

		public static string Solution_Icon { get { return GetResourceString("Solution_Icon"); } }
//Resources:DeploymentAdminResources:Solution_Settings

		public static string Solution_Settings { get { return GetResourceString("Solution_Settings"); } }
//Resources:DeploymentAdminResources:Solution_Settings_Help

		public static string Solution_Settings_Help { get { return GetResourceString("Solution_Settings_Help"); } }
//Resources:DeploymentAdminResources:Solution_Title

		public static string Solution_Title { get { return GetResourceString("Solution_Title"); } }
//Resources:DeploymentAdminResources:Solution_Version

		public static string Solution_Version { get { return GetResourceString("Solution_Version"); } }
//Resources:DeploymentAdminResources:Solutions_Title

		public static string Solutions_Title { get { return GetResourceString("Solutions_Title"); } }
//Resources:DeploymentAdminResources:SystemTest_Description

		public static string SystemTest_Description { get { return GetResourceString("SystemTest_Description"); } }
//Resources:DeploymentAdminResources:SystemTest_EstimatedDuration

		public static string SystemTest_EstimatedDuration { get { return GetResourceString("SystemTest_EstimatedDuration"); } }
//Resources:DeploymentAdminResources:SystemTest_EstimatedDuration_Help

		public static string SystemTest_EstimatedDuration_Help { get { return GetResourceString("SystemTest_EstimatedDuration_Help"); } }
//Resources:DeploymentAdminResources:SystemTest_OnFailedInstructions

		public static string SystemTest_OnFailedInstructions { get { return GetResourceString("SystemTest_OnFailedInstructions"); } }
//Resources:DeploymentAdminResources:SystemTest_Schedule

		public static string SystemTest_Schedule { get { return GetResourceString("SystemTest_Schedule"); } }
//Resources:DeploymentAdminResources:SystemTest_Steps

		public static string SystemTest_Steps { get { return GetResourceString("SystemTest_Steps"); } }
//Resources:DeploymentAdminResources:SystemTest_Title

		public static string SystemTest_Title { get { return GetResourceString("SystemTest_Title"); } }
//Resources:DeploymentAdminResources:SystemTestExecution_AbortedBy

		public static string SystemTestExecution_AbortedBy { get { return GetResourceString("SystemTestExecution_AbortedBy"); } }
//Resources:DeploymentAdminResources:SystemTestExecution_CompletedBy

		public static string SystemTestExecution_CompletedBy { get { return GetResourceString("SystemTestExecution_CompletedBy"); } }
//Resources:DeploymentAdminResources:SystemTestExecution_EndTimeStamp

		public static string SystemTestExecution_EndTimeStamp { get { return GetResourceString("SystemTestExecution_EndTimeStamp"); } }
//Resources:DeploymentAdminResources:SystemTestExecution_StartedBy

		public static string SystemTestExecution_StartedBy { get { return GetResourceString("SystemTestExecution_StartedBy"); } }
//Resources:DeploymentAdminResources:SystemTestExecution_StartTimeStamp

		public static string SystemTestExecution_StartTimeStamp { get { return GetResourceString("SystemTestExecution_StartTimeStamp"); } }
//Resources:DeploymentAdminResources:SystemTestExecution_State_Aborted

		public static string SystemTestExecution_State_Aborted { get { return GetResourceString("SystemTestExecution_State_Aborted"); } }
//Resources:DeploymentAdminResources:SystemTestExecution_State_Failed

		public static string SystemTestExecution_State_Failed { get { return GetResourceString("SystemTestExecution_State_Failed"); } }
//Resources:DeploymentAdminResources:SystemTestExecution_State_InProcess

		public static string SystemTestExecution_State_InProcess { get { return GetResourceString("SystemTestExecution_State_InProcess"); } }
//Resources:DeploymentAdminResources:SystemTestExecution_State_New

		public static string SystemTestExecution_State_New { get { return GetResourceString("SystemTestExecution_State_New"); } }
//Resources:DeploymentAdminResources:SystemTestExecution_State_Passed

		public static string SystemTestExecution_State_Passed { get { return GetResourceString("SystemTestExecution_State_Passed"); } }
//Resources:DeploymentAdminResources:SystemTestExecution_Status

		public static string SystemTestExecution_Status { get { return GetResourceString("SystemTestExecution_Status"); } }
//Resources:DeploymentAdminResources:SystemTestExecution_SystemTest

		public static string SystemTestExecution_SystemTest { get { return GetResourceString("SystemTestExecution_SystemTest"); } }
//Resources:DeploymentAdminResources:SystemTestExecutions_Description

		public static string SystemTestExecutions_Description { get { return GetResourceString("SystemTestExecutions_Description"); } }
//Resources:DeploymentAdminResources:SystemTestExecutions_Title

		public static string SystemTestExecutions_Title { get { return GetResourceString("SystemTestExecutions_Title"); } }
//Resources:DeploymentAdminResources:SystemTests_Title

		public static string SystemTests_Title { get { return GetResourceString("SystemTests_Title"); } }
//Resources:DeploymentAdminResources:SystemTestStep_Description

		public static string SystemTestStep_Description { get { return GetResourceString("SystemTestStep_Description"); } }
//Resources:DeploymentAdminResources:SystemTestStep_Details

		public static string SystemTestStep_Details { get { return GetResourceString("SystemTestStep_Details"); } }
//Resources:DeploymentAdminResources:SystemTestStep_Resources

		public static string SystemTestStep_Resources { get { return GetResourceString("SystemTestStep_Resources"); } }
//Resources:DeploymentAdminResources:SystemTestStep_Summary

		public static string SystemTestStep_Summary { get { return GetResourceString("SystemTestStep_Summary"); } }
//Resources:DeploymentAdminResources:SystemTestStep_Title

		public static string SystemTestStep_Title { get { return GetResourceString("SystemTestStep_Title"); } }
//Resources:DeploymentAdminResources:SystemTestStep_Troubleshooting

		public static string SystemTestStep_Troubleshooting { get { return GetResourceString("SystemTestStep_Troubleshooting"); } }
//Resources:DeploymentAdminResources:SystemTestStep_WebLink

		public static string SystemTestStep_WebLink { get { return GetResourceString("SystemTestStep_WebLink"); } }
//Resources:DeploymentAdminResources:TaggedContainer_CreationDate

		public static string TaggedContainer_CreationDate { get { return GetResourceString("TaggedContainer_CreationDate"); } }
//Resources:DeploymentAdminResources:TaggedContainer_Description

		public static string TaggedContainer_Description { get { return GetResourceString("TaggedContainer_Description"); } }
//Resources:DeploymentAdminResources:TaggedContainer_Help

		public static string TaggedContainer_Help { get { return GetResourceString("TaggedContainer_Help"); } }
//Resources:DeploymentAdminResources:TaggedContainer_ReleaseNotes

		public static string TaggedContainer_ReleaseNotes { get { return GetResourceString("TaggedContainer_ReleaseNotes"); } }
//Resources:DeploymentAdminResources:TaggedContainer_Status

		public static string TaggedContainer_Status { get { return GetResourceString("TaggedContainer_Status"); } }
//Resources:DeploymentAdminResources:TaggedContainer_Status_Alpha

		public static string TaggedContainer_Status_Alpha { get { return GetResourceString("TaggedContainer_Status_Alpha"); } }
//Resources:DeploymentAdminResources:TaggedContainer_Status_Beta

		public static string TaggedContainer_Status_Beta { get { return GetResourceString("TaggedContainer_Status_Beta"); } }
//Resources:DeploymentAdminResources:TaggedContainer_Status_Deprecated

		public static string TaggedContainer_Status_Deprecated { get { return GetResourceString("TaggedContainer_Status_Deprecated"); } }
//Resources:DeploymentAdminResources:TaggedContainer_Status_Prerelease

		public static string TaggedContainer_Status_Prerelease { get { return GetResourceString("TaggedContainer_Status_Prerelease"); } }
//Resources:DeploymentAdminResources:TaggedContainer_Status_Production

		public static string TaggedContainer_Status_Production { get { return GetResourceString("TaggedContainer_Status_Production"); } }
//Resources:DeploymentAdminResources:TaggedContainer_Status_Select

		public static string TaggedContainer_Status_Select { get { return GetResourceString("TaggedContainer_Status_Select"); } }
//Resources:DeploymentAdminResources:TaggedContainer_Tag

		public static string TaggedContainer_Tag { get { return GetResourceString("TaggedContainer_Tag"); } }
//Resources:DeploymentAdminResources:TaggedContainer_Title

		public static string TaggedContainer_Title { get { return GetResourceString("TaggedContainer_Title"); } }
//Resources:DeploymentAdminResources:Telemetry_ErrorQueryServer

		public static string Telemetry_ErrorQueryServer { get { return GetResourceString("Telemetry_ErrorQueryServer"); } }
//Resources:DeploymentAdminResources:Telemetry_Help

		public static string Telemetry_Help { get { return GetResourceString("Telemetry_Help"); } }
//Resources:DeploymentAdminResources:Telemetry_Title

		public static string Telemetry_Title { get { return GetResourceString("Telemetry_Title"); } }
//Resources:DeploymentAdminResources:Warning_NoDeviceConfigs

		public static string Warning_NoDeviceConfigs { get { return GetResourceString("Warning_NoDeviceConfigs"); } }
//Resources:DeploymentAdminResources:Warning_NoListeners

		public static string Warning_NoListeners { get { return GetResourceString("Warning_NoListeners"); } }
//Resources:DeploymentAdminResources:WatchdogConnectedDevice_Help

		public static string WatchdogConnectedDevice_Help { get { return GetResourceString("WatchdogConnectedDevice_Help"); } }
//Resources:DeploymentAdminResources:WatchdogConnectedDevice_Title

		public static string WatchdogConnectedDevice_Title { get { return GetResourceString("WatchdogConnectedDevice_Title"); } }
//Resources:DeploymentAdminResources:WatchdogExclusion_AllDay

		public static string WatchdogExclusion_AllDay { get { return GetResourceString("WatchdogExclusion_AllDay"); } }
//Resources:DeploymentAdminResources:WatchdogExclusion_AllDay_Help

		public static string WatchdogExclusion_AllDay_Help { get { return GetResourceString("WatchdogExclusion_AllDay_Help"); } }
//Resources:DeploymentAdminResources:WatchdogMessage_Help

		public static string WatchdogMessage_Help { get { return GetResourceString("WatchdogMessage_Help"); } }
//Resources:DeploymentAdminResources:WatchdogMessage_Title

		public static string WatchdogMessage_Title { get { return GetResourceString("WatchdogMessage_Title"); } }
//Resources:DeploymentAdminResources:WiFiConnectionProfile_Description

		public static string WiFiConnectionProfile_Description { get { return GetResourceString("WiFiConnectionProfile_Description"); } }
//Resources:DeploymentAdminResources:WiFiConnectionProfile_Help

		public static string WiFiConnectionProfile_Help { get { return GetResourceString("WiFiConnectionProfile_Help"); } }
//Resources:DeploymentAdminResources:WiFiConnectionProfile_Name

		public static string WiFiConnectionProfile_Name { get { return GetResourceString("WiFiConnectionProfile_Name"); } }
//Resources:DeploymentAdminResources:WiFiConnectionProfile_Notes

		public static string WiFiConnectionProfile_Notes { get { return GetResourceString("WiFiConnectionProfile_Notes"); } }
//Resources:DeploymentAdminResources:WiFiConnectionProfile_Password

		public static string WiFiConnectionProfile_Password { get { return GetResourceString("WiFiConnectionProfile_Password"); } }
//Resources:DeploymentAdminResources:WiFiConnectionProfile_SSID

		public static string WiFiConnectionProfile_SSID { get { return GetResourceString("WiFiConnectionProfile_SSID"); } }
//Resources:DeploymentAdminResources:WiFiConnectionProfile_Title

		public static string WiFiConnectionProfile_Title { get { return GetResourceString("WiFiConnectionProfile_Title"); } }
//Resources:DeploymentAdminResources:WorkingStorage

		public static string WorkingStorage { get { return GetResourceString("WorkingStorage"); } }
//Resources:DeploymentAdminResources:WorkingStorage_Cloud

		public static string WorkingStorage_Cloud { get { return GetResourceString("WorkingStorage_Cloud"); } }
//Resources:DeploymentAdminResources:WorkingStorage_Help

		public static string WorkingStorage_Help { get { return GetResourceString("WorkingStorage_Help"); } }
//Resources:DeploymentAdminResources:WorkingStorage_Local

		public static string WorkingStorage_Local { get { return GetResourceString("WorkingStorage_Local"); } }
//Resources:DeploymentAdminResources:WorkingStorage_Select

		public static string WorkingStorage_Select { get { return GetResourceString("WorkingStorage_Select"); } }

		public static class Names
		{
			public const string ClientApp_AppAuthKey1 = "ClientApp_AppAuthKey1";
			public const string ClientApp_AppAuthKey2 = "ClientApp_AppAuthKey2";
			public const string ClientApp_Description = "ClientApp_Description";
			public const string ClientApp_DeviceConfigs = "ClientApp_DeviceConfigs";
			public const string ClientApp_DeviceTypes = "ClientApp_DeviceTypes";
			public const string ClientApp_Help = "ClientApp_Help";
			public const string ClientApp_Instance = "ClientApp_Instance";
			public const string ClientApp_Kiosk = "ClientApp_Kiosk";
			public const string ClientApp_Kiosk_Select = "ClientApp_Kiosk_Select";
			public const string ClientApp_SelectInstance = "ClientApp_SelectInstance";
			public const string ClientApp_Title = "ClientApp_Title";
			public const string ClientApplications_Title = "ClientApplications_Title";
			public const string ClientApps_Title = "ClientApps_Title";
			public const string Common_Category = "Common_Category";
			public const string Common_Category_Select = "Common_Category_Select";
			public const string Common_Customer = "Common_Customer";
			public const string Common_Description = "Common_Description";
			public const string Common_Details = "Common_Details";
			public const string Common_Icon = "Common_Icon";
			public const string Common_Id = "Common_Id";
			public const string Common_IsPublic = "Common_IsPublic";
			public const string Common_IsPublic_Help = "Common_IsPublic_Help";
			public const string Common_Key = "Common_Key";
			public const string Common_Key_Help = "Common_Key_Help";
			public const string Common_Key_Validation = "Common_Key_Validation";
			public const string Common_Name = "Common_Name";
			public const string Common_Notes = "Common_Notes";
			public const string Common_Resources = "Common_Resources";
			public const string Common_Summary = "Common_Summary";
			public const string ContainerRepositories_Title = "ContainerRepositories_Title";
			public const string ContainerRepository_Description = "ContainerRepository_Description";
			public const string ContainerRepository_Help = "ContainerRepository_Help";
			public const string ContainerRepository_IsDefautForRuntime = "ContainerRepository_IsDefautForRuntime";
			public const string ContainerRepository_IsDefautForRuntime_Help = "ContainerRepository_IsDefautForRuntime_Help";
			public const string ContainerRepository_Namespace = "ContainerRepository_Namespace";
			public const string ContainerRepository_Namespace_Help = "ContainerRepository_Namespace_Help";
			public const string ContainerRepository_Password = "ContainerRepository_Password";
			public const string ContainerRepository_Password_Help = "ContainerRepository_Password_Help";
			public const string ContainerRepository_Preferred = "ContainerRepository_Preferred";
			public const string ContainerRepository_Preferred_Help = "ContainerRepository_Preferred_Help";
			public const string ContainerRepository_Preferred_Select = "ContainerRepository_Preferred_Select";
			public const string ContainerRepository_Registry = "ContainerRepository_Registry";
			public const string ContainerRepository_Registry_Help = "ContainerRepository_Registry_Help";
			public const string ContainerRepository_RepositoryName = "ContainerRepository_RepositoryName";
			public const string ContainerRepository_RepositoryName_Help = "ContainerRepository_RepositoryName_Help";
			public const string ContainerRepository_Tags = "ContainerRepository_Tags";
			public const string ContainerRepository_Title = "ContainerRepository_Title";
			public const string ContainerRepository_UserName = "ContainerRepository_UserName";
			public const string ContainerRepository_UserName_Help = "ContainerRepository_UserName_Help";
			public const string CotNotification_CustomCertExpires = "CotNotification_CustomCertExpires";
			public const string CotNotification_DataPackageFile = "CotNotification_DataPackageFile";
			public const string CotNotification_DataPackageFile_Help = "CotNotification_DataPackageFile_Help";
			public const string CotNotification_Description = "CotNotification_Description";
			public const string CotNotification_FillColor = "CotNotification_FillColor";
			public const string CotNotification_Help = "CotNotification_Help";
			public const string CotNotification_IgnoreCertificateErrors = "CotNotification_IgnoreCertificateErrors";
			public const string CotNotification_IgnoreCertificateErrors_Help = "CotNotification_IgnoreCertificateErrors_Help";
			public const string CotNotification_IncludeDeviceLocation = "CotNotification_IncludeDeviceLocation";
			public const string CotNotification_IncludeLocationPolygon = "CotNotification_IncludeLocationPolygon";
			public const string CotNotification_IncludeLocationPolygon_Help = "CotNotification_IncludeLocationPolygon_Help";
			public const string CotNotification_PublicRootCert = "CotNotification_PublicRootCert";
			public const string CotNotification_PublicRootCert_Help = "CotNotification_PublicRootCert_Help";
			public const string CotNotification_Remarks = "CotNotification_Remarks";
			public const string CotNotification_StaleSeconds = "CotNotification_StaleSeconds";
			public const string CotNotification_StaleSeconds_Help = "CotNotification_StaleSeconds_Help";
			public const string CotNotification_Title = "CotNotification_Title";
			public const string CotNotification_Type_Help = "CotNotification_Type_Help";
			public const string CotNotification_UseCustomRoot = "CotNotification_UseCustomRoot";
			public const string CotNotification_UseCustomRoot_Help = "CotNotification_UseCustomRoot_Help";
			public const string CotNotifications_Title = "CotNotifications_Title";
			public const string CotNotifiication_Type = "CotNotifiication_Type";
			public const string Deployment_Listeners = "Deployment_Listeners";
			public const string Deployment_Listeners_Help = "Deployment_Listeners_Help";
			public const string Deployment_Logging = "Deployment_Logging";
			public const string Deployment_Logging_Cloud = "Deployment_Logging_Cloud";
			public const string Deployment_Logging_Help = "Deployment_Logging_Help";
			public const string Deployment_Logging_Local = "Deployment_Logging_Local";
			public const string Deployment_Logging_Select = "Deployment_Logging_Select";
			public const string Deployment_Planner = "Deployment_Planner";
			public const string Deployment_Planner_Help = "Deployment_Planner_Help";
			public const string Deployment_Planner_Select = "Deployment_Planner_Select";
			public const string Deployment_Title = "Deployment_Title";
			public const string DeploymentActivities_Help = "DeploymentActivities_Help";
			public const string DeploymentActivities_TItle = "DeploymentActivities_TItle";
			public const string DeploymentActivity_ActivityType = "DeploymentActivity_ActivityType";
			public const string DeploymentActivity_Duration = "DeploymentActivity_Duration";
			public const string DeploymentActivity_ErrorMessage = "DeploymentActivity_ErrorMessage";
			public const string DeploymentActivity_ResourceType = "DeploymentActivity_ResourceType";
			public const string DeploymentActivity_Start = "DeploymentActivity_Start";
			public const string DeploymentActivity_Status = "DeploymentActivity_Status";
			public const string DeploymentConfiguration_DockerSwarm = "DeploymentConfiguration_DockerSwarm";
			public const string DeploymentConfiguration_Kubernetes = "DeploymentConfiguration_Kubernetes";
			public const string DeploymentConfiguration_SingleInstance = "DeploymentConfiguration_SingleInstance";
			public const string DeploymentConfiguration_UWPApp = "DeploymentConfiguration_UWPApp";
			public const string DeploymentInstance_HealthCheckEnabled = "DeploymentInstance_HealthCheckEnabled";
			public const string DeploymentInstance_HealthCheckEnabled_Help = "DeploymentInstance_HealthCheckEnabled_Help";
			public const string DeploymentInstance_Integrations = "DeploymentInstance_Integrations";
			public const string DeploymentInstance_SharedAccessKey_Help = "DeploymentInstance_SharedAccessKey_Help";
			public const string DeploymentInstance_SharedAccessKey1 = "DeploymentInstance_SharedAccessKey1";
			public const string DeploymentInstance_SharedAccessKey2 = "DeploymentInstance_SharedAccessKey2";
			public const string DeploymentInstance_TimeZone = "DeploymentInstance_TimeZone";
			public const string DeploymentInstance_TimeZone_RegEx = "DeploymentInstance_TimeZone_RegEx";
			public const string DeploymentInstance_Version = "DeploymentInstance_Version";
			public const string DeploymentInstance_Version_Help = "DeploymentInstance_Version_Help";
			public const string DeploymentInstance_Version_Select = "DeploymentInstance_Version_Select";
			public const string DeploymentInstance_WiFiConnectionProfiles = "DeploymentInstance_WiFiConnectionProfiles";
			public const string DeploymentInstanceCredentials__Expires = "DeploymentInstanceCredentials__Expires";
			public const string DeploymentInstanceCredentials__Expires_Help = "DeploymentInstanceCredentials__Expires_Help";
			public const string DeploymentInstanceCredentials__Password = "DeploymentInstanceCredentials__Password";
			public const string DeploymentInstanceCredentials__UserId = "DeploymentInstanceCredentials__UserId";
			public const string DeploymentInstanceCredentials_Description = "DeploymentInstanceCredentials_Description";
			public const string DeploymentInstanceCredentials_Help = "DeploymentInstanceCredentials_Help";
			public const string DeploymentInstanceCredentials_Title = "DeploymentInstanceCredentials_Title";
			public const string DeploymentInstanceStatus_Description = "DeploymentInstanceStatus_Description";
			public const string DeploymentInstanceStatus_Title = "DeploymentInstanceStatus_Title";
			public const string DeploymentQueueType_InMemory = "DeploymentQueueType_InMemory";
			public const string DeploymentQueueType_Kafka = "DeploymentQueueType_Kafka";
			public const string DeploymentQueueType_QueueTechnology = "DeploymentQueueType_QueueTechnology";
			public const string DeploymentQueueType_QueueTechnology_Help = "DeploymentQueueType_QueueTechnology_Help";
			public const string DeploymentQueueType_QueueTechnology_Select = "DeploymentQueueType_QueueTechnology_Select";
			public const string DeploymentQueueType_RabbitMQ = "DeploymentQueueType_RabbitMQ";
			public const string DeploymentQueueType_ServiceBus = "DeploymentQueueType_ServiceBus";
			public const string DeploymentType_Cloud = "DeploymentType_Cloud";
			public const string DeploymentType_Managed = "DeploymentType_Managed";
			public const string DeploymentType_OnPremise = "DeploymentType_OnPremise";
			public const string DeploymentType_Shared = "DeploymentType_Shared";
			public const string DeviceCommand_Description = "DeviceCommand_Description";
			public const string DeviceCommand_TItle = "DeviceCommand_TItle";
			public const string DeviceConfig_Commands = "DeviceConfig_Commands";
			public const string DeviceConfig_Commands_Help = "DeviceConfig_Commands_Help";
			public const string DeviceConfiguration_CustomStatusType = "DeviceConfiguration_CustomStatusType";
			public const string DeviceConfiguration_CustomStatusType_Help = "DeviceConfiguration_CustomStatusType_Help";
			public const string DeviceConfiguration_CustomStatusType_Watermark = "DeviceConfiguration_CustomStatusType_Watermark";
			public const string DeviceConfiguration_Description = "DeviceConfiguration_Description";
			public const string DeviceConfiguration_DeviceErrorCodes = "DeviceConfiguration_DeviceErrorCodes";
			public const string DeviceConfiguration_DeviceIdLabel = "DeviceConfiguration_DeviceIdLabel";
			public const string DeviceConfiguration_DeviceIdLabel_Default = "DeviceConfiguration_DeviceIdLabel_Default";
			public const string DeviceConfiguration_DeviceIdLabel_Help = "DeviceConfiguration_DeviceIdLabel_Help";
			public const string DeviceConfiguration_DeviceLabel = "DeviceConfiguration_DeviceLabel";
			public const string DeviceConfiguration_DeviceLabel_Default = "DeviceConfiguration_DeviceLabel_Default";
			public const string DeviceConfiguration_DeviceLabel_Help = "DeviceConfiguration_DeviceLabel_Help";
			public const string DeviceConfiguration_DeviceNameLabel = "DeviceConfiguration_DeviceNameLabel";
			public const string DeviceConfiguration_DeviceNameLabel_Default = "DeviceConfiguration_DeviceNameLabel_Default";
			public const string DeviceConfiguration_DeviceNameLabel_Help = "DeviceConfiguration_DeviceNameLabel_Help";
			public const string DeviceConfiguration_DeviceTypeLabel = "DeviceConfiguration_DeviceTypeLabel";
			public const string DeviceConfiguration_DeviceTypeLabel_Default = "DeviceConfiguration_DeviceTypeLabel_Default";
			public const string DeviceConfiguration_DeviceTypeLabel_Help = "DeviceConfiguration_DeviceTypeLabel_Help";
			public const string DeviceConfiguration_Help = "DeviceConfiguration_Help";
			public const string DeviceConfiguration_Icon = "DeviceConfiguration_Icon";
			public const string DeviceConfiguration_MessageWatchDogs = "DeviceConfiguration_MessageWatchDogs";
			public const string DeviceConfiguration_Properties = "DeviceConfiguration_Properties";
			public const string DeviceConfiguration_Properties_Help = "DeviceConfiguration_Properties_Help";
			public const string DeviceConfiguration_Routes = "DeviceConfiguration_Routes";
			public const string DeviceConfiguration_SensorDefintions = "DeviceConfiguration_SensorDefintions";
			public const string DeviceConfiguration_Sentinel = "DeviceConfiguration_Sentinel";
			public const string DeviceConfiguration_Title = "DeviceConfiguration_Title";
			public const string DeviceConfiguration_WatchDogEnabled_Default = "DeviceConfiguration_WatchDogEnabled_Default";
			public const string DeviceConfiguration_WatchDogEnabled_Default_Help = "DeviceConfiguration_WatchDogEnabled_Default_Help";
			public const string DeviceConfiguration_WatchDogTimeout = "DeviceConfiguration_WatchDogTimeout";
			public const string DeviceConfiguration_WatchDogTimeout_Help = "DeviceConfiguration_WatchDogTimeout_Help";
			public const string DeviceConfigurations_Title = "DeviceConfigurations_Title";
			public const string DeviceConfiguratoin_DeviceTypeLabel_Help = "DeviceConfiguratoin_DeviceTypeLabel_Help";
			public const string DeviceConfiugration_CustomMobilePage = "DeviceConfiugration_CustomMobilePage";
			public const string DeviceConfiugration_CustomMobilePage_Help = "DeviceConfiugration_CustomMobilePage_Help";
			public const string DeviceConfiugration_CustomPage = "DeviceConfiugration_CustomPage";
			public const string DeviceConfiugration_CustomPage_Help = "DeviceConfiugration_CustomPage_Help";
			public const string DeviceConfiugration_CustomPage_QuickLink = "DeviceConfiugration_CustomPage_QuickLink";
			public const string DeviceConfiugration_CustomPage_QuickLink_Help = "DeviceConfiugration_CustomPage_QuickLink_Help";
			public const string DeviceErrorCode_AutoExpiresTimespan = "DeviceErrorCode_AutoExpiresTimespan";
			public const string DeviceErrorCode_AutoExpiresTimespan_Help = "DeviceErrorCode_AutoExpiresTimespan_Help";
			public const string DeviceErrorCode_AutoExpiresTimespanQuantity = "DeviceErrorCode_AutoExpiresTimespanQuantity";
			public const string DeviceErrorCode_Days = "DeviceErrorCode_Days";
			public const string DeviceErrorCode_Description = "DeviceErrorCode_Description";
			public const string DeviceErrorCode_DeviceNotification = "DeviceErrorCode_DeviceNotification";
			public const string DeviceErrorCode_DeviceNotification_Help = "DeviceErrorCode_DeviceNotification_Help";
			public const string DeviceErrorCode_DeviceNotification_Select = "DeviceErrorCode_DeviceNotification_Select";
			public const string DeviceErrorCode_DistributionList = "DeviceErrorCode_DistributionList";
			public const string DeviceErrorCode_DistributionList_Help = "DeviceErrorCode_DistributionList_Help";
			public const string DeviceErrorCode_DistributionList_Select = "DeviceErrorCode_DistributionList_Select";
			public const string DeviceErrorCode_EmailSubject = "DeviceErrorCode_EmailSubject";
			public const string DeviceErrorCode_EmailSubject_Help = "DeviceErrorCode_EmailSubject_Help";
			public const string DeviceErrorCode_ErrorCode = "DeviceErrorCode_ErrorCode";
			public const string DeviceErrorCode_EscalateAfterAttemptCount = "DeviceErrorCode_EscalateAfterAttemptCount";
			public const string DeviceErrorCode_EscalateAfterAttemptCount_Help = "DeviceErrorCode_EscalateAfterAttemptCount_Help";
			public const string DeviceErrorCode_Help = "DeviceErrorCode_Help";
			public const string DeviceErrorCode_Hours = "DeviceErrorCode_Hours";
			public const string DeviceErrorCode_IncidentProtocol = "DeviceErrorCode_IncidentProtocol";
			public const string DeviceErrorCode_IncidentProtocol_Help = "DeviceErrorCode_IncidentProtocol_Help";
			public const string DeviceErrorCode_IncidentProtocol_Select = "DeviceErrorCode_IncidentProtocol_Select";
			public const string DeviceErrorCode_Minutes = "DeviceErrorCode_Minutes";
			public const string DeviceErrorCode_NotApplicable = "DeviceErrorCode_NotApplicable";
			public const string DeviceErrorCode_NotificationInterval = "DeviceErrorCode_NotificationInterval";
			public const string DeviceErrorCode_NotificationInterval_Help = "DeviceErrorCode_NotificationInterval_Help";
			public const string DeviceErrorCode_NotificationIntervalQuantity = "DeviceErrorCode_NotificationIntervalQuantity";
			public const string DeviceErrorCode_NotifyOnClear = "DeviceErrorCode_NotifyOnClear";
			public const string DeviceErrorCode_NotifyOnRaise = "DeviceErrorCode_NotifyOnRaise";
			public const string DeviceErrorCode_SelectTimespan = "DeviceErrorCode_SelectTimespan";
			public const string DeviceErrorCode_SendEmail = "DeviceErrorCode_SendEmail";
			public const string DeviceErrorCode_SendSMS = "DeviceErrorCode_SendSMS";
			public const string DeviceErrorCode_TicketTemplate = "DeviceErrorCode_TicketTemplate";
			public const string DeviceErrorCode_TicketTemplate_Help = "DeviceErrorCode_TicketTemplate_Help";
			public const string DeviceErrorCode_TicketTemplate_Select = "DeviceErrorCode_TicketTemplate_Select";
			public const string DeviceErrorCode_Title = "DeviceErrorCode_Title";
			public const string DeviceErrorCode_TriggerOnEachOccurrence = "DeviceErrorCode_TriggerOnEachOccurrence";
			public const string DeviceErrorCode_TriggerOnEachOccurrence_Help = "DeviceErrorCode_TriggerOnEachOccurrence_Help";
			public const string DeviceErrorCodes_Title = "DeviceErrorCodes_Title";
			public const string DeviceNotification_EmailSubject = "DeviceNotification_EmailSubject";
			public const string DeviceNotification_EmailSubject_Help = "DeviceNotification_EmailSubject_Help";
			public const string DeviceNotification_SMS_Help = "DeviceNotification_SMS_Help";
			public const string DeviceNotification_TagHelp = "DeviceNotification_TagHelp";
			public const string DeviceNotification_Title = "DeviceNotification_Title";
			public const string DeviceNotificationHistory_Description = "DeviceNotificationHistory_Description";
			public const string DeviceNotificationHistory_Title = "DeviceNotificationHistory_Title";
			public const string DeviceNotifications_CotNotifications = "DeviceNotifications_CotNotifications";
			public const string DeviceNotifications_Description = "DeviceNotifications_Description";
			public const string DeviceNotifications_EmailContent = "DeviceNotifications_EmailContent";
			public const string DeviceNotifications_EmailSubject = "DeviceNotifications_EmailSubject";
			public const string DeviceNotifications_Escalation_Notification = "DeviceNotifications_Escalation_Notification";
			public const string DeviceNotifications_Escalation_Notification_Help = "DeviceNotifications_Escalation_Notification_Help";
			public const string DeviceNotifications_ForwardToParent = "DeviceNotifications_ForwardToParent";
			public const string DeviceNotifications_ForwardToParentBody = "DeviceNotifications_ForwardToParentBody";
			public const string DeviceNotifications_IncludeLandingPageContent = "DeviceNotifications_IncludeLandingPageContent";
			public const string DeviceNotifications_LandingPageContent = "DeviceNotifications_LandingPageContent";
			public const string DeviceNotifications_MqttNotifications = "DeviceNotifications_MqttNotifications";
			public const string DeviceNotifications_RestNotifications = "DeviceNotifications_RestNotifications";
			public const string DeviceNotifications_SendEmail = "DeviceNotifications_SendEmail";
			public const string DeviceNotifications_SendSMS = "DeviceNotifications_SendSMS";
			public const string DeviceNotifications_SharedTemplate = "DeviceNotifications_SharedTemplate";
			public const string DeviceNotifications_SMSContent = "DeviceNotifications_SMSContent";
			public const string DeviceNotifications_Title = "DeviceNotifications_Title";
			public const string Err_CantPublishNotRunning = "Err_CantPublishNotRunning";
			public const string Err_CouldNotFindDestinationModule = "Err_CouldNotFindDestinationModule";
			public const string Err_CouldNotLoadDeviceConfiguration = "Err_CouldNotLoadDeviceConfiguration";
			public const string Err_CouldNotLoadInstance = "Err_CouldNotLoadInstance";
			public const string Err_CouldNotLoadListener = "Err_CouldNotLoadListener";
			public const string Err_CouldNotLoadPlanner = "Err_CouldNotLoadPlanner";
			public const string Err_CouldNotLoadSolution = "Err_CouldNotLoadSolution";
			public const string Err_CouldntStart_NotOffline = "Err_CouldntStart_NotOffline";
			public const string Err_CouldntStop_NotRunning = "Err_CouldntStop_NotRunning";
			public const string Err_EmptyRoute = "Err_EmptyRoute";
			public const string Err_ErrorCommunicatingWithHost = "Err_ErrorCommunicatingWithHost";
			public const string Err_InstanceAlreadyRunning = "Err_InstanceAlreadyRunning";
			public const string Err_InstanceNotRunning = "Err_InstanceNotRunning";
			public const string Err_InstanceWithoutHost = "Err_InstanceWithoutHost";
			public const string Err_InstanceWithoutSolution = "Err_InstanceWithoutSolution";
			public const string Err_MCPServerExists = "Err_MCPServerExists";
			public const string Err_MultipleMCPServersFound = "Err_MultipleMCPServersFound";
			public const string Err_MultipleNotificationServersFound = "Err_MultipleNotificationServersFound";
			public const string Err_NoMCPServerExists = "Err_NoMCPServerExists";
			public const string Err_NoMessageDefinitionOnRoute = "Err_NoMessageDefinitionOnRoute";
			public const string Err_NoNotificationsServerExists = "Err_NoNotificationsServerExists";
			public const string Err_NoPlannerHasBeenSpecified = "Err_NoPlannerHasBeenSpecified";
			public const string Err_NotificationServerExists = "Err_NotificationServerExists";
			public const string Err_RouteModule_ModuleIsRequired = "Err_RouteModule_ModuleIsRequired";
			public const string Err_RouteModule_ModuleTypeNotDefined = "Err_RouteModule_ModuleTypeNotDefined";
			public const string Err_RouteModule_NameNotDefined = "Err_RouteModule_NameNotDefined";
			public const string Errs_AlreadyDeployed = "Errs_AlreadyDeployed";
			public const string Errs_InstanceBusy = "Errs_InstanceBusy";
			public const string Errs_MustBeStoppedBeforeRemoving = "Errs_MustBeStoppedBeforeRemoving";
			public const string Errs_NotDeployed = "Errs_NotDeployed";
			public const string Host_AdminAPIUri = "Host_AdminAPIUri";
			public const string Host_AdminAPIUri_Help = "Host_AdminAPIUri_Help";
			public const string Host_AdminEndpoint = "Host_AdminEndpoint";
			public const string Host_AdminEndpoint_Help = "Host_AdminEndpoint_Help";
			public const string Host_AverageCPU_1_Minute = "Host_AverageCPU_1_Minute";
			public const string Host_AverageMemory_1_Minute = "Host_AverageMemory_1_Minute";
			public const string Host_CapacityStatus = "Host_CapacityStatus";
			public const string Host_CloudProvider = "Host_CloudProvider";
			public const string Host_CloudProvider_Help = "Host_CloudProvider_Help";
			public const string Host_CloudProviders = "Host_CloudProviders";
			public const string Host_ComputeResource_Uri = "Host_ComputeResource_Uri";
			public const string Host_ComputeResource_Uri_Help = "Host_ComputeResource_Uri_Help";
			public const string Host_ComputeResourceId = "Host_ComputeResourceId";
			public const string Host_ComputeResourceId_Help = "Host_ComputeResourceId_Help";
			public const string Host_ContainerRepository = "Host_ContainerRepository";
			public const string Host_ContainerRepository_Select = "Host_ContainerRepository_Select";
			public const string Host_ContainerTag = "Host_ContainerTag";
			public const string Host_ContainerTag_Select = "Host_ContainerTag_Select";
			public const string Host_DateStampOnline = "Host_DateStampOnline";
			public const string Host_DebugMode = "Host_DebugMode";
			public const string Host_DebugMode_Help = "Host_DebugMode_Help";
			public const string Host_DedicatedInstance = "Host_DedicatedInstance";
			public const string Host_Description = "Host_Description";
			public const string Host_DNSName = "Host_DNSName";
			public const string Host_HasSSLCert = "Host_HasSSLCert";
			public const string Host_Help = "Host_Help";
			public const string Host_InternalServiceName = "Host_InternalServiceName";
			public const string Host_IPv4_Address = "Host_IPv4_Address";
			public const string Host_LastPing = "Host_LastPing";
			public const string Host_MonitoringProvider = "Host_MonitoringProvider";
			public const string Host_MonitoringURI = "Host_MonitoringURI";
			public const string Host_SelectSize = "Host_SelectSize";
			public const string Host_ShowSiteDetails = "Host_ShowSiteDetails";
			public const string Host_ShowSiteDetails_Help = "Host_ShowSiteDetails_Help";
			public const string Host_Size = "Host_Size";
			public const string Host_SSLExpires = "Host_SSLExpires";
			public const string Host_Status = "Host_Status";
			public const string Host_StatusDetails = "Host_StatusDetails";
			public const string Host_StatusTimeStamp = "Host_StatusTimeStamp";
			public const string Host_Subscription = "Host_Subscription";
			public const string Host_SubscriptionSelect = "Host_SubscriptionSelect";
			public const string Host_Title = "Host_Title";
			public const string Host_Type = "Host_Type";
			public const string Host_Type_BackupMCP = "Host_Type_BackupMCP";
			public const string Host_Type_Clustered = "Host_Type_Clustered";
			public const string Host_Type_Community = "Host_Type_Community";
			public const string Host_Type_Dedicated = "Host_Type_Dedicated";
			public const string Host_Type_Free = "Host_Type_Free";
			public const string Host_Type_MCP = "Host_Type_MCP";
			public const string Host_Type_Notifications = "Host_Type_Notifications";
			public const string Host_Type_RemoteBackupMCP = "Host_Type_RemoteBackupMCP";
			public const string Host_Type_RemoteMCP = "Host_Type_RemoteMCP";
			public const string Host_Type_Select = "Host_Type_Select";
			public const string Host_Type_Shared = "Host_Type_Shared";
			public const string Host_Type_SharedHighPerformance = "Host_Type_SharedHighPerformance";
			public const string Host_UpSince = "Host_UpSince";
			public const string HostCapacity_75Percent = "HostCapacity_75Percent";
			public const string HostCapacity_90Percent = "HostCapacity_90Percent";
			public const string HostCapacity_AtCapacity = "HostCapacity_AtCapacity";
			public const string HostCapacity_FailureImminent = "HostCapacity_FailureImminent";
			public const string HostCapacity_Ok = "HostCapacity_Ok";
			public const string HostCapacity_OverCapacity = "HostCapacity_OverCapacity";
			public const string HostCapacity_Underutlized = "HostCapacity_Underutlized";
			public const string Hosts_Title = "Hosts_Title";
			public const string HostSize_ExtraLarge = "HostSize_ExtraLarge";
			public const string HostSize_ExtraSmall = "HostSize_ExtraSmall";
			public const string HostSize_Large = "HostSize_Large";
			public const string HostSize_Medium = "HostSize_Medium";
			public const string HostSize_Small = "HostSize_Small";
			public const string HostStatus_ConfiguringDNS = "HostStatus_ConfiguringDNS";
			public const string HostStatus_Deploying = "HostStatus_Deploying";
			public const string HostStatus_DeployingContainer = "HostStatus_DeployingContainer";
			public const string HostStatus_Destroying = "HostStatus_Destroying";
			public const string HostStatus_FailedDeployment = "HostStatus_FailedDeployment";
			public const string HostStatus_HealthCheckFailed = "HostStatus_HealthCheckFailed";
			public const string HostStatus_Offline = "HostStatus_Offline";
			public const string HostStatus_QueuedForDeployment = "HostStatus_QueuedForDeployment";
			public const string HostStatus_RestartingContainer = "HostStatus_RestartingContainer";
			public const string HostStatus_RestartingHost = "HostStatus_RestartingHost";
			public const string HostStatus_Running = "HostStatus_Running";
			public const string HostStatus_Starting = "HostStatus_Starting";
			public const string HostStatus_StartingContainer = "HostStatus_StartingContainer";
			public const string HostStatus_Stopped = "HostStatus_Stopped";
			public const string HostStatus_Stopping = "HostStatus_Stopping";
			public const string HostStatus_UpdatingRuntime = "HostStatus_UpdatingRuntime";
			public const string HostStatus_WaitingForServer = "HostStatus_WaitingForServer";
			public const string HostType_Development = "HostType_Development";
			public const string HostType_MultiTenantMQTT = "HostType_MultiTenantMQTT";
			public const string Incident_Description = "Incident_Description";
			public const string Incident_Title = "Incident_Title";
			public const string IncidentProtocol_Description = "IncidentProtocol_Description";
			public const string IncidentProtocol_Steps = "IncidentProtocol_Steps";
			public const string IncidentProtocol_Title = "IncidentProtocol_Title";
			public const string IncidentProtocols_Title = "IncidentProtocols_Title";
			public const string IncidentProtocolStep_AssignedTo = "IncidentProtocolStep_AssignedTo";
			public const string IncidentProtocolStep_Description = "IncidentProtocolStep_Description";
			public const string IncidentProtocolStep_Title = "IncidentProtocolStep_Title";
			public const string Incidents_Title = "Incidents_Title";
			public const string Instance_Caches = "Instance_Caches";
			public const string Instance_Credentials = "Instance_Credentials";
			public const string Instance_Credentials_Help = "Instance_Credentials_Help";
			public const string Instance_DataStreams = "Instance_DataStreams";
			public const string Instance_DebugMode = "Instance_DebugMode";
			public const string Instance_DebugMode_Help = "Instance_DebugMode_Help";
			public const string Instance_DeploymentConfiguration = "Instance_DeploymentConfiguration";
			public const string Instance_DeploymentConfiguration_Select = "Instance_DeploymentConfiguration_Select";
			public const string Instance_DeploymentType = "Instance_DeploymentType";
			public const string Instance_DeploymentType_Select = "Instance_DeploymentType_Select";
			public const string Instance_Description = "Instance_Description";
			public const string Instance_DeviceRepo = "Instance_DeviceRepo";
			public const string Instance_DeviceRepo_Help = "Instance_DeviceRepo_Help";
			public const string Instance_DeviceRepo_Select = "Instance_DeviceRepo_Select";
			public const string Instance_Help = "Instance_Help";
			public const string Instance_Host = "Instance_Host";
			public const string Instance_Host_Help = "Instance_Host_Help";
			public const string Instance_Host_Watermark = "Instance_Host_Watermark";
			public const string Instance_InputCommandAnonymous = "Instance_InputCommandAnonymous";
			public const string Instance_InputCommandAnonymous_Help = "Instance_InputCommandAnonymous_Help";
			public const string Instance_InputCommandBasicAuthPassword = "Instance_InputCommandBasicAuthPassword";
			public const string Instance_InputCommandBasicAuthUserName = "Instance_InputCommandBasicAuthUserName";
			public const string Instance_InputCommandPort = "Instance_InputCommandPort";
			public const string Instance_InputCommandPort_Help = "Instance_InputCommandPort_Help";
			public const string Instance_InputCommandSSL = "Instance_InputCommandSSL";
			public const string Instance_InputCommandSSL_Help = "Instance_InputCommandSSL_Help";
			public const string Instance_IsDeployed = "Instance_IsDeployed";
			public const string Instance_IsDeployed_Help = "Instance_IsDeployed_Help";
			public const string Instance_LastPing = "Instance_LastPing";
			public const string Instance_LocalLogs = "Instance_LocalLogs";
			public const string Instance_LocalMessageStorage = "Instance_LocalMessageStorage";
			public const string Instance_LocalUsageStatistics = "Instance_LocalUsageStatistics";
			public const string Instance_PrimaryCache = "Instance_PrimaryCache";
			public const string Instance_PrimaryCache_Select = "Instance_PrimaryCache_Select";
			public const string Instance_PrimaryCacheType = "Instance_PrimaryCacheType";
			public const string Instance_PrimaryCacheType_Select = "Instance_PrimaryCacheType_Select";
			public const string Instance_QueueConnection = "Instance_QueueConnection";
			public const string Instance_SettingsValues = "Instance_SettingsValues";
			public const string Instance_Solution = "Instance_Solution";
			public const string Instance_Solution_Help = "Instance_Solution_Help";
			public const string Instance_Solution_Select = "Instance_Solution_Select";
			public const string Instance_Status = "Instance_Status";
			public const string Instance_StatusDetails = "Instance_StatusDetails";
			public const string Instance_StatusTimeStamp = "Instance_StatusTimeStamp";
			public const string Instance_Subscription_Help = "Instance_Subscription_Help";
			public const string Instance_TestMode = "Instance_TestMode";
			public const string Instance_Title = "Instance_Title";
			public const string Instance_UpSince = "Instance_UpSince";
			public const string Instances_Title = "Instances_Title";
			public const string InstanceStates_CreatingRuntime = "InstanceStates_CreatingRuntime";
			public const string InstanceStates_Degraded = "InstanceStates_Degraded";
			public const string InstanceStates_DeployingContainer = "InstanceStates_DeployingContainer";
			public const string InstanceStates_DeployingRuntime = "InstanceStates_DeployingRuntime";
			public const string InstanceStates_FailedToDeploy = "InstanceStates_FailedToDeploy";
			public const string InstanceStates_FailedToInitialize = "InstanceStates_FailedToInitialize";
			public const string InstanceStates_FailedToStart = "InstanceStates_FailedToStart";
			public const string InstanceStates_FatalError = "InstanceStates_FatalError";
			public const string InstanceStates_HostFailedHealthCheck = "InstanceStates_HostFailedHealthCheck";
			public const string InstanceStates_HostRestarting = "InstanceStates_HostRestarting";
			public const string InstanceStates_Initializing = "InstanceStates_Initializing";
			public const string InstanceStates_NotDeployed = "InstanceStates_NotDeployed";
			public const string InstanceStates_Offline = "InstanceStates_Offline";
			public const string InstanceStates_Paused = "InstanceStates_Paused";
			public const string InstanceStates_Pausing = "InstanceStates_Pausing";
			public const string InstanceStates_Ready = "InstanceStates_Ready";
			public const string InstanceStates_RestartingRuntime = "InstanceStates_RestartingRuntime";
			public const string InstanceStates_Running = "InstanceStates_Running";
			public const string InstanceStates_Starting = "InstanceStates_Starting";
			public const string InstanceStates_StartingRuntime = "InstanceStates_StartingRuntime";
			public const string InstanceStates_Stopped = "InstanceStates_Stopped";
			public const string InstanceStates_Stopping = "InstanceStates_Stopping";
			public const string InstanceStates_Undeploying = "InstanceStates_Undeploying";
			public const string InstanceStates_UpdatingRuntime = "InstanceStates_UpdatingRuntime";
			public const string InstanceStates_UpdatingSolution = "InstanceStates_UpdatingSolution";
			public const string Integeration_APIKey = "Integeration_APIKey";
			public const string Integration_AccountId = "Integration_AccountId";
			public const string Integration_Description = "Integration_Description";
			public const string Integration_FromAddress = "Integration_FromAddress";
			public const string Integration_Help = "Integration_Help";
			public const string Integration_RoutingKey = "Integration_RoutingKey";
			public const string Integration_SMS = "Integration_SMS";
			public const string Integration_SMTP = "Integration_SMTP";
			public const string Integration_Title = "Integration_Title";
			public const string Integration_Uri = "Integration_Uri";
			public const string IntegrationType = "IntegrationType";
			public const string IntegrationType_PagerDuty = "IntegrationType_PagerDuty";
			public const string IntegrationType_Select_Watermark = "IntegrationType_Select_Watermark";
			public const string IntegrationType_SendGrid = "IntegrationType_SendGrid";
			public const string IntegrationType_Twillio = "IntegrationType_Twillio";
			public const string MessageWatchDog_Description = "MessageWatchDog_Description";
			public const string MessageWatchDog_DeviceMessage_Select = "MessageWatchDog_DeviceMessage_Select";
			public const string MessageWatchDog_ErrorCode = "MessageWatchDog_ErrorCode";
			public const string MessageWatchDog_ErrorCode_Help = "MessageWatchDog_ErrorCode_Help";
			public const string MessageWatchDog_ErrorCode_Select = "MessageWatchDog_ErrorCode_Select";
			public const string MessageWatchDog_ExcludeHolidays = "MessageWatchDog_ExcludeHolidays";
			public const string MessageWatchDog_Exclusion_Description = "MessageWatchDog_Exclusion_Description";
			public const string MessageWatchDog_Exclusion_End = "MessageWatchDog_Exclusion_End";
			public const string MessageWatchDog_Exclusion_End_Help = "MessageWatchDog_Exclusion_End_Help";
			public const string MessageWatchDog_Exclusion_Help = "MessageWatchDog_Exclusion_Help";
			public const string MessageWatchDog_Exclusion_Start = "MessageWatchDog_Exclusion_Start";
			public const string MessageWatchDog_Exclusion_Start_Help = "MessageWatchDog_Exclusion_Start_Help";
			public const string MessageWatchDog_Exclusion_Title = "MessageWatchDog_Exclusion_Title";
			public const string MessageWatchDog_Help = "MessageWatchDog_Help";
			public const string MessageWatchDog_Message = "MessageWatchDog_Message";
			public const string MessageWatchDog_Message_Help = "MessageWatchDog_Message_Help";
			public const string MessageWatchDog_SaturdayExclusions = "MessageWatchDog_SaturdayExclusions";
			public const string MessageWatchDog_StartupBuffer = "MessageWatchDog_StartupBuffer";
			public const string MessageWatchDog_StartupBuffer_Help = "MessageWatchDog_StartupBuffer_Help";
			public const string MessageWatchDog_SundayExclusions = "MessageWatchDog_SundayExclusions";
			public const string MessageWatchdog_Timeout = "MessageWatchdog_Timeout";
			public const string MessageWatchdog_Timeout_Help = "MessageWatchdog_Timeout_Help";
			public const string MessageWatchdog_Timeout_Interval = "MessageWatchdog_Timeout_Interval";
			public const string MessageWatchdog_Timeout_Interval_Help = "MessageWatchdog_Timeout_Interval_Help";
			public const string MessageWatchdog_Timeout_Interval_Select = "MessageWatchdog_Timeout_Interval_Select";
			public const string MessageWatchDog_Title = "MessageWatchDog_Title";
			public const string MessageWatchDog_WeekdayExclusions = "MessageWatchDog_WeekdayExclusions";
			public const string MqttNotification_Address = "MqttNotification_Address";
			public const string MqttNotification_Anonymous = "MqttNotification_Anonymous";
			public const string MqttNotification_Certificate = "MqttNotification_Certificate";
			public const string MqttNotification_CertificatePassword = "MqttNotification_CertificatePassword";
			public const string MqttNotification_ClientId = "MqttNotification_ClientId";
			public const string MqttNotification_ClientId_Help = "MqttNotification_ClientId_Help";
			public const string MqttNotification_Password = "MqttNotification_Password";
			public const string MqttNotification_Payload = "MqttNotification_Payload";
			public const string MqttNotification_Payload_Help = "MqttNotification_Payload_Help";
			public const string MqttNotification_Port = "MqttNotification_Port";
			public const string MqttNotification_SecureConnection = "MqttNotification_SecureConnection";
			public const string MqttNotification_Title = "MqttNotification_Title";
			public const string MqttNotification_Topic = "MqttNotification_Topic";
			public const string MqttNotification_Topic_Help = "MqttNotification_Topic_Help";
			public const string MqttNotification_UserName = "MqttNotification_UserName";
			public const string MqttNotifications_Help = "MqttNotifications_Help";
			public const string MqttNotifications_Title = "MqttNotifications_Title";
			public const string NuvIoT_Edition = "NuvIoT_Edition";
			public const string NuvIoTEdition_App = "NuvIoTEdition_App";
			public const string NuvIoTEdition_Cluster = "NuvIoTEdition_Cluster";
			public const string NuvIoTEdition_Container = "NuvIoTEdition_Container";
			public const string NuvIoTEdition_Select = "NuvIoTEdition_Select";
			public const string NuvIoTEdition_Shared = "NuvIoTEdition_Shared";
			public const string RemoteDeployment_Description = "RemoteDeployment_Description";
			public const string RemoteDeployment_Help = "RemoteDeployment_Help";
			public const string RemoteDeployment_Instances = "RemoteDeployment_Instances";
			public const string RemoteDeployment_PrimaryMCP = "RemoteDeployment_PrimaryMCP";
			public const string RemoteDeployment_SecondaryMCP = "RemoteDeployment_SecondaryMCP";
			public const string RemoteDeployment_Title = "RemoteDeployment_Title";
			public const string RestNotificaiton_AdditionalHeader_Help = "RestNotificaiton_AdditionalHeader_Help";
			public const string RestNotificaiton_AdditionalHeader_Title = "RestNotificaiton_AdditionalHeader_Title";
			public const string RestNotification_AdditionalHeaders = "RestNotification_AdditionalHeaders";
			public const string RestNotification_Address = "RestNotification_Address";
			public const string RestNotification_Address_Help = "RestNotification_Address_Help";
			public const string RestNotification_Anonymous = "RestNotification_Anonymous";
			public const string RestNotification_ContentType = "RestNotification_ContentType";
			public const string RestNotification_HeaderName = "RestNotification_HeaderName";
			public const string RestNotification_HeaderValue = "RestNotification_HeaderValue";
			public const string RestNotification_Help = "RestNotification_Help";
			public const string RestNotification_Method = "RestNotification_Method";
			public const string RestNotification_Method_DELETE = "RestNotification_Method_DELETE";
			public const string RestNotification_Method_GET = "RestNotification_Method_GET";
			public const string RestNotification_Method_PATCH = "RestNotification_Method_PATCH";
			public const string RestNotification_Method_POST = "RestNotification_Method_POST";
			public const string RestNotification_Method_PUT = "RestNotification_Method_PUT";
			public const string RestNotification_Method_Select = "RestNotification_Method_Select";
			public const string RestNotification_Password = "RestNotification_Password";
			public const string RestNotification_Payload = "RestNotification_Payload";
			public const string RestNotification_Title = "RestNotification_Title";
			public const string RestNotification_UserName = "RestNotification_UserName";
			public const string RestNotifications_Title = "RestNotifications_Title";
			public const string Route_Description = "Route_Description";
			public const string Route_Help = "Route_Help";
			public const string Route_InputTranslator = "Route_InputTranslator";
			public const string Route_IsDefault = "Route_IsDefault";
			public const string Route_IsDefault_Help = "Route_IsDefault_Help";
			public const string Route_Message_Help = "Route_Message_Help";
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
			public const string RouteModuleConfig_Help = "RouteModuleConfig_Help";
			public const string RouteModuleConfig_Title = "RouteModuleConfig_Title";
			public const string RouteModuleConfig_Unassigned = "RouteModuleConfig_Unassigned";
			public const string Solution_DefaultListener = "Solution_DefaultListener";
			public const string Solution_DefaultListener_Help = "Solution_DefaultListener_Help";
			public const string Solution_DefaultListener_Select = "Solution_DefaultListener_Select";
			public const string Solution_Description = "Solution_Description";
			public const string Solution_DeviceConfigurations = "Solution_DeviceConfigurations";
			public const string Solution_DeviceConfigurations_Help = "Solution_DeviceConfigurations_Help";
			public const string Solution_Environment = "Solution_Environment";
			public const string Solution_Help = "Solution_Help";
			public const string Solution_Icon = "Solution_Icon";
			public const string Solution_Settings = "Solution_Settings";
			public const string Solution_Settings_Help = "Solution_Settings_Help";
			public const string Solution_Title = "Solution_Title";
			public const string Solution_Version = "Solution_Version";
			public const string Solutions_Title = "Solutions_Title";
			public const string SystemTest_Description = "SystemTest_Description";
			public const string SystemTest_EstimatedDuration = "SystemTest_EstimatedDuration";
			public const string SystemTest_EstimatedDuration_Help = "SystemTest_EstimatedDuration_Help";
			public const string SystemTest_OnFailedInstructions = "SystemTest_OnFailedInstructions";
			public const string SystemTest_Schedule = "SystemTest_Schedule";
			public const string SystemTest_Steps = "SystemTest_Steps";
			public const string SystemTest_Title = "SystemTest_Title";
			public const string SystemTestExecution_AbortedBy = "SystemTestExecution_AbortedBy";
			public const string SystemTestExecution_CompletedBy = "SystemTestExecution_CompletedBy";
			public const string SystemTestExecution_EndTimeStamp = "SystemTestExecution_EndTimeStamp";
			public const string SystemTestExecution_StartedBy = "SystemTestExecution_StartedBy";
			public const string SystemTestExecution_StartTimeStamp = "SystemTestExecution_StartTimeStamp";
			public const string SystemTestExecution_State_Aborted = "SystemTestExecution_State_Aborted";
			public const string SystemTestExecution_State_Failed = "SystemTestExecution_State_Failed";
			public const string SystemTestExecution_State_InProcess = "SystemTestExecution_State_InProcess";
			public const string SystemTestExecution_State_New = "SystemTestExecution_State_New";
			public const string SystemTestExecution_State_Passed = "SystemTestExecution_State_Passed";
			public const string SystemTestExecution_Status = "SystemTestExecution_Status";
			public const string SystemTestExecution_SystemTest = "SystemTestExecution_SystemTest";
			public const string SystemTestExecutions_Description = "SystemTestExecutions_Description";
			public const string SystemTestExecutions_Title = "SystemTestExecutions_Title";
			public const string SystemTests_Title = "SystemTests_Title";
			public const string SystemTestStep_Description = "SystemTestStep_Description";
			public const string SystemTestStep_Details = "SystemTestStep_Details";
			public const string SystemTestStep_Resources = "SystemTestStep_Resources";
			public const string SystemTestStep_Summary = "SystemTestStep_Summary";
			public const string SystemTestStep_Title = "SystemTestStep_Title";
			public const string SystemTestStep_Troubleshooting = "SystemTestStep_Troubleshooting";
			public const string SystemTestStep_WebLink = "SystemTestStep_WebLink";
			public const string TaggedContainer_CreationDate = "TaggedContainer_CreationDate";
			public const string TaggedContainer_Description = "TaggedContainer_Description";
			public const string TaggedContainer_Help = "TaggedContainer_Help";
			public const string TaggedContainer_ReleaseNotes = "TaggedContainer_ReleaseNotes";
			public const string TaggedContainer_Status = "TaggedContainer_Status";
			public const string TaggedContainer_Status_Alpha = "TaggedContainer_Status_Alpha";
			public const string TaggedContainer_Status_Beta = "TaggedContainer_Status_Beta";
			public const string TaggedContainer_Status_Deprecated = "TaggedContainer_Status_Deprecated";
			public const string TaggedContainer_Status_Prerelease = "TaggedContainer_Status_Prerelease";
			public const string TaggedContainer_Status_Production = "TaggedContainer_Status_Production";
			public const string TaggedContainer_Status_Select = "TaggedContainer_Status_Select";
			public const string TaggedContainer_Tag = "TaggedContainer_Tag";
			public const string TaggedContainer_Title = "TaggedContainer_Title";
			public const string Telemetry_ErrorQueryServer = "Telemetry_ErrorQueryServer";
			public const string Telemetry_Help = "Telemetry_Help";
			public const string Telemetry_Title = "Telemetry_Title";
			public const string Warning_NoDeviceConfigs = "Warning_NoDeviceConfigs";
			public const string Warning_NoListeners = "Warning_NoListeners";
			public const string WatchdogConnectedDevice_Help = "WatchdogConnectedDevice_Help";
			public const string WatchdogConnectedDevice_Title = "WatchdogConnectedDevice_Title";
			public const string WatchdogExclusion_AllDay = "WatchdogExclusion_AllDay";
			public const string WatchdogExclusion_AllDay_Help = "WatchdogExclusion_AllDay_Help";
			public const string WatchdogMessage_Help = "WatchdogMessage_Help";
			public const string WatchdogMessage_Title = "WatchdogMessage_Title";
			public const string WiFiConnectionProfile_Description = "WiFiConnectionProfile_Description";
			public const string WiFiConnectionProfile_Help = "WiFiConnectionProfile_Help";
			public const string WiFiConnectionProfile_Name = "WiFiConnectionProfile_Name";
			public const string WiFiConnectionProfile_Notes = "WiFiConnectionProfile_Notes";
			public const string WiFiConnectionProfile_Password = "WiFiConnectionProfile_Password";
			public const string WiFiConnectionProfile_SSID = "WiFiConnectionProfile_SSID";
			public const string WiFiConnectionProfile_Title = "WiFiConnectionProfile_Title";
			public const string WorkingStorage = "WorkingStorage";
			public const string WorkingStorage_Cloud = "WorkingStorage_Cloud";
			public const string WorkingStorage_Help = "WorkingStorage_Help";
			public const string WorkingStorage_Local = "WorkingStorage_Local";
			public const string WorkingStorage_Select = "WorkingStorage_Select";
		}
	}
}

