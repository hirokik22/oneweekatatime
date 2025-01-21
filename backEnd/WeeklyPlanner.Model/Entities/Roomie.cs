namespace WeeklyPlanner.Model.Entities
{
    public class Roomie
    {
        public int roomieid { get; set; } // Matches the DB column

        public string roomiename { get; set; } // Matches the DB column

        public int loginid { get; set; } // Matches the DB column

// Property for the many-to-many relationship with PlannerTask
        public virtual ICollection<PlannerTask> Tasks { get; set; }

        public Roomie()
        {
            Tasks = new List<PlannerTask>();
        }
    }
}