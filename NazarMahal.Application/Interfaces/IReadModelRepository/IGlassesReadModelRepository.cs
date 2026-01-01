using NazarMahal.Application.DTOs.GlassesDto;
using NazarMahal.Core.ReadModels;

namespace NazarMahal.Application.Interfaces.IReadModelRepository
{
    public interface IGlassesReadModelRepository
    {
        Task<IReadOnlyList<GlassesReadModel>> GetGlassesAsync(SearchGlassessDto searchGlassess);
        Task<GlassesReadModel> GetGlassesById(int GlassesId);
        Task<IReadOnlyList<GlassesAttachmentReadModel>> GetGlassesAttachments(int GlassesId);
        Task<IReadOnlyList<GlassesCategoryReadModel>> GetGlassesCategories(bool showActiveOnly, bool showHavingSubcategories);
        Task<GlassesCategoryReadModel> GetGlassesCategoryById(int categoryId);
        Task<IReadOnlyList<GlassesSubcategoryReadModel>> GeAllGlassesSubcategoriesByCategoryId(int categoryId);

        Task<IReadOnlyList<GlassesSubcategoryReadModel>> GetGlassesSubcategories(bool showActiveOnly, int categoryId);
        Task<GlassesSubcategoryReadModel> GetGlassesSubcategoryById(int subcategoryId, int categoryId = 0);
    }
}
