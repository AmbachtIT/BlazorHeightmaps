using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ambacht.Common.Authentication
{

    public class AuthInfo // structure based on sample here: https://cgillum.tech/2016/03/07/app-service-token-store/
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("provider_name")]
        public string ProviderName { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("user_claims")]
        public List<AuthUserClaim> UserClaims { get; set; }

        [JsonPropertyName("access_token_secret")]
        public string AccessTokenSecret { get; set; }

        [JsonPropertyName("authentication_token")]
        public string AuthenticationToken { get; set; }

        [JsonPropertyName("expires_on")]
        public string ExpiresOn { get; set; }

        [JsonPropertyName("id_token")]
        public string IdToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }



        public static AuthInfo FromPrincipal(ClaimsPrincipal principal)
        {
            var result = new AuthInfo();
            var identity = principal?.Identity;
            if (identity != null)
            {
                result.ProviderName = identity.AuthenticationType;
                result.UserClaims =
                    principal
                        .Claims
                        .Select(c => new AuthUserClaim()
                        {
                            Type = c.Type,
                            Value = c.Value
                        })
                        .ToList();
            }
            return result;
        }


        public ClaimsPrincipal CreatePrincipal()
        {
            List<Claim> userClaims = new List<Claim>();
            foreach (AuthUserClaim userClaim in UserClaims)
            {
                userClaims.Add(new Claim(userClaim.Type, userClaim.Value));
            }

            var nameClaim = ClaimTypes.Name;
            if (userClaims.All(c => c.Type != nameClaim))
            {
                nameClaim = "name";
            }

            var identity = new ClaimsIdentity(claims: userClaims, nameType: nameClaim, authenticationType: ProviderName, roleType: null);
            return new ClaimsPrincipal(identity);
        }
    }
    public class AuthUserClaim
    {
        [JsonPropertyName("typ")]
        public string Type { get; set; }
        [JsonPropertyName("val")]
        public string Value { get; set; }
    }

    public class AuthToken
    {
        [JsonPropertyName("authenticationToken")]
        public string AuthenticationToken { get; set; }

        [JsonPropertyName("user")]
        public AuthenticationUser User { get; set; }
    }
    public class AuthenticationUser
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }
    }
}
