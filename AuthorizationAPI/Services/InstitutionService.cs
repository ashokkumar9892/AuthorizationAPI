using AuthorizationAPI.Repositories;
using EducationPortalModel;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace AuthorizationAPI.Services
{
    public class InstitutionService
    {
        private readonly string _hostDomainOverride;
        private readonly InstitutionRepository _institutionRepository;

        public InstitutionService(IConfiguration configuration,
            InstitutionRepository institutionRepository)
        {
            _institutionRepository = institutionRepository;

            try
            {
                if (!string.IsNullOrWhiteSpace(_hostDomainOverride))
                    return; // we are not dev or already have our override, so no need to replace this all the time

                _hostDomainOverride = configuration["DEV_ONLY_EducationPortalHostDomainOverride"];
                if (!string.IsNullOrWhiteSpace(_hostDomainOverride))
                    _hostDomainOverride = _hostDomainOverride.Trim().ToLower();
            }
            catch
            {
                // ignored
            }
        }

        public async Task<INSTITUTION> GetInstitutionByHost(string hostName)
        {
            var hostNameInUse = string.IsNullOrWhiteSpace(_hostDomainOverride)
                ? hostName.Trim().ToLower()
                : _hostDomainOverride;

            // strip out the www.
            hostNameInUse = hostNameInUse.Replace("www.", "");

            return await _institutionRepository.GetInstitutionByHostName(hostNameInUse);
        }

        public async Task<string> GetInstitutionBrandingConfigurationByHost(string hostName)
        {
            var institution = await GetInstitutionByHost(hostName);
            return institution.BRANDING_CONFIGURATION;
        }
    }
}