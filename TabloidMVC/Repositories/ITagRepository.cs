using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface ITagRepository
    {
        List<Tag> GetAll();
        void Add(Tag tag);
        void Delete(int id);
        Tag GetById(int id);
        void Update(Tag tag);
        List<Tag> GetTagsOnPost(int id);
        List<int> GetTagsByPostId(int id);
        void DeletePostTagsByPost(int postId);

    }
}
