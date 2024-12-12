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
                conn.Open();
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing GetData", ex);
            }
        }

        protected bool InsertData(NpgsqlConnection conn, NpgsqlCommand cmd)
        {
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
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
                conn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
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
                conn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing DeleteData", ex);
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
