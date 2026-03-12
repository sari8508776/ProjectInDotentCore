using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using myProject.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace myProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class IceCreamController : ControllerBase
    {
        private readonly IIceCreamService _iceCreamService;
        private readonly myProject.Services.IActivityRepository _activityRepository;

        public IceCreamController(IIceCreamService iceCreamService, myProject.Services.IActivityRepository activityRepository)
        {
            _iceCreamService = iceCreamService;
            _activityRepository = activityRepository;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst("userid")?.Value ?? "0");
        }

        [HttpGet()]
        public ActionResult<IEnumerable<IceCream>> Get()
        {
            var userId = GetUserId();
            return _iceCreamService.Get().Where(x => x.UserId == userId).ToList();
        }

        [HttpGet("me")]
        public ActionResult<IEnumerable<IceCream>> GetMe()
        {
            var userId = GetUserId();
            return _iceCreamService.Get().Where(x => x.UserId == userId).ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<IceCream> Get(int id)
        {
            var userId = GetUserId();
            var iceCream = _iceCreamService.Get(id);
            if (iceCream == null || iceCream.UserId != userId)
                return NotFound();
            return iceCream;
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Create(IceCream newIceCream)
        {
            newIceCream.UserId = GetUserId();
            var postedIceCream = _iceCreamService.Create(newIceCream);
            var username = User.FindFirst("username")?.Value ?? "system";
            await _activityRepository.BroadcastAsync(username, "created", postedIceCream.Name);
            return CreatedAtAction(nameof(Get), new { id = postedIceCream.Id }, postedIceCream);
        }

        [HttpPut("{id}")]
        public async System.Threading.Tasks.Task<ActionResult> Update(int id, IceCream newIceCream)
        {
            var userId = GetUserId();
            var iceCream = _iceCreamService.Find(id);
            if (iceCream == null || iceCream.UserId != userId)
                return NotFound();
            newIceCream.Id = id;
            newIceCream.UserId = userId;
            if (!_iceCreamService.Update(id, newIceCream))
                return BadRequest();
            var username = User.FindFirst("username")?.Value ?? "system";
            await _activityRepository.BroadcastAsync(username, "updated", newIceCream.Name);
            return Ok(newIceCream);
        }

        [HttpDelete("{id}")]
        public async System.Threading.Tasks.Task<ActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var iceCream = _iceCreamService.Find(id);
            if (iceCream == null || iceCream.UserId != userId)
                return NotFound();
            if (!_iceCreamService.Delete(id))
                return NotFound();
            var username = User.FindFirst("username")?.Value ?? "system";
            await _activityRepository.BroadcastAsync(username, "deleted", iceCream.Name);
            return Ok(iceCream);
        }
    }
}
