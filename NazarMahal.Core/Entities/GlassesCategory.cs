using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NazarMahal.Core.Entities
{
    public class GlassesCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }


        protected GlassesCategory() { }
        protected GlassesCategory(string name, bool isActive)
        {
            Name = name;
            IsActive = isActive;
        }
        public static GlassesCategory Create(string name, bool isActive)
        {
            return new GlassesCategory(name, isActive);
        }
        
        public void UpdateGlassesCategoryInfo(string name, bool isActive)
        {
            Name = name;
            IsActive = isActive;
        }


    }
}
