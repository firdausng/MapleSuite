using CloudNative.CloudEvents.NewtonsoftJson;
using Common.Messaging;
using Confluent.Kafka;
using leave.api.infra.data;
using leave.worker.Models;
using Newtonsoft.Json;
using Member = leave.api.domains.Member;

namespace leave.worker;

public class Worker : BackgroundService
{
    public const string ActivitySourceName = "Leave Worker";
    private readonly ILogger<Worker> _logger;
    private readonly IConsumer<string?, byte[]> _consumer;
    private readonly IServiceScopeFactory _scopeFactory;

    public Worker(
        ILogger<Worker> logger, 
        IConsumer<string?, byte[]> consumer, 
        IServiceScopeFactory scopeFactory
        )
    {
        _logger = logger;
        _consumer = consumer;
        _scopeFactory = scopeFactory;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // Subscribe to topic(s)
            var topics = new List<string> { "MemberRegisteredEvent" };
            _consumer.Subscribe(topics);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Consume messages with a timeout
                    var consumeResult = _consumer.Consume(stoppingToken);
                    if (consumeResult == null) continue;
                    // Process the message
                    var message = consumeResult.Message;
                    _logger.LogInformation(
                        "Consumed message: Key: {Key}, Value: {Value}, Partition: {Partition}, Offset: {Offset}",
                        message.Key, message.Value, consumeResult.Partition, consumeResult.Offset);

                    switch (consumeResult.Topic)
                    {
                        case "MemberRegisteredEvent":
                        {
                            var cloudEvent = message.ToCloudEvent(new JsonEventFormatter(), null);
                            var memberRegisteredEvent = JsonConvert.DeserializeObject<MemberRegisteredEvent>(cloudEvent.Data as string ?? string.Empty);
                            if (memberRegisteredEvent == null) continue;
                            
                            using var scope = _scopeFactory.CreateScope();
                            var dbContext = scope.ServiceProvider.GetRequiredService<LeaveContext>();   
                            dbContext.Members.Add(new Member
                            {
                                Status = "Active",
                                Id = memberRegisteredEvent.Member.Id
                            });
                            await dbContext.SaveChangesAsync(stoppingToken);
                            
                            break;
                        }
                            
                    }
                    // Add your message processing logic here
                    

                    // Optionally commit the offset if auto-commit is disabled
                    // _consumer.Commit(consumeResult);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError($"Error consuming message: {ex.Message}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Graceful shutdown
            _logger.LogInformation("Worker cancellation requested");
        }
        finally
        {
            // Clean up
            _consumer.Close();
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Stopping consumer");
        _consumer.Close();
        await base.StopAsync(stoppingToken);
    }
}
