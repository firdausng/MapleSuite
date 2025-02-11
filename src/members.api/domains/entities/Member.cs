using Common.Entities;

namespace members.api.domains.entities;

public class Member: Entity
{
    public DateTime StartDate { get; set; }
    public Demographic Demographic { get; set; }
    public Biographical Biographical { get; set; }
    public Contact Contact { get; set; }
}