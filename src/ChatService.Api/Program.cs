using System.Text;
using ChatService.Api.Helpers;
using ChatService.Api.Middleware;
using ChatService.Application.Queries.GetConversations;
using ChatService.Application.Queries.GetMessages;
using ChatService.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ── Serilog ───────────────────────────────────────────────────────────────────
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/chatservice-.log", rollingInterval: RollingInterval.Day));

// ── Infrastructure (DbContext, repos, services) ───────────────────────────────
builder.Services.AddInfrastructure(builder.Configuration);

// ── Application query/command handlers ───────────────────────────────────────
builder.Services.AddScoped<GetDirectMessagesHandler>();
builder.Services.AddScoped<GetConversationsHandler>();

// ── JWT Authentication ────────────────────────────────────────────────────────
var jwtSecret = builder.Configuration["Authentication:JwtSecret"]!;
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["Authentication:Issuer"],
            ValidAudience            = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        };
        // SignalR: read token from query string ?access_token=
        opts.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                var token = ctx.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(token) &&
                    ctx.HttpContext.Request.Path.StartsWithSegments("/hubs/chat"))
                    ctx.Token = token;
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// ── SignalR ───────────────────────────────────────────────────────────────────
builder.Services.AddSignalR(opts => opts.EnableDetailedErrors = builder.Environment.IsDevelopment());
builder.Services.AddSingleton<IUserIdProvider, JwtUserIdProvider>();

// ── CORS ──────────────────────────────────────────────────────────────────────
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:3000"];

builder.Services.AddCors(opts =>
    opts.AddPolicy("ChatPolicy", p => p
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()));

// ── Controllers + Swagger ─────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ChatService API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header, Type = SecuritySchemeType.Http,
        Scheme = "bearer", BearerFormat = "JWT", Name = "Authorization",
        Description = "Enter your JWT token"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {{
        new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }},
        Array.Empty<string>()
    }});
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<ChatService.Infrastructure.Persistence.AppDbContext>("database");

var app = builder.Build();

// ── Middleware pipeline (ORDER MATTERS) ───────────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("ChatPolicy");          // CORS before Auth
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatService.Api.Hubs.ChatHub>("/hubs/chat");
app.MapHealthChecks("/health");

app.Run();
