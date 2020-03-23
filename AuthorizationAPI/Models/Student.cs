using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationAPI.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }

        public ApplicationUser User { get; set; }
    }
    public class StudentAssociation
    {
        public string StudentGuid { get; set; }
        public string InstitutionGuid { get; set; }
        public string Type { get; set; } //school, classroom, etc
    }
}
