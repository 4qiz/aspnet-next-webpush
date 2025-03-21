using WebPush;
using WebPushApi.Models;

namespace WebPushApi.Mappers
{
    public static class SubscriptionMapper
    {
        public static Subscription ToSubscription(this PushSubscription pushSub)
        {
            return new Subscription
            {
                Auth = pushSub.Auth,
                Endpoint = pushSub.Endpoint,
                P256DH = pushSub.P256DH
            };
        }

        public static PushSubscription ToPushSubscription(this Subscription sub)
        {
            return new PushSubscription
            {
                Auth = sub.Auth,
                Endpoint = sub.Endpoint,
                P256DH = sub.P256DH
            };
        }
    }
}
