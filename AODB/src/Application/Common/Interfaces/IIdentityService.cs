using AODB.Application.Common.Models;

namespace AODB.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);
    Task<string?> GetUserIdByNameAsync(string username);
    Task<IList<string>> GetUserRolesAsync(string userId);
    Task<bool> IsInRoleAsync(string userId, string role);
    Task<bool> AuthorizeAsync(string userId, string policyName);
    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password, IList<string>? roles = null);
    Task<Result> DeleteUserAsync(string userId);
    Task<Result> AssignRoleAsync(string userId, string role);
    Task<Result> RemoveRoleAsync(string userId, string role);
    Task<Result> UpdateUserAsync(string userId, string? username = null, string? email = null);
    Task<bool> ValidateTokenAsync(string token);
}