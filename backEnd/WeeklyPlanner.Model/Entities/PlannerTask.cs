namespace WeeklyPlanner.Model.Entities
{
    public class PlannerTask
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public int AssignedRoomie { get; set; }
        public string Note { get; set; }
        public bool IsCompleted { get; set; }
        public string DayOfWeek { get; set; }
        public int TaskOrder { get; set; }

        public PlannerTask() { } // Parameterless constructor for deserialization

        public PlannerTask(int taskId)
        {
            TaskId = taskId;
        }
    }
}
