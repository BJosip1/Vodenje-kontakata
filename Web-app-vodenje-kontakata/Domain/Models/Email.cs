namespace Domain.Models
{
    public class Email
    {
        public int Id {  get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public int ContactId { get; set; }
        public Contact Contact { get; set; }
    }
}
