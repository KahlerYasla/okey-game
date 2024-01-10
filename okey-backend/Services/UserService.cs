using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetProjects.Models;

public class UserService : IUserService
{
    private readonly OkeyPlusApiDbContext _context;

    public UserService(OkeyPlusApiDbContext context)
    {
        _context = context;
    }

    public Task<User> GetUserInformationByIdAsync(GetUserInformationByIdRequest request)
    {
        var loaded = _context.Users!.Where(x => x.Id == request.Id).FirstOrDefault() ?? throw new Exception("User not found");
        return Task.FromResult(loaded);
    }
}
