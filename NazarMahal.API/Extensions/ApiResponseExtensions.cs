using AutoMapper;
using NazarMahal.Application.Common;
using Microsoft.AspNetCore.Mvc;
using NazarMahal.Core.ActionResponses;


namespace NazarMahal.API.Extensions
{
    public static class ApiResponseExtensions
    {
        public static ActionResult<ApiResponseDto<TDestination>> ToApiResponse<TSource, TDestination>(this ActionResponse<TSource> apiResponse, IMapper mapper)
        {
            var result = mapper.Map<ApiResponseDto<TDestination>>(apiResponse);

            return new OkObjectResult(result);
        }

        public static ActionResult<ApiResponseDto<T>> ToApiResponse<T>(this ActionResponse<T> apiResponse, IMapper mapper)
        {
            var result = mapper.Map<ApiResponseDto<T>>(apiResponse);

            return new OkObjectResult(result);
        }

        public static ActionResult<ApiResponseDto<T>> ToApiResponse<T>(this ActionResponse<T> apiResponse)
        {
            var result = new ApiResponseDto<T>
            {
                IsSuccessful = apiResponse.IsSuccessful,
                StatusCode = apiResponse.StatusCode,
                Messages = [.. apiResponse.Messages],
                Payload = apiResponse.Payload
            };

            return new OkObjectResult(result);
        }
    }
}
