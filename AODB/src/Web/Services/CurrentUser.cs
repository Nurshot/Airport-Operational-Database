using System.Security.Claims;
using AODB.Application.Common.Interfaces;

namespace AODB.Web.Services;

public class CurrentUser : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? Id 
    {
        get
        {
            // .NET'in standart NameIdentifier claim'ini kullan (JWT'de sub'a karşılık gelir)

            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            //Console.WriteLine($"🔍 CurrentUser.Id: {userId ?? "NULL"}");
            return userId;
        }
    }
    
    public string? Username => _httpContextAccessor.HttpContext?.User?.FindFirstValue("preferred_username");
    
    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email) 
                                ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("email");
    
    public IList<string> Roles 
    {
        get
        {
            // Keycloak'tan gelen realm_access rollerini parse et
            var realmAccess = _httpContextAccessor.HttpContext?.User?.FindFirstValue("realm_access");
            if (!string.IsNullOrEmpty(realmAccess))
            {
                try
                {
                    var realmAccessObj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string[]>>(realmAccess);
                    if (realmAccessObj?.ContainsKey("roles") == true)
                    {
                        return realmAccessObj["roles"].ToList();
                    }
                }
                catch (System.Text.Json.JsonException)
                {
                    Console.WriteLine("🔍 realm_access parse error");
                }
            }
            
            return new List<string>();
        }
    }
    
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
