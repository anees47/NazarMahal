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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GlassesController : ControllerBase
    {
        private readonly IGlassesService _glassesService;
        public GlassesController(IGlassesService glassesService)
        {
            _glassesService = glassesService;
        }

        #region Glasses Categories

        [AllowAnonymous]
        [HttpGet("GetAllCategories")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<GlassesCategoryDto>>>> GetAllCategories()
        {
            var response = await _glassesService.GetAllCategories();
            return response.ToApiResponse();
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpPost("CreateNewGlassesCategory")]
        public async Task<ActionResult<ApiResponseDto<GlassesCategoryDto>>> CreateNewGlassesCategory(CreateNewGlassesCategoryRequestDto createNewGlassesCategoryRequestDto)
        {
            var response = await _glassesService.CreateNewGlassesCategory(createNewGlassesCategoryRequestDto);
            return response.ToApiResponse();
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpPut("UpdateGlassesCategory")]
        public async Task<ActionResult<ApiResponseDto<GlassesCategoryDto>>> UpdateGlassesCategory(UpdateGlassesCategoryRequestDto updateGlassesCategoryRequestDto)
        {
            var response = await _glassesService.UpdateGlassesCategory(updateGlassesCategoryRequestDto);
            return response.ToApiResponse();
        }

        #endregion

        //TODO: #3, fix this
        #region Glasses SubCategory 
        [AllowAnonymous]
        [HttpGet("GetAllGlassesSubCategories/{categoryId}")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<GlassesSubcategoriesListDto>>>> GetGlassesSubCategories(int categoryId)
        {
            var response = await _glassesService.GetAllGlassesSubCategoriesByCategoryId(categoryId);
            return response.ToApiResponse();
        }

        [AllowAnonymous]
        [HttpGet("GetGlassesSubCategories/{showActiveOnly}/{categoryId}")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<GlassesSubcategoriesListDto>>>> GetGlassesSubCategories(bool showActiveOnly, int categoryId)
        {
            var response = await _glassesService.GetGlassesSubCategories(showActiveOnly, categoryId);
            return response.ToApiResponse();
        }


        [AllowAnonymous]
        [HttpGet("GetGlassesSubCategoryById/{subCategoryId}")]
        public async Task<ActionResult<ApiResponseDto<GlassesSubcategoriesListDto>>> GetGlassesSubCategoryById(int subCategoryId)
        {
            var response = await _glassesService.GetGlassesSubCategoryById(subCategoryId);
            return response.ToApiResponse();
        }


        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpPost("CreateNewGlassesSubCategory")]
        public async Task<ActionResult<ApiResponseDto<GlassesSubCategoryDto>>> CreateNewGlassesSubCategory(CreateNewGlassesSubCategoryRequestDto createNewGlassesSubCategoryRequestDto)
        {
            var response = await _glassesService.CreateNewGlassesSubCategory(createNewGlassesSubCategoryRequestDto);
            return response.ToApiResponse();
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpPut("UpdateGlassesSubCategory")]
        public async Task<ActionResult<ApiResponseDto<GlassesSubCategoryDto>>> UpdateGlassesSubCategory(UpdateGlassesSubCategoryRequestDto updateGlassesSubCategoryRequestDto)
        {
            var response = await _glassesService.UpdateGlassesSubCategory(updateGlassesSubCategoryRequestDto);
            return response.ToApiResponse();
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpDelete("DeleteGlassesSubCategory/{Id}")]
        public async Task<ActionResult<ApiResponseDto<GlassesSubCategoryDto>>> DeleteGlassesSubCategory(int Id)
        {
            var response = await _glassesService.SoftDeleteGlassesSubCategory(Id);
            return response.ToApiResponse();
        }

        #endregion
        //TODO: #4,Fix this
        #region Glasses
        [AllowAnonymous]
        [HttpPost("GetAllGlassess")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<GlassesListDto>>>> GetGlasses(SearchGlassesRequestDto requestDto)
        {
            var response = await _glassesService.GetGlasses(requestDto);
            return response.ToApiResponse();
        }

        [AllowAnonymous]
        [HttpGet("GetGlassesById/{id}")]
        public async Task<ActionResult<ApiResponseDto<GlassesListDto>>> GetGlassesById(int id)
        {
            var response = await _glassesService.GetGlassesById(id);
            return response.ToApiResponse();
        }


        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpPost("CreateNewGlasses")]
        public async Task<ActionResult<ApiResponseDto<GlassesDto>>> CreateNewGlasses([FromForm] CreateNewGlassesRequestDto createNewGlassesRequestDto)
        {
            var response = await _glassesService.CreateNewGlasses(createNewGlassesRequestDto);
            return response.ToApiResponse();
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpPut("UpdateGlasses")]
        public async Task<ActionResult<ApiResponseDto<GlassesDto>>> UpdateGlasses([FromForm] UpdateGlassesRequestDto updateGlassesRequestDto)
        {
            var response = await _glassesService.UpdateGlasses(updateGlassesRequestDto);
            return response.ToApiResponse();
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpDelete("DeleteGlasses/{Id}")]
        public async Task<ActionResult<ApiResponseDto<GlassesDto>>> DeleteGlasses(int Id)
        {
            var response = await _glassesService.SoftDeleteGlasses(Id);
            return response.ToApiResponse();
        }
        #endregion
    }
}
