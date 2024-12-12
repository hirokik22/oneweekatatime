using WeeklyPlanner.Model.Repositories;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "WeeklyPlanner.API",
        Version = "v1"
    });
});

// Register repositories
builder.Services.AddScoped<TaskRepository>();
builder.Services.AddScoped<RoomieRepository>();
builder.Services.AddScoped<LoginRepository>();

// Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowAnyOrigin();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ensure Swagger bypasses authorization
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/swagger"))
    {
        context.User = new System.Security.Claims.ClaimsPrincipal();
    }

    await next();
});

// Enable CORS
app.UseCors("AllowAll");

// Temporarily disable authentication and authorization
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();

app.Run();