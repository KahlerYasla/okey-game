using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

[ApiController]
[Route("/Settings")]
[Authorize] // Requires authentication for all actions in this controller
public class SettingsController : ControllerBase
{
    private readonly ILogger<SettingsController> _logger;
    private readonly OkeyPlusApiDbContext _context;

    public SettingsController(ILogger<SettingsController> logger, OkeyPlusApiDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    // GET: api/settings
    [HttpGet("GetSettingsById")]
    public ActionResult<GenericResponse<object>> GetSettingsById()
    {
        // solve the userId from the token
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(Request.Headers["Authorization"].ToString().Split(" ")[1]);
        string userId = token.Claims.ToArray()[0].Value;

        // fetch settings from database
        var settings = FetchSettingsFromDatabase(userId);

        // generate response
        GenericResponse<Settings> response = new()
        {
            Data = settings.Result,
            Result = true,
            Message = string.Empty
        };


        // return settings
        return Ok(response);
    }

    // PUT: api/settings
    [HttpPut("UpdateSettings")]
    public ActionResult<GenericResponse<object>> UpdateSettings([FromBody] Settings request)
    {
        // update settings in database with request where userId matches
        var result = UpdateSettingsInDatabase(request);

        // generate response
        GenericResponse<object> response = new()
        {
            Data = null,
            Result = result,
            Message = result ? "Settings updated successfully" : "Settings update failed"
        };

        // return success or failure message
        return response;
    }


    private async Task<Settings> FetchSettingsFromDatabase(string userId)
    {
        // Fetch settings from database
        // Return settings if found, null otherwise
        try
        {
            var settings = await _context.Settings!.FirstOrDefaultAsync(s => s.UserId == userId);
            return settings!;
        }
        catch (Exception)
        {
            return null!;
        }
    }

    private bool UpdateSettingsInDatabase(Settings updatedSettings)
    {
        // Update settings from database
        // Return true if the update is successful, false otherwise
        try
        {
            _context.Settings!.Update(updatedSettings);
            _context.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
