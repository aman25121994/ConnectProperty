using System.ComponentModel.DataAnnotations;

namespace PropertyConnect.Models
{
    public class PropertyFormViewModel
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal Price { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Range(0, 50)]
        public int Bedrooms { get; set; } = 1;

        [Range(0, 50)]
        public int Bathrooms { get; set; } = 1;

        [StringLength(100)]
        public string? ContactName { get; set; }

        [Required]
        [StringLength(50)]
        public string ContactPhone { get; set; } = string.Empty;

        [StringLength(100)]
        [EmailAddress]
        public string? ContactEmail { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
