using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TabloidMVC.Models;
using TabloidMVC.Utils;

namespace TabloidMVC.Repositories
{
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        public CategoryRepository(IConfiguration config) : base(config) { }

        public List<Category> GetAll()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT id, name FROM Category order by Name";
                    var reader = cmd.ExecuteReader();

                    var categories = new List<Category>();

                    while (reader.Read())
                    {
                        categories.Add(new Category()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                        });
                    }

                    reader.Close();

                    return categories;
                }
            }
        }

        public Category GetById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Name FROM Category
                        WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);
                    var reader = cmd.ExecuteReader();

                    Category category = null;

                    if (reader.Read())
                    {
                        category = NewCategoryFromReader(reader);
                        reader.Close();
                        return category;
                    }

                    else
                    {
                        reader.Close();
                        return null;
                    }
                }
            }
        }

        public void Add(Post post)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Post (
                            Title, Content, ImageLocation, CreateDateTime, PublishDateTime,
                            IsApproved, CategoryId, UserProfileId )
                        OUTPUT INSERTED.ID
                        VALUES (
                            @Title, @Content, @ImageLocation, @CreateDateTime, @PublishDateTime,
                            @IsApproved, @CategoryId, @UserProfileId )";
                    cmd.Parameters.AddWithValue("@Title", post.Title);
                    cmd.Parameters.AddWithValue("@Content", post.Content);
                    cmd.Parameters.AddWithValue("@ImageLocation", DbUtils.ValueOrDBNull(post.ImageLocation));
                    cmd.Parameters.AddWithValue("@CreateDateTime", post.CreateDateTime);
                    cmd.Parameters.AddWithValue("@PublishDateTime", DbUtils.ValueOrDBNull(post.PublishDateTime));
                    cmd.Parameters.AddWithValue("@IsApproved", post.IsApproved);
                    cmd.Parameters.AddWithValue("@CategoryId", post.CategoryId);
                    cmd.Parameters.AddWithValue("@UserProfileId", post.UserProfileId);

                    post.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(Category category)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            UPDATE Category
                            SET [Name] = @name 
                            WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@name", category.Name);
                    cmd.Parameters.AddWithValue("@id", category.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private Category NewCategoryFromReader(SqlDataReader reader)
        {
            return new Category()
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
            };
        }








    }
}