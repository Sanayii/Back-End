using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sanayii.Core.Entities
{
    public class Violation
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }

        // Foreign key reference to Contract
        public int ContractId { get; set; }

        [ForeignKey(nameof(ContractId))]
        public Contract Contract { get; set; }
    }
}
