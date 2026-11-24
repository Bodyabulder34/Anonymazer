using Anonymazer.Data;
using Anonymazer.Interfaces;
using Anonymazer.Services;
using Anonymazer.Worker;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration
        .GetSection("Redis")["Connection"];

    var options = ConfigurationOptions.Parse(configuration!);
    options.AbortOnConnectFail = false;

    return ConnectionMultiplexer.Connect(options);
});

builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();

builder.Services.AddDataDetectors();

builder.Services.AddScoped<AnonymizationService>();
builder.Services.AddScoped<AnonymizationHistoryService>();
builder.Services.AddScoped<AnonymizationJobService>();

builder.Services.AddSingleton<RabbitMqConnectionService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.Run();