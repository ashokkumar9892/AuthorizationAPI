using System;
using System.Linq;
using System.Threading.Tasks;
using EducationPortalModel;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationAPI.Repositories
{
    public class UserRepository
    {
        private readonly EducationPortalModel.EducationPortalModel _educationPortalModel;

        public UserRepository(EducationPortalModel.EducationPortalModel educationPortalModel)
        {
            _educationPortalModel = educationPortalModel;
        }

        public async Task<EDUCATION_PORTAL_USER> GetUserByFirebaseId(string firebaseId)
        {
            return await _educationPortalModel.EDUCATION_PORTAL_USERs.Where(epu => epu.FIREBASEID == firebaseId)
                .Include(epu => epu.EDUCATION_PORTAL_USER_ACCESSES)
                .ThenInclude(epua => epua.CANDIDATE)
                .FirstOrDefaultAsync();
        }

        public async Task<EDUCATION_PORTAL_USER_ACCESS> GetUserByFirebaseIdAndInstitutionId(string firebaseId,
            string institutionGuid)
        {
            return await _educationPortalModel.EDUCATION_PORTAL_USER_ACCESSES.Where(epua =>
                    epua.EDUCATION_PORTAL_USER.FIREBASEID == firebaseId && epua.INSTITUTION_GUID == institutionGuid)
                .Include(epua => epua.EDUCATION_PORTAL_USER)
                .Include(epua => epua.CANDIDATE)
                .FirstOrDefaultAsync();
        }

        public async Task<EDUCATION_PORTAL_USER_ACCESS> GetCandidateIdByUserId(decimal userId)
        {
            return await _educationPortalModel.EDUCATION_PORTAL_USER_ACCESSES.Where(epua => epua.USERID == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<EDUCATION_PORTAL_USER> GetUserByFirstNameAndEmail(string firstname, string username)
        {
            return await _educationPortalModel.EDUCATION_PORTAL_USERs.Where(epu =>
                    epu.FIRST_NAME.ToLower() == firstname.ToLower() &&
                    epu.EMAIL_ADDRESS.ToLower() == username.ToLower())
                .FirstOrDefaultAsync();
        }

        public async Task SetPasswordResetTimeStamp(decimal userId)
        {
            var user = await _educationPortalModel.EDUCATION_PORTAL_USERs.Where(epu => epu.USERID == userId)
                .FirstOrDefaultAsync();
            user.PASSWORD_RESET = DateTime.Now;
            await UpdatePortalUser(user);
        }

        public async Task<EDUCATION_PORTAL_USER> GetUserByUserId(decimal userId)
        {
            return await _educationPortalModel.EDUCATION_PORTAL_USERs.Where(epu => epu.USERID == userId)
                .FirstOrDefaultAsync();
        }

        public async Task UpdatePortalUser(EDUCATION_PORTAL_USER educationPortalUser)
        {
            _educationPortalModel.EDUCATION_PORTAL_USERs.Update(educationPortalUser);
            await _educationPortalModel.SaveChangesAsync();
        }
    }
}