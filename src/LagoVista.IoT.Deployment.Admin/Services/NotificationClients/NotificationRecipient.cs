// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 49928d881a8843c99f52108f7680a06af5e64e07a4e00ecb813d2f1759e5e83a
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core;
using LagoVista.UserAdmin.Models.Orgs;
using LagoVista.UserAdmin.Models.Users;
using System;

namespace LagoVista.IoT.Deployment.Admin.Services.NotificationClients
{
    public class NotificationRecipient
    {
        public NotificationRecipient()
        {
            NotificationRecipientId = Guid.NewGuid().ToId();
        } 

        public string NotificationRecipientId { get;  } 
        public bool IsAppUser { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool SendEmail { get; set; }
        public bool SendSMS { get; set; }

        public static NotificationRecipient FromAppUser(AppUser appUser)
        {
            return new NotificationRecipient()
            {
                IsAppUser = true,
                Id = appUser.Id,
                Phone = appUser.PhoneNumber,
                Email = appUser.Email,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                SendEmail = true,
                SendSMS = true,
            };
        }

        public static NotificationRecipient FromExternalContext(ExternalContact externalContact)
        {
            return new NotificationRecipient()
            {
                IsAppUser = false,
                Id = externalContact.Id,
                FirstName = externalContact.FirstName,
                LastName = externalContact.LastName,
                Email = externalContact.Email,
                Phone = externalContact.Phone,
                SendEmail = externalContact.SendEmail,
                SendSMS = externalContact.SendSMS,
            };
        }

    }
}
