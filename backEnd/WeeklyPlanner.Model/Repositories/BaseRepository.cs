using Npgsql;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace WeeklyPlanner.Model.Repositories
{
    public class BaseRepository
    {
        protected string ConnectionString { get; }

        public BaseRepository(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("AppProgDb");
            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new Exception("Connection string 'AppProgDb' is not configured.");
            }
        }

        protected NpgsqlDataReader GetData(NpgsqlConnection conn, NpgsqlCommand cmd)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open(); // Ensure connection is open only once
                }

                Console.WriteLine("Executing Query: " + cmd.CommandText);
                foreach (NpgsqlParameter parameter in cmd.Parameters)
                {
                    Console.WriteLine($"Parameter: {parameter.ParameterName} = {parameter.Value}");
                }

                return cmd.ExecuteReader(CommandBehavior.CloseConnection); // Close connection after data is read
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetData: {ex.Message}");
                throw new Exception("Error executing GetData", ex);
            }
        }

        protected bool InsertData(NpgsqlConnection conn, NpgsqlCommand cmd)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                Console.WriteLine("Executing Insert Query: " + cmd.CommandText);
                foreach (NpgsqlParameter parameter in cmd.Parameters)
                {
                    Console.WriteLine($"Parameter: {parameter.ParameterName} = {parameter.Value}");
                }

                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in InsertData: {ex.Message}");
                throw new Exception("Error executing InsertData", ex);
            }
            finally
            {
                conn.Close();
            }
        }

        protected bool UpdateData(NpgsqlConnection conn, NpgsqlCommand cmd)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                Console.WriteLine("Executing Update Query: " + cmd.CommandText);
                foreach (NpgsqlParameter parameter in cmd.Parameters)
                {
                    Console.WriteLine($"Parameter: {parameter.ParameterName} = {parameter.Value}");
                }

                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateData: {ex.Message}");
                throw new Exception("Error executing UpdateData", ex);
            }
            finally
            {
                conn.Close();
            }
        }

        protected bool DeleteData(NpgsqlConnection conn, NpgsqlCommand cmd)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                Console.WriteLine("Executing Delete Query: " + cmd.CommandText);
                foreach (NpgsqlParameter parameter in cmd.Parameters)
                {
                    Console.WriteLine($"Parameter: {parameter.ParameterName} = {parameter.Value}");
                }

                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteData: {ex.Message}");
                throw new Exception("Error executing DeleteData", ex);
            }
            finally
            {
                conn.Close();
            }
        }
    }
}