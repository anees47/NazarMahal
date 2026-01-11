using Microsoft.EntityFrameworkCore;
using NazarMahal.Application.Interfaces.IRepository;
using NazarMahal.Core.Entities;
using NazarMahal.Infrastructure.Data;

namespace NazarMahal.Infrastructure.Repository
{
    public class GlassesRepository : IGlassesRepository
    {

        private readonly ApplicationDbContext _dbContext;
        public GlassesRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task CompletedAsync()
        {
            _ = await _dbContext.SaveChangesAsync();
        }

        #region Glasses Category
        public async Task<GlassesCategory?> FindGlassesCategoryByName(string categoryName)
        {
            try
            {
                var category = await _dbContext.GlassesCategories
                    .Where(c => c.Name.Trim().ToLower() == categoryName.ToLower())
                    .FirstOrDefaultAsync();

                if (category == null)
                {
                    Console.WriteLine($"Category '{categoryName}' not found.");
                    return null;
                }

                return category;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FindGlassesCategoryByName: {ex.Message}");
                return null;
            }
        }

        public async Task<GlassesCategory> AddGlassesCategory(GlassesCategory glassesCategory)
        {
            var newGlassesCategory = await _dbContext.GlassesCategories.AddAsync(glassesCategory);
            return newGlassesCategory.Entity;
        }
        public async Task<GlassesCategory> GetGlassesCategoryById(int categoryId)
        {
            return await _dbContext.GlassesCategories.Where(c => c.Id == categoryId).FirstOrDefaultAsync();
        }

        #endregion


        #region Glasses SubCategory
        public async Task<GlassesSubCategory> FindGlassesSubCategoryByName(string subCategoryName, int categoryId)
        {
            var subCategory = await _dbContext.GlassesSubCategories.
                Where(c => c.Name.Trim() == subCategoryName.ToLower() && c.IsActive == false && c.CategoryId == categoryId).FirstOrDefaultAsync();
            return subCategory;
        }



        public async Task<GlassesSubCategory> AddGlassesSubCategory(GlassesSubCategory glassesSubCategory)
        {
            var newGlassesSubCategory = await _dbContext.GlassesSubCategories.AddAsync(glassesSubCategory);
            return newGlassesSubCategory.Entity;
        }

        public async Task<GlassesSubCategory> GetGlassesSubCategoryById(int subCategoryId)
        {
            return await _dbContext.GlassesSubCategories
                .Where(s => s.Id == subCategoryId)
                .FirstOrDefaultAsync();
        }
        public async Task<GlassesSubCategory> DeleteGlassesSubcategoryById(int SubcategoryId)
        {
            var entity = await _dbContext.GlassesSubCategories.FindAsync(SubcategoryId);

            if (entity == null)
            {
                return null;
            }
            _ = _dbContext.GlassesSubCategories.Remove(entity);
            _ = await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<GlassesSubCategory>> GetGlassesSubcategoryByCategoryId(int categoryId = 0)
        {
            return await _dbContext.GlassesSubCategories.Where(x => x.CategoryId == categoryId).ToListAsync();
        }

        #endregion


        #region Glasses
        public async Task<Glasses> GetGlassesById(int glassesId)
        {
            return await _dbContext.Glasses
                .Where(g => g.Id == glassesId)
                .FirstOrDefaultAsync();
        }

        public async Task<Glasses> AddGlasses(Glasses glasses)
        {
            var newGlasses = await _dbContext.Glasses.AddAsync(glasses);
            return newGlasses.Entity;
        }

        public async Task<GlassesAttachment> GetGlassesAttachmentById(int glassesId, int attachementId)
        {
            return await _dbContext.GlassesAttachments.Where(f => f.GlassesId == glassesId && f.Id == attachementId).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<GlassesAttachment>> GetGlassesAttachmentsByIds(int glassesId, IEnumerable<int> attachmentIds)
        {
            return await _dbContext.GlassesAttachments
                .Where(g => g.GlassesId == glassesId && attachmentIds.Contains(g.Id))
                .ToListAsync();
        }


        public void DeleteGlassesAttachment(IEnumerable<GlassesAttachment> glassesAttachments)
        {
            _dbContext.GlassesAttachments.RemoveRange(glassesAttachments);
        }

        public async Task<GlassesAttachment> AddGlassesAttachment(GlassesAttachment glassesAttachment)
        {
            var updatedGlasses = await _dbContext.GlassesAttachments.AddAsync(glassesAttachment);
            return updatedGlasses.Entity;
        }
        public async Task UpdateGlasses(Glasses glasses)
        {
            _ = _dbContext.Glasses.Update(glasses);
            _ = await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> DeleteGlassesById(int glassesId)
        {
            var entity = await _dbContext.Glasses.FindAsync(glassesId);

            if (entity == null)
            {
                return false;
            }

            // Soft delete: Set IsActive to false instead of removing from database
            entity.IsActive = false;
            _ = _dbContext.Glasses.Update(entity);
            _ = await _dbContext.SaveChangesAsync();
            return true;
        }



        #endregion

    }
}
