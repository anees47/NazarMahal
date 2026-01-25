using NazarMahal.Application.DTOs.GlassesDto;
using NazarMahal.Application.RequestDto.GlassesRequestDto;
using NazarMahal.Core.ActionResponses;
using NazarMahal.Core.Entities;

namespace NazarMahal.Application.Interfaces
{
    public interface IGlassesService
    {
        #region Glasses Category
        Task<ActionResponse<IEnumerable<GlassesCategoryDto>>> GetAllCategories();
        Task<ActionResponse<GlassesCategoryDto>> CreateNewGlassesCategory(CreateNewGlassesCategoryRequestDto createNewGlassesCategoryRequestDto);
        Task<ActionResponse<GlassesCategoryDto>> UpdateGlassesCategory(UpdateGlassesCategoryRequestDto updateGlassesCategoryRequest);
        #endregion

        #region Glasses SubCategory
        Task<ActionResponse<IEnumerable<GlassesSubcategoriesListDto>>> GetAllGlassesSubCategoriesByCategoryId(int categoryId);
        Task<ActionResponse<IEnumerable<GlassesSubcategoriesListDto>>> GetGlassesSubCategories(bool showActiveOnly, int categoryId);
        Task<ActionResponse<GlassesSubcategoriesListDto>> GetGlassesSubCategoryById(int glassesSubCategoryId);
        Task<ActionResponse<GlassesSubCategoryDto>> CreateNewGlassesSubCategory(CreateNewGlassesSubCategoryRequestDto newGlassesSubCategoryRequest);
        Task<ActionResponse<GlassesSubCategoryDto>> UpdateGlassesSubCategory(UpdateGlassesSubCategoryRequestDto updateGlassesSubCategoryRequest);
        Task<ActionResponse<GlassesSubCategoryDto>> SoftDeleteGlassesSubCategory(int glassesSubCategoryId);
        #endregion

        #region Glasses
        Task<ActionResponse<IEnumerable<GlassesListDto>>> GetGlasses(SearchGlassesRequestDto glassesRequestDto);
        Task<ActionResponse<GlassesListDto>> GetGlassesById(int GlassesId);
        Task<ActionResponse<GlassesDto>> CreateNewGlasses(CreateNewGlassesRequestDto newGlassesRequest);
        Task<ActionResponse<GlassesDto>> UpdateGlasses(UpdateGlassesRequestDto updateGlassesRequest);
        Task<ActionResponse<GlassesDto>> SoftDeleteGlasses(int GlassesId);
        Task<ActionResponse<GlassesAttachment>> GetAttachmentById(int attachmentId);
        #endregion



    }

}
