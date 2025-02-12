using Common.Models;
using leave.api.domains;
using leave.api.infra.data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace leave.api.Commands;

public sealed record ApplyLeaveCommand : IRequest<Result<CreatedItemDto>>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; }
    public string LeaveType { get; set; }
    public Guid MemberId { get; set; }

    internal sealed record Handler : IRequestHandler<ApplyLeaveCommand, Result<CreatedItemDto>>
    {
        private readonly LeaveContext _context;

        public Handler(LeaveContext context)
        {
            _context = context;
        }
        
        public async Task<Result<CreatedItemDto>> Handle(ApplyLeaveCommand request, CancellationToken cancellationToken)
        {
            var member = await _context.Members.FirstOrDefaultAsync(m => m.Id.Equals(request.MemberId), cancellationToken);
            if (member == null)
            {
                return new Result<CreatedItemDto>
                {
                    IsSuccess = false,
                    ErrorCode = StatusCodes.Status404NotFound,
                    Message = "Member not found"
                };
            }
            
            var entity = new Leave
            {
                StartDate = request.StartDate.ToUniversalTime(),
                EndDate = request.EndDate.ToUniversalTime(),
                Reason = request.Reason,
                Status = "Pending",
                LeaveType = request.LeaveType,
                MemberId = member.Id,
                Member = member
            };
            _context.Leaves.Add(entity);
            
            await _context.SaveChangesAsync(cancellationToken);
            return new Result<CreatedItemDto>
            {
                IsSuccess = true,
                Data = new CreatedItemDto(entity.Id)
            };
        }
    }
    
}

public record CreatedItemDto(Guid Id);