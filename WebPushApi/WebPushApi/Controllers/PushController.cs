using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebPush;
using WebPushApi.Services;

namespace WebPushApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PushController : ControllerBase
    {
        private static readonly List<PushSubscription> subscriptions = [];
        private readonly PushNotificationService _pushService;

        public PushController(PushNotificationService pushService)
        {
            _pushService = pushService;
        }

        [HttpPost("subscribe")]
        public IActionResult Subscribe([FromBody] PushSubscription subscription)
        {
            if (!subscriptions.Exists(s => s.Endpoint == subscription.Endpoint))
            {
                subscriptions.Add(subscription);
            }
            return Ok(new { message = "Subscribed successfully!" });
        }

        [HttpPost("notify")]
        public async Task<IActionResult> Notify([FromBody] string message)
        {
            foreach (var sub in subscriptions)
            {
                await _pushService.SendNotificationAsync(sub, message);
            }
            return Ok(new { message = "Notifications sent!" });
        }
    }
}
