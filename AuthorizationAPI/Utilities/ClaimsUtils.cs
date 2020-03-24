using AuthorizationAPI.Models;
using AuthorizationAPI.Services;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Configuration;
using NetCore3_0Authorization;
using NetCore3_0AuthorizationModels;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthorizationAPI.Utilities
{
    public class ClaimsUtils
    {
        private readonly IConfiguration _config;
        private readonly UserService _userService;

        public ClaimsUtils(IConfiguration config, UserService userService)
        {
            _config = config;
            _userService = userService;
        }

        public async Task<UserClaims> GetClaimsPrincipal(FirebaseToken decodedToken, string userAuthType,
            string institutionGuid)
        {
            try
            {
                var userAccess =
                    await _userService.GetUserByFirebaseIdAndInstitutionGuid(decodedToken.Uid, institutionGuid);
                // create student token
                var adminUserAuth = new AdminUserAuth
                {
                    UserType = userAuthType,
                    InstitutionGuid = institutionGuid,
                    FirstName = userAccess.EDUCATION_PORTAL_USER.FIRST_NAME,
                    LastName = userAccess.EDUCATION_PORTAL_USER.LASTNAME,
                    UserId = userAccess.USERID,
                    UserAccessId = userAccess.USER_ACCESS_ID,
                    LanguageId = userAccess.EDUCATION_PORTAL_USER.PREFERREDLANGUAGECODEID != null
                        ? (int) userAccess.EDUCATION_PORTAL_USER.PREFERREDLANGUAGECODEID
                        : 53
                };


                return new UserClaims
                {
                    ClaimsPrincipal = GenericUserClaimsService.CreateUserClaimsPrincipal(adminUserAuth),
                    User = adminUserAuth
                };
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

    public class UserClaims
    {
        public ClaimsPrincipal ClaimsPrincipal;
        public IUserAuth User;
    }
}