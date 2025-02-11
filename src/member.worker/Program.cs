using member.worker;
using members.api.infra.data;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));
builder.Services.AddSingleton<DistributedLockProvider>();
builder.AddKafkaProducer<string?, byte[]>("kafka");
builder.AddRedisClient(connectionName: "cache");

builder.Services.AddDbContext<MemberContext>((sp, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("memberdb"));
});
builder.EnrichNpgsqlDbContext<MemberContext>(settings =>
{
    settings.DisableRetry = false;
    settings.CommandTimeout = 30;
});

var host = builder.Build();
host.Run();