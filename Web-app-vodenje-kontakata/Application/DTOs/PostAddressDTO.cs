using Domain.Models;

namespace Application.DTOs
{
    public class PostAddressDTO
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public Address ToModel()
        {  
            return new Address
            { 
                Street = Street, 
                City = City,
                PostalCode = PostalCode,
                Country = Country
            }; 
        }
    }
}
