using NazarMahal.Core.Constants;

namespace NazarMahal.Core.ActionResponses
{
    public class OkActionResponse<T> : ActionResponse<T>
    {

        public OkActionResponse()
        {
            SetDefault(default!);
        }

        public OkActionResponse(string message) : this(default!, message)
        {
        }

        public OkActionResponse(List<string> messages) : this(default!, messages)
        {
        }

        public OkActionResponse(T payload, string? message = null)
        {
            Message = message;
            SetDefault(payload);
        }

        public OkActionResponse(T payload, List<string> messages)
        {
            Messages = messages;
            SetDefault(payload);
        }

        private void SetDefault(T payload)
        {
            IsSuccessful = true;
            StatusCode = SystemStatusCode.Ok;
            Payload = payload;
        }

    }
}
