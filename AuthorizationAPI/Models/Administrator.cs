using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationAPI.Models
{
    public class Administrator
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ApplicationUser User { get; set; }
    }
}
