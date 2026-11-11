using Anonymazer.Data;
using Anonymazer.Interfaces;
using Anonymazer.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration
        .GetSection("Redis")["Connection"];

    var options = ConfigurationOptions.Parse(configuration!);
    options.AbortOnConnectFail = false;

    return ConnectionMultiplexer.Connect(options);
});

builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDataDetectors();

builder.Services.AddScoped<AnonymizationService>();
builder.Services.AddScoped<AnonymizationHistoryService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
