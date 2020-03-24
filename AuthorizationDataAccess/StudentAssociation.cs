using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizationDataAccess
{
    public class StudentAssociation
    {
        public string StudentGuid { get; set; }
        public string InstitutionGuid { get; set; }
        public string Type { get; set; } //school, classroom, etc
    }
}
