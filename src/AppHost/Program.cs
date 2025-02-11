var builder = DistributedApplication.CreateBuilder(args);

var kafka = builder.AddKafka("kafka")
    .WithKafkaUI(kafkaUI =>
    {
        kafkaUI.WithHostPort(9100);
    });

var postgres  = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050));

var cache = builder.AddRedis("cache").WithRedisCommander();

var memberdb = postgres.AddDatabase("memberdb");

var members = builder
    .AddProject<Projects.members_api>("MembersApi")
    .WithReference(memberdb)
    .WaitFor(memberdb);

builder.AddProject<Projects.MemberMigrationWorker>("MemberMigrations")
    .WithReference(memberdb)
    .WaitFor(memberdb);

builder.AddProject<Projects.member_worker>("MemberWorker")
    .WithReference(kafka)
    .WithReference(cache)
    .WithReference(memberdb)
    .WaitFor(memberdb);

var leavedb = postgres.AddDatabase("leavedb");

var leaves = builder
    .AddProject<Projects.leave_api>("LeavesApi")
    .WithReference(leavedb)
    .WaitFor(leavedb);

builder.AddProject<Projects.leave_migration_worker>("LeaveMigrations")
    .WithReference(leavedb)
    .WaitFor(leavedb);

builder.AddProject<Projects.leave_worker>("LeaveWorker")
    .WithReference(kafka)
    // .WithReference(cache)
    .WithReference(leavedb)
    .WaitFor(leavedb)
    .WaitFor(kafka);

builder.AddPnpmApp("pnpm-demo", "../frontend", "dev")
    .WithExternalHttpEndpoints();

builder.Build().Run();