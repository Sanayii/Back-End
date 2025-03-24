using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanayii.Core.Entities
{
    public class Customer : AppUser
    {
        [ForeignKey("AppUser")]
        public string Id { get; set; }
        public Discount Discount { get; set; }
    }
}
