namespace Soa.UserService.Exceptions;

public class UserServiceException : Exception
{
    public string Type { get; }

    public UserServiceException(string type, string message) : base(message)
    {
        Type = type;
    }
}
