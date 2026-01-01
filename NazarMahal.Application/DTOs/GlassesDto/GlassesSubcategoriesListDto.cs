namespace NazarMahal.Application.DTOs.GlassesDto
{
    public class GlassesSubcategoriesListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
    }
}
