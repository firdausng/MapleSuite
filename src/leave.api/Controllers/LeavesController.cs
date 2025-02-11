using leave.api.Commands;
using MediatR;
using members.api.queries;
using Microsoft.AspNetCore.Mvc;

namespace leave.api.Controllers;

[ApiController]
[Route("[controller]")]
public class LeavesController : ControllerBase
{

    private readonly ILogger<LeavesController> _logger;
    private readonly IMediator _mediator;

    public LeavesController(ILogger<LeavesController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    
    [HttpPost(Name = nameof(RegisterLeave))]
    public async Task<IActionResult> RegisterLeave(ApplyLeaveCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
        {
            return Problem(
                "Cannot Register Leave", 
                $"{HttpContext.Request.Method} {HttpContext.Request.Path}",
                result.ErrorCode,
                result.Message,
                null
            );
        }
        var link = Url.Link(nameof(GetLeave), new { leaveId = result.Data.Id });
        return Created(link, result.Data);
    }

    [HttpGet(Name = nameof(GetLeaveList))]
    public async Task<IActionResult> GetLeaveList()
    {
        var list = await _mediator.Send(new GetLeaveListQuery());
        if (!list.IsSuccess)
        {
            return Problem(
                "Something went wrong", 
                $"{HttpContext.Request.Method} {HttpContext.Request.Path}",
                list.ErrorCode,
                list.Message,
                null
            );
        }
        return Ok(list.Data);
    }
    
    [HttpGet("{leaveId}", Name = nameof(GetLeave))]
    public async Task<IActionResult> GetLeave(Guid leaveId)
    {
        var result = await _mediator.Send(new GetLeaveQuery(leaveId));
        if (result.IsSuccess) return Ok(result.Data);
        
        return Problem(
            "Cannot find leave with id given", 
            $"{HttpContext.Request.Method} {HttpContext.Request.Path}",
            result.ErrorCode,
            result.Message,
            null
        );
    }
    
    [HttpGet("users/{userId}", Name = nameof(GetUserLeave))]
    public async Task<IActionResult> GetUserLeave(Guid userId)
    {
        var result = await _mediator.Send(new GetMemberLeaveListQuery(userId));
        if (result.IsSuccess) return Ok(result.Data);
        
        return Problem(
            "Cannot find user with id given", 
            $"{HttpContext.Request.Method} {HttpContext.Request.Path}",
            result.ErrorCode,
            result.Message,
            null
        );
    }
}