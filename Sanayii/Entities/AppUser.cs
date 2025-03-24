using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snai3y.Core.Entities
{
    public class AppUser:IdentityUser
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public int Age { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Governate { get; set; }
    }
}
