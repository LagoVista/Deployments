﻿using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.UserAdmin.Models.Orgs;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Services.NotificationClients
{
    public class SMSSender : ISMSSender
    {

        private readonly ITagReplacementService _tagReplacer;
        private readonly ILogger _logger;
        private readonly IAppConfig _appConfig;
        private readonly ILinkShortener _linkShortener;
        private readonly LagoVista.UserAdmin.Interfaces.Managers.ISmsSender _smsSender;

        private string _smsContent;

        public int TextMessagesSent { get; private set; }

        public SMSSender(IAdminLogger logger, IAppConfig appConfig, ITagReplacementService tagReplacer, ILinkShortener linkShortener, LagoVista.UserAdmin.Interfaces.Managers.ISmsSender smsSender)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tagReplacer = tagReplacer ?? throw new ArgumentNullException(nameof(tagReplacer));
            _linkShortener = linkShortener ?? throw new ArgumentNullException(nameof(linkShortener));
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
            _smsSender = smsSender ?? throw new ArgumentNullException(nameof(smsSender));
        }

        public async Task<InvokeResult> PrepareMessage(DeviceNotification notification, bool testMode, Device device, OrgLocation location)
        {
            if (!String.IsNullOrEmpty(notification.SmsContent))
                _smsContent = await _tagReplacer.ReplaceTagsAsync(notification.SmsContent, false, device, location);
            else
                _smsContent = "[no content]";

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> SendAsync(NotificationRecipient recipient, NotificationLinks links, EntityHeader org, EntityHeader user)
        {
            var actualInk = String.IsNullOrEmpty(links.FullLandingPageLink) ? links.AcknowledgeLink.Replace("[RecipientId]", recipient.Id)
                            : links.FullLandingPageLink.Replace("[RecipientId]", recipient.Id);

            var shortenedLink = await _linkShortener.ShortenLinkAsync(actualInk);
            if (!shortenedLink.Successful) return shortenedLink.ToInvokeResult();

            if (String.IsNullOrEmpty(recipient.Phone))
            {
                _logger.AddCustomEvent(LogLevel.Warning, $"[NotificationSender__RaiseNotificationAsync__SendSms__ExternalContact]",
                    $"Request to send msms to {recipient.FirstName} {recipient.LastName} ({recipient.Id}) but phone number is empty");
                return InvokeResult.FromError($"Request to send msms to {recipient.FirstName} {recipient.LastName} ({recipient.Id}) but phone number is empty");
            }
            else
            {
                _logger.Trace($"[NotificationSender__RaiseNotificationAsync__ExternalContact] - Sending SMS To {recipient.FirstName} {recipient.LastName} {recipient.Phone} -  {shortenedLink.Result} ({actualInk})");
                var result = await _smsSender.SendAsync(recipient.Phone, $"{_smsContent} {shortenedLink.Result}");
                if (result.Successful)
                {
                    TextMessagesSent++;
                    return InvokeResult.Success;
                }
                else
                {
                    _logger.AddException($"[NotificationSender__RaiseNotificationAsync__SendSms__ExternalContact]", new Exception($"Error sending sms to {recipient.Phone} - {result.ErrorMessage}"));
                    return InvokeResult.FromError($"Error sending sms to {recipient.Phone} - {result.ErrorMessage}");
                }
            }
        }
    }
}