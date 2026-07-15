using Application.Activities.Commands;
using Application.Activities.DTOs;
using Application.Activities.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // Contrôleur de gestion des activités
    // Permet de créer, modifier, supprimer et consulter les activités
    public class ActivitiesController : BaseApiController
    {
        // GET: api/activities
        // Récupère la liste paginée des activités - Route publique
        // Filtres disponibles: "upcoming" (à venir), "past" (passées), null (toutes)
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<ActivityDto>>> GetActivities(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? filter = null)
        {
            var result = await Mediator.Send(new GetActivityList.Query
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Filter = filter
            });
            return HandleResult(result);
        }

        // GET: api/activities/{id}
        // Récupère les détails d'une activité spécifique - Route publique
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityDto>> GetActivityDetail(string id)
        {
            var result = await Mediator.Send(new GetActivityDetails.Query { Id = id });
            return HandleResult(result);
        }

        // POST: api/activities
        // Crée une nouvelle activité - Route protégée (authentification requise)
        // L'utilisateur connecté devient automatiquement l'hôte
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<string>> CreateActivity(CreateActivityDto activityDto)
        {
            return HandleResult(await Mediator.Send(new CreateActivity.Command { ActivityDto = activityDto }));
        }

        // POST: api/activities/{id}/attend
        // Gère la participation à une activité - Route protégée (authentification requise)
        // Non inscrit -> inscrit | Participant -> désinscrit | Hôte -> annule/réactive
        [Authorize]
        [HttpPost("{id}/attend")]
        public async Task<ActionResult> Attend(string id)
        {
            return HandleResult(await Mediator.Send(new UpdateAttendance.Command { Id = id }));
        }

        // PUT: api/activities
        // Modifie une activité existante - Route protégée (authentification requise)
        // Seul l'hôte peut modifier l'activité
        [Authorize]
        [HttpPut]
        public async Task<ActionResult> EditActivity(EditActivityDto activityDto)
        {
            return HandleResult(await Mediator.Send(new EditActivity.Command { ActivityDto = activityDto }));
        }

        // DELETE: api/activities/{id}
        // Supprime une activité - Route protégée (authentification requise)
        // Seul l'hôte peut supprimer l'activité
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteActivity(string id)
        {
            return HandleResult(await Mediator.Send(new DeleteActivity.Command { Id = id }));
        }
    }
}
