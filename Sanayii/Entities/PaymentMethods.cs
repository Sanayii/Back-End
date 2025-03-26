using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanayii.Core.Entities
{
    public class PaymentMethods
    {
        public int PaymentId { get; set; }
        public string Method { get; set; }

        public Payment Payment { get; set; } //Navigation property

       

    }
}
