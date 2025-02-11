using Common.Entities;
using members.api.domains.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace members.api.infra.data;

public class MemberContext : DbContext
{
    public DbSet<Member> Members { get; set; }

    public MemberContext(DbContextOptions<MemberContext> options) : base(options)
    {
        // ChangeTracker.Tracked  += AddOutboxMessage;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MemberContext).Assembly);
        
        modelBuilder.Entity<Member>()
            .ComplexProperty(m => m.Biographical, y => y.IsRequired());
        modelBuilder.Entity<Member>()
            .ComplexProperty(m => m.Demographic, y => y.IsRequired());
        modelBuilder.Entity<Member>()
            .ComplexProperty(m => m.Contact, y => y.IsRequired());

    }
    
    private void AddOutboxMessage(object sender, EntityEntryEventArgs e)
    {
        if (e.Entry.Entity is Entity entity)
        {
            var date = DateTime.UtcNow;

            JsonSerializerSettings _options = new()
            {
                TypeNameHandling = TypeNameHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
            var domainEvents = entity.DomainEvents
                .Select(e =>
                {
                    var message = new OutboxMessage
                    {
                        Payload = JsonConvert.SerializeObject(new
                        {
                            eventData= e,
                            entity = entity
                        }, _options),
                        CreatedOnUtc = date,
                        Type = e.GetType().Name,
                        Status = "Created"
                    };
                    return message;
                })
                .ToList(); 
            
            switch (e.Entry.State)
            {
                // case EntityState.Deleted:
                //     entityWithTimestamps.Deleted = DateTime.UtcNow;
                //     Console.WriteLine($"Stamped for delete: {e.Entry.Entity}");
                //     break;
                // case EntityState.Modified:
                //     entityWithTimestamps.Modified = DateTime.UtcNow;
                //     Console.WriteLine($"Stamped for update: {e.Entry.Entity}");
                //     break;
                case EntityState.Added:
                    Set<OutboxMessage>().AddRangeAsync(domainEvents);
                    // entityWithTimestamps.Added = DateTime.UtcNow;
                    // Console.WriteLine($"Stamped for insert: {e.Entry.Entity}");
                    break;
            }
        }
    }
}