using Common.Models;
using MediatR;
using members.api.infra.data;
using Microsoft.EntityFrameworkCore;

namespace members.api.queries;

public class GetMemberListQuery: IRequest<Result<PaginationDto<GetMemberDto>>>
{
    
    public class GetMemberQueryHandler: IRequestHandler<GetMemberListQuery, Result<PaginationDto<GetMemberDto>>>
    {
        private readonly MemberContext _context;

        public GetMemberQueryHandler(MemberContext context)
        {
            _context = context;
        }

        public async Task<Result<PaginationDto<GetMemberDto>>> Handle(GetMemberListQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.Members
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
            
            var dto = entity.Select(e => new GetMemberDto(e.Biographical.FirstName, e.Biographical.LastName, e.Id)).ToList();
            return new Result<PaginationDto<GetMemberDto>>
            {
                IsSuccess = true,
                Data = new PaginationDto<GetMemberDto>
                {
                    Data = dto,
                    Page = 0,
                    Size = entity.Count
                }
            };
        }
    }
}