using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sanayii.Core.Entities
{
    public class Contract
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxViolationsAllowed { get; set; }
        public string Status { get; set; }

        // Foreign key reference to Artisan
        public string ArtisanId { get; set; }

        [ForeignKey(nameof(ArtisanId))]
        public Artisan Artisan { get; set; }

        public ICollection<Violation> Violations { get; set; } = new HashSet<Violation>();
    }
}
