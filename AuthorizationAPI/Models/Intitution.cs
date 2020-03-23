using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationAPI.Models
{
    public class Institution
    {
        public string Guid { get; set; }
        public string ParentGuid { get; set; }
        public string ObjectType { get; set; }
        public string Website { get; set; }
        public int Level { get; set; }
    }
}
