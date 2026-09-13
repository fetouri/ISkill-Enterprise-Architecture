using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entity
{
    public class ServiceApp
    {
        [Key]
        public int Id { get; set; }

        public required string Name { get; set; }
        public required string Description { get; set; }

        //public int MinPrice { get; set; }
        //public int MaxPrice { get; set; }
        public bool IsActive { get; set; }
        public  string UrlHandle { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; } // مفتاح أجنبي إلى جدول المستخدمين
        public User User { get; set; } // علاقة مع كيان User

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; } 

     
        public ICollection<CustomerReview> CustomerReviews { get; set; }
        public ICollection<ImageUrl> ImageUrl { get; set; }
        public ICollection<ServiceAppCity> ServiceAppCities { get; set; }
        public ICollection<ServiceAppArea>? ServiceAppAreas { get; set; }


    }

}
