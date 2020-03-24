using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationDataAccess
{
    public class Role
    {
        public int Id { get; set; }

        //Role is not predefined (not constants, or enum), 
        //as the Roles keep on changing with evolution of application.
        //For example, roles can be SuperUser, Student and Administrator today. 
        //After a year, role Administrator can be replaced with multiple roles: 
        //Student's Courses Administrator, Student's Grades Administrator, etc.

        //Therefore, providing capability of adding/removing/editing roles is better way.
        public string Name { get; set; }


        /// <summary>
        /// A set of permissions associated to the role.
        /// Application need provide functionality to associate/dessociate permissions
        /// </summary>
        public List<string> Permissions { get; set; }


        public Role()
        {
            Permissions = new List<string>();
        }
    }
}
