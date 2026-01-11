using NazarMahal.Core.Constants;

namespace NazarMahal.Core.ActionResponses
{
    public class PermissionDeniedActionResponse<T> : ActionResponse<T>
    {

        public PermissionDeniedActionResponse() : this(default!, null)
        {

        }

        public PermissionDeniedActionResponse(string message) : this(default!, message)
        {

        }

        public PermissionDeniedActionResponse(T payload, string? message = null)
        {
            IsSuccessful = false;
            StatusCode = SystemStatusCode.PermissionDenied;
            Message = message;
            Payload = payload;
        }
    }
}
