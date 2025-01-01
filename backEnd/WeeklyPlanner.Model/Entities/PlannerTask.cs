using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeeklyPlanner.Model.Entities
{
    public class PlannerTask
    {
        [Key]
        public int TaskId { get; set; } // Primary Key

        [Required(ErrorMessage = "Task Name is required.")]
        [StringLength(100, ErrorMessage = "Task Name cannot exceed 100 characters.")]
        public string TaskName { get; set; }

        public int? AssignedRoomie { get; set; }

        [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters.")]
        public string Note { get; set; }

        public bool IsCompleted { get; set; }

        [Required(ErrorMessage = "Day of the Week is required.")]
        [RegularExpression(@"^(Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Sunday)$", 
            ErrorMessage = "Day of the Week must be a valid day.")]
        public string DayOfWeek { get; set; }

        public int TaskOrder { get; set; }

        // New property to associate with Login
        [Required(ErrorMessage = "LoginId is required.")]
        public int LoginId { get; set; } // Foreign Key

        // Navigation property for the relationship
        [ForeignKey("LoginId")]
        public virtual Login Login { get; set; } // Refers to the Login entity

        public PlannerTask() { } // Parameterless constructor for deserialization

        public PlannerTask(int taskId)
        {
            TaskId = taskId;
        }
    }
}