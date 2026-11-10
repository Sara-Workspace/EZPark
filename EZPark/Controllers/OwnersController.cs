using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BLL.Services;
using DTO;
using Parking.Extensions;
using BLL;

namespace Parking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OwnersController : ControllerBase
    {
        private readonly OwnerService _ownerService;

        public OwnersController(OwnerService ownerService)
        {
            _ownerService = ownerService;
        }

        [HttpGet]
        public ActionResult<List<OwnerDTO>> GetAllOwners()
        {
            var owners = _ownerService.GetAllOwners();
            return Ok(owners);
        }

        [HttpGet("{id}")]
        public ActionResult<OwnerDTO> GetOwnerById(int id)
        {
            var owner = _ownerService.GetOwnerById(id);
            if (owner == null)
            {
                return NotFound($"Owner with ID {id} not found.");
            }
            return Ok(owner);
        }

        [HttpGet("{id}/spots")]
        public ActionResult<OwnerWithSpotsDTO> GetOwnerWithSpots(int id)
        {
            var ownerWithSpots = _ownerService.GetOwnerWithSpots(id);
            if (ownerWithSpots == null)
            {
                return NotFound($"Owner with ID {id} not found.");
            }
            return Ok(ownerWithSpots);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<OwnerDTO> CreateOwner([FromBody] OwnerCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdOwner = _ownerService.CreateOwner(dto);
            return CreatedAtAction(nameof(GetOwnerById), new { id = createdOwner.OwnerID }, createdOwner);
        }

        [Authorize(Roles = "Owner")]
        [HttpPut("{id}")]
        public ActionResult<OwnerDTO> UpdateOwner(int id, [FromBody] OwnerUpdateDTO dto)
        {
            if (id != User.GetUserId())
            {
                return Forbid();
            }

            var updatedOwner = _ownerService.UpdateOwner(id, dto);
            if (updatedOwner == null)
            {
                return NotFound($"Owner with ID {id} not found.");
            }
            return Ok(updatedOwner);
        }

        [Authorize(Roles = "Owner")]
        [HttpDelete("{id}")]
        public IActionResult DeleteOwner(int id)
        {
            if (id != User.GetUserId())
            {
                return Forbid();
            }

            var success = _ownerService.DeleteOwner(id);
            if (!success)
            {
                return NotFound($"Owner with ID {id} not found.");
            }
            return NoContent();
        }
    }
}