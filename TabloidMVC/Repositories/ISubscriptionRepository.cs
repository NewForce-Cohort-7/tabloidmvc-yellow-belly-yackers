using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface ISubscriptionRepository
    {
        void Add(Subscription subscription);
        int? AlreadySubbedId(int subscriberId, int providerId);
        Subscription GetById(int id);
        List<Subscription> GetAllSubscribersSubs(int subscriberId);
    }
}
