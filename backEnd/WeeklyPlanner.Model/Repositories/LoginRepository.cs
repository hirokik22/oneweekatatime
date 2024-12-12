using System;
using System.Collections.Generic;
using WeeklyPlanner.Model.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;



namespace WeeklyPlanner.Model.Repositories
{
    public class LoginRepository : BaseRepository
    {
        public LoginRepository(IConfiguration configuration) : base(configuration) { }

        public Login GetLoginByUsername(string email)
        {
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                try
                {
                    var cmd = dbConn.CreateCommand();
                    cmd.CommandText = "SELECT * FROM login WHERE email = @Email"; // Correct property name
                    cmd.Parameters.AddWithValue("@Email", NpgsqlDbType.Varchar, email);

                    var data = GetData(dbConn, cmd);

                    if (data != null && data.Read())
                    {
                        return new Login
                        {
                            LoginId = Convert.ToInt32(data["loginid"]),
                            Email = data["email"].ToString(), // Correct property name
                            PasswordHash = data["passwordhash"].ToString() // Correct property name
                        };
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error fetching login by username", ex);
                }
            }
        }

        // Retrieve all logins
        public List<Login> GetAllLogins()
        {
            var logins = new List<Login>();
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                try
                {
                    var cmd = dbConn.CreateCommand();
                    cmd.CommandText = "SELECT * FROM login";

                    var data = GetData(dbConn, cmd);

                    while (data != null && data.Read())
                    {
                        logins.Add(new Login
                        {
                            LoginId = Convert.ToInt32(data["loginid"]),
                            Email = data["email"].ToString(),
                            PasswordHash = data["passwordhash"].ToString()
                        });
                    }

                    return logins;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error fetching all logins", ex);
                }
            }
        }

        // Retrieve a login by ID
        public Login GetLoginById(int loginId)
        {
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                try
                {
                    var cmd = dbConn.CreateCommand();
                    cmd.CommandText = "SELECT * FROM login WHERE loginid = @loginid";
                    cmd.Parameters.AddWithValue("@loginid", NpgsqlDbType.Integer, loginId);

                    var data = GetData(dbConn, cmd);

                    if (data != null && data.Read())
                    {
                        return new Login
                        {
                            LoginId = Convert.ToInt32(data["loginid"]),
                            Email = data["email"].ToString(),
                            PasswordHash = data["passwordhash"].ToString()
                        };
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error fetching login by ID", ex);
                }
            }
        }

        // Add a new login
        public bool CreateLogin(Login login)
        {
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                try
                {
                    var cmd = dbConn.CreateCommand();
                    cmd.CommandText = @"INSERT INTO login (email, passwordhash)
                                        VALUES (@Email, @PasswordHash)";
                    cmd.Parameters.AddWithValue("@Email", NpgsqlDbType.Varchar, login.Email);
                    cmd.Parameters.AddWithValue("@PasswordHash", NpgsqlDbType.Varchar, login.PasswordHash);

                    return InsertData(dbConn, cmd);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error creating login", ex);
                }
            }
        }

        // Update an existing login
        public bool UpdateLogin(Login login)
        {
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                try
                {
                    var cmd = dbConn.CreateCommand();
                    cmd.CommandText = @"UPDATE login 
                                        SET email = @Email, 
                                            passwordhash = @PasswordHash 
                                        WHERE loginid = @LoginId";
                    cmd.Parameters.AddWithValue("@Email", NpgsqlDbType.Varchar, login.Email);
                    cmd.Parameters.AddWithValue("@PasswordHash", NpgsqlDbType.Varchar, login.PasswordHash);
                    cmd.Parameters.AddWithValue("@LoginId", NpgsqlDbType.Integer, login.LoginId);

                    return UpdateData(dbConn, cmd);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error updating login", ex);
                }
            }
        }

        // Delete a login
        public bool DeleteLogin(int loginId)
        {
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                try
                {
                    var cmd = dbConn.CreateCommand();
                    cmd.CommandText = "DELETE FROM login WHERE loginid = @loginid";
                    cmd.Parameters.AddWithValue("@loginid", NpgsqlDbType.Integer, loginId);

                    return DeleteData(dbConn, cmd);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error deleting login", ex);
                }
            }
        }
    }
}
