using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Repositories
{
    public class ContactRepository: IContactRepository
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILogger<ContactRepository> _logger;

        public ContactRepository(IApplicationDbContext dbContext, ILogger<ContactRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task<(List<Contact> Items, int TotalCount)> GetContacts(
            string? search, string? tag, string? sortBy, int page, int pageSize)
        {
            IQueryable<Contact> query = _dbContext.Contacts
                .Include(c => c.Address)
                .Include(c => c.PhoneNumbers)
                .Include(c => c.Emails)
                .Include(c => c.ContactTags).ThenInclude(ct => ct.Tag)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(c =>
                    c.FirstName.ToLower().Contains(term) ||
                    c.LastName.ToLower().Contains(term) ||
                    c.PhoneNumbers.Any(p => p.Value.ToLower().Contains(term)) ||
                    c.Emails.Any(e => e.Value.ToLower().Contains(term)) ||
                    c.ContactTags.Any(ct => ct.Tag.Name.ToLower().Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(tag))
            {
                var tagLower = tag.Trim().ToLower();
                query = query.Where(c => c.ContactTags.Any(ct => ct.Tag.Name.ToLower() == tagLower));
            }

            query = ApplySorting(query, sortBy);

            var totalCount = await query.CountAsync();

            var safePage = page < 1 ? 1 : page;
            var safePageSize = pageSize is < 1 or > 200 ? 20 : pageSize;

            var items = await query
                .Skip((safePage - 1) * safePageSize)
                .Take(safePageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        private static IQueryable<Contact> ApplySorting(IQueryable<Contact> query, string? sortBy)
        {
            var descending = false;
            var field = sortBy ?? "lastName";
            if (field.StartsWith("-"))
            {
                descending = true;
                field = field[1..];
            }

            return field.ToLower() switch
            {
                "firstname" => descending ? query.OrderByDescending(c => c.FirstName) : query.OrderBy(c => c.FirstName),
                "city" => descending ? query.OrderByDescending(c => c.Address.City) : query.OrderBy(c => c.Address.City),
                _ => descending ? query.OrderByDescending(c => c.LastName) : query.OrderBy(c => c.LastName),
            };
        }

        public async Task<Contact?> GetContactById(int id)
        {
            return await _dbContext.Contacts
                .Include(c => c.Address)
                .Include(c => c.PhoneNumbers)
                .Include(c => c.Emails)
                .Include(c => c.ContactTags).ThenInclude(ct => ct.Tag)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public void CreateContact(Contact contact)
        {
            _dbContext.Contacts.Add(contact);
        }

        public Task UpdateContact(Contact contact)
        {
            _dbContext.Contacts.Update(contact);
            return Task.CompletedTask;
        }

        public async Task<bool> DeleteContact(int id)
        {
            var contact = await _dbContext.Contacts.FirstOrDefaultAsync(c => c.Id == id);
            if (contact == null)
                return false;

            _dbContext.Contacts.Remove(contact);
            return true;
        }

        public async Task<Tag> GetOrCreateTag(string name)
        {
            var normalized = name.Trim();
            var existing = await _dbContext.Tags
                .FirstOrDefaultAsync(t => t.Name.ToLower() == normalized.ToLower());

            if (existing != null)
                return existing;

            var tag = new Tag { Name = normalized };
            _dbContext.Tags.Add(tag);
            return tag;
        }

        public async Task<Address> GetOrCreateAddress(string street, string city, string postalCode, string country)
        {
            var existing = await _dbContext.Addresses.FirstOrDefaultAsync(a =>
                a.Street.ToLower() == street.ToLower() &&
                a.City.ToLower() == city.ToLower() &&
                a.PostalCode.ToLower() == postalCode.ToLower() &&
                a.Country.ToLower() == country.ToLower());

            if (existing != null)
                return existing;

            var address = new Address { Street = street, City = city, PostalCode = postalCode, Country = country };
            _dbContext.Addresses.Add(address);
            return address;
        }

    }
}
