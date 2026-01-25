using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NazarMahal.API.Extensions;
using NazarMahal.Application.Common;
using NazarMahal.Application.DTOs.GlassesDto;
using NazarMahal.Application.Interfaces;
using NazarMahal.Application.RequestDto.GlassesRequestDto;
using NazarMahal.Core.Enums;

namespace NazarMahal.API.Controllers
{
    [Route("api/glasses")]
    [ApiController]
    [Authorize]
    public class GlassesController(IGlassesService glassesService) : ControllerBase
    {
        private readonly IGlassesService _glassesService = glassesService;

        #region Categories


        [AllowAnonymous]
        [HttpGet("categories")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<GlassesCategoryDto>>>> GetCategories()
        {
            var response = await _glassesService.GetAllCategories();
            return response.ToApiResponse();
        }


        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpPost("categories")]
        public async Task<ActionResult<ApiResponseDto<GlassesCategoryDto>>> CreateCategory([FromBody] CreateNewGlassesCategoryRequestDto request)
        {
            var response = await _glassesService.CreateNewGlassesCategory(request);
            return response.ToApiResponse();
        }


        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpPut("categories/{id}")]
        public async Task<ActionResult<ApiResponseDto<GlassesCategoryDto>>> UpdateCategory(
            int id,
            [FromBody] UpdateGlassesCategoryRequestDto request)
        {
            request.CategoryId = id;
            var response = await _glassesService.UpdateGlassesCategory(request);
            return response.ToApiResponse();
        }

        #endregion

        #region Subcategories

        [AllowAnonymous]
        [HttpGet("subcategories")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<GlassesSubcategoriesListDto>>>> GetSubcategories(
            [FromQuery] int categoryId,
            [FromQuery] bool activeOnly = false)
        {
            if (categoryId <= 0)
            {
                var errorResponse = await _glassesService.GetAllGlassesSubCategoriesByCategoryId(0);
                if (!errorResponse.IsSuccessful)
                    return errorResponse.ToApiResponse();
            }

            var response = activeOnly
                ? await _glassesService.GetGlassesSubCategories(activeOnly, categoryId)
                : await _glassesService.GetAllGlassesSubCategoriesByCategoryId(categoryId);

            return response.ToApiResponse();
        }

        [AllowAnonymous]
        [HttpGet("subcategories/{id}")]
        public async Task<ActionResult<ApiResponseDto<GlassesSubcategoriesListDto>>> GetSubcategory(int id)
        {
            var response = await _glassesService.GetGlassesSubCategoryById(id);
            return response.ToApiResponse();
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpPost("subcategories")]
        public async Task<ActionResult<ApiResponseDto<GlassesSubCategoryDto>>> CreateSubcategory([FromBody] CreateNewGlassesSubCategoryRequestDto request)
        {
            var response = await _glassesService.CreateNewGlassesSubCategory(request);
            return response.ToApiResponse();
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpPut("subcategories/{id}")]
        public async Task<ActionResult<ApiResponseDto<GlassesSubCategoryDto>>> UpdateSubcategory(
            int id,
            [FromBody] UpdateGlassesSubCategoryRequestDto request)
        {
            request.SubCategoryId = id;
            var response = await _glassesService.UpdateGlassesSubCategory(request);
            return response.ToApiResponse();
        }


        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpDelete("subcategories/{id}")]
        public async Task<ActionResult<ApiResponseDto<GlassesSubCategoryDto>>> DeleteSubcategory(int id)
        {
            var response = await _glassesService.SoftDeleteGlassesSubCategory(id);
            return response.ToApiResponse();
        }

        #endregion

        #region Glasses Products

        /// <summary>
        /// Search/Get glasses with filters
        /// Query params:
        /// - glassesName: search by name
        /// - brand: filter by brand
        /// - color: filter by color
        /// - categoryId: filter by category
        /// - subCategoryId: filter by subcategory
        /// - isActive: filter active status (default: true)
        /// - pageNum: page number (default: 1)
        /// - pageSize: page size (default: 10)
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<GlassesListDto>>>> GetGlasses(
            [FromQuery] string? glassesName = null,
            [FromQuery] string? brand = null,
            [FromQuery] string? color = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] int? subCategoryId = null,
            [FromQuery] bool? isActive = true,
            [FromQuery] int pageNum = 1,
            [FromQuery] int pageSize = 25)
        {
            var searchRequest = new SearchGlassesRequestDto
            {
                GlassesName = glassesName,
                Brand = brand,
                Color = color,
                CategoryId = categoryId,
                SubCategoryId = subCategoryId,
                IsActive = isActive ?? true,
                PageNum = pageNum,
                PageSize = pageSize
            };

            var response = await _glassesService.GetGlasses(searchRequest);
            return response.ToApiResponse();
        }


        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<GlassesListDto>>> GetGlasses(int id)
        {
            var response = await _glassesService.GetGlassesById(id);
            return response.ToApiResponse();
        }


        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ApiResponseDto<GlassesDto>>> CreateGlasses([FromForm] CreateNewGlassesRequestDto request)
        {
            var response = await _glassesService.CreateNewGlasses(request);
            return response.ToApiResponse();
        }


        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ApiResponseDto<GlassesDto>>> UpdateGlasses(int id, [FromForm] UpdateGlassesRequestDto request)
        {
            request.GlassesId = id;
            var response = await _glassesService.UpdateGlasses(request);
            return response.ToApiResponse();
        }


        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<GlassesDto>>> DeleteGlasses(int id)
        {
            var response = await _glassesService.SoftDeleteGlasses(id);
            return response.ToApiResponse();
        }

        [AllowAnonymous]
        [HttpGet("attachments/{attachmentId}")]
        public async Task<IActionResult> GetAttachmentImage(int attachmentId)
        {
            var response = await _glassesService.GetAttachmentById(attachmentId);
            
            if (!response.IsSuccessful || response.Payload == null)
            {
                return NotFound();
            }

            var attachment = response.Payload;
            return File(attachment.FileData, attachment.FileType, attachment.FileName);
        }

        #endregion
    }
}
