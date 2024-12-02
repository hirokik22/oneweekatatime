using System;
using System.Collections.Generic;
using WeeklyPlanner.Model.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace WeeklyPlanner.Model.Repositories
{
    public class TaskRepository : BaseRepository
    {
        public TaskRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public PlannerTask GetTaskById(int taskId)
        {
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                try
                {
                    var cmd = dbConn.CreateCommand();
                    cmd.CommandText = "SELECT * FROM task WHERE taskid = @taskId";
                    cmd.Parameters.AddWithValue("@taskId", NpgsqlDbType.Integer, taskId);

                    var data = GetData(dbConn, cmd);

                    if (data != null && data.Read())
                    {
                        return new PlannerTask(Convert.ToInt32(data["taskid"]))
                        {
                            TaskName = data["taskname"].ToString(),
                            AssignedRoomie = Convert.ToInt32(data["assignedroomie"]),
                            Note = data["note"].ToString(),
                            IsCompleted = Convert.ToBoolean(data["iscompleted"]),
                            DayOfWeek = data["dayofweek"].ToString(),
                            TaskOrder = Convert.ToInt32(data["taskorder"])
                        };
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error fetching task by ID", ex);
                }
            }
        }

        public List<PlannerTask> GetTask()
        {
            var task = new List<PlannerTask>();
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                try
                {
                    var cmd = dbConn.CreateCommand();
                    cmd.CommandText = "SELECT * FROM task";

                    var data = GetData(dbConn, cmd);

                    while (data != null && data.Read())
                    {
                        task.Add(new PlannerTask(Convert.ToInt32(data["taskid"]))
                        {
                            TaskName = data["taskname"].ToString(),
                            AssignedRoomie = Convert.ToInt32(data["assignedroomie"]),
                            Note = data["note"].ToString(),
                            IsCompleted = Convert.ToBoolean(data["iscompleted"]),
                            DayOfWeek = data["dayofweek"].ToString(),
                            TaskOrder = Convert.ToInt32(data["taskorder"])
                        });
                    }

                    return task;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error fetching task", ex);
                }
            }
        }

        public bool InsertTask(PlannerTask task)
        {
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                try
                {
                    var cmd = dbConn.CreateCommand();
                    cmd.CommandText = @"INSERT INTO task (taskname, assignedroomie, note, iscompleted, dayofweek, taskorder)
                                        VALUES (@taskName, @assignedRoomie, @note, @isCompleted, @dayOfWeek, @taskOrder)";

                    cmd.Parameters.AddWithValue("@taskName", NpgsqlDbType.Text, task.TaskName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@assignedRoomie", NpgsqlDbType.Integer, task.AssignedRoomie);
                    cmd.Parameters.AddWithValue("@note", NpgsqlDbType.Text, task.Note ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@isCompleted", NpgsqlDbType.Boolean, task.IsCompleted);
                    cmd.Parameters.AddWithValue("@dayOfWeek", NpgsqlDbType.Text, task.DayOfWeek ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@taskOrder", NpgsqlDbType.Integer, task.TaskOrder);

                    return InsertData(dbConn, cmd);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error inserting task", ex);
                }
            }
        }

        public bool UpdateTask(PlannerTask task)
        {
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                try
                {
                    var cmd = dbConn.CreateCommand();
                    cmd.CommandText = @"UPDATE task SET 
                                        taskname = @taskName,
                                        assignedroomie = @assignedRoomie,
                                        note = @note,
                                        iscompleted = @isCompleted,
                                        dayofweek = @dayOfWeek,
                                        taskorder = @taskOrder
                                    WHERE taskid = @taskId";

                    cmd.Parameters.AddWithValue("@taskName", NpgsqlDbType.Text, task.TaskName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@assignedRoomie", NpgsqlDbType.Integer, task.AssignedRoomie);
                    cmd.Parameters.AddWithValue("@note", NpgsqlDbType.Text, task.Note ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@isCompleted", NpgsqlDbType.Boolean, task.IsCompleted);
                    cmd.Parameters.AddWithValue("@dayOfWeek", NpgsqlDbType.Text, task.DayOfWeek ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@taskOrder", NpgsqlDbType.Integer, task.TaskOrder);
                    cmd.Parameters.AddWithValue("@taskId", NpgsqlDbType.Integer, task.TaskId);

                    return UpdateData(dbConn, cmd);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error updating task", ex);
                }
            }
        }

        public bool DeleteTask(int taskId)
        {
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                try
                {
                    var cmd = dbConn.CreateCommand();
                    cmd.CommandText = "DELETE FROM task WHERE taskid = @taskId";
                    cmd.Parameters.AddWithValue("@taskId", NpgsqlDbType.Integer, taskId);

                    return DeleteData(dbConn, cmd);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error deleting task", ex);
                }
            }
        }
    }
}