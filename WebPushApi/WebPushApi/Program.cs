using Microsoft.EntityFrameworkCore;
using Npgsql;
using WebPushApi.Data;
using WebPushApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
var dataSourceBuilder = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("Postgres"));
var dataSource = dataSourceBuilder.Build();
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(dataSource));

builder.Services.AddScoped<PushNotificationService>();
builder.Services.AddScoped<SubscriptionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.UseCors(options => options
 .AllowAnyMethod()
 .AllowAnyHeader()
 .AllowAnyOrigin()
 //.WithOrigins("https://localhost:3000")
 .SetIsOriginAllowed(o => true)
 );

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}


app.Run();
