using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using NetCore3_0AuthorizationModels;
using Newtonsoft.Json;

namespace NetCore3_0Authorization
{
    public static class GenericUserClaimsService
    {
        public static Dictionary<string, Type> RoleToUserAuthTypes = new Dictionary<string, Type>();

        public static ClaimsPrincipal CreateUserClaimsPrincipal(IUserAuth userAuth, List<Claim> additionalClaims = null)
        {
            if (userAuth?.UserType == null)
                throw new Exception("Cannot create security token, user is null or user type unknown.");
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name,
                ClaimTypes.Role);

            var claims = new List<Claim>
            {
                // always in this strict order
                new Claim(ClaimTypes.Role, userAuth.UserType),
                new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(userAuth, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }))
            };
            if (additionalClaims?.Any() == true) claims.AddRange(additionalClaims);

            identity.AddClaims(claims);
            // generate auth principal
            return new ClaimsPrincipal(identity);
        }

        public static IUserAuth GetUserFromClaims(List<Claim> claims)
        {
            var userRole = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ??
                           throw new InvalidOperationException();
            if (!RoleToUserAuthTypes.ContainsKey(userRole))
                throw new InvalidOperationException($"No user type class definition type defined for role {userRole}");

            var userType = RoleToUserAuthTypes[userRole];

            var userJson = claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData)?.Value ??
                           throw new InvalidOperationException();
            var user = JsonConvert.DeserializeObject(userJson, userType) as IUserAuth;
            return user;
        }
    }
}