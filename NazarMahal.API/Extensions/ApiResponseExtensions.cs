using Microsoft.AspNetCore.Mvc;
using NazarMahal.Application.Common;
using NazarMahal.Core.ActionResponses;


namespace NazarMahal.API.Extensions
{
    public static class ApiResponseExtensions
    {
        public static ActionResult<ApiResponseDto<T>> ToApiResponse<T>(this ActionResponse<T> apiResponse)
        {
            var result = new ApiResponseDto<T>
            {
                IsSuccessful = apiResponse.IsSuccessful,
                StatusCode = apiResponse.StatusCode,
                Messages = apiResponse.Messages != null ? [.. apiResponse.Messages] : [],
                Payload = apiResponse.Payload
            };

            return new OkObjectResult(result);
        }
    }
}
