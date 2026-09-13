using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entity
{
    public class CustomerReview
    {
        [Key]
        public int ReviewId { get; set; }

        [ForeignKey("ServiceApp")]
        public int? ServiceId { get; set; }
        public ServiceApp? serviceApp { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; } // مفتاح أجنبي إلى جدول المستخدمين
        public User User { get; set; } // علاقة مع كيان User


        public string Comment { get; set; }
        public int Rating { get; set; } // من 1 إلى 5
        public DateTime ReviewDate { get; set; }
    }
}
