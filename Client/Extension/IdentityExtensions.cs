using System.Security.Claims;
using System.Security.Principal;

namespace D69soft.Client.Extension
{
    public static class IdentityExtensions
    {
        public static string GetUserId(this IPrincipal user)
        {
            if (user == null)
                return string.Empty;

            var identity = (ClaimsIdentity)user.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            return claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
        }
    }
}
