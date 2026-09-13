using API.Dtos;
using AutoMapper;
using Core.Entity;
using Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerReviewController : ControllerBase
    {
        private readonly ICustomerReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public CustomerReviewController(ICustomerReviewRepository reviewRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        // 1. جلب التقييمات حسب رقم الخدمة

        [HttpGet("{serviceId}")]
        public async Task<ActionResult<IEnumerable<CustomerReviewRespondDto>>> GetReviewsByService(int serviceId)
        {
            var reviews = await _reviewRepository.GetCustomerReviewlist();
            var filteredReviews = reviews.Where(r => r.ServiceId == serviceId);

            var response = _mapper.Map<List<CustomerReviewRespondDto>>(filteredReviews);

            return Ok(response);
        }

        // . إضافة تقييم جديد

        [HttpPost]
        [Authorize(Roles = "Reader")]
        public async Task<ActionResult<CustomerReviewDto>> CreateReview([FromBody] CustomerReviewDto reviewDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var newReview = _mapper.Map<CustomerReview>(reviewDto);
            newReview.UserId = userId;
            newReview.ReviewDate = DateTime.UtcNow;

            var createdReview = await _reviewRepository.CreateCustomerReviewAsync(newReview);

            return CreatedAtAction(nameof(GetReviewsByService), new { serviceId = createdReview.ServiceId }, reviewDto);
        }
    }
}
