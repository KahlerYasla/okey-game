using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotnetProjects.Models;
using System.Text.Json;


[ApiController]
[Route("[controller]")]
public class AuthController : Controller
{
    private readonly ILogger<AuthController> _logger;
    private readonly OkeyPlusApiDbContext _context;

    // inject the services
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;

    public AuthController(ILogger<AuthController> logger, OkeyPlusApiDbContext context, IAuthService authService, ITokenService tokenService)
    {
        _logger = logger;
        _context = context;
        _tokenService = tokenService;
        _authService = authService;
    }


    // Post: /Auth
    // login user and return the user data and token
    // and get the specific user from the database
    [HttpPost("LoginUser")]
    [AllowAnonymous]
    public async Task<ActionResult<GenericResponse<object>>> LoginUserAsync([FromBody] UserLoginRequest request)
    {
        // call the login service and get the result
        GenericResponse<User> genericResponse = await _authService.LoginUserAsync(request);

        // if result is true then generate token and return all the data and token as response message
        if (genericResponse != null)
        {
            // write the result to the console with the json format
            Console.WriteLine(JsonSerializer.Serialize(genericResponse));

            if (genericResponse.Result == false)
            {
                return BadRequest("Invalid username or password");
            }

            // null checks
            if (genericResponse.Data == null)
            {
                return BadRequest("User not found");
            }
            else if (genericResponse.Data.Name == null)
            {
                return BadRequest("User's name not found");
            }

            // generate token
            var token = _tokenService.GenerateToken(new GenerateTokenRequest
            {
                UserID = genericResponse.Data!.Id,
            });

            // add the token to the response data with appending the data
            var Data = new
            {
                genericResponse.Data,
                token.Result.Token
            };

            GenericResponse<object> response = new()
            {
                Data = Data,
                Result = true,
                Message = "User logged in successfully"
            };

            // return the response message


            return Ok(response);
        }
        else
        {
            return BadRequest("Invalid username or password");
        }
    }

    // Post: /Auth
    // register user and return the user data and token
    [HttpPost("RegisterUser")]
    [AllowAnonymous]
    public async Task<ActionResult<GenericResponse<object>>> RegisterUserAsync([FromBody] User request)
    {
        // call the register service and get the result
        GenericResponse<User> result = await _authService.RegisterUserAsync(request);

        // if result is true then generate token and return all the data and token as response message
        if (result != null)
        {
            if (result.Result == false)
            {
                return BadRequest("result is false");
            }

            // null checks
            if (result.Data == null)
            {
                return BadRequest("User not found");

            }
            else if (result.Data.Name == null)
            {
                return BadRequest("User's name not found");
            }

            // check if the password is strong enough (at least 5 characters)
            if (result.Data.Password!.Length < 5)
            {
                return BadRequest("Password must be at least 5 characters");
            }

            var token = _tokenService.GenerateToken(new GenerateTokenRequest
            {
                UserID = result.Data.Id
            });

            // add the token to the response data with appending the data
            var Data = new
            {
                result.Data,
                token.Result.Token
            };

            GenericResponse<object> response = new()
            {
                Data = Data,
                Result = true,
                Message = "User registered successfully"
            };

            return Ok(response);
        }
        else
        {
            return BadRequest("result is null");
        }
    }

    // Post: /Auth
    // verify user and return the user data and token
    [HttpPost("VerifyUser")]
    [AllowAnonymous]
    public async Task<ActionResult<GenericResponse<User>>> VerifyUserAsync([FromBody] VerificationRequest verificationRequest)
    {
        // get the user id from the token
        var userId = verificationRequest.UserId;

        // if verification code is matched in database then verify the user and return response message
        if (verificationRequest != null)
        {
            // null checks
            if (verificationRequest.FourDigitVerificationCode == 0)
            {
                return BadRequest("Verification code not found");
            }
            else if (userId == null)
            {
                return BadRequest("User's id not found");
            }

            // check if the verification code is correct
            if (_context.Verifications!.Any(x => x.UserInstance.Id == userId
            && x.FourDigitVerificationCode == verificationRequest.FourDigitVerificationCode))
            {
                // get the user from the database
                var user = await _context.Users!.FirstOrDefaultAsync(x => x.Id == userId);

                // verify the user
                user!.IsVerified = true;

                // update the user in the database
                _context.Users!.Update(user);

                // save the changes
                await _context.SaveChangesAsync();

                InitializeSettings(user.Id);

                GenericResponse<User> response = new()
                {
                    Data = null,
                    Result = true,
                    Message = "User verified successfully"
                };

                // return the response message
                return response;
            }
            else
            {
                return BadRequest("Invalid verification code");
            }
        }
        else
        {
            return BadRequest("Invalid verification code");
        }

        void InitializeSettings(string userId)
        {
            Settings settings = new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                FriendRequest = true,
                Chat = true,
                GameInvite = true,
                HandCount = 1,
                SingleOrPaired = 0,
                GameHelp = 0,
                Folded = 0,
            };

            _context.Settings!.Add(settings);
            _context.SaveChanges();
        }
    }


    // Post: /Auth
    // send password reset key code to the user's email address
    [HttpPost("SendPasswordResetKey/:email")]
    [AllowAnonymous]
    public async Task<ActionResult<GenericResponse<object>>> SendPasswordResetKeyAsync(String email)
    {
        // if user exists then send password reset key to the user's email address
        if (email != null)
        {
            // null checks
            if (email == null)
            {
                return BadRequest("Email address not found");
            }

            // check if the user exists
            if (_context.Users!.Any(x => x.EmailAddress == email))
            {
                // get the user from the database
                var user = await _context.Users!.FirstOrDefaultAsync(x => x.EmailAddress == email);

                // generate a random 10 char password reset key
                string passwordResetKey = GenerateRandomPasswordResetKey();

                // get the current date and time
                DateTime currentDateTime = DateTime.Now;
                currentDateTime = currentDateTime.ToUniversalTime();

                // get the current date and time and add 1 hour to it
                DateTime expiresAt = currentDateTime.AddHours(1);
                expiresAt = expiresAt.ToUniversalTime();

                // create a new password reset key object
                PasswordResetKey passwordResetKeyObject = new()
                {
                    Key = passwordResetKey,
                    CreatedAt = currentDateTime,
                    ExpiresAt = expiresAt,
                    UserId = user!.Id
                };

                // add the password reset key to the database
                _context.PasswordResetKeys!.Add(passwordResetKeyObject);

                // save the changves
                await _context.SaveChangesAsync();

                // send the password reset key to the user's email address
                _authService.SendPasswordResetKey(passwordResetKey, email);

                GenericResponse<object> response = new()
                {
                    Data = null,
                    Result = true,
                    Message = "Password reset key sent successfully"
                };

                return response;
            };
        }

        return BadRequest("User not found");

        static string GenerateRandomPasswordResetKey()
        {
            // generate a random 10 char password reset key
            return Guid.NewGuid().ToString()[..4];
        }

    }

    // Post: /Auth
    // reset user's password
    [HttpPost("ResetPassword")]
    [AllowAnonymous]
    public async Task<ActionResult<GenericResponse<object>>> ResetPasswordAsync([FromBody] ResetPasswordRequest resetPasswordRequest)
    {
        // if user exists then reset user's password
        if (resetPasswordRequest != null)
        {
            // null checks
            if (resetPasswordRequest.PasswordResetKey == null)
            {
                return BadRequest("Password reset key not found");
            }
            else if (resetPasswordRequest.NewPassword == null)
            {
                return BadRequest("New password not found");
            }

            // check if the password reset key exists
            if (_context.PasswordResetKeys!.Any(x => x.Key == resetPasswordRequest.PasswordResetKey))
            {
                // get the password reset key from the database
                var passwordResetKey = await _context.PasswordResetKeys!.FirstOrDefaultAsync(x => x.Key == resetPasswordRequest.PasswordResetKey);

                // check if the password reset key is expired
                if (passwordResetKey!.ExpiresAt < DateTime.Now.ToUniversalTime())
                {
                    return BadRequest("Password reset key is expired");
                }

                // get the user from the database
                var user = await _context.Users!.FirstOrDefaultAsync(x => x.Id == passwordResetKey.UserId);

                // update the user's password
                user!.Password = resetPasswordRequest.NewPassword;

                // update the user in the database
                _context.Users!.Update(user);

                // save the changes
                await _context.SaveChangesAsync();

                GenericResponse<object> response = new()
                {
                    Data = null,
                    Result = true,
                    Message = "Password reset successfully"
                };

                // return the response message
                return response;
            }
            else
            {
                return BadRequest("Invalid password reset key");
            }
        }
        else
        {
            return BadRequest("Invalid password reset key");
        }

    }
}