using Common.Models;
using leave.api.infra.data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace leave.api.Queries;

public sealed record GetMemberLeaveListQuery: IRequest<Result<PaginationDto<GetLeaveDto>>>
{
    public GetMemberLeaveListQuery(Guid memberId)
    {
        MemberId = memberId;
    }

    public Guid MemberId { get; set; }
    
    internal sealed record GetMemberLeaveListQueryHandler: IRequestHandler<GetMemberLeaveListQuery, Result<PaginationDto<GetLeaveDto>>>
    {
        private readonly LeaveContext _context;

        public GetMemberLeaveListQueryHandler(LeaveContext context)
        {
            _context = context;
        }

        public async Task<Result<PaginationDto<GetLeaveDto>>> Handle(GetMemberLeaveListQuery request, CancellationToken cancellationToken)
        {
            var member = await _context.Members
                .AsNoTracking()
                .Include(m => m.Leaves)
                .FirstOrDefaultAsync(m => m.Id == request.MemberId, cancellationToken: cancellationToken);

            if (member == null)
            {
                return new Result<PaginationDto<GetLeaveDto>>
                {
                    IsSuccess = false,
                    ErrorCode = StatusCodes.Status404NotFound,
                    Message = "Member not found"
                };
            }
            
            var dto = member.Leaves.Select(entity => new GetLeaveDto(entity.MemberId, entity.StartDate, entity.EndDate, entity.LeaveType, entity.Reason, entity.Status)).ToList();
            return new Result<PaginationDto<GetLeaveDto>>
            {
                IsSuccess = true,
                Data = new PaginationDto<GetLeaveDto>
                {
                    Data = dto,
                    Page = 0,
                    Size = member.Leaves.Count
                }
            };
        }
    }
}