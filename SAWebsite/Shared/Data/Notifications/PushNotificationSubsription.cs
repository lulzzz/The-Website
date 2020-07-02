using System;

namespace SAWebsite.Shared.Data.Notifications
{
    public abstract class PushNotificationSubsription
    {
        public string Url { get; set; }
        public string P256dh { get; set; }
        public string Auth { get; set; }
    }
}
