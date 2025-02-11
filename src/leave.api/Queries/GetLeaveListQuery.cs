using Common.Models;
using leave.api.infra.data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace members.api.queries;

public class GetLeaveListQuery: IRequest<Result<PaginationDto<GetLeaveDto>>>
{
    
    public class GetMemberQueryHandler: IRequestHandler<GetLeaveListQuery, Result<PaginationDto<GetLeaveDto>>>
    {
        private readonly LeaveContext _context;

        public GetMemberQueryHandler(LeaveContext context)
        {
            _context = context;
        }

        public async Task<Result<PaginationDto<GetLeaveDto>>> Handle(GetLeaveListQuery request, CancellationToken cancellationToken)
        {
            var list = await _context.Leaves
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);
            
            var dto = list.Select(entity => new GetLeaveDto(entity.MemberId, entity.StartDate, entity.EndDate, entity.LeaveType, entity.Reason, entity.Status)).ToList();
            return new Result<PaginationDto<GetLeaveDto>>
            {
                IsSuccess = true,
                Data = new PaginationDto<GetLeaveDto>
                {
                    Data = dto,
                    Page = 0,
                    Size = list.Count
                }
            };
        }
    }
}