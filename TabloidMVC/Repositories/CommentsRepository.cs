using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public class CommentsRepository : BaseRepository, ICommentsRepository
    {
        public CommentsRepository(IConfiguration config) : base(config) { }

        public List<Comment> GetAll()
        {
            List<Comment> comments = new List<Comment>();

            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT c.Id, c.PostId, c.UserProfileId, c.Subject, c.Content, c.CreateDateTime,
                        p.Title AS PostTitle, p.Content AS PostContent, p.ImageLocation AS PostImageLocation,
                        u.DisplayName AS UserProfileDisplayName
                        FROM Comment c
                        LEFT JOIN Post p ON c.PostId = p.Id
                        LEFT JOIN UserProfile u ON c.UserProfileId = u.Id";

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Comment comment = new Comment();
                        comment.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                        comment.PostId = reader.GetInt32(reader.GetOrdinal("PostId"));
                        comment.UserProfileId = reader.GetInt32(reader.GetOrdinal("UserProfileId"));
                        comment.Subject = reader.GetString(reader.GetOrdinal("Subject"));
                        comment.Content = reader.GetString(reader.GetOrdinal("Content"));
                        comment.CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime"));

                        comment.Post = new Post
                        {
                            Title = reader.GetString(reader.GetOrdinal("PostTitle")),
                            Content = reader.GetString(reader.GetOrdinal("PostContent")),
                            ImageLocation = reader.GetString(reader.GetOrdinal("PostImageLocation"))
                        };

                        comment.UserProfile = new UserProfile
                        {
                            DisplayName = reader.GetString(reader.GetOrdinal("UserProfileDisplayName"))
                        };

                        comments.Add(comment);
                    }
                }
            }

            return comments;
        }

        // This method is used to get all comments for a specific post
        public List<Comment> GetByPostId(int postId)
        {
            List<Comment> comments = new List<Comment>();

            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT c.Id, c.PostId, c.UserProfileId, c.Subject, c.Content, c.CreateDateTime,
                p.Title AS PostTitle, p.Content AS PostContent, p.ImageLocation AS PostImageLocation,
                u.DisplayName AS UserProfileDisplayName
                FROM Comment c
                LEFT JOIN Post p ON c.PostId = p.Id
                LEFT JOIN UserProfile u ON c.UserProfileId = u.Id
                WHERE c.PostId = @PostId";

                    cmd.Parameters.AddWithValue("@PostId", postId);

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Comment comment = new Comment();
                        comment.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                        comment.PostId = reader.GetInt32(reader.GetOrdinal("PostId"));
                        comment.UserProfileId = reader.GetInt32(reader.GetOrdinal("UserProfileId"));
                        comment.Subject = reader.GetString(reader.GetOrdinal("Subject"));
                        comment.Content = reader.GetString(reader.GetOrdinal("Content"));
                        comment.CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime"));

                        comment.Post = new Post
                        {
                            Title = reader.GetString(reader.GetOrdinal("PostTitle")),
                            Content = reader.GetString(reader.GetOrdinal("PostContent")),
                            ImageLocation = reader.GetString(reader.GetOrdinal("PostImageLocation"))
                        };

                        comment.UserProfile = new UserProfile
                        {
                            DisplayName = reader.GetString(reader.GetOrdinal("UserProfileDisplayName"))
                        };

                        comments.Add(comment);
                    }
                }
            }

            return comments;
        }

        // This adds a new comment to the database
        public void Add(Comment comment)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText=@"INSERT INTO Comment (PostId, UserProfileId, Subject, Content, CreateDateTime) VALUES (@PostId, @UserProfileId, @Subject, @Content, @CreateDateTime)";

                    cmd.Parameters.AddWithValue("@PostId", comment.PostId);
                    cmd.Parameters.AddWithValue("@UserProfileId", comment.UserProfileId);
                    cmd.Parameters.AddWithValue("@Subject", comment.Subject);
                    cmd.Parameters.AddWithValue("@Content", comment.Content);
                    cmd.Parameters.AddWithValue("@CreateDateTime", comment.CreateDateTime);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Deletes a comment by its Id
        public void Delete(int commentId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText=@"DELETE FROM Comment WHERE Id = @Id";

                    cmd.Parameters.AddWithValue("@Id", commentId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // This grabs a single comment by its Id
        public Comment GetById(int commentId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id, c.PostId, c.UserProfileId, c.Subject, c.Content, c.CreateDateTime,
                        p.Title AS PostTitle, p.Content AS PostContent, p.ImageLocation AS PostImageLocation,
                        u.DisplayName AS UserProfileDisplayName
                        FROM Comment c
                        LEFT JOIN Post p ON c.PostId = p.Id
                        LEFT JOIN UserProfile u ON c.UserProfileId = u.Id
                        WHERE c.Id = @id";
                    cmd.Parameters.AddWithValue("@id", commentId);
                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        Comment comment = new Comment();
                        comment.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                        comment.PostId = reader.GetInt32(reader.GetOrdinal("PostId"));
                        comment.UserProfileId = reader.GetInt32(reader.GetOrdinal("UserProfileId"));
                        comment.Subject = reader.GetString(reader.GetOrdinal("Subject"));
                        comment.Content = reader.GetString(reader.GetOrdinal("Content"));
                        comment.CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime"));

                        comment.Post = new Post
                        {
                            Title = reader.GetString(reader.GetOrdinal("PostTitle")),
                            Content = reader.GetString(reader.GetOrdinal("PostContent")),
                            ImageLocation = reader.GetString(reader.GetOrdinal("PostImageLocation"))
                        };

                        comment.UserProfile = new UserProfile
                        {
                            DisplayName = reader.GetString(reader.GetOrdinal("UserProfileDisplayName"))
                        };

                        return comment;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }
}
