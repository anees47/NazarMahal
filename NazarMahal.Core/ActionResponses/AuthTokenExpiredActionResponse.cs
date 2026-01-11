using NazarMahal.Core.Constants;

namespace NazarMahal.Core.ActionResponses
{
    public class AuthTokenExpiredActionResponse<T> : ActionResponse<T>
    {

        public AuthTokenExpiredActionResponse()
        {
            SetDefault(default!);
        }

        public AuthTokenExpiredActionResponse(string message) : this(default!, message)
        {

        }

        public AuthTokenExpiredActionResponse(List<string> messages) : this(default!, messages)
        {
        }

        public AuthTokenExpiredActionResponse(T payload, string? message = null)
        {
            Message = message;
            SetDefault(payload);
        }

        public AuthTokenExpiredActionResponse(T payload, List<string> messages)
        {
            Messages = messages;
            SetDefault(payload);
        }

        private void SetDefault(T payload)
        {
            IsSuccessful = false;
            StatusCode = SystemStatusCode.AuthTokenExpired;
            Payload = payload;
        }
    }
}
