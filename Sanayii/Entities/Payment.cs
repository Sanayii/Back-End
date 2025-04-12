using Sanayii.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanayii.Core.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public int Amount { get; set; }
        public PaymentMethod Method { get; set; } 
        public ICollection<ServiceRequestPayment> ServiceRequestPayments { get; set; } = new List<ServiceRequestPayment>(); 
    }
}
