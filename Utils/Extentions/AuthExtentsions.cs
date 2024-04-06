using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Utils.Exceptions;

namespace Utils.Extentions
{
    public static class AuthExtentsions
    {
        public static Claim GetClaim(this IEnumerable<Claim> claims, string claimType)
        {
            Claim? result = claims.FirstOrDefault(claim => claim.Type == claimType);
            if (result == null) throw new Exception($"No Claim with Type {claimType} in token");

            return result;
        }
        public static IEnumerable<Claim> GetClaims(this IEnumerable<Claim> claims, string claimType)
        {
            var result = claims.Where(claim => claim.Type == claimType).AsEnumerable();
            if (!result.Any()) throw new Exception($"No Claim with Type {claimType} in token");

            return result;
        }
    }
}
