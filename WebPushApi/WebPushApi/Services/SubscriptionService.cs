
using Microsoft.EntityFrameworkCore;
using WebPushApi.Data;
using WebPushApi.Models;

namespace WebPushApi.Services
{
    public class SubscriptionService(AppDbContext context)
    {
        public async Task CreateSubscriptionAsync(Subscription subscription)
        {
            var existingSubscription = await context.Subscriptions
                .FirstOrDefaultAsync(s => s.Endpoint == subscription.Endpoint);

            if (existingSubscription == null)
            {
                context.Subscriptions.Add(subscription);
            }
            else
            {
                existingSubscription.P256DH = subscription.P256DH;
                existingSubscription.Auth = subscription.Auth;
            }

            await context.SaveChangesAsync();
        }

        public async Task<List<Subscription>> GetSubscriptionsAsync()
        {
            var subscriptions = await context.Subscriptions.AsNoTracking().ToListAsync();
            return subscriptions;
        }
    }
}
