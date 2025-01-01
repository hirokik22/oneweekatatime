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
                            Note = data["note"].ToString(),
                            IsCompleted = Convert.ToBoolean(data["iscompleted"]),
                            DayOfWeek = data["dayofweek"].ToString(),
                            TaskOrder = Convert.ToInt32(data["taskorder"]),
                            LoginId = Convert.ToInt32(data["loginid"]) // Add this line
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

        public List<PlannerTask> GetTaskByLoginId(int loginId)
        {
            var tasks = new List<PlannerTask>();

            // Use a using statement to ensure the connection is properly disposed
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                try
                {
                    dbConn.Open(); // Open the connection
                    using (var cmd = dbConn.CreateCommand())
                    {
                        // Prepare the SQL query
                        cmd.CommandText = "SELECT * FROM task WHERE loginid = @LoginId";
                        cmd.Parameters.AddWithValue("@LoginId", NpgsqlDbType.Integer, loginId);

                        // Execute the query and fetch data
                        using (var data = cmd.ExecuteReader())
                        {
                            while (data != null && data.Read())
                            {
                                tasks.Add(new PlannerTask(Convert.ToInt32(data["taskid"]))
                                {
                                    TaskName = data["taskname"].ToString(),
                                    Note = data["note"].ToString(),
                                    IsCompleted = Convert.ToBoolean(data["iscompleted"]),
                                    DayOfWeek = data["dayofweek"].ToString(),
                                    TaskOrder = Convert.ToInt32(data["taskorder"]),
                                    LoginId = Convert.ToInt32(data["loginid"]) // Include the LoginId field
                                });
                            }
                        }
                    }

                    return tasks; // Return the task list
                }
                catch (Exception ex)
                {
                    throw new Exception("Error fetching tasks by LoginId", ex);
                }
                finally
                {
                    // Ensure the connection is closed even if an exception occurs
                    if (dbConn.State == System.Data.ConnectionState.Open)
                    {
                        dbConn.Close();
                    }
                }
            }
        }



        public List<Roomie> GetRoomiesForTask(int taskId)
        {
            var roomies = new List<Roomie>();
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                try
                {
                    var cmd = dbConn.CreateCommand();
                    cmd.CommandText = @"SELECT r.roomieid, r.roomiename 
                                        FROM taskroomies tr 
                                        INNER JOIN roomie r ON tr.roomieid = r.roomieid 
                                        WHERE tr.taskid = @taskId";
                    cmd.Parameters.AddWithValue("@taskId", NpgsqlDbType.Integer, taskId);

                    var data = GetData(dbConn, cmd);

                    while (data != null && data.Read())
                    {
                        roomies.Add(new Roomie
                        {
                            roomieid = Convert.ToInt32(data["roomieid"]),
                            roomiename = data["roomiename"].ToString()
                        });
                    }

                    return roomies;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error fetching roomies for task", ex);
                }
            }
        }

        public bool AssignRoomieToTask(int taskId, int roomieId)
        {
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                try
                {
                    // Validate taskId exists
                    var validateTaskCmd = dbConn.CreateCommand();
                    validateTaskCmd.CommandText = "SELECT COUNT(*) FROM task WHERE taskid = @taskId";
                    validateTaskCmd.Parameters.AddWithValue("@taskId", NpgsqlDbType.Integer, taskId);

                    dbConn.Open();
                    var taskExists = Convert.ToInt32(validateTaskCmd.ExecuteScalar()) > 0;
                    dbConn.Close();

                    if (!taskExists)
                        throw new Exception($"Task ID {taskId} does not exist.");

                    // Validate roomieId exists
                    var validateRoomieCmd = dbConn.CreateCommand();
                    validateRoomieCmd.CommandText = "SELECT COUNT(*) FROM roomie WHERE roomieid = @roomieId";
                    validateRoomieCmd.Parameters.AddWithValue("@roomieId", NpgsqlDbType.Integer, roomieId);

                    dbConn.Open();
                    var roomieExists = Convert.ToInt32(validateRoomieCmd.ExecuteScalar()) > 0;
                    dbConn.Close();

                    if (!roomieExists)
                        throw new Exception($"Roomie ID {roomieId} does not exist.");

                    // Perform insert into taskroomies
                    var cmd = dbConn.CreateCommand();
                    cmd.CommandText = @"INSERT INTO taskroomies (taskid, roomieid)
                                        VALUES (@taskId, @roomieId)";
                    cmd.Parameters.AddWithValue("@taskId", NpgsqlDbType.Integer, taskId);
                    cmd.Parameters.AddWithValue("@roomieId", NpgsqlDbType.Integer, roomieId);

                    return InsertData(dbConn, cmd);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error assigning roomie to task", ex);
                }
            }
        }


        public bool RemoveRoomieFromTask(int taskId, int roomieId)
        {
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                try
                {
                    var cmd = dbConn.CreateCommand();
                    cmd.CommandText = @"DELETE FROM taskroomies 
                                        WHERE taskid = @taskId AND roomieid = @roomieId";
                    cmd.Parameters.AddWithValue("@taskId", NpgsqlDbType.Integer, taskId);
                    cmd.Parameters.AddWithValue("@roomieId", NpgsqlDbType.Integer, roomieId);

                    return DeleteData(dbConn, cmd);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error removing roomie from task", ex);
                }
            }
        }


        public int InsertTask(PlannerTask task)
        {
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                try
                {
                    var cmd = dbConn.CreateCommand();
                    cmd.CommandText = @"INSERT INTO task (taskname, note, iscompleted, dayofweek, taskorder, loginid)
                                        VALUES (@taskName, @note, @isCompleted, @dayOfWeek, @taskOrder, @loginId)
                                        RETURNING taskid";

                    cmd.Parameters.AddWithValue("@taskName", NpgsqlDbType.Text, task.TaskName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@note", NpgsqlDbType.Text, task.Note ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@isCompleted", NpgsqlDbType.Boolean, task.IsCompleted);
                    cmd.Parameters.AddWithValue("@dayOfWeek", NpgsqlDbType.Text, task.DayOfWeek ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@taskOrder", NpgsqlDbType.Integer, task.TaskOrder);
                    cmd.Parameters.AddWithValue("@loginId", NpgsqlDbType.Integer, task.LoginId);

                    dbConn.Open();
                    var taskId = cmd.ExecuteScalar();
                    return Convert.ToInt32(taskId); // Return the generated task ID
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
                                        note = @note,
                                        iscompleted = @isCompleted,
                                        dayofweek = @dayOfWeek,
                                        taskorder = @taskOrder,
                                        loginid = @loginId
                                    WHERE taskid = @taskId";

                    cmd.Parameters.AddWithValue("@taskName", NpgsqlDbType.Text, task.TaskName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@note", NpgsqlDbType.Text, task.Note ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@isCompleted", NpgsqlDbType.Boolean, task.IsCompleted);
                    cmd.Parameters.AddWithValue("@dayOfWeek", NpgsqlDbType.Text, task.DayOfWeek ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@taskOrder", NpgsqlDbType.Integer, task.TaskOrder);
                    cmd.Parameters.AddWithValue("@loginId", NpgsqlDbType.Integer, task.LoginId);
                    cmd.Parameters.AddWithValue("@taskId", NpgsqlDbType.Integer, task.TaskId);

                    dbConn.Open();
                    var rowsAffected = cmd.ExecuteNonQuery();

                    // Handle roomie assignment
                    if (task.AssignedRoomie.HasValue)
                    {
                        var roomieCmd = dbConn.CreateCommand();
                        roomieCmd.CommandText = @"DELETE FROM taskroomies WHERE taskid = @taskId;
                                                INSERT INTO taskroomies (taskid, roomieid)
                                                VALUES (@taskId, @roomieId)";
                        roomieCmd.Parameters.AddWithValue("@taskId", NpgsqlDbType.Integer, task.TaskId);
                        roomieCmd.Parameters.AddWithValue("@roomieId", NpgsqlDbType.Integer, task.AssignedRoomie.Value);
                        roomieCmd.ExecuteNonQuery();
                    }

                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error updating task", ex);
                }
            }
        }

        public bool DeleteTask(int taskId, int loginId)
        {
            using (var dbConn = new NpgsqlConnection(ConnectionString))
            {
                try
                {
                    var cmd = dbConn.CreateCommand();
                    cmd.CommandText = "DELETE FROM task WHERE taskid = @taskId AND loginid = @loginId";
                    cmd.Parameters.AddWithValue("@taskId", NpgsqlDbType.Integer, taskId);
                    cmd.Parameters.AddWithValue("@loginId", NpgsqlDbType.Integer, loginId);

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