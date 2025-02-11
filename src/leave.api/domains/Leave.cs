using Common.Entities;

namespace leave.api.domains;

public class Leave: Entity
{
    public Guid MemberId { get; set; }
    public Member Member { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string LeaveType { get; set; }
    public string Reason { get; set; }
    public string Status { get; set; }
}