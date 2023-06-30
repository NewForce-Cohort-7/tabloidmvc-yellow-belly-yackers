using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface ICommentsRepository
    {
        List<Comment> GetAll();
        List<Comment> GetByPostId(int postId);
        public void Add(Comment comment);
    }
}
