using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("[controller]")]
public class ProfileController : Controller
{
    private readonly ILogger<ProfileController> _logger;
    private readonly OkeyPlusApiDbContext _context;
    readonly HostingEnvironment _webHostEnvironment = new();

    public ProfileController(ILogger<ProfileController> logger, OkeyPlusApiDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    // GET: /profile
    // get user's profile picture 
    [HttpGet("GetProfilePicture")]
    [Authorize]
    public ActionResult<GenericResponse<object>> GetPost()
    {
        // get user id from token
        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken token = handler.ReadJwtToken(Request.Headers["Authorization"].ToString().Split(" ")[1]);

        string userId = token.Claims.ToArray()[0].Value;

        HttpContext.Response.Headers.Add("Accept-Ranges", "bytes");

        // get profile picture's path
        string profilePicturesDirectory = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources", "ProfilePictures");

        // get the extension of the profile picture
        string? extension = Path.GetExtension(_context.Users!.FirstOrDefault(u => u.Id == userId)!.ProfileImagePath);

        // null check
        if (extension == null)
        {
            return BadRequest("Profile picture not exists with this user id.");
        }

        // directory + userId + extension
        string profilePicturePath = Path.Combine(profilePicturesDirectory, userId + extension);

        Byte[] b;
        try
        {
            b = System.IO.File.ReadAllBytes(path: profilePicturePath);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

        // generate response
        GenericResponse<object> response = new()
        {
            Data = File(b, extension == ".jpg" ? "image/jpeg" : "image/png"),
            Result = true,
            Message = "Profile picture fetched successfully"
        };

        return Ok(response);

    }


    [HttpPost("UpdateProfilePicture")]
    [Authorize]
    public ActionResult<GenericResponse<object>> UpdateProfilePicture()
    {
        // get user id from token
        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken token = handler.ReadJwtToken(Request.Headers["Authorization"].ToString().Split(" ")[1]);

        string userId = token.Claims.ToArray()[0].Value;

        // get profile picture from request body 
        try
        {
            var httpRequest = Request.Form;
            var postedFile = httpRequest.Files[0];

            // Ensure the ProfilePictures directory exists, if not, create itent
            var profilePicturesDirectory = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources", "ProfilePictures");
            Directory.CreateDirectory(profilePicturesDirectory);

            string filename = userId + Path.GetExtension(postedFile.FileName);
            var physicalPath = Path.Combine(profilePicturesDirectory, filename);

            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                postedFile.CopyTo(stream);
            }

            // save profile picture's path to database
            User user = _context.Users!.FirstOrDefault(u => u.Id == userId)!;
            user.ProfileImagePath = filename;
            _context.Users.Update(user);
            _context.SaveChanges();

            // generate response
            GenericResponse<object> response = new()
            {
                Data = null,
                Result = true,
                Message = "Profile picture updated successfully"
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest("An error occurred while uploading the image: " + ex.Message);
        }
    }

}