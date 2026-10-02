namespace Application.DTOs
{
    public class RemoveContactDetailsDTO
    {
        public List<int> PhoneNumberIds { get; set; } = new();
        public List<int> EmailIds { get; set; } = new();
        public List<int> TagIds { get; set; } = new();
    }
}
