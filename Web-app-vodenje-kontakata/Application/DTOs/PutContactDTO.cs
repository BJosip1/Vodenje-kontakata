using Domain.Models;

namespace Application.DTOs
{
    public class PutContactDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Note { get; set; }
        public  PostAddressDTO Address { get; set; }

        public void ApplyTo(Contact contact)
        {
            contact.FirstName = FirstName;
            contact.LastName = LastName;
            contact.Note = Note;
        }
    }
}
