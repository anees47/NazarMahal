namespace NazarMahal.Application.RequestDto.GlassesRequestDto
{
    public class UpdateGlassesSubCategoryRequestDto
    {
        public int SubCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }
    }
}
