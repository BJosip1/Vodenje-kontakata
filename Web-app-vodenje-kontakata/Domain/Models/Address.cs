using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        #region Reverse Navigation Properties
        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        #endregion

    }
}
