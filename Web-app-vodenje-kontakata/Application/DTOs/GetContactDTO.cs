namespace Application.DTOs
{
    public class GetContactDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Note { get; set; }
        public GetAddressDTO Address { get; set; }

        public List<GetPhoneNumberDTO> PhoneNumbers { get; set; } = new List<GetPhoneNumberDTO>();
        public List<GetEmailDTO> Emails { get; set; } = new List<GetEmailDTO>();
        public List<GetTagDTO> Tags { get; set; } = new List<GetTagDTO>();
    }
}
