using Domain.Models;

namespace Application.DTOs
{
    public class PostEmailDTO
    {
        public string Type { get; set; }
        public string Value { get; set; }

        public Email ToModel()
        {
            return new Email
            {
                Type = Type,
                Value = Value
            };
        }
    }
}
