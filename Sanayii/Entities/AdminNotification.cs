using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sanayii.Core.Entities;
using Sanayii.Entities;


namespace Sanayii.Entities
{
    public class AdminNotification
    {
        [Key]
        public int NotificationId { get; set; }

        [ForeignKey("Admin")]
        public string AdminId { get; set; }
        public Admin Admin { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
    }
}
