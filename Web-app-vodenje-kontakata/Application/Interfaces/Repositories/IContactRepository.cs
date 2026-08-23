using Domain.Models;

namespace Application.Interfaces.Repositories
{
    public interface IContactRepository
    {
        Task<(List<Contact> Items, int TotalCount)> GetContacts(string? search, string? tag, string? sortBy, int page, int pageSize);
        Task<Contact?> GetContactById(int id);
        void CreateContact(Contact contact);
        Task UpdateContact(Contact contact);
        Task<bool> DeleteContact(int id);
        Task<Tag> GetOrCreateTag(string name);
        Task<Address> GetOrCreateAddress(string street, string city, string postalCode, string country);
    }
}
