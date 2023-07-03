using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface ICommentsRepository
    {
        List<Comment> GetAll();
        List<Comment> GetByPostId(int postId);
        void Add(Comment comment);
        Comment GetById(int commentId);
        void Delete(int commentId);
    }
}
