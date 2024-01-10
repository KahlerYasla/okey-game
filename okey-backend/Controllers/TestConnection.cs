using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


[ApiController]
[Route("[controller]")]
public class TestConnection : Controller
{
    public TestConnection(IUserService userService)
    {
    }

    [HttpGet("Test")]
    [AllowAnonymous]
    public ActionResult<object> TEST()
    {
        string status = "Connection is successful.";
        return Ok(status);
    }

}