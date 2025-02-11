using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;

namespace members.api.infra.data.interceptors;

public class InsertOutboxMessagesInterceptor: SaveChangesInterceptor
{
    private static JsonSerializerSettings _options = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    };
    
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            await InsertOutboxMessagesAsync(eventData.Context);
        }
        
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static async Task InsertOutboxMessagesAsync(DbContext dbContext)
    {
        var date = DateTime.UtcNow;
        var entities = dbContext
            .ChangeTracker
            .Entries<Entity>()
            .Select(e => e.Entity)
            .ToList();
            
            
        var domainEvents = entities
            .SelectMany(entity =>
            {
                var domainEvents = entity.DomainEvents;
                return domainEvents;
            })
            .Select(e =>
            {
                var message = new OutboxMessage
                {
                    Payload = JsonConvert.SerializeObject(e, _options),
                    CreatedOnUtc = date,
                    Type = e.GetType().Name,
                    Status = "Created"
                };
                return message;
            })
            .ToList(); 
        
        entities.ForEach(e => e.ClearDomainEvents());
        
        await dbContext.Set<OutboxMessage>().AddRangeAsync(domainEvents);
    }
}