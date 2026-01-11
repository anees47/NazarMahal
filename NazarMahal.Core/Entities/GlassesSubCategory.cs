using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NazarMahal.Core.Entities
{
    public class GlassesSubCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public virtual GlassesCategory Category { get; set; } = null!;

        protected GlassesSubCategory() { }
        protected GlassesSubCategory(string name, int categoryId, bool isActive)
        {
            CategoryId = categoryId;
            Name = name;
            IsActive = isActive;
        }

        public static GlassesSubCategory Create(string name, int categoryId, bool isActive)
        {
            return new GlassesSubCategory(name, categoryId, isActive);
        }

        public void UpdateGlassesSubCategoryInfo(string name, int categoryId, bool isActive)
        {
            CategoryId = categoryId;
            Name = name;
            IsActive = isActive;
        }

        public void SoftDeleteSubCategory()
        {
            IsActive = false;
        }
    }
}
