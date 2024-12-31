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

        public TaskController(TaskRepository repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        // GET: api/task/{taskId}
        [Authorize]
        [HttpGet("{taskId}")]
        public ActionResult<PlannerTask> GetTask([FromRoute] int taskId)
        {
            var task = Repository.GetTaskById(taskId);
            if (task == null)
            {
                return NotFound($"Task with ID {taskId} not found.");
            }

            return Ok(task);
        }

        // GET: api/task
        // GET: api/task?loginId=1
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<PlannerTask>> GetAllTasks([FromQuery] int? loginId)
        {
            IEnumerable<PlannerTask> tasks;

            if (loginId.HasValue)
            {
                // Fetch tasks specific to the login ID
                tasks = Repository.GetTaskByLoginId(loginId.Value);
            }
            else
            {
                // Fetch all tasks
                tasks = Repository.GetTask();
            }

            return Ok(tasks);
        }


        // GET: api/task/getRoomiesForTask/{taskId}
        [Authorize]
        [HttpGet("getRoomiesForTask/{taskId}")]
        public ActionResult<IEnumerable<Roomie>> GetRoomiesForTask([FromRoute] int taskId)
        {
            var roomies = Repository.GetRoomiesForTask(taskId);
            if (roomies == null || !roomies.Any())
            {
                return NotFound($"No roomies found for Task ID {taskId}.");
            }

            return Ok(roomies);
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

            bool result = Repository.InsertTask(task);
            if (result)
            {
                return Ok(new { message = "Task created successfully.", task });
            }

            return BadRequest(new { error = "Failed to create task." });
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
                return BadRequest(new { error = "Task data is invalid or Task IDs do not match.", taskId, providedTaskId = task?.TaskId });
            }

            var loginId = GetLoginIdFromClaims();
            if (loginId == null)
            {
                return Unauthorized(new { error = "You are not authenticated.", claimType = "LoginId" });
            }

            var existingTask = Repository.GetTaskById(taskId);
            if (existingTask == null)
            {
                return NotFound(new { error = $"Task with ID {taskId} not found." });
            }

            if (existingTask.LoginId != loginId)
            {
                return Unauthorized(new { error = "You are not authorized to perform this action.", existingTaskLoginId = existingTask.LoginId, currentLoginId = loginId });
            }

            try
            {
                bool result = Repository.UpdateTask(task);
                if (result)
                {
                    return Ok(new { message = "Task updated successfully.", updatedTask = task });
                }
                else
                {
                    return BadRequest(new { error = "Failed to update task.", taskId });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating the task.", exceptionMessage = ex.Message, taskId });
            }
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

        // helper method
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