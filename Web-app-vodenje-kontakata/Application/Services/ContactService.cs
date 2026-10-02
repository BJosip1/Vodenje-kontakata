using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Models;

namespace Application.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ContactService(IContactRepository contactRepository, IUnitOfWork unitOfWork)
        {
            _contactRepository = contactRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResult<GetContactDTO>>> GetContacts(
            string? search, string? tag, string? sortBy, int page, int pageSize)
        {
            var (items, totalCount) = await _contactRepository.GetContacts(search, tag, sortBy, page, pageSize);

            var pagedResult = new PagedResult<GetContactDTO>
            {
                Items = items.Select(MapToDTO).ToList(),
                TotalCount = totalCount,
                Page = page < 1 ? 1 : page,
                PageSize = pageSize is < 1 or > 200 ? 20 : pageSize
            };

            return Result<PagedResult<GetContactDTO>>.Success(pagedResult);
        }

        public async Task<Result<GetContactDTO>> GetContactById(int id)
        {
            var contact = await _contactRepository.GetContactById(id);
            if (contact == null)
                return Result<GetContactDTO>.NotFound($"Kontakt s ID-em {id} nije pronađen.");

            return Result<GetContactDTO>.Success(MapToDTO(contact));
        }

        public async Task<Result<GetContactDTO>> AddContact(PostContactDTO dto)
        {
            var validation = ValidateContact(dto.FirstName, dto.LastName, dto.Address, dto.PhoneNumbers, dto.Emails);
            if (!validation.IsSuccess)
                return Result<GetContactDTO>.Failure(validation.ValidationItems);

            var contact = dto.ToModel();

            contact.Address = await _contactRepository.GetOrCreateAddress(
                dto.Address.Street, dto.Address.City, dto.Address.PostalCode, dto.Address.Country);

            foreach (var tagName in dto.Tags.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var tag = await _contactRepository.GetOrCreateTag(tagName);
                contact.ContactTags.Add(new ContactTag { Tag = tag });
            }

            _contactRepository.CreateContact(contact);
            await _unitOfWork.SaveChangesAsync();

            return Result<GetContactDTO>.Success(MapToDTO(contact));
        }

        public async Task<Result<GetContactDTO>> UpdateContact(int id, PutContactDTO dto)
        {
            var contact = await _contactRepository.GetContactById(id);
            if (contact == null)
                return Result<GetContactDTO>.NotFound($"Kontakt s ID-em {id} nije pronađen.");

            var validation = ValidateBasicFields(dto.FirstName, dto.LastName, dto.Address);
            if (!validation.IsSuccess)
                return Result<GetContactDTO>.Failure(validation.ValidationItems);

            dto.ApplyTo(contact);

            contact.Address = await _contactRepository.GetOrCreateAddress(
                dto.Address.Street, dto.Address.City, dto.Address.PostalCode, dto.Address.Country);

            await _contactRepository.UpdateContact(contact);
            await _unitOfWork.SaveChangesAsync();

            return Result<GetContactDTO>.Success(MapToDTO(contact));
        }

        public async Task<Result<object>> DeleteContact(int id)
        {
            var deleted = await _contactRepository.DeleteContact(id);
            if (!deleted)
                return Result<object>.NotFound($"Kontakt s ID-em {id} nije pronađen.");

            await _unitOfWork.SaveChangesAsync();
            return Result<object>.Success();
        }

        public async Task<Result<GetContactDTO>> AddContactDetails(int id, AddContactDetailsDTO dto)
        {
            var contact = await _contactRepository.GetContactById(id);
            if (contact == null)
                return Result<GetContactDTO>.NotFound($"Kontakt s ID-em {id} nije pronađen.");

            var validation = new ValidationResult();
            ValidatePhones(dto.PhoneNumbers, validation);
            ValidateEmails(dto.Emails, validation);
            if (!validation.IsSuccess)
                return Result<GetContactDTO>.Failure(validation.ValidationItems);

            foreach (var phoneDto in dto.PhoneNumbers)
                contact.PhoneNumbers.Add(phoneDto.ToModel());

            foreach (var emailDto in dto.Emails)
                contact.Emails.Add(emailDto.ToModel());

            foreach (var tagName in dto.Tags.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var tag = await _contactRepository.GetOrCreateTag(tagName);
                if (!contact.ContactTags.Any(ct => ct.TagId == tag.Id))
                    contact.ContactTags.Add(new ContactTag { Tag = tag });
            }

            await _contactRepository.UpdateContact(contact);
            await _unitOfWork.SaveChangesAsync();

            return Result<GetContactDTO>.Success(MapToDTO(contact));
        }

        public async Task<Result<GetContactDTO>> RemoveContactDetails(int id, RemoveContactDetailsDTO dto)
        {
            var contact = await _contactRepository.GetContactById(id);
            if (contact == null)
                return Result<GetContactDTO>.NotFound($"Kontakt s ID-em {id} nije pronađen.");

            var validation = new ValidationResult();

            var missingPhoneIds = dto.PhoneNumberIds.Except(contact.PhoneNumbers.Select(p => p.Id)).ToList();
            if (missingPhoneIds.Any())
                validation.ValidationItems.Add($"Telefonski broj(evi) s ID-em {string.Join(", ", missingPhoneIds)} ne pripadaju ovom kontaktu.");

            var missingEmailIds = dto.EmailIds.Except(contact.Emails.Select(e => e.Id)).ToList();
            if (missingEmailIds.Any())
                validation.ValidationItems.Add($"E-mail(ovi) s ID-em {string.Join(", ", missingEmailIds)} ne pripadaju ovom kontaktu.");

            var missingTagIds = dto.TagIds.Except(contact.ContactTags.Select(ct => ct.TagId)).ToList();
            if (missingTagIds.Any())
                validation.ValidationItems.Add($"Tag(ovi) s ID-em {string.Join(", ", missingTagIds)} ne pripadaju ovom kontaktom.");

            if (!validation.IsSuccess)
                return Result<GetContactDTO>.Failure(validation.ValidationItems);

            contact.PhoneNumbers = contact.PhoneNumbers.Where(p => !dto.PhoneNumberIds.Contains(p.Id)).ToList();
            contact.Emails = contact.Emails.Where(e => !dto.EmailIds.Contains(e.Id)).ToList();
            contact.ContactTags = contact.ContactTags.Where(ct => !dto.TagIds.Contains(ct.TagId)).ToList();

            await _contactRepository.UpdateContact(contact);
            await _unitOfWork.SaveChangesAsync();

            return Result<GetContactDTO>.Success(MapToDTO(contact));
        }

        private static ValidationResult ValidateContact(
            string firstName, string lastName, PostAddressDTO address,
            List<PostPhoneNumberDTO> phones, List<PostEmailDTO> emails)
        {
            var result = ValidateBasicFields(firstName, lastName, address);
            ValidatePhones(phones, result);
            ValidateEmails(emails, result);
            return result;
        }

        private static ValidationResult ValidateBasicFields(string firstName, string lastName, PostAddressDTO address)
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(firstName))
                result.ValidationItems.Add("Ime je obavezno.");
            else if (firstName.Length > 100)
                result.ValidationItems.Add("Ime ne smije biti duže od 100 znakova.");

            if (string.IsNullOrWhiteSpace(lastName))
                result.ValidationItems.Add("Prezime je obavezno.");
            else if (lastName.Length > 100)
                result.ValidationItems.Add("Prezime ne smije biti duže od 100 znakova.");
            

            if (address == null)
            {
                result.ValidationItems.Add("Adresa je obavezna.");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(address.Street))
                    result.ValidationItems.Add("Ulica je obavezna.");
                if (address.Street.Length > 100)
                    result.ValidationItems.Add("Ulica ne smije sadržavati više od 100 znakova.");
                if (string.IsNullOrWhiteSpace(address.City))
                    result.ValidationItems.Add("Grad je obavezan.");
                if (address.City.Length > 100)
                    result.ValidationItems.Add("Grad ne smije sadržavati više od 100 znakova.");
                if (string.IsNullOrWhiteSpace(address.PostalCode))
                    result.ValidationItems.Add("Poštanski broj je obavezan.");
                if (address.PostalCode.Length > 15)
                    result.ValidationItems.Add("Poštanski broj ne smije sadržavati više od 15 znakova.");
                if (string.IsNullOrWhiteSpace(address.Country))
                    result.ValidationItems.Add("Država je obavezna.");
                if (address.Country.Length > 100)
                    result.ValidationItems.Add("Država ne smije sadržavati više od 100 znakova.");
            }

            return result;
        }

        private static void ValidatePhones(List<PostPhoneNumberDTO> phones, ValidationResult result)
        {
            foreach (var phone in phones)
            {
                if (string.IsNullOrWhiteSpace(phone.Type) || string.IsNullOrWhiteSpace(phone.Value))
                    result.ValidationItems.Add("Tip i vrijednost telefonskog broja su obavezni.");
                if(phone.Type.Length>50)
                    result.ValidationItems.Add("Tip telefonskog broja ne smije sadržavati više od 50 znakova.");
                if (phone.Value.Length > 50)
                    result.ValidationItems.Add("Vrijednost telefonskog broja ne smije sadržavati više od 50 znakova.");
            }
        }

        private static void ValidateEmails(List<PostEmailDTO> emails, ValidationResult result)
        {
            foreach (var email in emails)
            {
                if (string.IsNullOrWhiteSpace(email.Type) || string.IsNullOrWhiteSpace(email.Value))
                {
                    result.ValidationItems.Add("Tip i vrijednost e-maila su obavezni.");
                    continue;
                }

                if (!IsValidEmail(email.Value))
                    result.ValidationItems.Add($"E-mail adresa '{email.Value}' nije u ispravnom formatu.");
                else if (email.Value.Length > 200)
                    result.ValidationItems.Add("Vrijednost e-maila ne smije sadržavati više od 200 znakova.");
                if (email.Type.Length > 50)
                    result.ValidationItems.Add("Tip e-maila ne smije sadržavati više od 50 znakova.");
                
            }
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                _ = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static GetContactDTO MapToDTO(Contact contact)
        {
            if (contact == null)
                return null;

            return new GetContactDTO
            {
                Id = contact.Id,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Note = contact.Note,
                Address = contact.Address == null ? null : new GetAddressDTO
                {
                    Id = contact.Address.Id,
                    Street = contact.Address.Street,
                    City = contact.Address.City,
                    PostalCode = contact.Address.PostalCode,
                    Country = contact.Address.Country
                },
                PhoneNumbers = contact.PhoneNumbers?.Select(p => new GetPhoneNumberDTO
                {
                    Id = p.Id,
                    Type = p.Type,
                    Value = p.Value
                }).ToList() ?? new List<GetPhoneNumberDTO>(),
                Emails = contact.Emails?.Select(e => new GetEmailDTO
                {
                    Id = e.Id,
                    Type = e.Type,
                    Value = e.Value
                }).ToList() ?? new List<GetEmailDTO>(),
                Tags = contact.ContactTags?.Select(ct => new GetTagDTO
                {
                    Id = ct.Tag.Id,
                    Name = ct.Tag.Name
                }).ToList() ?? new List<GetTagDTO>()
            };
        }
    }
}