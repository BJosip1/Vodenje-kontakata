using Domain.Models;

namespace Application.DTOs
{
    public class PostContactDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Note { get; set; }

        public PostAddressDTO Address { get; set; }
        public List<PostPhoneNumberDTO> PhoneNumbers { get; set; } = new();
        public List<PostEmailDTO> Emails { get; set; } = new();
        public List<string> Tags { get; set; } = new();

        public Contact ToModel()
        {
            return new Contact
            {
                FirstName = FirstName,
                LastName = LastName,
                Note = Note,
                PhoneNumbers=PhoneNumbers.Select(x => x.ToModel()).ToList(),
                Emails = Emails.Select(x => x.ToModel()).ToList()
            };
        }
    }
}
