namespace NazarMahal.Application.RequestDto.GlassesRequestDto
{
    public class SearchGlassesRequestDto
    {
        public string? GlassesName { get; set; }
        public string? Description { get; set; }
        public string? Brand { get; set; }
        public string? Color { get; set; }
        public string? Style { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int? SubCategoryId { get; set; }
        public string? SubCategoryName { get; set; }
        public bool? IsActive { get; set; }
        public int? PageSize { get; set; }
        public int PageNum { get; set; } = 1; // Default to page 1
    }
}
