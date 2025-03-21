using System.Text.Json;
using WebPush;

namespace WebPushApi.Services
{
    public class PushNotificationService
    {
        private readonly string _publicKey;
        private readonly string _privateKey;
        private readonly string _email;

        public PushNotificationService(IConfiguration config)
        {
            var vapidConfig = config.GetSection("VAPID");
            _publicKey = vapidConfig["PublicKey"] ?? throw new Exception("VAPID__PublicKey is null");
            _privateKey = vapidConfig["PrivateKey"] ?? throw new Exception("VAPID__PrivateKey is null");
            _email = vapidConfig["Email"] ?? throw new Exception("VAPID__Email is null");
        }

        public async Task SendNotificationAsync(PushSubscription subscription, Notification notification)
        {
            var vapidDetails = new VapidDetails(_email, _publicKey, _privateKey);
            var webPushClient = new WebPushClient();

            var payload = JsonSerializer.Serialize(new { title = notification.Title, body = notification.Body });

            try
            {
                await webPushClient.SendNotificationAsync(subscription, payload, vapidDetails);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending push notification: {ex.Message}");
            }
        }
    }

    public class Notification
    {
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
