namespace NazarMahal.Application.RequestDto.GlassesRequestDto
{
    public class UpdateGlassesCategoryRequestDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
