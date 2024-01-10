

public interface IAuthService
{
    public Task<GenericResponse<User>> LoginUserAsync(UserLoginRequest request);
    public Task<GenericResponse<User>> RegisterUserAsync(User request);
    public void SendPasswordResetKey(string passwordResetKey, string email);

}