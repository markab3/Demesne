using System.Text;
using System.Threading.RateLimiting;
using Demesne.Server.Auth;
using Demesne.Server.Hubs;
using Demesne.Server.Tick;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Database — fail fast on startup if not configured
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connStr))
    throw new InvalidOperationException(
        "DefaultConnection is not configured. Set ConnectionStrings:DefaultConnection " +
        "via environment variable or appsettings.");
var dataSource = NpgsqlDataSource.Create(connStr);
builder.Services.AddSingleton(dataSource);

// Authentication — JWT Bearer
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtSecret = jwtSection["Secret"];
if (string.IsNullOrWhiteSpace(jwtSecret) || jwtSecret.Length < 32)
    throw new InvalidOperationException(
        "Jwt:Secret is not configured or is too short (minimum 32 characters). " +
        "Inject it via environment variable Jwt__Secret or a secrets manager.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.MapInboundClaims = false;
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };
        // SignalR WebSocket connections cannot send custom headers, so the client passes
        // the JWT as ?access_token=<token>. This copies it into the auth context.
        // TRADE-OFF: the token appears verbatim in every server access log, browser URL
        // history, and HTTP Referer headers. Mitigate in production by scrubbing access
        // logs and using short-lived hub-specific tokens once auth is fully wired (Milestone 3).
        opts.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                var token = ctx.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(token) && ctx.Request.Path.StartsWithSegments("/hubs"))
                    ctx.Token = token;
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Rate limiting — per-IP partitioned windows so one client cannot exhaust
// the global budget and lock out all other players (REQ-203).
builder.Services.AddRateLimiter(opts =>
{
    opts.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Auth endpoints: 10 attempts per IP per minute
    opts.AddPolicy("auth", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
            }));

    // General API endpoints: 120 requests per IP per minute
    opts.AddPolicy("api", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 120,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
            }));
});

builder.Services.AddControllers();
builder.Services.AddSignalR();

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()));

builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<TickService>();
builder.Services.AddSingleton<ITickState>(sp => sp.GetRequiredService<TickService>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<TickService>());

var app = builder.Build();

// Warn early if CORS is unconfigured — otherwise silently broken with no startup crash
if (allowedOrigins.Length == 0)
    app.Services.GetRequiredService<ILoggerFactory>()
        .CreateLogger("Startup")
        .LogWarning(
            "AllowedOrigins is empty. CORS will reject all browser cross-origin requests. " +
            "Set AllowedOrigins in appsettings or appsettings.Development.json.");

app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/healthz", () => Results.Ok());
app.MapControllers();
app.MapHub<GameHub>("/hubs/game").RequireAuthorization();

app.Run();
