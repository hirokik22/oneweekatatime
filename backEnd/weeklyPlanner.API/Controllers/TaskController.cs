using Microsoft.AspNetCore.Mvc;
using weeklyPlanner.Model.Entities;
using CourseAdminSystem.Model.Repositories;

namespace weeklyPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        protected TaskRepository Repository {get;}

        public TaskController(TaskRepository repository)
        {
            Repository = repository;
        }

        // GET: api/task/{taskId}
        [HttpGet("{taskId}")]
        public ActionResult<Tasks> GetTask([FromRoute] int taskId)
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
        public ActionResult<IEnumerable<Tasks>> GetAllTasks()
        {
            var tasks = Repository.GetTasks();
            return Ok(tasks);
        }

        // POST: api/task
        [HttpPost]
        public ActionResult CreateTask([FromBody] Tasks task)
        {
            // Validate the model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the task object is null
            if (task == null)
            {
                return BadRequest("Task data is invalid.");
            }

            // Attempt to insert the task
            bool result = Repository.InsertTask(task);

            if (result)
            {
                return Ok();
            }

            return BadRequest("Failed to create task.");
        }


        // PUT: api/task/{taskId}
        [HttpPut("{taskId}")]
        public ActionResult UpdateTask([FromRoute] int taskId, [FromBody] Tasks task)
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
