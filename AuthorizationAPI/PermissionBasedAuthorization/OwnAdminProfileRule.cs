using AuthorizationAPI.DAL;
using AuthorizationAPI.Models;
using AuthorizationAPICore.Rules;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Example.StudentsManagement.PermissionBasedAuthorization
{
    public class OwnAdminProfileRule : AuthorizationRuleForPermission
    {
        public OwnAdminProfileRule(string permission, string resourceType = null) : base(permission, resourceType)
        {
        }

        public override bool Authorize(string userId, string resourceId)
        {
            InMemoryRepository repository = new InMemoryRepository();
            var resource = repository.GetAll<Administrator>().Where(a => a.Id.ToString() == resourceId).FirstOrDefault();
            return resource != null && resource.User.Username == userId;
        }
    }
}