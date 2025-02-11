using Common.Entities;
using members.api.domains.entities;

namespace members.api.domains.Events;

public record MemberRegisteredEvent(Member Member) : BaseEvent;
