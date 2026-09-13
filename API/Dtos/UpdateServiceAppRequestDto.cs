namespace API.Dtos
{
    public class UpdateServiceAppRequestDto
    {
        
        public string Name { get; set; }
        public string Description { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public bool IsActive { get; set; }
        public string UserId { get; set; }
       
        public int CategoryId { get; set; }

    }
}
