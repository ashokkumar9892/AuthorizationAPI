using System.Collections.Generic;

namespace AuthorizationAPI.Models
{
    public class PackageConfigurationDto
    {
        public bool LaunchReady = false;
        public List<PackageLaunchRule> PackageLaunchRules = new List<PackageLaunchRule>();
    }

    public class PackageLaunchRule
    {
        public ILaunchRuleData LaunchRuleData;
        public bool RuleCompleted;
        public RuleTypeEnum RuleType;
    }

    public enum RuleTypeEnum
    {
        ProctorLogin = 1,
        ExamAccessCode = 2,
        Attestation = 3,
        LaunchInstructions = 4,
        GenerateAccessCodeRule = 5,
        SiteRestrictions = 6,
        TestingWindowRestrictions = 7,
        OwnerRestrictions = 8,
        RetakesAllowed = 9,
        OnlyOneExamInProgress = 10,
        SecureExamLogOut = 11,
        EligibilityLaunchDate = 12,
        Scheduling = 13,
        LockdownBrowser = 14,
        ResultsAccess = 15
    }

    public class AttestationLaunchRuleData : ILaunchRuleData
    {
        public string AttestationRichText;
    }

    public interface ILaunchRuleData
    {
    }
}