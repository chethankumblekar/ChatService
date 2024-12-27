using PlayGround.ChatService.DataService;
using PlayGround.ChatService.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PlayGround.ChatService.Services;
using Microsoft.EntityFrameworkCore;
using PlayGround.ChatService.Infrastructure;
using Playground.ChatService.Core.Services;
using PlayGround.ChatService.Application.Services;
using PlayGround.ChatService.Core;
using PlayGround.ChatService.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddSignalR();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var Configuration = builder.Configuration;

builder.Services.AddDbContextFactory<DataContext>(options => options.UseSqlServer(Configuration["DbConnection:ConnectionString"]),ServiceLifetime.Transient);


builder.Services.AddCors(options =>
{
    options.AddPolicy("reactApp", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<IChatService, ChatService>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IMessageRepository, MessageRepository>();
builder.Services.AddSingleton<IGroupRepository, GroupRepository>();

builder.Services.AddSingleton<SharedDb>();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwtOption =>
{
    jwtOption.RequireHttpsMetadata = false;
    jwtOption.SaveToken = true;
    jwtOption.TokenValidationParameters = new TokenValidationParameters()
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Authentication:JwtSecret"])),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("reactApp");
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.MapHub<ChatHub>("/chat").RequireAuthorization();

app.Run();
