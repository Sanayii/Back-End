using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanayii.Core.Entities
{
    public class Admin : AppUser
    {
        [ForeignKey("AppUser")]
        public string Id { get; set; }

        public string Name { get; set; }
        public decimal Salary { get; set; }
        public ICollection<Contract> Contracts { get; set; }
        
    }
}
