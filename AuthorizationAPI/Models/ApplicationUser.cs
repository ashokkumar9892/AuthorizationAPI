using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationAPI.Models
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string Guid { get; set; }
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

    }
    public class UserPermissions
    {
        public string UserGuid;
        public string InstitutionGuid;
        public List<ApplicationRole> Roles { get; set; }
        public List<string> Permissions { get; set; }
    }
}
