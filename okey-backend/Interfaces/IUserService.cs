using DotnetProjects.Models;

public interface IUserService
{
    public Task<User> GetUserInformationByIdAsync(GetUserInformationByIdRequest request);
}