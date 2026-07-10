using Application.Activities.Commands;
using Application.Activities.DTOs;
using Application.Activities.Queries;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class ActivitiesController : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<Activity>>> GetActivities()
        {
            return await Mediator.Send(new GetActivityList.Query());
          //  return await context.Activites.ToListAsync();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> GetActivityDetail(string id)
        {
            var result = await Mediator.Send(new GetActivityDetails.Query{Id = id});
            return HandleResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreateActivity(CreateActivityDto activityDto)
        {
            return HandleResult(await Mediator.Send(new CreateActivity.Command{ActivityDto = activityDto}));
        }

        [HttpPut]
        public async Task<ActionResult> EditActivity(Activity activity)
        {
            return HandleResult(await Mediator.Send(new EditActivity.Command{Activity = activity}));
            
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteActivity(string id)
        {
         return HandleResult(await Mediator.Send(new DeleteActivity.Command{Id = id}));        
   
        }
    }
}
