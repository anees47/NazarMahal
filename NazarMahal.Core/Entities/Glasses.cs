using System.Text.Json.Serialization;

namespace NazarMahal.Core.Entities
{
    public class Glasses
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
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


        [JsonIgnore]
        public ICollection<Order> Orders { get; set; } = [];

        [JsonIgnore]
        public ICollection<GlassesAttachment> Attachments { get; set; } = [];

        public Glasses()
        {

        }
        protected Glasses(string name, string? description, decimal price, string? brand, string? model, string? frameType, string? lensType, string? color, int categoryId, int subCategoryId, bool isActive, int availableQuantity)
        {
            Name = name;
            Description = description;
            Price = price;
            Brand = brand;
            Model = model;
            FrameType = frameType;
            LensType = lensType;
            Color = color;
            CategoryId = categoryId;
            SubCategoryId = subCategoryId;
            IsActive = isActive;
            AvailableQuantity = availableQuantity;
        }

        public static Glasses Create(string name, string? description, decimal price, string? model, string? brand, string? lensType, string? frameType, string? color, int categoryId, int subCategoryId, bool isActive, int availableQuantity)
        {
            return new Glasses(name, description, price, brand, model, frameType, lensType, color, categoryId, subCategoryId, isActive, availableQuantity);
        }

        public void UpdateGlassesInfo(string name, string? description, decimal price,
            string? brand, string? model, string? frameType, string? lensType, string? color, int categoryId, int subCategoryId, bool isActive, int availableQuantity)
        {
            Name = name;
            Description = description;
            Price = price;
            Brand = brand;
            Model = model;
            FrameType = frameType;
            LensType = lensType;
            Color = color;
            CategoryId = categoryId;
            SubCategoryId = subCategoryId;
            IsActive = isActive;
            AvailableQuantity = availableQuantity;
        }

        public void SoftDeleteGlasses()
        {
            IsActive = false;
        }
    }
}
