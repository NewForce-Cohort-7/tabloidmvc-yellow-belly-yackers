﻿using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface IPostRepository
    {
        void Add(Post post);
        List<Post> GetAllPublishedPosts();
        Post GetPublishedPostById(int id);
        Post GetUserPostById(int id, int userProfileId);
        void Delete(Post post);
        void Update(Post post);
        List<Post> GetAllPostsByUser(int userProfileId);
        void AddPostTag(PostTag postTag);
        void DeletePostTagsOnPost(int id);
    }
}