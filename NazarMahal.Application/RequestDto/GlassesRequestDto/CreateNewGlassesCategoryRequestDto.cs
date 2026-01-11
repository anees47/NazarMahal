namespace NazarMahal.Application.RequestDto.GlassesRequestDto
{
    public class CreateNewGlassesCategoryRequestDto
    {
        public required string CategoryName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
