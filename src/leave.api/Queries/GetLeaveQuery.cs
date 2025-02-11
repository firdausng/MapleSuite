using Common.Models;
using leave.api.infra.data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace members.api.queries;

public class GetLeaveQuery: IRequest<Result<GetLeaveDto>>
{
    public GetLeaveQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; private set; }
    
    public class GetMemberQueryHandler: IRequestHandler<GetLeaveQuery, Result<GetLeaveDto>>
    {
        private readonly LeaveContext _context;

        public GetMemberQueryHandler(LeaveContext context)
        {
            _context = context;
        }

        public async Task<Result<GetLeaveDto>> Handle(GetLeaveQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.Leaves
                .AsNoTracking()
                .Where(m => m.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                return new Result<GetLeaveDto>
                {
                    IsSuccess = false,
                    ErrorCode = StatusCodes.Status404NotFound,
                    Message = "Entity not found"
                };
            }
            
            var dto = new GetLeaveDto(entity.MemberId, entity.StartDate, entity.EndDate, entity.LeaveType, entity.Reason, entity.Status);
            return new Result<GetLeaveDto>
            {
                IsSuccess = true,
                Data = dto
            };
        }
    }
}

public record GetLeaveDto(Guid MemberId, DateTime StartDate, DateTime EndDate, string LeaveType, string Reason, string Status);