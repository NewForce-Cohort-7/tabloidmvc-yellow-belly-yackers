using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface IUserProfileRepository
    {
        UserProfile GetByEmail(string email);
        List<UserProfile> GetAllUserProfiles();
        UserProfile GetProfileById(int id);
        void RegisterUser(UserProfile userProfile);
        void DeactivateUser(UserProfile userProfile);
        void ReactivateUser(UserProfile userProfile);
    }
}