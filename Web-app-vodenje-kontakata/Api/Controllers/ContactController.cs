using Api.Common;
using Application.DTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        public async Task<IActionResult> GetContacts(
            [FromQuery] string? search,
            [FromQuery] string? tag,
            [FromQuery] string? sortBy,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _contactService.GetContacts(search, tag, sortBy, page, pageSize);
            return this.HandleResult(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetContactById(int id)
        {
            var result = await _contactService.GetContactById(id);
            return this.HandleResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddContact([FromBody] PostContactDTO dto)
        {
            var result = await _contactService.AddContact(dto);
            return this.HandleCreated(result, nameof(GetContactById), new { id = result.Value?.Id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateContact(int id, [FromBody] PutContactDTO dto)
        {
            var result = await _contactService.UpdateContact(id, dto);
            return this.HandleResult(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var result = await _contactService.DeleteContact(id);
            return this.HandleDeleted(result);
        }

        [HttpPost("{id:int}/details")]
        public async Task<IActionResult> AddContactDetails(int id, [FromBody] AddContactDetailsDTO dto)
        {
            var result = await _contactService.AddContactDetails(id, dto);
            return this.HandleResult(result);
        }

        [HttpPost("{id:int}/details/remove")]
        public async Task<IActionResult> RemoveContactDetails(int id, [FromBody] RemoveContactDetailsDTO dto)
        {
            var result = await _contactService.RemoveContactDetails(id, dto);
            return this.HandleResult(result);
        }
    }
}
