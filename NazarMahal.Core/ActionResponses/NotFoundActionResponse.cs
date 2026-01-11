using NazarMahal.Core.Constants;

namespace NazarMahal.Core.ActionResponses
{
    public class NotFoundActionResponse<T> : ActionResponse<T>
    {

        public NotFoundActionResponse()
        {
            SetDefault(default!);
        }

        public NotFoundActionResponse(string message) : this(default!, message)
        {

        }

        public NotFoundActionResponse(List<string> messages) : this(default!, messages)
        {

        }

        public NotFoundActionResponse(T payload, string? message = null)
        {
            Message = message;
            SetDefault(payload);
        }

        public NotFoundActionResponse(T payload, List<string> messages)
        {
            Messages = messages;
            SetDefault(payload);
        }

        private void SetDefault(T payload)
        {
            IsSuccessful = false;
            StatusCode = SystemStatusCode.NotFound;
            Payload = payload;
        }

    }
}
