using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Note {  get; set; }
        public int AddressId { get; set; }
        public Address Address { get; set; }

        #region Reverse Navigation Properties
        public ICollection<PhoneNumber> PhoneNumbers { get; set; } = new List<PhoneNumber>();
        public ICollection<Email> Emails { get; set; } = new List<Email>();
        public ICollection<ContactTag> ContactTags { get; set; } = new List<ContactTag>();
        #endregion

    }
}
