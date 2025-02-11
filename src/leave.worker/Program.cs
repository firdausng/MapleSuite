using leave.api.infra.data;
using leave.worker;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));
builder.AddKafkaConsumer<string?, byte[]>("kafka", opt =>
{
    opt.Config.GroupId = "leave-worker";
    opt.Config.AllowAutoCreateTopics = true;
});
builder.AddRedisClient(connectionName: "cache");

builder.Services.AddDbContext<LeaveContext>((sp, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("leavedb"));
});
builder.EnrichNpgsqlDbContext<LeaveContext>(settings =>
{
    settings.DisableRetry = false;
    settings.CommandTimeout = 30;
});

var host = builder.Build();
host.Run();