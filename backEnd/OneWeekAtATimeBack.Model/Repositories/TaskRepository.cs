using System;
using System.Collections.Generic;
using OneWeekAtATimeBack.Model.Entities; // Make sure this using statement is correct
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace OneWeekAtATimeBack.Model.Repositories;

public class TaskRepository : BaseRepository
{
    public TaskRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public ProjectTask GetTaskById(int taskId) // Change Task to ProjectTask
    {
        using (var dbConn = new NpgsqlConnection(ConnectionString))
        {
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "SELECT * FROM tasks WHERE task_id = @taskId";
            cmd.Parameters.Add(new NpgsqlParameter("@taskId", NpgsqlDbType.Integer) { Value = taskId });

            dbConn.Open();
            using (var data = cmd.ExecuteReader())
            {
                if (data.Read())
                {
                    return new ProjectTask(Convert.ToInt32(data["task_id"])) 
                    {
                        TaskName = data["task_name"].ToString(),
                        AssignedRoomie = data.IsDBNull(data.GetOrdinal("assigned_roomie")) ? null : data["assigned_roomie"] as int?,
                        Note = data["note"].ToString(),
                        IsCompleted = Convert.ToBoolean(data["is_completed"]),
                        DayOfWeek = data["day_of_week"].ToString(),
                        TaskOrder = Convert.ToInt32(data["task_order"])
                    };
                }
            }
        }
        return null;
    }


   public List<ProjectTask> GetAllTasks()
{
    List<ProjectTask> tasks = new List<ProjectTask>();
    using (var dbConn = new NpgsqlConnection(ConnectionString))
    {
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = "SELECT * FROM tasks";
        dbConn.Open();
        using (var data = cmd.ExecuteReader())
        {
            while (data.Read())
            {
                tasks.Add(new ProjectTask(Convert.ToInt32(data["task_id"]))
                {
                    TaskName = data["task_name"].ToString(),
                    AssignedRoomie = data.IsDBNull(data.GetOrdinal("assigned_roomie")) ? null : data["assigned_roomie"] as int?,
                    Note = data["note"].ToString(),
                    IsCompleted = Convert.ToBoolean(data["is_completed"]),
                    DayOfWeek = data["day_of_week"].ToString(),
                    TaskOrder = Convert.ToInt32(data["task_order"])
                });
            }
        }
    }
    return tasks;
}

public bool InsertTask(ProjectTask task)
{
    using (var dbConn = new NpgsqlConnection(ConnectionString))
    {
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = "INSERT INTO tasks (task_name, assigned_roomie, note, is_completed, day_of_week, task_order) VALUES (@taskName, @assignedRoomie, @note, @isCompleted, @dayOfWeek, @taskOrder)";
        cmd.Parameters.AddWithValue("@taskName", NpgsqlDbType.Text, task.TaskName);
        cmd.Parameters.AddWithValue("@assignedRoomie", NpgsqlDbType.Integer, task.AssignedRoomie.HasValue ? (object)task.AssignedRoomie.Value : DBNull.Value);
        cmd.Parameters.AddWithValue("@note", NpgsqlDbType.Text, task.Note);
        cmd.Parameters.AddWithValue("@isCompleted", NpgsqlDbType.Boolean, task.IsCompleted);
        cmd.Parameters.AddWithValue("@dayOfWeek", NpgsqlDbType.Text, task.DayOfWeek);
        cmd.Parameters.AddWithValue("@taskOrder", NpgsqlDbType.Integer, task.TaskOrder);

        dbConn.Open();
        return cmd.ExecuteNonQuery() > 0;
    }
}

public bool UpdateTask(ProjectTask task)
{
    using (var dbConn = new NpgsqlConnection(ConnectionString))
    {
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = "UPDATE tasks SET task_name = @taskName, assigned_roomie = @assignedRoomie, note = @note, is_completed = @isCompleted, day_of_week = @dayOfWeek, task_order = @taskOrder WHERE task_id = @taskId";
        cmd.Parameters.AddWithValue("@taskName", NpgsqlDbType.Text, task.TaskName);
        cmd.Parameters.AddWithValue("@assignedRoomie", NpgsqlDbType.Integer, task.AssignedRoomie.HasValue ? (object)task.AssignedRoomie.Value : DBNull.Value);
        cmd.Parameters.AddWithValue("@note", NpgsqlDbType.Text, task.Note);
        cmd.Parameters.AddWithValue("@isCompleted", NpgsqlDbType.Boolean, task.IsCompleted);
        cmd.Parameters.AddWithValue("@dayOfWeek", NpgsqlDbType.Text, task.DayOfWeek);
        cmd.Parameters.AddWithValue("@taskOrder", NpgsqlDbType.Integer, task.TaskOrder);
        cmd.Parameters.AddWithValue("@taskId", NpgsqlDbType.Integer, task.TaskID);

        dbConn.Open();
        return cmd.ExecuteNonQuery() > 0;
    }
}

public bool DeleteTask(int taskId)
{
    using (var dbConn = new NpgsqlConnection(ConnectionString))
    {
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = "DELETE FROM tasks WHERE task_id = @taskId";
        cmd.Parameters.AddWithValue("@taskId", NpgsqlDbType.Integer, taskId);

        dbConn.Open();
        return cmd.ExecuteNonQuery() > 0;
    }
}
}
