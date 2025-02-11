using Common.Models;
using MediatR;
using members.api.domains.entities;
using members.api.infra.data;
using Microsoft.EntityFrameworkCore;

namespace members.api.queries;

public class GetMemberQuery: IRequest<Result<GetMemberDto>>
{
    public GetMemberQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; private set; }
    
    public class GetMemberQueryHandler: IRequestHandler<GetMemberQuery, Result<GetMemberDto>>
    {
        private readonly MemberContext _context;

        public GetMemberQueryHandler(MemberContext context)
        {
            _context = context;
        }

        public async Task<Result<GetMemberDto>> Handle(GetMemberQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.Members
                .AsNoTracking()
                .Where(m => m.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                return new Result<GetMemberDto>
                {
                    IsSuccess = false,
                    ErrorCode = StatusCodes.Status404NotFound,
                    Message = "Entity not found"
                };
            }
            
            var dto = new GetMemberDto(entity.Biographical.FirstName, entity.Biographical.LastName, entity.Id);
            return new Result<GetMemberDto>
            {
                IsSuccess = true,
                Data = dto
            };
        }
    }
}

public record GetMemberDto(string FirstName, string LastName, Guid Id);