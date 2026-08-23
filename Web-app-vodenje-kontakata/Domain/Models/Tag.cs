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
