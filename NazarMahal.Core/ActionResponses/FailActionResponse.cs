using NazarMahal.Core.Constants;

namespace NazarMahal.Core.ActionResponses
{
    public class FailActionResponse<T> : ActionResponse<T>
    {

        public FailActionResponse()
        {
            SetDefault(default!);
        }

        public FailActionResponse(string message) : this(default!, message)
        {
        }

        public FailActionResponse(List<string> messages) : this(default!, messages)
        {
        }

        public FailActionResponse(T payload, string? message = null)
        {
            Message = message;
            SetDefault(payload);
        }

        public FailActionResponse(T payload, List<string> messages)
        {
            Messages = messages;
            SetDefault(payload);
        }

        private void SetDefault(T payload)
        {
            IsSuccessful = false;
            StatusCode = SystemStatusCode.Fail;
            Payload = payload;
        }

    }
}
