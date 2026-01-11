namespace NazarMahal.Core.ReadModels
{
    public class GlassesSubcategoryReadModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public bool IsActive { get; set; }

    }
}
