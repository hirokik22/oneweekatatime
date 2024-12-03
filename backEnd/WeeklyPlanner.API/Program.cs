<<<<<<<< HEAD:backEnd/WeeklyPlanner.API/Program.cs
using WeeklyPlanner.Model.Repositories;
========
using CourseAdminSystem.Model.Repositories;
>>>>>>>> a8a9e355109f31792d240072d6c88e0e7457e1a6:backEnd/weeklyPlanner.API/Program.cs

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<TaskRepository, TaskRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
