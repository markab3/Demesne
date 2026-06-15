using Demesne.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy
            .WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [])
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()));

var app = builder.Build();

app.UseCors();
app.MapControllers();
app.MapHub<GameHub>("/hubs/game");

app.Run();
