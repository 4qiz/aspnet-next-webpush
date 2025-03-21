using Microsoft.AspNetCore.Mvc;
using WebPush;
using WebPushApi.Mappers;
using WebPushApi.Services;

namespace WebPushApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PushController(PushNotificationService pushService, SubscriptionService subscriptionService) : ControllerBase
    {
        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] PushSubscription pushSub)
        {
            if (pushSub == null || string.IsNullOrEmpty(pushSub.Endpoint))
            {
                return BadRequest("Invalid subscription data.");
            }

            await subscriptionService.CreateSubscriptionAsync(pushSub.ToSubscription());

            return Ok("Subscription saved.");
        }

        [HttpPost("notify")]
        public async Task<IActionResult> Notify([FromBody] string message)
        {
            var subscriptions = await subscriptionService.GetSubscriptionsAsync();

            foreach (var sub in subscriptions)
            {
                await pushService.SendNotificationAsync(sub.ToPushSubscription(), new Notification { Title = "notification", Body = message });
            }
            return Ok(new { message = "Notifications sent!" });
        }
    }
}
