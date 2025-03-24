using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snai3y.Core.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public DateTime ReviewDate { get; set; }
        public int Rating { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        public string CustomerId { get; set; }
        
        [ForeignKey("ArtisanId")]
        public Artisan Artisan { get; set; }
        public string ArtisanId { get; set; }

        [ForeignKey("ServiceId")]
        public Service Service { get; set; }
        public int ServiceId { get; set; }
    }
}
