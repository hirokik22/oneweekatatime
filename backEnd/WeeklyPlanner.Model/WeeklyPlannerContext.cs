using Microsoft.EntityFrameworkCore;
using WeeklyPlanner.Model.Entities;

namespace WeeklyPlanner.Model
{
    public class WeeklyPlannerContext : DbContext
    {
        // Constructor
        public WeeklyPlannerContext(DbContextOptions<WeeklyPlannerContext> options)
            : base(options) { }
 
        // DbSet properties for entities
        public DbSet<PlannerTask> PlannerTasks { get; set; }
        public DbSet<Roomie> Roomies { get; set; }
        public DbSet<Login> Logins { get; set; } 

   protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Many-to-Many Relationship Configuration between PlannerTask and Roomies
    modelBuilder.Entity<PlannerTask>()
        .HasMany(pt => pt.Roomies)
        .WithMany(r => r.Tasks)
        .UsingEntity<Dictionary<string, object>>(
            "TaskRoomie",
            tr => tr.HasOne<Roomie>()
                    .WithMany()
                    .HasForeignKey("RoomieId")
                    .OnDelete(DeleteBehavior.Cascade),
            tr => tr.HasOne<PlannerTask>()
                    .WithMany()
                    .HasForeignKey("TaskId")
                    .OnDelete(DeleteBehavior.Cascade)
        );
}

        // Factory Method for Testing
        public static WeeklyPlannerContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<WeeklyPlannerContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            return new WeeklyPlannerContext(options);
        }
    }
}
