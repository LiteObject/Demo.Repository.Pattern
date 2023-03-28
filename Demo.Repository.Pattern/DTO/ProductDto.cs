using System.ComponentModel.DataAnnotations;

namespace Demo.Repository.Pattern.DTO
{
    public class ProductDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public double UnitPrice { get; set; }
    }
}
