using Data;
using Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quartz;
using Services;
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
//var _confstring = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
//var connectionString = _confstring.GetConnectionString("DefaultConnection");
// Add services to the container.
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory() + "/Config").AddJsonFile("appsettings.json").AddJsonFile("config.json", optional: true, reloadOnChange: true).Build();
//var _confstring = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory() + "/Config").AddJsonFile("appsettings.json").AddJsonFile("config.json", optional: true, reloadOnChange: true).Build();
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
builder.Services.Configure<SyncByScheduleSetting>(builder.Configuration.GetSection("SyncBySchedule"));
builder.Services.Configure<SyncByPeriodSetting>(builder.Configuration.GetSection("SyncByPeriod"));

// Регистрация Quartz.NET
//builder.Services.AddQuartz(q =>
//{  
//    // Создание задачи
//    var jobKey = new JobKey("SyncBySheduleJob");
//    q.AddJob<SyncByScheduleJob>(opts => opts.WithIdentity(jobKey));
//    var CronSchedule = builder.Configuration.GetSection("SyncBySchedule:CronSсhedule").Value;
//    // Настройка триггера
//    q.AddTrigger(opts => opts
//        .ForJob(jobKey)
//        .WithIdentity("CurrencySyncTrigger")
//        .WithCronSchedule(CronSchedule));
//});

//builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

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
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
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
