using CloudNative.CloudEvents;
using CloudNative.CloudEvents.NewtonsoftJson;
using Common.Entities;
using Common.Messaging;
using Confluent.Kafka;
using members.api.infra.data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace member.worker;

public class Worker : BackgroundService
{
    public const string ActivitySourceName = "Member Worker";
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IProducer<string?, byte[]> _producer;
    private readonly DistributedLockProvider _lockProvider;
    private readonly string _workerName;
    
    private readonly int _pollingInterval = 5000; // 5 seconds

    public Worker(
        ILogger<Worker> logger, 
        IServiceScopeFactory scopeFactory, 
        IProducer<string?, byte[]> producer, 
        DistributedLockProvider lockProvider
        )
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _producer = producer;
        _lockProvider = lockProvider;
        _workerName = $"worker-{Guid.NewGuid()}"; // Unique worker identifier
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Try to acquire distributed lock
                using var lockResult = await _lockProvider.AcquireLockAsync(
                    "outbox-processor-lock",
                    TimeSpan.FromSeconds(30)
                );

                if (lockResult.IsAcquired)
                {
                    _logger.LogInformation($"Worker {_workerName} acquired lock");
                    await ProcessOutboxMessagesAsync(stoppingToken);
                }
                else
                {
                    _logger.LogInformation($"Worker {_workerName} failed to acquire lock");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in worker {_workerName}");
            }

            await Task.Delay(_pollingInterval, stoppingToken);
        }
    }
    
    private async Task ProcessOutboxMessagesAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MemberContext>();
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async() =>
        {
// Use pessimistic locking at database level
            await using var transaction = await dbContext.Database.BeginTransactionAsync(stoppingToken);

            var messages = await dbContext.Database
                .SqlQueryRaw<OutboxMessage>(
                    $""""
                     SELECT * FROM "OutboxMessages"
                     WHERE "Status" = 'Created' 
                     ORDER BY "CreatedOnUtc"
                     LIMIT @limit
                     FOR UPDATE SKIP LOCKED
                     """"
                    ,new NpgsqlParameter("@limit", 10)
                )
                .ToListAsync(stoppingToken);

            foreach (var message in messages)
            {
                try
                {
                    // message.Status = "Processing";
                    // await dbContext.SaveChangesAsync(stoppingToken);

                    // Publish to Kafka with message type as topic and ID as key
                    
                    CloudEvent cloudEvent = new ()
                    {
                        Id = message.Id.ToString(),
                        Type = "member-registered",
                        Source = new Uri("https://maplesuite.com/member"),
                        Time = DateTimeOffset.UtcNow,
                        DataContentType = "text/application-json",
                        Data = message.Payload
                    };

                    var cloudMessage = cloudEvent.ToKafkaMessage(ContentMode.Binary, new JsonEventFormatter());
                    if (cloudMessage == null)
                    {
                        throw new Exception("Failed to create Kafka message");
                    };
                    
                    var result = await _producer.ProduceAsync(
                        message.Type,
                        cloudMessage, stoppingToken);
                    dbContext.Attach(message);
                    message.Status = "Completed";
                    message.ProcessedOnUtc = DateTime.UtcNow;
                    
                    await dbContext.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    message.Status = "Failed";
                    message.Error = ex.Message;
                    message.RetryCount++;
                    _logger.LogError(ex, "Error processing message {MessageId}", message.Id);
                }
            }

            // await dbContext.SaveChangesAsync(stoppingToken);
            await transaction.CommitAsync(stoppingToken);
        });
        
    }
}
