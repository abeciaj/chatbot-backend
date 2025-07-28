using ChatSupport.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using ChatSupport.Hubs;
using ChatSupport.Models;
using Npgsql;


var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR();


//builder.Services.AddDbContext<UserDbContext>(options =>
//    options.UseNpgsql(
//        builder.Configuration.GetConnectionString("DefaultConnection"),
//        npgsqlOptionsAction: npgsqlOptions =>
//        {
//            npgsqlOptions.MapEnum<Role>("Role");
//        }));

//NpgsqlConnection.GlobalTypeMapper.MapEnum<Role>();
//NpgsqlConnection.GlobalTypeMapper.MapEnum<Role>("role");

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.MapEnum<Role>("role")));

// Program.cs
//var dataSourceBuilder = new NpgsqlDataSourceBuilder(
//    builder.Configuration.GetConnectionString("DefaultConnection")
//);
//dataSourceBuilder.MapEnum<Role>();
//var dataSource = dataSourceBuilder.Build();
//builder.Services.AddDbContext<UserDbContext>(options => options.UseNpgsql(dataSource));

//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(policy =>
//    {
//        policy.AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials()
//              .SetIsOriginAllowed(origin => true); // allow all origins
//    });
//});


var dataSourceBuilder = new NpgsqlDataSourceBuilder(
    builder.Configuration.GetConnectionString("DefaultConnection")!
);

dataSourceBuilder.MapEnum<Role>("role");

var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(dataSource)
);



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
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseCors(MyAllowSpecificOrigins);
app.MapHub<ChatHub>("/chatHub");
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
