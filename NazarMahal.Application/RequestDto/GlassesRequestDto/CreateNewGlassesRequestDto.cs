using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace NazarMahal.Application.RequestDto.GlassesRequestDto
{
    public class CreateNewGlassesRequestDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(100, 10000000, ErrorMessage = "Price must be between 100 and 1,000,000")]
        public decimal Price { get; set; }

        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? FrameType { get; set; }
        public string? LensType { get; set; }
        public string? Color { get; set; }

        [Required(ErrorMessage = "CategoryId is required")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "SubCategoryId is required")]
        public int SubCategoryId { get; set; }

        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "AvailableQuantity is required")]
        public int AvailableQuantity { get; set; }

        public IList<IFormFile>? Attachments { get; set; }
    }
}
