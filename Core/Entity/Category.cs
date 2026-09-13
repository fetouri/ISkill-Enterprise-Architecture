using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entity
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        
        public required string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<ServiceApp>? Services { get; set; } = new List<ServiceApp>();
       

    }
}
