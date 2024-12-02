using Microsoft.AspNetCore.Mvc;
using WeeklyPlanner.Model.Entities;
using WeeklyPlanner.Model.Repositories;

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
        [HttpGet]
        public ActionResult<IEnumerable<PlannerTask>> GetAllTasks()
        {
            var tasks = Repository.GetTask();
            return Ok(tasks);
        }

        // POST: api/task
        [HttpPost]
        public ActionResult CreateTask([FromBody] PlannerTask task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (task == null)
            {
                return BadRequest("Task data is invalid.");
            }

            bool result = Repository.InsertTask(task);
            if (result)
            {
                return Ok();
            }

            return BadRequest("Failed to create task.");
        }

        // PUT: api/task/{taskId}
        [HttpPut("{taskId}")]
        public ActionResult UpdateTask([FromRoute] int taskId, [FromBody] PlannerTask task)
        {
            if (task == null || taskId != task.TaskId)
            {
                return BadRequest("Task data is invalid or TaskIds do not match.");
            }

            var existingTask = Repository.GetTaskById(taskId);
            if (existingTask == null)
            {
                return NotFound($"Task with ID {taskId} not found.");
            }

            bool result = Repository.UpdateTask(task);
            if (result)
            {
                return Ok();
            }

            return BadRequest("Failed to update task.");
        }

        // DELETE: api/task/{taskId}
        [HttpDelete("{taskId}")]
        public ActionResult DeleteTask([FromRoute] int taskId)
        {
            var existingTask = Repository.GetTaskById(taskId);
            if (existingTask == null)
            {
                return NotFound($"Task with ID {taskId} not found.");
            }

            bool result = Repository.DeleteTask(taskId);
            if (result)
            {
                return NoContent();
            }

            return BadRequest("Failed to delete task.");
        }
    }
}
