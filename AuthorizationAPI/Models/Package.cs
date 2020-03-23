using System.Collections.Generic;

namespace AuthorizationAPI.Models
{
    public enum PackageType
    {
        Exam = 1,
        Quiz = 2,
        Other = 3
    }


    public class Package
    {
        public decimal CandidateId;
        public decimal CandidatePackageId;
        public string DemoLink;
        public string Description;
        public Portal.Models.PackageConfiguration PackageConfiguration;
        public decimal PackageId;
        public string PackageInstructions;
        public List<PackageResult> PackageResults;
        public List<PackageSite> PackageSites;
        public PackageState PackageState = new PackageState();
        public PackageType PackageType;
        public int PackageDuration { get; set; }
    }


    public class PackageResult
    {
        public string Display;
        public decimal ExamResultsId;
        public string RedirectUrl { get; set; }
        public string ResultStatus { get; set; }
    }

    public class PackageSite
    {
        public bool Enabled;
        public decimal SiteId;
        public string SiteName;
    }
}