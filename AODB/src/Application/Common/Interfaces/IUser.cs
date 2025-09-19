namespace AODB.Application.Common.Interfaces;

public interface IUser
{
    string? Id { get; }
    string? Username { get; }
    string? Email { get; }
    IList<string> Roles { get; }
    bool IsAuthenticated { get; }
}