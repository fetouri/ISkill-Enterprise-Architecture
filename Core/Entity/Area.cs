using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entity
{
    public class Area
    { 
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        [ForeignKey("City")]
        public int CityId { get; set; } 
        public City City { get; set; }
        public ICollection<ServiceAppArea> ServiceAppAreas { get; set; }
    }
}

