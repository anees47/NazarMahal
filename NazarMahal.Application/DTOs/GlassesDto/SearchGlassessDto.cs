namespace NazarMahal.Application.DTOs.GlassesDto
{
    public class SearchGlassessDto
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
        public int PageNum { get; set; }

        public SearchGlassessDto() { }

        public SearchGlassessDto(string? glassesName, string? brand, string? color, string? style, int? categoryId, int? subCategoryId, string? categoryName, string? subCategoryName, bool? isActive, int? pageSize, int pageNum)
        {
            GlassesName = glassesName;
            Brand = brand;
            Color = color;
            Style = style;
            CategoryId = categoryId;
            SubCategoryId = subCategoryId;
            CategoryName = categoryName;
            SubCategoryName = subCategoryName;
            IsActive = isActive;
            PageSize = pageSize;
            PageNum = pageNum;
        }
    }
}
