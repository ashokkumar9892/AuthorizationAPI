using AuthorizationAPI.Services;
using Cache.Infrastructure;
using EducationPortalModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationAPI.Repositories
{
    public class InstitutionRepository
    {
        private readonly ICache _cache;
        private readonly IConfiguration _configuration;
        private readonly EducationPortalModel.EducationPortalModel _educationPortalModel;

        public InstitutionRepository(EducationPortalModel.EducationPortalModel educationPortalModel,
            IConfiguration configuration, PackageConfigurationService packageConfigurationService, ICache cache)
        {
            _educationPortalModel = educationPortalModel;
            _configuration = configuration;
            _cache = cache;
        }

        public async Task<INSTITUTION> GetInstitutionByHostName(string hostName)
        {
            var cacheKey = $"portal-institution-{hostName}";
            var institutionJson = await _cache.GetCache(cacheKey);
            if (!string.IsNullOrWhiteSpace(institutionJson))
                return JsonConvert.DeserializeObject<INSTITUTION>(institutionJson);

            var institution = await _educationPortalModel.INSTITUTIONs.Where(i => i.HOST == hostName).Select(i => i)
                .FirstOrDefaultAsync();
            if (institution == null) return null;

            await _cache.SetCache(cacheKey, JsonConvert.SerializeObject(institution));

            return institution;
        }

        public async Task<string> GetInstitutionConfigurationByHostName(string hostName)
        {
            hostName = hostName.ToLower().Trim();
            if (hostName.StartsWith("wwww.")) hostName = hostName.Substring(4);

            var configJson = await _educationPortalModel.INSTITUTIONs.Where(i => i.HOST.ToLower() == hostName.ToLower())
                .Select(i => i.BRANDING_CONFIGURATION).FirstOrDefaultAsync();
            return configJson ?? await _educationPortalModel.INSTITUTIONs.Where(i => i.HOST.ToLower() == "default")
                       .Select(i => i.BRANDING_CONFIGURATION).FirstOrDefaultAsync();
        }
    }
}