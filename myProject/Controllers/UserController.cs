
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using myProject.Interfaces;
using System.Security.Claims;
using myProject.Services;
using myProject.Models;
using Microsoft.AspNetCore.Authorization;
namespace myProject.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    IUserService userService;
    private readonly myProject.Interfaces.IIceCreamService _iceCreamService;
    private readonly myProject.Services.IActivityRepository _activityRepository;

    public UserController(IUserService userService, myProject.Interfaces.IIceCreamService iceCreamService, myProject.Services.IActivityRepository activityRepository)
    {
        this.userService = userService;
        this._iceCreamService = iceCreamService;
        _activityRepository = activityRepository;
    }

    [HttpGet("me")]
    public ActionResult<User> GetMe()
    {
        var userId = int.Parse(User.FindFirst("userid")?.Value ?? "0");
        var user = userService.Get(userId);
        if (user == null)
            return NotFound();
        return user;
    }

    [HttpGet()]
    [Authorize(Policy = "Admin")]
    public ActionResult<IEnumerable<User>> Get()
    {
        return userService.Get();
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "Admin")]
    public ActionResult<User> Get(int id)
    {
        var user = userService.Get(id);
        if (user == null)
            return NotFound();
        return user;
    }

    [HttpPost]
    [Authorize(Policy = "Admin")]
    public async System.Threading.Tasks.Task<ActionResult> Create(User newUser)
    {
        var postedUser = userService.Create(newUser);
        var username = User.FindFirst("username")?.Value ?? "system";
        await _activityRepository.BroadcastAsync(username, "created", postedUser.Name);
        return CreatedAtAction(nameof(Get), new { id = postedUser.Id }, postedUser);
    }

    [HttpPut("{id}")]
    public async System.Threading.Tasks.Task<ActionResult> Update(int id, User newUser)
    {
        var currentUserId = int.Parse(User.FindFirst("userid")?.Value ?? "0");
        var isAdmin = User.FindFirst("usertype")?.Value == "Admin";

        if (currentUserId != id && !isAdmin)
            return Forbid();

        var user = userService.find(id);
        if (user == null)
            return NotFound();
        newUser.Id = id;
        if (!userService.Update(id, newUser))
            return BadRequest();
        var username = User.FindFirst("username")?.Value ?? "system";
        await _activityRepository.BroadcastAsync(username, "updated", newUser.Name);
        return Ok(newUser);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Admin")]
    public async System.Threading.Tasks.Task<ActionResult> Delete(int id)
    {
        var user = userService.find(id);
        if (user == null)
            return NotFound();

        // remove user's ice creams first
        try
        {
            _iceCreamService?.DeleteByUserId(id);
        }
        catch
        {
            // if ice cream deletion fails, continue to attempt user deletion
        }

        if (!userService.Delete(id))
            return NotFound();
        var username = User.FindFirst("username")?.Value ?? "system";
        await _activityRepository.BroadcastAsync(username, "deleted", user.Name);
        return Ok(user);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public ActionResult Login(LoginRequest request)
    {
        Console.WriteLine($"Login attempt: Name='{request.Name}', Password='{request.Password}'");
        var user = userService.Login(request.Name, request.Password);
        Console.WriteLine($"User found: {user != null}");
        if (user != null)
        {
            Console.WriteLine($"User: {user.Name}, Password: {user.Password}");
        }
        if (user == null)
            return Unauthorized();

        // קביעת usertype לפי שם המשתמש (Admin או User)
        var userType = user.Name == "admin" || user.Name == "sari Rabinovitch" ? "Admin" : "User";

        var claims = new List<Claim>
        {
            new Claim("username", user.Name),
            new Claim("userid", user.Id.ToString()),
            new Claim("usertype", userType)
        };
        var token = UserTokenService.GetToken(claims);
        var tokenString = UserTokenService.WriteToken(token);
        return Ok(new { token = tokenString });
    }

}