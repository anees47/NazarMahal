using AutoMapper;
using NazarMahal.Application.Common;
using NazarMahal.Application.DTOs.GlassesDto;
using NazarMahal.Application.Interfaces;
using NazarMahal.Application.Interfaces.IReadModelRepository;
using NazarMahal.Application.Interfaces.IRepository;
using NazarMahal.Application.RequestDto.GlassesRequestDto;
using NazarMahal.Core.ActionResponses;
using NazarMahal.Core.Entities;
using static NazarMahal.Core.ActionResponses.NothingActionResponse;

namespace NazarMahal.Application.Services
{
    public class GlassesService(IGlassesRepository glassesRepository, IMapper mapper, IGlassesReadModelRepository glassesReadModelRepository) : IGlassesService
    {
        private readonly IGlassesRepository _glassesRepository = glassesRepository;
        private readonly IGlassesReadModelRepository _glassesReadModelRepository = glassesReadModelRepository;
        private readonly IMapper _mapper = mapper;

        #region Glasses Category

        public async Task<ActionResponse<IEnumerable<GlassesCategoryDto>>> GetAllCategories()
        {
            try
            {
                var categories = await _glassesReadModelRepository.GetGlassesCategories(showActiveOnly: false, showHavingSubcategories: false);
                var categoryDtos = _mapper.Map<IEnumerable<GlassesCategoryDto>>(categories);
                return new OkActionResponse<IEnumerable<GlassesCategoryDto>>(categoryDtos);
            }
            catch (Exception ex)
            {
                return new FailActionResponse<IEnumerable<GlassesCategoryDto>>($"Error occurred in GlassesService.GetAllCategories: {ex.Message}");
            }
        }

        public async Task<ActionResponse<GlassesCategoryDto>> CreateNewGlassesCategory(CreateNewGlassesCategoryRequestDto newGlassesCategoryRequest)
        {
            try
            {
                if (newGlassesCategoryRequest == null)
                    return new FailActionResponse<GlassesCategoryDto>("Request body cannot be null.");

                var categoryName = await _glassesRepository.FindGlassesCategoryByName(newGlassesCategoryRequest.CategoryName?.Trim());

                if (categoryName != null)
                    return new FailActionResponse<GlassesCategoryDto>($"Category name already exists.");

                var newGlassesCategory = GlassesCategory.Create(newGlassesCategoryRequest.CategoryName, newGlassesCategoryRequest.isActive);
                var savedCategory = await _glassesRepository.AddGlassesCategory(newGlassesCategory);
                await _glassesRepository.CompletedAsync();

                return new OkActionResponse<GlassesCategoryDto>(_mapper.Map<GlassesCategoryDto>(newGlassesCategory));
            }
            catch (Exception ex)
            {
                return new FailActionResponse<GlassesCategoryDto>($"Error occurred in GlassesService.CreateNewGlassesCategory: {ex.Message}");
            }
        }
        public async Task<ActionResponse<GlassesCategoryDto>> UpdateGlassesCategory(UpdateGlassesCategoryRequestDto updateGlassesCategoryRequestDto)
        {
            try
            {
                var glassesCategory = await _glassesRepository.GetGlassesCategoryById(updateGlassesCategoryRequestDto.CategoryId);
                if (glassesCategory == null)
                    return new NotFoundActionResponse<GlassesCategoryDto>("Category not found");
                var existingCategory = await _glassesRepository.FindGlassesCategoryByName(updateGlassesCategoryRequestDto.Name);
                if (existingCategory != null)
                {
                    if (updateGlassesCategoryRequestDto.CategoryId != existingCategory.Id) return ActionResponse<GlassesCategoryDto>.Fail("Category Name already exist");

                }
                glassesCategory.UpdateGlassesCategoryInfo(updateGlassesCategoryRequestDto.Name, updateGlassesCategoryRequestDto.IsActive);

                await _glassesRepository.CompletedAsync();

                return new OkActionResponse<GlassesCategoryDto>(_mapper.Map<GlassesCategoryDto>(glassesCategory));
            }
            catch (Exception ex)
            {
                return new FailActionResponse<GlassesCategoryDto>($"Error occurred in GlassesService.UpdateGlassesCategory: {ex.Message}");
            }
        }


        #endregion


        #region Glasses SubCategory

        public async Task<ActionResponse<GlassesSubCategoryDto>> CreateNewGlassesSubCategory(CreateNewGlassesSubCategoryRequestDto newGlassesSubCategoryRequest)
        {
            try
            {
                var glassesCategory = await _glassesReadModelRepository.GetGlassesCategoryById(newGlassesSubCategoryRequest.CategoryId);
                if (glassesCategory == null)
                    return new NotFoundActionResponse<GlassesSubCategoryDto>("Category not found.");


                var subCategoryName = await _glassesRepository.FindGlassesSubCategoryByName(newGlassesSubCategoryRequest.Name?.Trim(), newGlassesSubCategoryRequest.CategoryId);
                if (subCategoryName != null)
                    return new FailActionResponse<GlassesSubCategoryDto>("SubCategory Name already exists.");

                var newGlassesSubCategory = GlassesSubCategory.Create(newGlassesSubCategoryRequest.Name, newGlassesSubCategoryRequest.CategoryId, newGlassesSubCategoryRequest.IsActive);
                var savedSubCategory = await _glassesRepository.AddGlassesSubCategory(newGlassesSubCategory);
                await _glassesRepository.CompletedAsync();

                var outputGlassesSubCategory = _mapper.Map<GlassesSubCategoryDto>(savedSubCategory);

                outputGlassesSubCategory.CategoryName = glassesCategory.Name;
                return new OkActionResponse<GlassesSubCategoryDto>(outputGlassesSubCategory);
            }
            catch (Exception ex)
            {
                return new FailActionResponse<GlassesSubCategoryDto>($"Error occured in GlassesService.CreateNewGlassesSubCategory: {ex.Message}");
            }
        }
        public async Task<ActionResponse<IEnumerable<GlassesSubcategoriesListDto>>> GetAllGlassesSubCategoriesByCategoryId(int categoryId)
        {
            try
            {
                var glassesSubCategories = await _glassesReadModelRepository.GeAllGlassesSubcategoriesByCategoryId(categoryId);
                return new OkActionResponse<IEnumerable<GlassesSubcategoriesListDto>>(_mapper.Map<IEnumerable<GlassesSubcategoriesListDto>>(glassesSubCategories));

            }
            catch (Exception ex)
            {
                return new FailActionResponse<IEnumerable<GlassesSubcategoriesListDto>>($"Error occurred in GlassesService.GetGlassesSubCategories: {ex.Message}");
            }

        }

        public async Task<ActionResponse<IEnumerable<GlassesSubcategoriesListDto>>> GetGlassesSubCategories(bool showActiveOnly, int categoryId)
        {
            try
            {
                var glassesSubCategories = await _glassesReadModelRepository.GetGlassesSubcategories(showActiveOnly, categoryId);
                return new OkActionResponse<IEnumerable<GlassesSubcategoriesListDto>>(_mapper.Map<IEnumerable<GlassesSubcategoriesListDto>>(glassesSubCategories));

            }
            catch (Exception ex)
            {
                return new FailActionResponse<IEnumerable<GlassesSubcategoriesListDto>>($"Error occurred in GlassesService.GetGlassesSubCategories: {ex.Message}");
            }

        }

        public async Task<ActionResponse<GlassesSubcategoriesListDto>> GetGlassesSubCategoryById(int glassesSubCategoryId)
        {
            try
            {
                var glassesSubCategory = await _glassesRepository.GetGlassesSubCategoryById(glassesSubCategoryId);
                if (glassesSubCategory == null)

                    return new NotFoundActionResponse<GlassesSubcategoriesListDto>("SubCategory not found.");
                return new OkActionResponse<GlassesSubcategoriesListDto>(_mapper.Map<GlassesSubcategoriesListDto>(glassesSubCategory));

            }
            catch (Exception ex)
            {
                return new FailActionResponse<GlassesSubcategoriesListDto>($"Error occurred in GlassesService.GetGlassesSubCategoryById: {ex.Message}");
            }
        }

        public async Task<ActionResponse<GlassesSubCategoryDto>> SoftDeleteGlassesSubCategory(int glassesSubCategoryId)
        {
            try
            {
                var glassesSubCategory = await _glassesRepository.GetGlassesSubCategoryById(glassesSubCategoryId);
                if (glassesSubCategory == null)
                    return new NotFoundActionResponse<GlassesSubCategoryDto>("SubCategory not found.");

                await _glassesRepository.DeleteGlassesSubcategoryById(glassesSubCategoryId);
                await _glassesRepository.CompletedAsync();

                return new OkActionResponse<GlassesSubCategoryDto>();

            }
            catch (Exception ex)
            {
                return new FailActionResponse<GlassesSubCategoryDto>($"Error occurred in GlassesService.SoftDeleteGlassesSubCategory: {ex.Message}");

            }

        }


        public async Task<ActionResponse<GlassesSubCategoryDto>> UpdateGlassesSubCategory(UpdateGlassesSubCategoryRequestDto updateGlassesSubCategoryRequest)
        {
            try
            {
                var glassesCategory = await _glassesReadModelRepository.GetGlassesCategoryById(updateGlassesSubCategoryRequest.CategoryId);
                if (glassesCategory == null)
                    return new NotFoundActionResponse<GlassesSubCategoryDto>("Category not found.");

                var glassesSubCategory = await _glassesRepository.GetGlassesSubCategoryById(updateGlassesSubCategoryRequest.SubCategoryId);
                if (glassesSubCategory == null)
                    return new NotFoundActionResponse<GlassesSubCategoryDto>("SubCategory not found.");

                var existingSubCategory = await _glassesRepository.FindGlassesSubCategoryByName(updateGlassesSubCategoryRequest.Name?.Trim(), updateGlassesSubCategoryRequest.CategoryId);
                if (existingSubCategory != null)
                {
                    if (updateGlassesSubCategoryRequest.SubCategoryId != existingSubCategory.Id) return ActionResponse<GlassesSubCategoryDto>.Fail("SubCategory name already exists.");
                }

                glassesSubCategory.UpdateGlassesSubCategoryInfo(updateGlassesSubCategoryRequest.Name, updateGlassesSubCategoryRequest.CategoryId, updateGlassesSubCategoryRequest.IsActive);

                await _glassesRepository.CompletedAsync();

                var outputGlassesSubCategory = _mapper.Map<GlassesSubCategoryDto>(glassesSubCategory);

                outputGlassesSubCategory.CategoryName = glassesCategory.Name;

                return new OkActionResponse<GlassesSubCategoryDto>(outputGlassesSubCategory);
            }
            catch (Exception ex)
            {
                return new FailActionResponse<GlassesSubCategoryDto>($"Error occurred in GlassesService.UpdateGlassesSubCategory: {ex.Message}");
            }
        }
        #endregion


        #region Glasses
        public async Task<ActionResponse<IEnumerable<GlassesListDto>>> GetGlasses(SearchGlassesRequestDto glassesRequestDto)
        {
            try
            {
                if (glassesRequestDto.CategoryId != null && glassesRequestDto.CategoryId > 0)
                {
                    var glassesCategory = await _glassesReadModelRepository.GetGlassesCategoryById((int)glassesRequestDto.CategoryId);
                    if (glassesCategory == null)
                    {
                        return new FailActionResponse<IEnumerable<GlassesListDto>>("Category not found");
                    }
                }

                if (glassesRequestDto.SubCategoryId != null && glassesRequestDto.SubCategoryId > 0)
                {
                    var glassesSubCategory = await _glassesRepository.GetGlassesSubcategoryByCategoryId((int)glassesRequestDto.SubCategoryId);
                    if (glassesSubCategory == null)
                    {
                        return new FailActionResponse<IEnumerable<GlassesListDto>>("SubCategory not found");
                    }
                }

                var glassesSearch = new SearchGlassessDto(
                    glassesRequestDto.GlassesName,
                    glassesRequestDto.Brand,
                    glassesRequestDto.Color,
                    glassesRequestDto.Style,
                    glassesRequestDto.CategoryId,
                    glassesRequestDto.SubCategoryId,
                    glassesRequestDto.CategoryName,
                    glassesRequestDto.SubCategoryName,
                    glassesRequestDto.IsActive,
                    glassesRequestDto.PageSize,
                    glassesRequestDto.PageNum
                );

                var result = await _glassesReadModelRepository.GetGlassesAsync(glassesSearch);
                var mappedResult = _mapper.Map<List<GlassesListDto>>(result);

                return ActionResponse<IEnumerable<GlassesListDto>>.Ok(mappedResult);
            }
            catch (Exception ex)
            {
                return new FailActionResponse<IEnumerable<GlassesListDto>>($"Error occurred in GlassesService.GetGlasses: {ex.Message}");
            }
        }


        public async Task<ActionResponse<GlassesListDto>> GetGlassesById(int glassesId)
        {
            try
            {
                if (glassesId <= 0)
                    return new FailActionResponse<GlassesListDto>("Invalid glasses ID. ID must be greater than 0.");

                var glasses = await _glassesReadModelRepository.GetGlassesById(glassesId);
                if (glasses == null)
                    return new NotFoundActionResponse<GlassesListDto>("Glasses not found");

                var outputGlasses = _mapper.Map<GlassesListDto>(glasses);

                var glassesAttachment = await _glassesReadModelRepository.GetGlassesAttachments(glassesId);
                if (glassesAttachment.Any())
                {
                    outputGlasses.Attachments = _mapper.Map<IEnumerable<GlassesAttachmentDto>>(glassesAttachment);
                }

                return new OkActionResponse<GlassesListDto>(outputGlasses);
            }
            catch (Exception ex)
            {
                return new FailActionResponse<GlassesListDto>($"Error occurred in GlassesService.GetGlassesById: {ex.Message}");
            }
        }

        public async Task<ActionResponse<GlassesDto>> CreateNewGlasses(CreateNewGlassesRequestDto newGlassesRequest)
        {
            if (newGlassesRequest == null)
                return new FailActionResponse<GlassesDto>("Request body cannot be null.");

            // Note: Transactions should be handled through UnitOfWork pattern
            try
            {
                var glassesCategory = await _glassesReadModelRepository.GetGlassesCategoryById((int)newGlassesRequest.CategoryId);
                if (glassesCategory == null) return new NotFoundActionResponse<GlassesDto>("Category not found");

                var glassesSubCategory = await _glassesRepository.GetGlassesSubCategoryById(newGlassesRequest.SubCategoryId);
                if (glassesSubCategory == null) return new NotFoundActionResponse<GlassesDto>("SubCategory not found");

                // Validate that the subcategory belongs to the specified category
                var subCategoriesForCategory = await _glassesRepository.GetGlassesSubcategoryByCategoryId(newGlassesRequest.CategoryId);
                var subCategoryExists = subCategoriesForCategory.Any(sc => sc.Id == newGlassesRequest.SubCategoryId);
                if (!subCategoryExists)
                    return new NotFoundActionResponse<GlassesDto>("Invalid SubCategoryId or CategoryId. The provided SubCategory does not belong to the specified Category.");

                var newGlasses = Glasses.Create(newGlassesRequest.Name, newGlassesRequest.Description, newGlassesRequest.Price, newGlassesRequest.Model, newGlassesRequest.Brand,
                    newGlassesRequest.LensType, newGlassesRequest.FrameType, newGlassesRequest.Color, newGlassesRequest.CategoryId, newGlassesRequest.SubCategoryId,
                    newGlassesRequest.IsActive, newGlassesRequest.AvailableQuantity);

                // Add glasses through repository
                await _glassesRepository.AddGlasses(newGlasses);
                await _glassesRepository.CompletedAsync();

                // Add attachments if they exist
                if (newGlassesRequest.Attachments != null && newGlassesRequest.Attachments.Any())
                {
                    var attachmentsResponse = await AddGlassesAttachments(newGlasses.Id, newGlassesRequest.Attachments);
                    if (!attachmentsResponse.IsSuccessful)
                    {
                        return new FailActionResponse<GlassesDto>("Failed to save images. Please try again.");
                    }
                }

                // Transaction committed through repository
                return new OkActionResponse<GlassesDto>(_mapper.Map<GlassesDto>(newGlasses));
            }
            catch (Exception ex)
            {
                return new FailActionResponse<GlassesDto>($"Error occurred in GlassesService.CreateNewGlasses: {ex.Message}");
            }
        }

        public async Task<ActionResponse<IReadOnlyList<GlassesAttachment>>> AddGlassesAttachments(int glassesId, IEnumerable<Core.Abstractions.IFile> files)
        {
            var newImageList = new List<Tuple<GlassesAttachment, Core.Abstractions.IFile, string>>();
            try
            {
                foreach (var file in files)
                {
                    var fileContentTypeResult = FileContentType.Create(file.ContentType);
                    if (!fileContentTypeResult.IsSuccessful)
                        return ActionResponse<IReadOnlyList<GlassesAttachment>>.Fail(fileContentTypeResult.Messages);

                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var fullFileAbsolutePath = Path.Combine("uploads", fileName).Replace("\\", "/");
                    var fullFileRelativePath = Path.Combine("\\uploads", fileName);

                    var newImage = GlassesAttachment.Create(glassesId, fileName, fullFileRelativePath, fileContentTypeResult.Payload.Value);
                    await _glassesRepository.AddGlassesAttachment(newImage);

                    newImageList.Add(Tuple.Create(newImage, file, fullFileAbsolutePath));
                }

                foreach (var (attachment, file, path) in newImageList)
                {
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.OpenReadStream().CopyToAsync(stream);
                    }
                }

                return ActionResponse<IReadOnlyList<GlassesAttachment>>.Ok(newImageList.Select(x => x.Item1).ToList());
            }
            catch (Exception ex)
            {
                return ActionResponse<IReadOnlyList<GlassesAttachment>>.Fail($"Error occurred in GlassesService.AddGlassesAttachments: {ex.Message}");
            }
        }


        public async Task<ActionResponse<GlassesDto>> UpdateGlasses(UpdateGlassesRequestDto updateGlassesRequest)
        {
            // Note: Transactions should be handled through UnitOfWork pattern
            try
            {
                var glassesCategory = await _glassesReadModelRepository.GetGlassesCategoryById(updateGlassesRequest.CategoryId);
                if (glassesCategory == null) return new NotFoundActionResponse<GlassesDto>("Category not found");

                var glassesSubCategory = await _glassesRepository.GetGlassesSubCategoryById(updateGlassesRequest.SubCategoryId);
                if (glassesSubCategory == null) return new NotFoundActionResponse<GlassesDto>("SubCategory not found");

                // Validate that the subcategory belongs to the specified category
                var subCategoriesForCategory = await _glassesRepository.GetGlassesSubcategoryByCategoryId(updateGlassesRequest.CategoryId);
                var subCategoryExists = subCategoriesForCategory.Any(sc => sc.Id == updateGlassesRequest.SubCategoryId);
                if (!subCategoryExists)
                    return new NotFoundActionResponse<GlassesDto>("Invalid SubCategoryId or CategoryId. The provided SubCategory does not belong to the specified Category.");

                var existingGlasses = await _glassesRepository.GetGlassesById(updateGlassesRequest.GlassesId);
                if (existingGlasses == null) return new NotFoundActionResponse<GlassesDto>("Glasses not found");

                existingGlasses.UpdateGlassesInfo(updateGlassesRequest.Name, updateGlassesRequest.Description, updateGlassesRequest.Price, updateGlassesRequest.Brand,
                    updateGlassesRequest.Model, updateGlassesRequest.Color, updateGlassesRequest.FrameType, updateGlassesRequest.LensType, updateGlassesRequest.CategoryId,
                    updateGlassesRequest.SubCategoryId, updateGlassesRequest.IsActive, updateGlassesRequest.AvailableQuantity);

                // Update glasses in context
                await _glassesRepository.UpdateGlasses(existingGlasses);
                await _glassesRepository.CompletedAsync();

                // Handle new attachments
                if (updateGlassesRequest.Attachments != null && updateGlassesRequest.Attachments.Any())
                {
                    var attachmentsResponse = await AddGlassesAttachments(existingGlasses.Id, updateGlassesRequest.Attachments);
                    if (!attachmentsResponse.IsSuccessful)
                    {
                        return new FailActionResponse<GlassesDto>("Failed to save new images. Please try again.");
                    }
                }

                // Handle deleted attachments
                if (updateGlassesRequest.DeletedAttachmentIds != null && updateGlassesRequest.DeletedAttachmentIds.Any())
                {
                    var deleteGlassesAttachmentsResult = await DeleteGlassesAttachments(existingGlasses.Id, updateGlassesRequest.DeletedAttachmentIds);
                    if (!deleteGlassesAttachmentsResult.IsSuccessful)
                    {
                        return new FailActionResponse<GlassesDto>(deleteGlassesAttachmentsResult.Messages);
                    }
                }

                // Transaction committed through repository
                return new OkActionResponse<GlassesDto>(_mapper.Map<GlassesDto>(existingGlasses));
            }
            catch (Exception ex)
            {
                return new FailActionResponse<GlassesDto>($"Error occurred in GlassesService.UpdateGlasses: {ex.Message}");
            }
        }


        public async Task<ActionResponse<GlassesDto>> SoftDeleteGlasses(int glassesId)
        {
            try
            {
                var glasses = await _glassesRepository.GetGlassesById(glassesId);
                if (glasses == null)
                    return new NotFoundActionResponse<GlassesDto>("Glasses not found");

                var deleteResult = await _glassesRepository.DeleteGlassesById(glassesId);
                if (!deleteResult)
                    return new FailActionResponse<GlassesDto>("Error occurred while deleting glasses");

                await _glassesRepository.CompletedAsync();

                return new OkActionResponse<GlassesDto>(_mapper.Map<GlassesDto>(glasses));
            }
            catch (Exception ex)
            {
                return new FailActionResponse<GlassesDto>(
                    $"Error occurred in GlassesService.SoftDeleteGlasses: {ex.Message}"
                );
            }
        }
        public async Task<ActionResponse<NothingResponseDto>> DeleteGlassesAttachments(int glassesId, IEnumerable<int> deletedAttachmentIds)
        {
            try
            {
                if (deletedAttachmentIds == null) return ActionResponse<NothingResponseDto>.Ok(NothingResponseDto.Value);
                var glassesAttachmentByIds = await _glassesRepository.GetGlassesAttachmentsByIds(glassesId, deletedAttachmentIds);
                _glassesRepository.DeleteGlassesAttachment(glassesAttachmentByIds);

                foreach (var glassesAttachment in glassesAttachmentByIds)
                {
                    var attachmentToDeletePath = $"{glassesAttachment.StoragePath.Replace("/", "\\")}";
                    File.Delete(attachmentToDeletePath);
                }
                return ActionResponse<NothingResponseDto>.Ok(NothingResponseDto.Value);

            }
            catch (Exception ex)
            {
                return new FailActionResponse<NothingResponseDto>($"An Error has occured in GlassesService.DeleteGlassesAttachment: {ex.Message}");
            }
        }
        #endregion
    }
}
