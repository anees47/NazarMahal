namespace NazarMahal.Application.Common
{
    public class ApiResponseDto<T>
    {
        public bool IsSuccessful { get; set; }
        public required string StatusCode { get; set; }
        public required string[] Messages { get; set; }
        public required T Payload { get; set; }
    }

}
