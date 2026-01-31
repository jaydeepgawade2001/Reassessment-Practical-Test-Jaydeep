using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReassessmentApp.API.Middleware;
using ReassessmentApp.Application.Interfaces;
using ReassessmentApp.Application.Services;
using ReassessmentApp.Application.Validators;
using ReassessmentApp.Domain.Interfaces;
using ReassessmentApp.Infrastructure.Data;
using ReassessmentApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Repositories (Infrastructure)
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// 3. Services (Application)
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IBookingService, BookingService>();

// 4. Validators (FluentValidation)
builder.Services.AddValidatorsFromAssemblyContaining<CreateRoomDtoValidator>();
builder.Services.AddFluentValidationAutoValidation(); // Req: FluentValidation.AspNetCore

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment()) // Enable for all envs for now
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect Root to Swagger
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger/index.html");
        return;
    }
    await next();
});

// AUTO MIGRATION
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate(); // Applies any pending migrations
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.UseMiddleware<ExceptionHandlingMiddleware>(); // GLOBAL EXCEPTION HANDLER

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
