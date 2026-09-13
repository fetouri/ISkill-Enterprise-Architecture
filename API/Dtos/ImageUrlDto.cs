using Core.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
    public class ImageUrlDto
    {
        public int Id { get; set; }
        public  string SImageUrl { get; set; }
        public string mainImageUrl { get; set; }


        public int ServiceAppId { get; set; } 
       
    }
}

