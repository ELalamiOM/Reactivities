using Application.Inscriptions.Commands;
using Application.Inscriptions.DTOs;
using Application.Inscriptions.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class InscriptionsController : BaseApiController
{
    /// <summary>
    /// S'inscrire à une activité
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> Register([FromQuery] string activityId)
    {
        return HandleResult(await Mediator.Send(new Register.Command { ActivityId = activityId }));
    }

    /// <summary>
    /// Annuler l'inscription à une activité
    /// </summary>
    [HttpDelete]
    public async Task<ActionResult> Unregister([FromQuery] string activityId)
    {
        return HandleResult(await Mediator.Send(new Unregister.Command { ActivityId = activityId }));
    }

    /// <summary>
    /// Liste des inscrits à une activité
    /// </summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<AttendeeDto>>> GetAttendees([FromQuery] string activityId)
    {
        return HandleResult(await Mediator.Send(new GetAttendeesForActivity.Query { ActivityId = activityId }));
    }
}