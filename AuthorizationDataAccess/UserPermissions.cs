using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizationDataAccess
{
    public class UserPermissions
    {
        public string UserGuid;
        public string InstitutionGuid;
        public List<Role> Roles { get; set; }
        public List<string> Permissions { get; set; }
    }
}
