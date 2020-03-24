using Newtonsoft.Json;

namespace NetCore3_0AuthorizationModels
{
    public class Role
    {
        public const string Admin = "Admin";
        public const string Teacher = "Teacher";
        public const string Proctor = "Proctor";
        public const string Student = "Student";
    }

    public interface IUserAuth
    {
        decimal UserId { get; set; }
        string InstitutionGuid { get; set; }
        decimal UserAccessId { get; set; }
        string Name { get; set; }
        string UserType { get; set; }
        int LanguageId { get; set; }
    }

    public class UserAuth : IUserAuth
    {
        [JsonProperty(PropertyName = "0")] public decimal UserId { get; set; }

        [JsonProperty(PropertyName = "1")] public string InstitutionGuid { get; set; }

        [JsonProperty(PropertyName = "2")] public decimal UserAccessId { get; set; }

        [JsonProperty(PropertyName = "3")] public string Name { get; set; }

        [JsonProperty(PropertyName = "4")] public string UserType { get; set; }

        [JsonProperty(PropertyName = "5")] public int LanguageId { get; set; }
    }
}