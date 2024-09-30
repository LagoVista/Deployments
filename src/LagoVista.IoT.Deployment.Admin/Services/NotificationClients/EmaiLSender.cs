﻿using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.UserAdmin.Interfaces.Managers;
using LagoVista.UserAdmin.Models.Orgs;
using RingCentral;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Services.NotificationClients
{
    public class EmailSender : Interfaces.IEmailSender
    {
        private readonly ITagReplacementService _tagReplacer;
        private readonly ILogger _logger;
        private readonly UserAdmin.Interfaces.Managers.IEmailSender _emailSender;
        private readonly ILinkShortener _linkShortener;

        private string _emailContent;
        public int EmailsSent { get; private set; }

        public EmailSender(IAdminLogger logger, ITagReplacementService tagReplacer, UserAdmin.Interfaces.Managers.IEmailSender emailSender, ILinkShortener linkShortener)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tagReplacer = tagReplacer ?? throw new ArgumentNullException(nameof(tagReplacer));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _linkShortener = linkShortener ?? throw new ArgumentNullException(nameof(linkShortener));
        }

        public async Task<InvokeResult> PrepareMessage(DeviceNotification notification, bool testMode, Device device, OrgLocation location)
        {
            if (!String.IsNullOrEmpty(notification.EmailContent))
            {
                _emailContent = await _tagReplacer.ReplaceTagsAsync(notification.EmailContent, true, device, location);
                if (testMode)
                    _emailContent = $"<h1>TESTING - TESTING</h1> {_emailContent}";
            }
            else
            {
                _emailContent = "[no content]";
            }

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> SendAsync(DeviceNotification notification, NotificationRecipient recipient, NotificationLinks links, EntityHeader org, EntityHeader user)
        {
            if (!recipient.SendEmail)
                return InvokeResult.FromError("Should not send email to recipient (SendEmail is false).");

            if (String.IsNullOrEmpty(_emailContent))
                return InvokeResult.FromError("No email content to send to recipient");

            var actualInk = String.IsNullOrEmpty(links.FullLandingPageLink) ?  links.AcknowledgeLink.Replace("[RecipientId]", recipient.Id)
                : links.FullLandingPageLink.Replace("[RecipientId]", recipient.Id);

            var shortenedLink = await _linkShortener.ShortenLinkAsync(actualInk);
            if (!shortenedLink.Successful) return shortenedLink.ToInvokeResult();

            var linkLabel = String.IsNullOrEmpty(links.FullLandingPageLink) ? "Details" : "Acknowledge";

            if (String.IsNullOrEmpty(recipient.Email))
            {
                _logger.AddCustomEvent(LogLevel.Warning, $"[NotificationSender__RaiseNotificationAsync__SendEmail__ExternalContact]",
                    $"Request to send email to {recipient.FirstName} {recipient.LastName} ({recipient.Id}) but email addess is empty");

                return InvokeResult.FromError($"Request to send email to {recipient.FirstName} {recipient.LastName} ({recipient.Id}) but email addess is empty");
            }
            else
            {
                _logger.Trace($"[NotificationSender__RaiseNotificationAsync__ExternalContact] - Sending Email To {recipient.FirstName} {recipient.LastName} {recipient.Email} -  {shortenedLink.Result} ({actualInk})");
                var result = await _emailSender.SendAsync(recipient.Email, notification.EmailSubject, _emailContent + $"<div style='font-size:16px'><a href='{shortenedLink.Result}'>{linkLabel}</a></div>", org, user);
                if (result.Successful)
                    EmailsSent++;
                else
                    _logger.AddException($"[NotificationSender__RaiseNotificationAsync__SendEmail__ExternalContact]", new Exception($"Error sending email to {recipient.Email} - {result.ErrorMessage}"));

                return result;
            }
        }

    }
}