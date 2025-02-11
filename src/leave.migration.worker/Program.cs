using leave.api.infra.data;
using leave.migration.worker;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddNpgsqlDbContext<LeaveContext>(connectionName: "leavedb");

var host = builder.Build();
host.Run();