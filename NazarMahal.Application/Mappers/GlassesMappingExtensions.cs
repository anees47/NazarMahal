using NazarMahal.Application.DTOs.GlassesDto;
using NazarMahal.Core.Entities;
using NazarMahal.Core.ReadModels;

namespace NazarMahal.Application.Mappers
{
    /// <summary>
    /// Custom mapping extensions for Glasses-related DTOs
    /// </summary>
    public static class GlassesMappingExtensions
    {

        public static GlassesCategoryDto? ToGlassesCategoryDto(this GlassesCategoryReadModel category)
        {
            if (category == null)
                return null;

            return new GlassesCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                IsActive = category.IsActive
            };
        }
        public static GlassesCategoryDto? ToGlassesCategoryDto(this GlassesCategory category)
        {
            if (category == null)
                return null;

            return new GlassesCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                IsActive = category.IsActive
            };
        }

        public static GlassesSubCategoryDto? ToGlassesSubCategoryDto(this GlassesSubCategory subCategory)
        {
            if (subCategory == null)
                return null;

            return new GlassesSubCategoryDto
            {
                Id = subCategory.Id,
                CategoryId = subCategory.CategoryId,
                Name = subCategory.Name,
                CategoryName = subCategory.Category?.Name ?? string.Empty,
                IsActive = subCategory.IsActive
            };
        }


        public static GlassesSubCategory? ToGlassesSubCategory(this GlassesSubCategoryDto dto)
        {
            if (dto == null)
                return null;

            return GlassesSubCategory.Create(dto.Name, dto.CategoryId, dto.IsActive);
        }

        public static GlassesSubcategoriesListDto? ToGlassesSubcategoriesListDto(this GlassesSubCategory subCategory)
        {
            if (subCategory == null)
                return null;

            return new GlassesSubcategoriesListDto
            {
                Id = subCategory.Id,
                Name = subCategory.Name,
                CategoryId = subCategory.CategoryId,
                CategoryName = subCategory.Category?.Name ?? string.Empty,
                IsActive = subCategory.IsActive
            };
        }


        public static GlassesDto? ToGlassesDto(this Glasses glasses)
        {
            if (glasses == null)
                return null;

            return new GlassesDto
            {
                Id = glasses.Id,
                GlassesName = glasses.Name,
                Description = glasses.Description,
                Price = glasses.Price,
                Brand = glasses.Brand,
                Model = glasses.Model,
                FrameType = glasses.FrameType,
                LensType = glasses.LensType,
                Color = glasses.Color,
                CategoryId = glasses.CategoryId,
                CategoryName = string.Empty,
                SubCategoryId = glasses.SubCategoryId,
                SubCategoryName = string.Empty,
                IsActive = glasses.IsActive,
                AvailableQuanity = glasses.AvailableQuantity,
                Attachments = glasses.Attachments?.Select(a => new GlassesAttachmentDto
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    FileType = a.FileType,
                    StoragePath = a.StoragePath,
                    ReferenceId = a.GlassesId
                }) ?? Enumerable.Empty<GlassesAttachmentDto>()
            };
        }

        public static GlassesAttachmentDto? ToGlassesAttachmentDto(this GlassesAttachmentReadModel? attachment, string? baseUrl = null)
        {
            if (attachment == null)
                return null;

            if (attachment.AttachmentId <= 0)
                return null;

            if (string.IsNullOrWhiteSpace(attachment.FileName))
                return null;

            var imageUrl = !string.IsNullOrEmpty(baseUrl) 
                ? $"{baseUrl}/api/glasses/attachments/{attachment.AttachmentId}"
                : $"/api/glasses/attachments/{attachment.AttachmentId}";

            return new GlassesAttachmentDto
            {
                Id = attachment.AttachmentId,
                FileName = attachment.FileName,
                FileType = attachment.FileType ?? string.Empty,
                StoragePath = attachment.StoragePath,
                ReferenceId = attachment.GlassesId,
                ImageUrl = imageUrl
            };
        }


        public static GlassesListDto? ToGlassesListDto(this GlassesReadModel glasses)
        {
            if (glasses == null)
                return null;

            return new GlassesListDto
            {
                GlassesId = glasses.GlassesId,
                GlassesName = glasses.GlassesName,
                Description = glasses.Description,
                Price = glasses.Price,
                Brand = glasses.Brand,
                Model = glasses.Model,
                FrameType = glasses.FrameType,
                LensType = glasses.LensType,
                Color = glasses.Color,
                CategoryId = glasses.CategoryId,
                CategoryName = glasses.CategoryName,
                SubCategoryId = glasses.SubCategoryId ?? 0,
                SubCategoryName = glasses.SubCategoryName,
                IsActive = glasses.IsActive,
                AvailableQuantity = glasses.AvailableQuantity,
                Attachments = glasses.AttachmentReadModels
                    .Select(a => a.ToGlassesAttachmentDto())
                    .Where(a => a != null)
                    .ToList() ?? []
            };
        }

        public static List<GlassesListDto> ToGlassesListDtoList(this IEnumerable<GlassesReadModel> glassesCollection)
        {
            if (glassesCollection == null)
                return [];
            return glassesCollection.Select(g => g.ToGlassesListDto()).ToList();
        }

        public static List<GlassesSubcategoriesListDto> ToGlassesSubcategoriesListDtoList(this IEnumerable<GlassesSubcategoryReadModel> subCategories)
        {
            if (subCategories == null)
                return [];
            return subCategories.Select(sc => new GlassesSubcategoriesListDto
            {
                Id = sc.Id,
                Name = sc.Name,
                CategoryId = sc.CategoryId,
                CategoryName = sc.CategoryName,
                IsActive = sc.IsActive
            }).ToList();
        }
    }
}
