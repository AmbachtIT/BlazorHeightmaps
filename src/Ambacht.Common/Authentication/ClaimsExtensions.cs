using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Authentication
{
    public static class ClaimsExtensions
    {

        public static string GetEmail(this ClaimsPrincipal principal)
        {
            return principal.GetClaim(ClaimTypes.Email) ?? principal.GetClaim("emailaddress");
        }

        public static string GetName(this ClaimsPrincipal principal)
        {
            return principal.GetClaim(ClaimTypes.Name) ?? principal.GetClaim("name");
        }

        public static string GetClaim(this ClaimsPrincipal principal, string type)
        {
            return principal.Claims.Where(c => c.Type == type).Select(c => c.Value).FirstOrDefault();
        }


        public static void WriteClaimsPrincipal(this TextWriter writer, ClaimsPrincipal principal)
        {
            writer.WriteLine("".PadRight(40, '-'));
            writer.WriteLine("IDENTITIES:");
            foreach (var identity in principal.Identities)
            {
                writer.WriteLine($"Name: {identity.Name}");
                writer.WriteLine($"- AuthenticationType: {identity.AuthenticationType}");
                writer.WriteLine($"- IsAuthenticated: {identity.IsAuthenticated}");
                writer.WriteLine($"- Claims:");
                foreach (var claim in identity.Claims)
                {
                    writer.WriteLine($"  - {claim.Type}: {claim.Value}");
                }
                writer.WriteLine();
            }
            writer.WriteLine("".PadRight(40, '-'));
        }


    }
}
