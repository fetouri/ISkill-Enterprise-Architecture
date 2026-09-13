using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entity
{
    public class ImageUrl
    {
        [Key]
        public int Id { get; set; }
        public required string SImageUrl { get; set; }
        public string? mainImageUrl { get; set; }


        [ForeignKey("ServiceApp")]
        public int? ServiceAppId { get; set; } // المفتاح الأجنبي إلى خدمة معينة
        public ServiceApp? ServiceApp { get; set; } // علاقة مع كيان ServiceApp
    }
}
