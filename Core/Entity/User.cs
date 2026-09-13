using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entity
{
    public class User : IdentityUser
    {
       
      
        public required string FullName { get; set; }




        public ICollection<ServiceApp>? Services { get; set; }

        // علاقة مع تقييمات الخدمات
        public ICollection<CustomerReview> CustomerReviews { get; set; }

    }
}
