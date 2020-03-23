using AuthorizationAPI.DAL;
using AuthorizationAPI.Models;
using AuthorizationAPI.PermissionBasedAuthorization;
using AuthorizationAPI.Services;
using AuthorizationAPI.Utilities;
using AuthorizationAPICore;
using AuthorizationAPICore.PermissionBased;
using Example.StudentsManagement.Models.Constants;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationAPI.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {

        private InMemoryRepository db = new InMemoryRepository();
        private readonly FirebaseUtils _firebaseUtils;
        private readonly InstitutionService _institutionService;

        public LoginController(FirebaseUtils firebaseUtils, InstitutionService institutionService)
        {
            _firebaseUtils = firebaseUtils;
            _institutionService = institutionService;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        [Route("GetLogin")]
        [AuthorizePermission(new string[] { AppPermissions.MANAGE_STUDENT_PROFILE, AppPermissions.VIEW_STUDENT_PROFILES, AppPermissions.VIEW_OWN_STUDENT_PROFILE }, IdParameterName = "studentId", ResourceType = ResourceTypes.STUDENT)]
        public async Task<ActionResult> GetLogin( string email= "ashok-admin@scantron.edu.portal.com", string password = "test1234")
        {
            if (email=="" || password=="")
            {
                return BadRequest("InCorrect Login Input");
                // return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            //Student student = db.GetAll<Student>().FirstOrDefault(s => s.Id == studentId);

            Student student = db.GetAll<Student>().FirstOrDefault(s => s.User.Email ==email && password==password);

            InMemoryRepository repository = new InMemoryRepository();

            var permissionProvider = MyPermissionsProvider.GetPermissionsByEmail(email);

            //var user = repository.GetAll<Student>().Where(u => u.User.Username == student.Name).First();

            string hostname = "localhost:54011";
            var institution = await _institutionService.GetInstitutionByHost(hostname);

            FirebaseToken decodedToken = null;
            
            try
            {
                decodedToken =
                    await _firebaseUtils.ValidateCredentials(email.Trim().ToLower(), password);
            }
            catch (Exception ex)
            {
                if (ex is BadUserOrPasswordException) return StatusCode(418);
                throw;
            }

            //if (student == null)+mm
            //{
            //    return HttpNotFound();
            //}
            return Ok();
        }

    }
}