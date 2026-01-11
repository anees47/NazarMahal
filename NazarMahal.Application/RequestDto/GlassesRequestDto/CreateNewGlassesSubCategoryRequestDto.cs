namespace NazarMahal.Application.RequestDto.GlassesRequestDto
{
    public class CreateNewGlassesSubCategoryRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }
    }
}
