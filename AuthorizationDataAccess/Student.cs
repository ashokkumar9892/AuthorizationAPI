using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizationDataAccess
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
}
