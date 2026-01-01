using NazarMahal.Core.Entities;


namespace NazarMahal.Application.Interfaces.IRepository
{
    public interface IGlassesRepository
    {

        Task CompletedAsync();

        #region Glasses Category

        Task<GlassesCategory> FindGlassesCategoryByName(string categoryName);
        Task<GlassesCategory> AddGlassesCategory(GlassesCategory category);
        Task<GlassesCategory> GetGlassesCategoryById(int categoryId);

        #endregion


        #region Glasses SubCategory

        Task<GlassesSubCategory> FindGlassesSubCategoryByName(string subCategoryName, int categoryId);
        Task<GlassesSubCategory> AddGlassesSubCategory(GlassesSubCategory glassesSubCategory);
        Task<GlassesSubCategory> GetGlassesSubCategoryById(int subCategoryId);

        Task<IEnumerable<GlassesSubCategory>> GetGlassesSubcategoryByCategoryId(int categoryId);

        Task<GlassesSubCategory> DeleteGlassesSubcategoryById (int subCategoryId);


        #endregion


        #region Glasses
        Task<Glasses> GetGlassesById(int glassesId);
        Task<Glasses> AddGlasses(Glasses glasses);
        Task<GlassesAttachment> GetGlassesAttachmentById(int glassesId, int attachementId);
        Task<IReadOnlyList<GlassesAttachment>> GetGlassesAttachmentsByIds(int glassesId, IEnumerable<int> attachmentIds);
        void DeleteGlassesAttachment(IEnumerable<GlassesAttachment> glassesAttachments);
        Task<GlassesAttachment> AddGlassesAttachment(GlassesAttachment glassesAttachment);
        Task UpdateGlasses(Glasses glasses);
        Task<bool> DeleteGlassesById(int glassesId);

        #endregion

    }
}
