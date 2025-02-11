using Common.Entities;

namespace leave.api.domains;

public class Member: Entity
{
    public string Status { get; set; }
    public List<Leave> Leaves { get; set; }
}