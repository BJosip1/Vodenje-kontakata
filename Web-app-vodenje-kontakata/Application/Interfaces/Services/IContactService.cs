using Application.Common;
using Application.DTOs;

namespace Application.Interfaces.Services
{
        public interface IContactService
        {
            Task<Result<PagedResult<GetContactDTO>>> GetContacts(string? search, string? tag, string? sortBy, int page, int pageSize);
            Task<Result<GetContactDTO>> GetContactById(int id);
            Task<Result<GetContactDTO>> AddContact(PostContactDTO dto);
            Task<Result<GetContactDTO>> UpdateContact(int id, PutContactDTO dto);
            Task<Result<object>> DeleteContact(int id);
            Task<Result<GetContactDTO>> AddContactDetails(int id, AddContactDetailsDTO dto);
            Task<Result<GetContactDTO>> RemoveContactDetails(int id, RemoveContactDetailsDTO dto);
        }
}
