using Application.Inscriptions.Commands;
using Application.Inscriptions.DTOs;
using Application.Inscriptions.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// Contrôleur de gestion des inscriptions aux activités
// Permet de s'inscrire, se désinscrire et consulter les participants
public class InscriptionsController : BaseApiController
{
    // POST: api/inscriptions?activityId={id}
    // Inscrit l'utilisateur connecté à une activité - Route protégée (authentification requise)
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> Register([FromQuery] string activityId)
    {
        return HandleResult(await Mediator.Send(new Register.Command { ActivityId = activityId }));
    }

    // DELETE: api/inscriptions?activityId={id}
    // Annule l'inscription de l'utilisateur à une activité - Route protégée (authentification requise)
    // L'hôte ne peut pas se désinscrire (doit annuler l'activité)
    [Authorize]
    [HttpDelete]
    public async Task<ActionResult> Unregister([FromQuery] string activityId)
    {
        return HandleResult(await Mediator.Send(new Unregister.Command { ActivityId = activityId }));
    }

    // GET: api/inscriptions?activityId={id}
    // Récupère la liste des participants d'une activité - Route publique
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<AttendeeDto>>> GetAttendees([FromQuery] string activityId)
    {
        return HandleResult(await Mediator.Send(new GetAttendeesForActivity.Query { ActivityId = activityId }));
    }
}