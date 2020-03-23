using AuthorizationAPI.Models;
using CoreUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationAPI.Services
{
    public class PackageConfigurationService
    {
        // TODO make this state a generic Interface per rule set, that can validate, and add to state, DTO etc as its own implementation 
        private const string _cachePrefix = "package-configuration-state:";


        private readonly IServiceProvider _serviceProvider;

        public PackageConfigurationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static Portal.Models.PackageConfiguration GetDefaultConfiguration()
        {
            return new Portal.Models.PackageConfiguration();
        }

        public Portal.Models.PackageConfiguration ApplyPackageConfigurationInheritance(
            List<Portal.Models.PackageConfiguration> packageConfigurations)
        {
            var finalPackageConfiguration = packageConfigurations[0];

            for (var i = 1; i < packageConfigurations.Count; i++)
                ObjectUtility.MergeObjects(finalPackageConfiguration, packageConfigurations[i],
                    new Dictionary<string, string>
                    {
                        {"IRule", "RuleId"},
                        {"AllowedTestingWindow", "TestingWindowId"},
                        {"ScheduledTime", "Time"}
                    });

            return finalPackageConfiguration;
        }

        public Portal.Models.PackageConfiguration DeserializePackageConfiguration(string packageConfigurationJson)
        {
            return JsonConvert.DeserializeObject<Portal.Models.PackageConfiguration>(packageConfigurationJson);
        }

        public async Task<PackageConfigurationDto> ConvertConfigurationToConfigurationDto(Package package)
        {
            var packageConfigurationDto = new PackageConfigurationDto();
            if (package.PackageConfiguration.LaunchRules != null)
            {
                // Proctor Login Required
                if (package.PackageConfiguration.GetLaunchRule((int) RuleTypeEnum.ProctorLogin)?.Enabled == true)
                    packageConfigurationDto.PackageLaunchRules.Add(new PackageLaunchRule
                    {
                        RuleType = RuleTypeEnum.ProctorLogin,
                        RuleCompleted = package.PackageState != null && package.PackageState.LaunchRuleStates
                                            .FirstOrDefault(r => r.RuleTypeId == (int) RuleTypeEnum.ProctorLogin)
                                            ?.Complete == true
                    });
                // Access Code Required
                if (package.PackageConfiguration.GetLaunchRule((int) RuleTypeEnum.ExamAccessCode)?.Enabled != null)
                    packageConfigurationDto.PackageLaunchRules.Add(new PackageLaunchRule
                    {
                        RuleType = RuleTypeEnum.ExamAccessCode,
                        RuleCompleted = package.PackageState != null && package.PackageState.LaunchRuleStates
                                            .FirstOrDefault(r => r.RuleTypeId == (int) RuleTypeEnum.ExamAccessCode)
                                            ?.Complete == true
                    });
            }

            if (package.PackageState != null) return packageConfigurationDto;

            // create the base state since we don't have one
            package.PackageState = new PackageState
            {
                LaunchRuleStates = new List<LaunchRuleState>()
            };

            foreach (var packageLaunchRule in packageConfigurationDto.PackageLaunchRules)
                package.PackageState.LaunchRuleStates.Add(new LaunchRuleState
                {
                    Complete = false,
                    RuleTypeId = (int) packageLaunchRule.RuleType
                });

            return packageConfigurationDto;
        }

        public PackageState CreateDefaultPackageConfigurationState(
            Portal.Models.PackageConfiguration packageConfiguration)
        {
            // create the base state since we don't have one
            var packageConfigurationState = new PackageState
            {
                LaunchRuleStates = new List<LaunchRuleState>()
            };
            foreach (var packageLaunchRule in packageConfiguration.LaunchRules)
                packageConfigurationState.LaunchRuleStates.Add(new LaunchRuleState
                {
                    Complete = false,
                    RuleTypeId = packageLaunchRule.RuleId
                });

            return packageConfigurationState;
        }

        public class CandidatePackageLaunchAccessRequest
        {
            public string Data;
            public int RuleId;
        }

        public class CandidatePackageLaunchAccessResponse
        {
            public string Data;
            public bool LaunchReady;
        }

        public class CandidatePackageEligibilityRequest
        {
            public string Data;
            public int RuleId;
        }
    }
}