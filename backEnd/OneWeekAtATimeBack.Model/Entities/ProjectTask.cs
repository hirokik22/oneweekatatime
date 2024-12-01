using System;
using System.Data.Common;
using System.Text.Json.Serialization;

namespace OneWeekAtATimeBack.Model.Entities;

public class ProjectTask
{
    public ProjectTask(int taskId)
    {
        TaskID = taskId;
    }
    
    public int TaskID { get; set; } // Maps to TaskID in the database
    public string TaskName { get; set; } // Maps to TaskName
    public int? AssignedRoomie { get; set; } // Changed from string to number | null
    public string Note { get; set; } // Task details or notes
    public bool IsCompleted { get; set; } // Task completion status
    public string DayOfWeek { get; set; } // Day the task is scheduled for
    public int TaskOrder { get; set; } // Order of the task within the day
}