using System;
using System.Collections.Generic;
using WeeklyPlanner.Model.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace WeeklyPlanner.Model.Repositories
{
    public class RoomieRepository : BaseRepository
    {
        public RoomieRepository(IConfiguration configuration) : base(configuration)
        {
        }

        // Add a roomie to the database
        public bool AddRoomie(Roomie roomie)
        {
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                try
                {
                    var cmd = dbConn.CreateCommand();
                    cmd.CommandText = @"INSERT INTO roomie (roomiename, loginid) 
                                        VALUES (@roomiename, @loginid)";
                    cmd.Parameters.AddWithValue("@roomiename", NpgsqlDbType.Text, roomie.roomiename);
                    cmd.Parameters.AddWithValue("@loginid", NpgsqlDbType.Integer, roomie.loginid);

                    return InsertData(dbConn, cmd);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error adding roomie", ex);
                }
            }
        }

        // Retrieve all roomies for a specific login ID
        public List<Roomie> GetRoomiesByLoginId(int loginid)
        {
            var roomies = new List<Roomie>();
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                try
                {
                    var cmd = dbConn.CreateCommand();
                    cmd.CommandText = @"SELECT * FROM roomie WHERE loginid = @loginid";
                    cmd.Parameters.AddWithValue("@loginid", NpgsqlDbType.Integer, loginid);

                    var data = GetData(dbConn, cmd);

                    while (data != null && data.Read())
                    {
                        roomies.Add(new Roomie
                        {
                            roomieid = Convert.ToInt32(data["roomieid"]),
                            roomiename = data["roomiename"].ToString(),
                            loginid = Convert.ToInt32(data["loginid"])
                        });
                    }

                    return roomies;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error fetching roomies by login ID", ex);
                }
            }
        }

        // Delete a specific roomie
        public bool DeleteRoomie(int roomieid)
        {
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                try
                {
                    var cmd = dbConn.CreateCommand();
                    cmd.CommandText = "DELETE FROM roomie WHERE roomieid = @roomieid";
                    cmd.Parameters.AddWithValue("@roomieid", NpgsqlDbType.Integer, roomieid);

                    return DeleteData(dbConn, cmd);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error deleting roomie", ex);
                }
            }
        }
    }
}
