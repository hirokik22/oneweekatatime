using Microsoft.AspNetCore.Mvc;
using WeeklyPlanner.Model.Entities;
using WeeklyPlanner.Model.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WeeklyPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        protected TaskRepository Repository { get; }
        protected RoomieRepository RoomieRepository { get; }

        public TaskController(TaskRepository repository, RoomieRepository roomieRepository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            RoomieRepository = roomieRepository ?? throw new ArgumentNullException(nameof(roomieRepository));
        }

        // GET: api/task/{taskId}
        [Authorize]
        [HttpGet("{taskId}")]
        public ActionResult<object> GetTask([FromRoute] int taskId)
        {
            var task = Repository.GetTaskById(taskId);
            if (task == null)
            {
                return NotFound($"Task with ID {taskId} not found.");
            }

            // Fetch all assigned roomies
            var roomies = Repository.GetRoomiesForTask(taskId);

            // Build the response
            var response = new
            {
                task.TaskId,
                task.TaskName,
                AssignedRoomies = roomies.Select(r => r.roomiename).ToList(),
                task.Note,
                task.IsCompleted,
                task.DayOfWeek,
                task.TaskOrder,
                task.LoginId
            };

            return Ok(response);
        }

        // GET: api/task
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<object>> GetAllTasks()
        {
            var loginId = GetLoginIdFromClaims();
            if (loginId == null)
            {
                return Unauthorized("You are not authenticated.");
            }

            try
            {
                // Fetch tasks only for the authenticated user
                var tasks = Repository.GetTaskByLoginId(loginId.Value);

                // Fetch associated roomies and build response
                var tasksWithRoomies = tasks.Select(task =>
                {
                    var roomies = Repository.GetRoomiesForTask(task.TaskId);

                    // Log fetched roomies for the task
                    Console.WriteLine($"Fetched roomies for TaskId {task.TaskId}: {roomies.Count}");
                    foreach (var roomie in roomies)
                    {
                        Console.WriteLine($"Roomie: {roomie.roomieid}, {roomie.roomiename}");
                    }

                    return new
                    {
                        task.TaskId,
                        task.TaskName,
                        AssignedRoomies = roomies.Select(r => r.roomiename).ToList(),
                        task.Note,
                        task.IsCompleted,
                        task.DayOfWeek,
                        task.TaskOrder,
                        task.LoginId
                    };
                });

                return Ok(tasksWithRoomies);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllTasks for LoginId {loginId}: {ex.Message}");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        // GET: api/task/getRoomiesForTask/{taskId}
        [Authorize]
        [HttpGet("getRoomiesForTask/{taskId}")]
        public ActionResult<IEnumerable<Roomie>> GetRoomiesForTask([FromRoute] int taskId)
        {
            var loginId = GetLoginIdFromClaims();
            if (loginId == null)
            {
                return Unauthorized("You are not authenticated.");
            }

            // Validate that the task belongs to the authenticated user
            var task = Repository.GetTaskById(taskId);
            if (task == null || task.LoginId != loginId)
            {
                return Unauthorized("You are not authorized to view roomies for this task.");
            }

            try
            {
                var roomies = Repository.GetRoomiesForTask(taskId);
                if (roomies == null || !roomies.Any())
                {
                    return NotFound($"No roomies found for Task ID {taskId}.");
                }

                return Ok(roomies);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetRoomiesForTask: {ex.Message}");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        // POST: api/task
        [Authorize]
        [HttpPost]
        public ActionResult CreateTask([FromBody] PlannerTask task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loginId = GetLoginIdFromClaims();
            if (loginId == null)
            {
                return Unauthorized("You are not authenticated.");
            }

            if (task.LoginId != loginId)
            {
                return Unauthorized("You are not authorized to create tasks for this LoginId.");
            }

            // Insert the task and retrieve the generated taskId
            int taskId = Repository.InsertTask(task);
            if (taskId <= 0)
            {
                return BadRequest(new { error = "Failed to create task." });
            }

            // Assign roomies to the task
            var roomieIds = task.Roomies.Select(r => r.roomieid).ToList();
            foreach (var roomieId in roomieIds)
            {
                bool result = Repository.AssignRoomieToTask(taskId, roomieId);
                if (!result)
                {
                    return BadRequest(new { error = $"Task created but failed to assign roomie ID {roomieId}." });
                }
            }

            // Retrieve the created task with assigned roomies
            var createdTask = Repository.GetTaskById(taskId);
            var roomies = Repository.GetRoomiesForTask(taskId);

            return Ok(new
            {
                message = "Task created successfully.",
                task = new
                {
                    createdTask.TaskId,
                    createdTask.TaskName,
                    AssignedRoomies = roomies.Select(r => r.roomiename).ToList(),
                    createdTask.Note,
                    createdTask.IsCompleted,
                    createdTask.DayOfWeek,
                    createdTask.TaskOrder,
                    createdTask.LoginId
                }
            });
        }

        // POST: api/task/addRoomiesToTask/{taskId}
        [Authorize]
        [HttpPost("addRoomiesToTask/{taskId}")]
        public ActionResult AddRoomiesToTask([FromRoute] int taskId, [FromBody] List<int> roomieIds)
        {
            if (roomieIds == null || !roomieIds.Any())
            {
                return BadRequest("No roomie IDs provided.");
            }

            foreach (var roomieId in roomieIds)
            {
                bool result = Repository.AssignRoomieToTask(taskId, roomieId);
                if (!result)
                {
                    return BadRequest($"Failed to assign Roomie ID {roomieId} to Task ID {taskId}.");
                }
            }

            return Ok("Roomies assigned successfully.");
        }

        // PUT: api/task/{taskId}
        [Authorize]
        [HttpPut("{taskId}")]
public ActionResult UpdateTask([FromRoute] int taskId, [FromBody] PlannerTask task)
{
    if (task == null || taskId != task.TaskId)
    {
        return BadRequest(new { error = "Task data is invalid or Task IDs do not match." });
    }

    var loginId = GetLoginIdFromClaims();
    if (loginId == null || task.LoginId != loginId)
    {
        return Unauthorized("You are not authorized to update this task.");
    }

    var existingTask = Repository.GetTaskById(taskId);
    if (existingTask == null)
    {
        return NotFound(new { error = $"Task with ID {taskId} not found." });
    }

    // Preserve existing Roomies if not provided in the update
    List<int> roomieIds;
    if (task.Roomies == null || !task.Roomies.Any())
    {
        roomieIds = Repository.GetRoomiesForTask(taskId).Select(r => r.roomieid).ToList();
    }
    else
    {
        // Use Roomies provided in the update request
        roomieIds = task.Roomies.Select(r => r.roomieid).ToList();
    }

    // Update the task with the provided data
    bool result = Repository.UpdateTask(task, roomieIds);

    if (!result)
    {
        return BadRequest(new { error = "Failed to update task." });
    }

    return Ok(new { message = "Task updated successfully.", updatedTask = task });
}


        // DELETE: api/task/{taskId}
        [Authorize]
        [HttpDelete("{taskId}")]
        public ActionResult DeleteTask([FromRoute] int taskId)
        {
            var loginId = GetLoginIdFromClaims();
            if (loginId == null)
            {
                return Unauthorized("You are not authenticated.");
            }

            var existingTask = Repository.GetTaskById(taskId);
            if (existingTask == null)
            {
                return NotFound($"Task with ID {taskId} not found.");
            }

            if (existingTask.LoginId != loginId)
            {
                return Unauthorized("You are not authorized to delete this task.");
            }

            bool result = Repository.DeleteTask(taskId, loginId.Value);
            if (result)
            {
                return NoContent();
            }

            return BadRequest("Failed to delete task.");
        }

        // DELETE: api/task/removeRoomieFromTask/{taskId}/{roomieId}
        [Authorize]
        [HttpDelete("removeRoomieFromTask/{taskId}/{roomieId}")]
        public ActionResult RemoveRoomieFromTask([FromRoute] int taskId, [FromRoute] int roomieId)
        {
            var loginId = GetLoginIdFromClaims();
            if (loginId == null)
            {
                return Unauthorized("You are not authenticated.");
            }
            var existingTask = Repository.GetTaskById(taskId);
            if (existingTask == null || existingTask.LoginId != loginId)
            {
                return Unauthorized("You are not authorized to modify this task.");
            }

            bool result = Repository.RemoveRoomieFromTask(taskId, roomieId);
            if (!result)
            {
                return BadRequest($"Failed to remove Roomie ID {roomieId} from Task ID {taskId}.");
            }

            return NoContent();
        }

        private int? GetLoginIdFromClaims()
        {
            var loginIdClaim = User.Claims.FirstOrDefault(c => c.Type == "LoginId");
            if (loginIdClaim != null && int.TryParse(loginIdClaim.Value, out int loginId))
            {
                return loginId;
            }
            return null;
        }
    }
}