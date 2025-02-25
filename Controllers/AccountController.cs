using Microsoft.AspNetCore.Mvc;

namespace esn_dictionary.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    [HttpGet("index")]
    public IActionResult Index()
    {
       return Ok("Account Controller");
    }

    [HttpPost("login")]
    public IActionResult Login()
    {
        return Ok("Account Controller");
    }
}