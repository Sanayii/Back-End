using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanayii.Core.Entities
{
    public class UserConnection
    {
        public int UserId { get; set; }
        public int ConnectionId { get; set; }
        public virtual AppUser User { get; set; }
    }
}
