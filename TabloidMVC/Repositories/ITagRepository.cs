using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface ITagRepository
    {
        List<Tag> GetAll();
        void Add(Tag tag);
        void Delete(int id);
        Tag GetById(int id);

    }
}
