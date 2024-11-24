using System;
using Npgsql;
using Microsoft.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;

//This class allow some generic method that you cacn call to get data
namespace OneWeekAtATimeBack.Model.Repositories;
public class BaseRepository
{
protected string ConnectionString {get;}
public BaseRepository(IConfiguration configuration) {
       ConnectionString = configuration.GetConnectionString("AppProgDb");
    }
    protected NpgsqlDataReader GetData(NpgsqlConnection conn, NpgsqlCommand cmd)
       //This allow to read data from database
    {
       conn.Open();

       return cmd.ExecuteReader();
}
    protected bool InsertData(NpgsqlConnection conn, NpgsqlCommand cmd)
           //This allow to insert data from database
    {
       conn.Open();
       cmd.ExecuteNonQuery();
       return true;
}
    protected bool UpdateData(NpgsqlConnection conn, NpgsqlCommand cmd)
    {
       conn.Open();
       cmd.ExecuteNonQuery();
       return true;
}
    protected bool DeleteData(NpgsqlConnection conn, NpgsqlCommand cmd)
    {
       conn.Open();
       cmd.ExecuteNonQuery();
       return true;
}
}
