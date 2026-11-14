using BLL.Interfaces;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parking.Extensions;                    // בשביל User.GetUserId()

namespace Parking.Controllers
{
    // אין כאן בדיקות null: אם משהו לא נמצא, הסרוויס זורק שגיאה וה-GlobalExceptionFilter מחזיר 404
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult<List<ReviewDTO>> GetAllReviews()
        {
            return Ok(_reviewService.GetAllReviews());
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public ActionResult<ReviewDTO> GetReviewById(int id)
        {
            return Ok(_reviewService.GetReviewById(id));
        }

        [AllowAnonymous]
        [HttpGet("spot/{spotId}")]
        public ActionResult<List<ReviewDTO>> GetReviewsBySpotId(int spotId)
        {
            return Ok(_reviewService.GetReviewsBySpotId(spotId));
        }

        [AllowAnonymous]
        [HttpGet("spot/{spotId}/average-rating")]
        public ActionResult<double> GetAverageRatingBySpotId(int spotId)
        {
            return Ok(_reviewService.GetAverageRatingBySpotId(spotId));
        }

        [Authorize(Roles = "Driver")]
        [HttpPost]
        public ActionResult<ReviewDTO> CreateReview([FromBody] ReviewCreateDTO dto)
        {
            // בדיקת ModelState לא צריך: [ApiController] מחזיר 400 אוטומטית אם ה-DTO לא תקין
            // מזהה הנהג מגיע מהטוקן
            var createdReview = _reviewService.CreateReview(dto, User.GetUserId());
            return CreatedAtAction(nameof(GetReviewById), new { id = createdReview.ReviewID }, createdReview);
        }

        [Authorize(Roles = "Driver")]
        [HttpDelete("{id}")]
        public IActionResult DeleteReview(int id)
        {
            // מוחקים רק אם הביקורת של הנהג המחובר
            _reviewService.DeleteReview(id, User.GetUserId());
            return NoContent();
        }
    }
}