
using Anonymazer.Data;
using Anonymazer.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IDataDetector, EmailDetector>();
builder.Services.AddScoped<IDataDetector, PassportDetector>();
builder.Services.AddScoped<IDataDetector, PhoneDetector>();

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
