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
