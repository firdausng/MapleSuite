using MemberMigrationWorker;
using members.api.infra.data;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddNpgsqlDbContext<MemberContext>(connectionName: "memberdb");

var host = builder.Build();
host.Run();