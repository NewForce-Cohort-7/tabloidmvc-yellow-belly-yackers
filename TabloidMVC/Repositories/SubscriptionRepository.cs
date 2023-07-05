using System.Collections.Generic;
using Azure;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TabloidMVC.Models;
using TabloidMVC.Utils;

namespace TabloidMVC.Repositories
{
    public class SubscriptionRepository : BaseRepository, ISubscriptionRepository
    {
        public SubscriptionRepository(IConfiguration config) : base(config) { }

        public void Add(Subscription subscription)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Subscription ( SubscriberUserProfileId, ProviderUserProfileId, BeginDateTime, EndDateTime)
                        OUTPUT INSERTED.ID
                        VALUES ( @subId, @providerId, @beginDate, @endDate)";

                    cmd.Parameters.AddWithValue("@subId", subscription.SubscriberUserProfileId);
                    cmd.Parameters.AddWithValue("@providerId", subscription.ProviderUserProfileId);
                    cmd.Parameters.AddWithValue("@beginDate", subscription.BeginDateTime);
                    cmd.Parameters.AddWithValue("@endDate", DbUtils.ValueOrDBNull(subscription.EndDateTime));

                    subscription.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public int? AlreadySubbedId(int subscriberId, int providerId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, SubscriberUserProfileId, ProviderUserProfileId
                        FROM Subscription
                        WHERE SubscriberUserProfileId = @subId
                        AND ProviderUserProfileId = @providerId
                        AND EndDateTime IS NULL";

                    cmd.Parameters.AddWithValue("@subId", subscriberId);
                    cmd.Parameters.AddWithValue("@providerId", providerId);
                    

                    var reader = cmd.ExecuteReader();
                    int? alreadySubbedId = null;

                    //if subscription with same provider and subscriber IDs already exists, return true
                    if (reader.Read())
                    {
                        alreadySubbedId = reader.GetInt32(reader.GetOrdinal("Id"));
                    }

                    reader.Close();
                    return alreadySubbedId;

                }
            }
        }




        public Subscription GetById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, SubscriberUserProfileId, ProviderUserProfileId, BeginDateTime, EndDateTime FROM Subscription
                        WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);
                    var reader = cmd.ExecuteReader();

                    Subscription sub = null;

                    if (reader.Read())
                    {
                        sub = NewSubFromReader(reader);
                        reader.Close();
                        return sub;
                    }

                    else
                    {
                        reader.Close();
                        return null;
                    }
                }
            }
        }


        public List<Subscription> GetAllSubscribersSubs(int subscriberId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, SubscriberUserProfileId, ProviderUserProfileId, BeginDateTime, EndDateTime 
                        FROM Subscription
                        WHERE SubscriberUserProfileId = @subscriberId
                        AND EndDateTime IS NULL ";

                    cmd.Parameters.AddWithValue("@subscriberId", subscriberId);
                    var reader = cmd.ExecuteReader();

                    var subs = new List<Subscription>();

                    while (reader.Read())
                    {
                        subs.Add(NewSubFromReader(reader));
                    }

                    reader.Close();

                    return subs;
                }
            }
        }





        private Subscription NewSubFromReader(SqlDataReader reader)
        {
            return new Subscription()
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                SubscriberUserProfileId = reader.GetInt32(reader.GetOrdinal("SubscriberUserProfileId")),
                ProviderUserProfileId = reader.GetInt32(reader.GetOrdinal("ProviderUserProfileId")),
                BeginDateTime = reader.GetDateTime(reader.GetOrdinal("BeginDateTime")),
                EndDateTime = reader.IsDBNull(reader.GetOrdinal("EndDateTime")) ? null : reader.GetDateTime(reader.GetOrdinal("EndDateTime"))
            };
        }


        public void Unsubscribe(Subscription subscription)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            UPDATE Subscription
                            SET [EndDateTime] = @currentDateTime 
                            WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@currentDateTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@id", subscription.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }




    }
}
