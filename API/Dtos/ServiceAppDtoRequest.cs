using Core.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Dtos
{
    public class ServiceAppDtoRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public bool IsActive { get; set; }
        public string UserId { get; set; }
        
        public int CategoryId { get; set; }


    }
}
