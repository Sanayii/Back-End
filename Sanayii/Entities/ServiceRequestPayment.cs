using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanayii.Core.Entities
{
    public class ServiceRequestPayment
    {
        public DateTime CreatedAt { get; set; }
        public int ExecutionTime { get; set; }


       
        public Customer Customer { get; set; }
        public string CustomerId { get; set; }

        
        public Payment Payment { get; set; }
        public int PaymentId { get; set; }
        
       
        public Service Service { get; set; }
        public int ServiceId { get; set; } 
        
    }
}
