using System;
using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace weeklyPlanner.Model.Repositories
{
    public abstract class BaseRepository
    {
        protected string ConnectionString { get; }

        protected BaseRepository(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        protected IDataReader GetData(NpgsqlConnection connection, NpgsqlCommand command)
        {
            try
            {
                connection.Open();
                command.Connection = connection;
                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing GetData", ex);
            }
        }

        protected bool InsertData(NpgsqlConnection connection, NpgsqlCommand command)
        {
            try
            {
                connection.Open();
                command.Connection = connection;
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing InsertData", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        protected bool UpdateData(NpgsqlConnection connection, NpgsqlCommand command)
        {
            try
            {
                connection.Open();
                command.Connection = connection;
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing UpdateData", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        protected bool DeleteData(NpgsqlConnection connection, NpgsqlCommand command)
        {
            try
            {
                connection.Open();
                command.Connection = connection;
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing DeleteData", ex);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
