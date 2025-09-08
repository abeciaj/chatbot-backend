using ChatSupport.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using ChatSupport.Hubs;
using ChatSupport.Models;
using Npgsql;


var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        x.JsonSerializerOptions.WriteIndented = true; // Optional
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // Your Vue dev server URL
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // If you need to send cookies/auth headers
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    
    
// }
app.MapOpenApi();
app.MapScalarApiReference();


app.UseCors(MyAllowSpecificOrigins);
app.MapHub<ChatHub>("/chatHub");
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
