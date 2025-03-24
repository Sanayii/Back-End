using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snai3y.Core.Entities
{
    public class Admin : AppUser
    {
        [ForeignKey("AppUser")]
        public string Id { get; set; }
        public decimal Salary { get; set; }
    }
}
