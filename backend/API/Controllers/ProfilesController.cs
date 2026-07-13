using Application.Profiles.Commands;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace API.Controllers;

public class ProfilesController : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<Photo>> UploadPhoto(IFormFile file)
    {
        return HandleResult(await Mediator.Send(new AddPhoto.Command { File = file }));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePhoto(string id)
    {
        return HandleResult(await Mediator.Send(new DeletePhoto.Command { Id = id }));
    }
}
