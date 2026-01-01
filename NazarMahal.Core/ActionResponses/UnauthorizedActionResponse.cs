using NazarMahal.Core.Constants;

namespace NazarMahal.Core.ActionResponses
{
    public class UnauthorizedActionResponse<T> : ActionResponse<T>
    {

        public UnauthorizedActionResponse()
        {
            SetDefault(default);
        }

        public UnauthorizedActionResponse(string message) : this(default, message)
        {
        }

        public UnauthorizedActionResponse(List<string> messages) : this(default, messages)
        {
        }

        public UnauthorizedActionResponse(T payload, string message = null)
        {
            Message = message;
            SetDefault(payload);
        }

        public UnauthorizedActionResponse(T payload, List<string> messages)
        {
            Messages = messages;
            SetDefault(payload);
        }

        private void SetDefault(T payload)
        {
            IsSuccessful = false;
            StatusCode = SystemStatusCode.Unauthorized;
            Payload = payload;
        }

    }
}
