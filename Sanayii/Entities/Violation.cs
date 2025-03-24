using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanayii.Core.Entities
{
    public class Violation
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("ContractId")]
        public Contract Contract { get; set; }
        public int ContractId { get; set; }



    }
}
