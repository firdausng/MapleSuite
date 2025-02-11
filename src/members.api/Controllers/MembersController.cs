using MediatR;
using members.api.Commands;
using members.api.queries;
using Microsoft.AspNetCore.Mvc;

namespace members.api.Controllers;

[ApiController]
[Route("[controller]")]
public class MembersController : ControllerBase
{

    private readonly ILogger<MembersController> _logger;
    private readonly IMediator _mediator;

    public MembersController(ILogger<MembersController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    [HttpPost(Name = nameof(RegisterMembers))]
    public async Task<IActionResult> RegisterMembers(RegisterMemberCommand command)
    {
        var createdItem = await _mediator.Send(command);
        var link = Url.Link(nameof(GetMember), new { memberId = createdItem.Id });
        return Created(link, createdItem);
    }

    [HttpGet(Name = nameof(GetMembers))]
    public async Task<IActionResult> GetMembers()
    {
        var list = await _mediator.Send(new GetMemberListQuery());
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
    
    [HttpGet("{memberId}", Name = nameof(GetMember))]
    public async Task<IActionResult> GetMember(Guid memberId)
    {
        var result = await _mediator.Send(new GetMemberQuery(memberId));
        if (result.IsSuccess) return Ok(result.Data);
        
        return Problem(
            "Cannot find member with id given", 
            $"{HttpContext.Request.Method} {HttpContext.Request.Path}",
            result.ErrorCode,
            result.Message,
            null
        );
    }
}