using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ContactTag
    {
        public int Id { get; set; }
        public Contact Contact { get; set; }
        public int ContactId { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
