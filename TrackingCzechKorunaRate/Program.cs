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
               .AllowAnyMethod() // ��������� ��� ������ (GET, POST, PUT, DELETE � �.�.)
               .AllowAnyHeader();
    });
});

// Add services to the container.
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory() + "/Config").AddJsonFile("appsettings.json").AddJsonFile("config.json", optional: true, reloadOnChange: true).Build();


//��������� �������
builder.Logging.ClearProviders();
builder.Logging.AddFile(Path.Combine(Directory.GetCurrentDirectory() + "/Log", "logger.log"));
builder.Logging.AddConsole();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


//����������� ��������
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDataAccess(connectionString);
builder.Services.AddBusinessLogic();


// ����������� ������������
builder.Services.Configure<SyncSetting>(builder.Configuration.GetSection("SyncSetting"));
builder.Services.Configure<BaseAddressSetting>(builder.Configuration.GetSection("BaseAddress"));

builder.Services.AddQuartz(q =>
{
    // ����������� ������
    q.AddJob<SyncByScheduleJob>(opts => opts.WithIdentity("CurrencySyncJob").StoreDurably());
});
// ����������� Quartz.NET ��� hosted service
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// ����������� ������� ��� ���������� ��������
builder.Services.AddSingleton<QuartzTaskScheduler>();
// ����������� hosted service ��� ������� QuartzTaskScheduler
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
