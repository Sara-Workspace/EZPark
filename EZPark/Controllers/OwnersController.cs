using BLL.Exceptions;
using BLL.Interfaces;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parking.Extensions;

namespace Parking.Controllers
{
    // אין כאן בדיקות null: אם בעל החניה לא נמצא, הסרוויס זורק שגיאה וה-GlobalExceptionFilter מחזיר 404
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OwnersController : ControllerBase
    {
        private readonly IOwnerService _ownerService;

        // הזרקת ה-Service של בעלי החניות
        public OwnersController(IOwnerService ownerService)
        {
            _ownerService = ownerService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult<List<OwnerDTO>> GetAllOwners()
        {
            return Ok(_ownerService.GetAllOwners());
        }

        [HttpGet("{id}")]
        public ActionResult<OwnerDTO> GetOwnerById(int id)
        {
            // רק בעל החניה עצמו, או מנהל.
            // מישהו אחר - עונים "לא נמצא" (404), כדי לא לגלות שהוא קיים
            bool isMe = User.IsInRole("Owner") && id == User.GetUserId();
            if (!isMe && !User.IsInRole("Admin"))
            {
                throw new NotFoundException($"בעל חניה מספר {id} לא נמצא.");
            }

            return Ok(_ownerService.GetOwnerById(id));
        }

        [HttpGet("{id}/spots")]
        public ActionResult<OwnerWithSpotsDTO> GetOwnerWithSpots(int id)
        {
            // רק בעל החניה עצמו, או מנהל
            bool isMe = User.IsInRole("Owner") && id == User.GetUserId();
            if (!isMe && !User.IsInRole("Admin"))
            {
                throw new NotFoundException($"בעל חניה מספר {id} לא נמצא.");
            }

            return Ok(_ownerService.GetOwnerWithSpots(id));
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<OwnerDTO> CreateOwner([FromBody] OwnerCreateDTO dto)
        {
            // בדיקת ModelState לא צריך: [ApiController] מחזיר 400 אוטומטית אם ה-DTO לא תקין
            var createdOwner = _ownerService.CreateOwner(dto);
            return CreatedAtAction(nameof(GetOwnerById), new { id = createdOwner.OwnerID }, createdOwner);
        }

        [Authorize(Roles = "Owner")]
        [HttpPut("{id}")]
        public ActionResult<OwnerDTO> UpdateOwner(int id, [FromBody] OwnerUpdateDTO dto)
        {
            // מותר לעדכן רק את עצמך
            if (id != User.GetUserId())
            {
                return Forbid();
            }

            return Ok(_ownerService.UpdateOwner(id, dto));
        }

        [Authorize(Roles = "Owner")]
        [HttpDelete("{id}")]
        public IActionResult DeleteOwner(int id)
        {
            // מותר למחוק רק את עצמך
            if (id != User.GetUserId())
            {
                return Forbid();
            }

            _ownerService.DeleteOwner(id);
            return NoContent();
        }
    }
}