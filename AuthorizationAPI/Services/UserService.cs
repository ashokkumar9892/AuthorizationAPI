using AuthorizationAPI.Repositories;
using EducationPortalModel;
using FirebaseAdmin.Auth;
using System;
using System.Threading.Tasks;

namespace AuthorizationAPI.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public int StudentTokenGoodForHours { get; } = 12;


        public async Task<EDUCATION_PORTAL_USER> VerifyAndReturnUser(string token)
        {
            try
            {
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);

                return await GetUserByFirebaseId(decodedToken.Uid);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<EDUCATION_PORTAL_USER_ACCESS> GetUserByFirebaseIdAndInstitutionGuid(string firebaseId,
            string institutionGuid)
        {
            var userAccess = await _userRepository.GetUserByFirebaseIdAndInstitutionId(firebaseId, institutionGuid);
            if (userAccess == null)
                throw new Exception("Unable to find user account information");

            return userAccess;
        }

        public async Task<EDUCATION_PORTAL_USER> GetUserByFirebaseId(string firebaseId)
        {
            var user = await _userRepository.GetUserByFirebaseId(firebaseId);
            if (user == null)
                throw new Exception("Unable to find user account information");

            return user;
        }

        public async Task<decimal> GetCandidateIdByUserId(decimal userId)
        {
            var userAccess = await _userRepository.GetCandidateIdByUserId(userId);
            if (userAccess == null)
                throw new Exception("Unable to find user access information");

            return (decimal) userAccess.CANDIDATEID;
        }

        public async Task<EDUCATION_PORTAL_USER> CheckUserExists(string firstname, string username)
        {
            var user = await _userRepository.GetUserByFirstNameAndEmail(firstname, username);
            if (user == null)
                throw new Exception("Unable to find user under that first name and email");
            return user;
        }

        public async Task SetPasswordResetTimeStamp(decimal userId)
        {
            await _userRepository.SetPasswordResetTimeStamp(userId);
        }
    }
}