namespace Application.DTOs
{
    public class AddContactDetailsDTO
    {
        public List<PostPhoneNumberDTO> PhoneNumbers { get; set; } = new();
        public List<PostEmailDTO> Emails { get; set; } = new();
        public List<string> Tags { get; set; } = new();
    }
}
