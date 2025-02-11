using leave.api.domains;
using Microsoft.EntityFrameworkCore;

namespace leave.api.infra.data;

public class LeaveContext : DbContext
{
    public DbSet<Leave> Leaves { get; set; }
    public DbSet<Member> Members { get; set; }
    
    public LeaveContext(DbContextOptions<LeaveContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}