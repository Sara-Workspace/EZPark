using BLL.Interfaces;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parking.Extensions;                    // ← תיקון 4: בשביל User.GetUserId()

namespace Parking.Controllers
{
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
            var reviews = _reviewService.GetAllReviews();
            return Ok(reviews);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public ActionResult<ReviewDTO> GetReviewById(int id)
        {
            var review = _reviewService.GetReviewById(id);
            if (review == null)
            {
                return NotFound($"Review with ID {id} not found.");
            }
            return Ok(review);
        }

        [AllowAnonymous]
        [HttpGet("spot/{spotId}")]
        public ActionResult<List<ReviewDTO>> GetReviewsBySpotId(int spotId)
        {
            var reviews = _reviewService.GetReviewsBySpotId(spotId);
            return Ok(reviews);
        }

        [AllowAnonymous]
        [HttpGet("spot/{spotId}/average-rating")]
        public ActionResult<double> GetAverageRatingBySpotId(int spotId)
        {
            double average = _reviewService.GetAverageRatingBySpotId(spotId);
            return Ok(average);
        }

        [Authorize(Roles = "Driver")]
        [HttpPost]
        public ActionResult<ReviewDTO> CreateReview([FromBody] ReviewCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // ← תיקון 4: מזהה הנהג מהטוקן
            var createdReview = _reviewService.CreateReview(dto, User.GetUserId());
            if (createdReview == null)
            {
                return NotFound("Reservation not found.");
            }
            return CreatedAtAction(nameof(GetReviewById), new { id = createdReview.ReviewID }, createdReview);
        }

        [Authorize(Roles = "Driver")]
        [HttpDelete("{id}")]
        public IActionResult DeleteReview(int id)
        {
            // ← תיקון 4: מוחקים רק אם הביקורת של הנהג המחובר
            var success = _reviewService.DeleteReview(id, User.GetUserId());
            if (!success)
            {
                return NotFound($"Review with ID {id} not found.");
            }
            return NoContent();
        }
    }
}