
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

    public UserController(IUserService userService)
    {
        this.userService = userService;
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
    public ActionResult Create(User newUser)
    {
        var postedUser = userService.Create(newUser);
        return CreatedAtAction(nameof(Get), new { id = postedUser.Id }, postedUser);
    }

    [HttpPut("{id}")]
    public ActionResult Update(int id, User newUser)
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
        return Ok(newUser);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Admin")]
    public ActionResult Delete(int id)
    {
        var user = userService.find(id);
        if (user == null)
            return NotFound();
        if (!userService.Delete(id))
            return NotFound();
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
            // custom claims used by the app
            new Claim("username", user.Name),
            new Claim("userid", user.Id.ToString()),
            new Claim("usertype", userType),
            // standard claims so ASP.NET Core and SignalR can identify the user
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name)
        };
        var token = FbiTokenService.GetToken(claims);
        var tokenString = FbiTokenService.WriteToken(token);
        return Ok(new { token = tokenString });
    }

}