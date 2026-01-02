namespace NazarMahal.Application.Common
{
    public class ApiResponseDto<T>
    {
        public bool IsSuccessful { get; set; }
        public string? StatusCode { get; set; }
        public string[]? Messages { get; set; }
        public required T Payload { get; set; }
    }

}
