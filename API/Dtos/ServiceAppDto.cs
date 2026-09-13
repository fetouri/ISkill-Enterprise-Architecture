using Core.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using API.Dtos;

namespace API.Dtos
{
    public class ServiceAppDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string UrlHandle { get; set; }

        public bool IsActive { get; set; }
        public string UserId { get; set; }
        public string UserPhoneNumber { get; set; }
        public int CategoryId { get; set; }
        public CategoryDto? categories { get; set; }
        public string? MainImageUrl { get; set; } // الصورة الرئيسية (nullable)
        public List<CustomerReviewRespondDto> CustomerReviews { get; set; }
        public List<ImageUrlDto> ImagUrls { get; set; } 

    }
       
}

