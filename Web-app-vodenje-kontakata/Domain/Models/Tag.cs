using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name {  get; set; }
        #region Reverse Navigation Properties
        public ICollection<ContactTag> ContactTags { get; set; } = new List<ContactTag>();
        #endregion
    }
}
