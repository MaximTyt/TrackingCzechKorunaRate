using Data;
using Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quartz;
using Services;
using Services.Utils;
using Settings;
using TrackingCzechKorunaRate.Jobs;
using TrackingCzechKorunaRate.QuartzTaskScheduler;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
    {
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod() // Разрешаем все методы (GET, POST, PUT, DELETE и т.д.)
               .AllowAnyHeader();
    });
});

// Add services to the container.
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory() + "/Config").AddJsonFile("appsettings.json").AddJsonFile("config.json", optional: true, reloadOnChange: true).Build();


//Настройка логгера
builder.Logging.ClearProviders();
builder.Logging.AddFile(Path.Combine(Directory.GetCurrentDirectory() + "/Log", "logger.log"));
builder.Logging.AddConsole();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


//Регистарция сервисов
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDataAccess(connectionString);
builder.Services.AddBusinessLogic();


// Регистрация конфигурации
builder.Services.Configure<SyncSetting>(builder.Configuration.GetSection("SyncSetting"));
builder.Services.Configure<BaseAddressSetting>(builder.Configuration.GetSection("BaseAddress"));

builder.Services.AddQuartz(q =>
{
    // Регистрация задачи
    q.AddJob<SyncByScheduleJob>(opts => opts.WithIdentity("CurrencySyncJob").StoreDurably());
});
// Регистрация Quartz.NET как hosted service
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// Регистрация сервиса для управления задачами
builder.Services.AddSingleton<QuartzTaskScheduler>();
// Регистрация hosted service для запуска QuartzTaskScheduler
builder.Services.AddHostedService<QuartzTaskSchedulerService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "KorunaRate API", Version = "v1" });
});
var app = builder.Build();
app.UseCors("AllowAll");


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
