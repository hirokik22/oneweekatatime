


/*using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using WeeklyPlanner.API.Controllers;
using WeeklyPlanner.Model.Entities;
using WeeklyPlanner.Model.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WeeklyPlanner.Tests.Controllers
{
    [TestClass]
    public class TaskControllerTests
    {
        private Mock<TaskRepository> _mockTaskRepository;
        private Mock<RoomieRepository> _mockRoomieRepository;
        private TaskController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            // Mock dependencies
            _mockTaskRepository = new Mock<TaskRepository>();
            _mockRoomieRepository = new Mock<RoomieRepository>();

            // Inject mocks into the controller
            _controller = new TaskController(_mockTaskRepository.Object, _mockRoomieRepository.Object);
        }

        // Helper to set user claims
        private void SetUserClaims(TaskController controller, int loginId)
        {
            var claims = new List<Claim>
            {
                new Claim("LoginId", loginId.ToString())
            };
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [TestMethod]
        public void GetTask_ReturnsTask_WhenTaskExists()
        {
            // Arrange
            int taskId = 1;
            var mockTask = new PlannerTask
            {
                TaskId = taskId,
                TaskName = "Test Task",
                Note = "Sample Note",
                DayOfWeek = "Monday",
                IsCompleted = false,
                TaskOrder = 1,
                LoginId = 123
            };

            var mockRoomies = new List<Roomie>
            {
                new Roomie { roomieid = 1, roomiename = "John" },
                new Roomie { roomieid = 2, roomiename = "Doe" }
            };

            _mockTaskRepository.Setup(repo => repo.GetTaskById(taskId)).Returns(mockTask);
            _mockTaskRepository.Setup(repo => repo.GetRoomiesForTask(taskId)).Returns(mockRoomies);

            // Act
            var result = _controller.GetTask(taskId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            dynamic response = okResult.Value;
            Assert.AreEqual(taskId, response.TaskId);
            Assert.AreEqual("Test Task", response.TaskName);
            Assert.AreEqual(2, response.AssignedRoomies.Count);
        }

        [TestMethod]
        public void GetTask_ReturnsNotFound_WhenTaskDoesNotExist()
        {
            // Arrange
            int taskId = 1;
            _mockTaskRepository.Setup(repo => repo.GetTaskById(taskId)).Returns((PlannerTask)null);

            // Act
            var result = _controller.GetTask(taskId);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual($"Task with ID {taskId} not found.", notFoundResult.Value);
        }

        [TestMethod]
        public void GetAllTasks_ReturnsTasksForAuthenticatedUser()
        {
            // Arrange
            int loginId = 123;
            SetUserClaims(_controller, loginId);

            var mockTasks = new List<PlannerTask>
            {
                new PlannerTask { TaskId = 1, TaskName = "Task 1", LoginId = loginId },
                new PlannerTask { TaskId = 2, TaskName = "Task 2", LoginId = loginId }
            };

            var mockRoomies = new List<Roomie>
            {
                new Roomie { roomieid = 1, roomiename = "John" },
                new Roomie { roomieid = 2, roomiename = "Doe" }
            };

            _mockTaskRepository.Setup(repo => repo.GetTaskByLoginId(loginId)).Returns(mockTasks);
            _mockTaskRepository.Setup(repo => repo.GetRoomiesForTask(It.IsAny<int>())).Returns(mockRoomies);

            // Act
            var result = _controller.GetAllTasks();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var tasks = okResult.Value as IEnumerable<dynamic>;
            Assert.AreEqual(2, tasks.Count());
            Assert.AreEqual("Task 1", tasks.First().TaskName);
            Assert.AreEqual(2, tasks.First().AssignedRoomies.Count);
        }

        [TestMethod]
        public void CreateTask_ReturnsCreatedTask_WhenTaskIsValid()
        {
            // Arrange
            int loginId = 123;
            SetUserClaims(_controller, loginId);

            var newTask = new PlannerTask
            {
                TaskId = 1,
                TaskName = "New Task",
                LoginId = loginId,
                Roomies = new List<Roomie>
                {
                    new Roomie { roomieid = 1, roomiename = "John" },
                    new Roomie { roomieid = 2, roomiename = "Doe" }
                }
            };

            _mockTaskRepository.Setup(repo => repo.InsertTask(newTask)).Returns(1);
            _mockTaskRepository.Setup(repo => repo.GetTaskById(It.IsAny<int>())).Returns(newTask);
            _mockTaskRepository.Setup(repo => repo.AssignRoomieToTask(It.IsAny<int>(), It.IsAny<int>())).Returns(true);

            // Act
            var result = _controller.CreateTask(newTask);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            dynamic response = okResult.Value;
            Assert.AreEqual("Task created successfully.", response.message);
            Assert.AreEqual("New Task", response.task.TaskName);
        }

        [TestMethod]
        public void DeleteTask_ReturnsNoContent_WhenTaskIsDeleted()
        {
            // Arrange
            int loginId = 123;
            int taskId = 1;
            SetUserClaims(_controller, loginId);

            var existingTask = new PlannerTask
            {
                TaskId = taskId,
                TaskName = "Existing Task",
                LoginId = loginId
            };

            _mockTaskRepository.Setup(repo => repo.GetTaskById(taskId)).Returns(existingTask);
            _mockTaskRepository.Setup(repo => repo.DeleteTask(taskId, loginId)).Returns(true);

            // Act
            var result = _controller.DeleteTask(taskId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}




































using Moq;
using NUnit.Framework;
using WeeklyPlanner.API.Controllers;
using WeeklyPlanner.Model.Entities;
using WeeklyPlanner.Model.Repositories;
using System.Collections.Generic;

namespace OneWeekAtATime.Tests.Controllers
{
    [TestFixture]
    public class TaskControllerTests
    {
        private Mock<ITaskRepository> _taskRepositoryMock;
        private Mock<IRoomieRepository> _roomieRepositoryMock;
        private TaskController _controller;

        [SetUp]
        public void Setup()
        {
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _roomieRepositoryMock = new Mock<IRoomieRepository>();
            _controller = new TaskController(_taskRepositoryMock.Object, _roomieRepositoryMock.Object);
        }

        [Test]
        public void GetTasks_ShouldReturnTasks()
        {
            // Arrange
            var mockTasks = new List<PlannerTask>
            {
                new PlannerTask { TaskId = 1, TaskName = "Test Task 1" },
                new PlannerTask { TaskId = 2, TaskName = "Test Task 2" }
            };
            _taskRepositoryMock.Setup(repo => repo.GetTasksByLoginId(It.IsAny<int>()))
                .Returns(mockTasks);

            // Act
            var result = _controller.GetTasks(123);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Test Task 1", result.First().TaskName);
        }

        [Test]
        public void AddTask_ShouldInvokeRepository()
        {
            // Arrange
            var task = new PlannerTask { TaskId = 1, TaskName = "New Task" };

            // Act
            _controller.AddTask(task);

            // Assert
            _taskRepositoryMock.Verify(repo => repo.AddTask(task), Times.Once);
        }
    }
}
*/