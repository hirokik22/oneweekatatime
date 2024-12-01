using System;
using System.Collections.Generic;
using OneWeekAtATimeBack.Model.Entities; // Ensure this namespace is correct
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace OneWeekAtATimeBack.Model.Repositories
{
    public class RoomieRepository : BaseRepository
    {
        public RoomieRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public Roomie GetRoomieById(int roomieId)
        {
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT * FROM roomies WHERE roomie_id = @roomieId";
                cmd.Parameters.AddWithValue("@roomieId", NpgsqlDbType.Integer, roomieId);

                dbConn.Open();
                using (var data = cmd.ExecuteReader())
                {
                    if (data.Read())
                    {
                        return new Roomie
                        {
                            RoomieID = Convert.ToInt32(data["roomie_id"]),
                            RoomieName = data["roomiename"].ToString(),
                            LoginID = Convert.ToInt32(data["login_id"])
                        };
                    }
                }
            }
            return null;
        }

        public List<Roomie> GetAllRoomies()
        {
            List<Roomie> roomies = new List<Roomie>();
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT * FROM roomies";
                dbConn.Open();
                using (var data = cmd.ExecuteReader())
                {
                    while (data.Read())
                    {
                        roomies.Add(new Roomie
                        {
                            RoomieID = Convert.ToInt32(data["roomie_id"]),
                            RoomieName = data["roomiename"].ToString(),
                            LoginID = Convert.ToInt32(data["login_id"])
                        });
                    }
                }
            }
            return roomies;
        }

        public bool InsertRoomie(Roomie roomie)
        {
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "INSERT INTO roomies (roomiename, login_id) VALUES (@roomieName, @loginId)";
                cmd.Parameters.AddWithValue("@roomieName", NpgsqlDbType.Text, roomie.RoomieName);
                cmd.Parameters.AddWithValue("@loginId", NpgsqlDbType.Integer, roomie.LoginID);

                dbConn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdateRoomie(Roomie roomie)
        {
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "UPDATE roomies SET roomiename = @roomieName, login_id = @loginId WHERE roomie_id = @roomieId";
                cmd.Parameters.AddWithValue("@roomieName", NpgsqlDbType.Text, roomie.RoomieName);
                cmd.Parameters.AddWithValue("@loginId", NpgsqlDbType.Integer, roomie.LoginID);
                cmd.Parameters.AddWithValue("@roomieId", NpgsqlDbType.Integer, roomie.RoomieID);

                dbConn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteRoomie(int roomieId)
        {
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "DELETE FROM roomies WHERE roomie_id = @roomieId";
                cmd.Parameters.AddWithValue("@roomieId", NpgsqlDbType.Integer, roomieId);

                dbConn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
