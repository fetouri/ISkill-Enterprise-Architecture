using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entity
{
    public class ServiceAppCity
    {

        [Key]
        public int Id { get; set; }
        // مفتاح أجنبي لـ ServiceApp
        [ForeignKey("ServiceApp")] // يتم ربط المفتاح الأجنبي باسم الخاصية في كلاس ServiceApp
        public int? ServiceAppId { get; set; }
        public ServiceApp ServiceApp { get; set; }

        // مفتاح أجنبي لـ City
        [ForeignKey("City")] // يتم ربط المفتاح الأجنبي باسم الخاصية في كلاس City
        public int? CityId { get; set; }
        public City City { get; set; }
    }
}
