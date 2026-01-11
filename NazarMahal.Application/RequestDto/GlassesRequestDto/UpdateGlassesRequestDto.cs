using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace NazarMahal.Application.RequestDto.GlassesRequestDto
{
    public class UpdateGlassesRequestDto
    {
        public int GlassesId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }

        [Range(0.01, 100000, ErrorMessage = "Price must be between 0.01 and 100,000")]
        public decimal Price { get; set; }

        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? FrameType { get; set; }
        public string? LensType { get; set; }
        public string? Color { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public bool IsActive { get; set; }
        public int AvailableQuantity { get; set; }
        public IList<IFormFile>? Attachments { get; set; }
        public IEnumerable<int>? DeletedAttachmentIds { get; set; }

    }
}
