using System.ComponentModel.DataAnnotations.Schema;

namespace WeeklyPlanner.Model.Entities
{
    public class PlannerTask
    {
        public int TaskId { get; set; }           // Primary Key
        public string TaskName { get; set; }
        public int AssignedRoomie { get; set; }
        public string Note { get; set; }
        public bool IsCompleted { get; set; }
        public string DayOfWeek { get; set; }
        public int TaskOrder { get; set; }

        // New property to associate with Login
        public int LoginId { get; set; }          // Foreign Key

        // Navigation property for the relationship
        [ForeignKey("LoginId")]
        public virtual Login Login { get; set; }  // Refers to the Login entity

        public PlannerTask() { } // Parameterless constructor for deserialization

        public PlannerTask(int taskId)
        {
            TaskId = taskId;
        }
    }
}