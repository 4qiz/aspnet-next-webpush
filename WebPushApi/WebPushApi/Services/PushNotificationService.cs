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
            _publicKey = vapidConfig["PublicKey"];
            _privateKey = vapidConfig["PrivateKey"];
            _email = vapidConfig["Email"];
        }

        public async Task SendNotificationAsync(PushSubscription subscription, string message)
        {
            var vapidDetails = new VapidDetails(_email, _publicKey, _privateKey);
            var webPushClient = new WebPushClient();

            var payload = JsonSerializer.Serialize(new { title = "Notification", body = message });

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
}
