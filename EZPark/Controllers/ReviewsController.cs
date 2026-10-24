using Microsoft.AspNetCore.Mvc;
using BLL.Services;
using BLL.Interfaces;
using DTO;

namespace Parking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public ActionResult<List<ReviewDTO>> GetAllReviews()
        {
            var reviews = _reviewService.GetAllReviews();
            return Ok(reviews);
        }

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

        [HttpGet("spot/{spotId}")]
        public ActionResult<List<ReviewDTO>> GetReviewsBySpotId(int spotId)
        {
            var reviews = _reviewService.GetReviewsBySpotId(spotId);
            return Ok(reviews);
        }

        [HttpGet("spot/{spotId}/average-rating")]
        public ActionResult<double> GetAverageRatingBySpotId(int spotId)
        {
            double average = _reviewService.GetAverageRatingBySpotId(spotId);
            return Ok(average);
        }

        [HttpPost]
        public ActionResult<ReviewDTO> CreateReview([FromBody] ReviewCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdReview = _reviewService.CreateReview(dto);
            // שים לב: אם ב-ReviewDTO שדה המזהה הוא ReviewID או Id, ודא שהוא תואם:
            return CreatedAtAction(nameof(GetReviewById), new { id = createdReview.ReviewID }, createdReview);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteReview(int id)
        {
            var success = _reviewService.DeleteReview(id);
            if (!success)
            {
                return NotFound($"Review with ID {id} not found.");
            }
            return NoContent();
        }
    }
}