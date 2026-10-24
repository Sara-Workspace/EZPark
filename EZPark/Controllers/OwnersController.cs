using Microsoft.AspNetCore.Mvc;
using BLL.Services;
using BLL.Interfaces;
using DTO;
using BLL;

namespace Parking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnersController : ControllerBase
    {
        private readonly IOwnerService _ownerService;

        // הזרקת ה-Service של בעלי החניות
        public OwnersController(IOwnerService ownerService)
        {
            _ownerService = ownerService;
        }

        // GET: api/owners
        [HttpGet]
        public ActionResult<List<OwnerDTO>> GetAllOwners()
        {
            var owners = _ownerService.GetAllOwners();
            return Ok(owners);
        }

        // GET: api/owners/5
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

        // GET: api/owners/5/spots (שליפת בעל חניה כולל רשימת החניות שלו)
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

        // POST: api/owners
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

        // PUT: api/owners/5
        [HttpPut("{id}")]
        public ActionResult<OwnerDTO> UpdateOwner(int id, [FromBody] OwnerUpdateDTO dto)
        {
            var updatedOwner = _ownerService.UpdateOwner(id, dto);
            if (updatedOwner == null)
            {
                return NotFound($"Owner with ID {id} not found.");
            }
            return Ok(updatedOwner);
        }

        // DELETE: api/owners/5
        [HttpDelete("{id}")]
        public IActionResult DeleteOwner(int id)
        {
            var success = _ownerService.DeleteOwner(id);
            if (!success)
            {
                return NotFound($"Owner with ID {id} not found.");
            }
            return NoContent();
        }
    }
}