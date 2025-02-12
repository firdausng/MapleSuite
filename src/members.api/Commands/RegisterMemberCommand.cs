using MediatR;
using members.api.domains.entities;
using members.api.domains.Events;
using members.api.infra.data;

namespace members.api.Commands;

public sealed record RegisterMemberCommand : IRequest<CreatedItemDto>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }

    public sealed record Handler : IRequestHandler<RegisterMemberCommand, CreatedItemDto>
    {
        private readonly MemberContext _context;

        public Handler(MemberContext context)
        {
            _context = context;
        }
        
        public async Task<CreatedItemDto> Handle(RegisterMemberCommand request, CancellationToken cancellationToken)
        {
            var entity = new Member
            {
                StartDate = DateTime.UtcNow,
                Biographical = new Biographical()
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    DateOfBirth = request.DateOfBirth.ToUniversalTime(),
                },
                Contact = new Contact
                {
                    Email = request.Email,
                    Address = "Nilai",
                    PhoneNumber = "0123456789",
                    WorkEmail = "nilai@gmail.com",
                },
                Demographic = new Demographic
                {
                    Country = "Malaysia",
                    Enthic = "Malay",
                    Gender = "Male",
                    Religion = "Islam",
                    MaritalStatus = "Married",
                }
            };
            
            entity.AddDomainEvent(new MemberRegisteredEvent(entity));
            _context.Members.Add(entity);
            
            await _context.SaveChangesAsync(cancellationToken);
            return new CreatedItemDto(entity.Id);
        }
    }
    
}

public sealed record CreatedItemDto(Guid Id);