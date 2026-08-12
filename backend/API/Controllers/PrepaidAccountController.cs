using Application.PrepaidAccounts.DTOs;
using Application.PrepaidAccounts.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class PrepaidAccountController : BaseApiController
{
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<PrepaidAccountDto>> GetAccount()
    {
        return HandleResult(await Mediator.Send(new GetPrepaidAccount.Query()));
    }
}
