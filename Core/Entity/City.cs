using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entity
{
    public class City
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public ICollection<Area> areas  { get; set; }
        public ICollection<ServiceAppCity> ServiceAppCities { get; set; }
    }
}
