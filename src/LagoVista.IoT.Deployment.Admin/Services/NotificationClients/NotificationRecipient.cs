using LagoVista.UserAdmin.Models.Orgs;
using LagoVista.UserAdmin.Models.Users;

namespace LagoVista.IoT.Deployment.Admin.Services.NotificationClients
{
    public class NotificationRecipient
    {
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
