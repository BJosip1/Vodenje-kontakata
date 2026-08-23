using Domain.Models;

namespace Application.DTOs
{
    public class PostPhoneNumberDTO
    {
        public string Type { get; set; }
        public string Value { get; set; }

        public PhoneNumber ToModel()
        {
            return new PhoneNumber
            {
                Type= Type,
                Value= Value
            };
        }
    }
}
