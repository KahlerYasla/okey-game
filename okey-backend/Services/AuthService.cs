using System.Net;
using System.Net.Mail;


public class AuthService : IAuthService
{
    // database context
    private readonly OkeyPlusApiDbContext _context;

    // Configure email settings
    string smtpServer = "smtp.gmail.com";
    int smtpPort = 587;
    string smtpUsername = "garpayyasla@gmail.com";
    string smtpPassword = "lsymgmtjlrmfrefk";

    public AuthService(OkeyPlusApiDbContext context)
    {
        _context = context;
    }


    public Task<GenericResponse<User>> LoginUserAsync(UserLoginRequest request)
    {
        GenericResponse<User> response = new();

        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            throw new ArgumentNullException(nameof(request));
        }

        User user = _context.Users!.FirstOrDefault(u => u.Name == request.Username && u.Password == request.Password)!;
        // call the database and check if the user exists
        if (user != null)
        {
            response.Data = user;
            response.Result = true;
            response.Message = string.Empty;
        }

        return Task.FromResult(response);
    }

    public Task<GenericResponse<User>> RegisterUserAsync(User request)
    {
        GenericResponse<User> response = new();

        if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Password))
        {
            throw new ArgumentNullException(nameof(request));
        }

        string Id = Guid.NewGuid().ToString();

        response.Data = new User
        {
            Id = Id,
            Name = request.Name,
            Surname = request.Surname,
            EmailAddress = request.EmailAddress,
            Password = request.Password,
            PhoneNumber = request.PhoneNumber,
            IsVerified = false,
            Status = true,
            CreatedDate = DateTime.Now.ToUniversalTime(),
            Level = 0,
        };

        // check uniqueness of email address on the database
        if (_context.Users!.Any(u => u.EmailAddress == request.EmailAddress))
        {
            response.Data = null!;
            response.Result = false;
            response.Message = "Email address already exists.";
            return Task.FromResult(response);
        }

        response.Result = true;
        response.Message = string.Empty;

        // Send an email with the verification code
        SendVerificationEmail(response.Data!);

        // Call the database and register the user
        _context.Users!.Add(response.Data!);
        _context.SaveChanges();

        return Task.FromResult(response);
    }

    private void SendVerificationEmail(User user)
    {
        using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
        {
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

            string verificationCode = GenerateVerificationCode();

            MailMessage mailMessage = new()
            {
                From = new MailAddress("garpayyasla@gmail.com"),
                Subject = "Account Verification",
                Body = $"Your verification code is: {verificationCode}",
            };

            mailMessage.To.Add(user.EmailAddress!);

            try
            {
                smtpClient.Send(mailMessage);

                // save the verification code to the database
                Verification verification = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserInstance = user,
                    FourDigitVerificationCode = int.Parse(verificationCode),
                    ExpirationDate = DateTime.Now.AddMinutes(10).ToUniversalTime(),
                };

                _context.Verifications!.Add(verification);
            }
            catch (Exception ex)
            {
                // Handle any exceptions related to email sending here
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }

    private string GenerateVerificationCode()
    {
        // Generate and return a 4-digit verification code here
        Random random = new();
        int code = random.Next(1000, 9999);
        return code.ToString();
    }

    public void SendPasswordResetKey(string passwordResetKey, string email)
    {
        using SmtpClient smtpClient = new(smtpServer, smtpPort);

        smtpClient.EnableSsl = true;
        smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

        MailMessage mailMessage = new()
        {
            From = new MailAddress("garpayyasla@gmail.com"),
            Subject = "Account Verification",
            Body = $"Your reset password code is: {passwordResetKey}",
        };

        mailMessage.To.Add(email!);

        try
        {
            smtpClient.Send(mailMessage);
        }
        catch (Exception ex)
        {
            // Handle any exceptions related to email sending here
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }

}