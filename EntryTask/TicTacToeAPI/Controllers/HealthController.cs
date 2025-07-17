using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TicTacToeAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("Healthy");
}
