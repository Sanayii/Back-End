using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snai3y.Core.Entities
{
    public class Discount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public int MinRequiredRequests { get; set; }
        public bool IsFixedAmount { get; set; }
        public bool IsPercentage { get; set; }
        public DateTime ExpireDate { get; set; }
        
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        public string CustomerId { get; set; }
    }
}
