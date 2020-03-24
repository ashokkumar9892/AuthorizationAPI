using NetCore3_0AuthorizationModels;

namespace AuthorizationAPI.Models
{
    public static class UserAuthType
    {
        public static string Admin => "Admin";
        public static string Proctor => "Proctor";
        public static string Teacher => "Teacher";
    }

    public class AdminUserAuth : UserAuth
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string InstitutionGuid { get; set; }
    }

    public class ProctorUserAuth : UserAuth
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string InstitutionGuid { get; set; }
    }
}