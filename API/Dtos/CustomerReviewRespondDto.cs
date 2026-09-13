using Core.Entity;
using System.ComponentModel.DataAnnotations.Schema;
namespace API.Dtos
{
    public class CustomerReviewRespondDto
    {
        public int ReviewId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; } // من 1 إلى 5
        public int ServiceId { get; set; }
        public DateTime ReviewDate { get; set; }


    }
}
