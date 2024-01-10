using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using DotnetProjects.Models;

[ApiController]
[Route("[controller]")]
public class UserController : Controller
{
    readonly IUserService userService;

    public UserController(IUserService userService)
    {
        this.userService = userService;
    }

    [HttpPost("GetUserInformationById")]
    [Authorize]
    public async Task<ActionResult<User>> GetUserInformationById([FromBody] GetUserInformationByIdRequest request)
    {
        var result = await userService.GetUserInformationByIdAsync(request);

        return result;
    }

}