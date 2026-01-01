namespace NazarMahal.Core.ActionResponses
{
    public abstract class ActionResponse
    {
        public bool IsSuccessful { get; protected set; }
        public string StatusCode { get; protected set; }
        public string Message
        {
            get => Messages == null || !Messages.Any() ? null : string.Join(",", Messages);

            protected set => Messages = string.IsNullOrEmpty(value) ? null : new List<string> { value };
        }
        public List<string> Messages { get; protected set; }

        public static ActionResponse Combine(params ActionResponse[] actionResponses)
        {
            if (actionResponses == null)
                return new OkActionResponse<string>();

            var errorMessages = new List<string>();
            foreach (var response in actionResponses)
            {
                if (response != null)
                {
                    if (!response.IsSuccessful)
                        errorMessages.AddRange(response.Messages);
                }
            }

            if (errorMessages.Count > 0)
                return new FailActionResponse<string>(errorMessages);

            return new OkActionResponse<string>();
        }

    }

    public abstract class ActionResponse<T> : ActionResponse
    {
        public T Payload { get; protected set; }

        //Chain OnSuccess with different return type
        public ActionResponse<TResult> OnSuccess<TResult>(Func<T, ActionResponse<TResult>> action)
        {
            if (!IsSuccessful)
                return new FailActionResponse<TResult>()
                {
                    Messages = Messages,
                    StatusCode = StatusCode
                };

            return action(Payload);
        }

        //Chain OnSuccess with same return type
        public ActionResponse<T> OnSuccess(Func<T, ActionResponse<T>> action)
        {
            if (!IsSuccessful)
                return this;

            return action(Payload);
        }


        #region Helper methods for Ok
        public static ActionResponse<T> Ok()
        {
            return new OkActionResponse<T>();
        }

        public static ActionResponse<T> Ok(string message)
        {
            return new OkActionResponse<T>(message);
        }

        public static ActionResponse<T> Ok(List<string> messages)
        {
            return new OkActionResponse<T>(messages);
        }

        public static ActionResponse<T> Ok(T payload, string message = null)
        {
            return new OkActionResponse<T>(payload, message);
        }

        public static ActionResponse<T> Ok(T payload, List<string> messages)
        {
            return new OkActionResponse<T>(payload, messages);
        }
        #endregion


        #region Helper methods for Fail
        public static ActionResponse<T> Fail()
        {
            return new FailActionResponse<T>();
        }

        public static ActionResponse<T> Fail(string message)
        {
            return new FailActionResponse<T>(message);
        }

        public static ActionResponse<T> Fail(List<string> messages)
        {
            return new FailActionResponse<T>(messages);
        }

        public static ActionResponse<T> Fail(T payload, string message = null)
        {
            return new FailActionResponse<T>(payload, message);
        }

        public static ActionResponse<T> Fail(T payload, List<string> messages)
        {
            return new FailActionResponse<T>(payload, messages);
        }
        #endregion


        #region Helper methods for Not Found
        public static ActionResponse<T> NotFound()
        {
            return new NotFoundActionResponse<T>();
        }

        public static ActionResponse<T> NotFound(string message)
        {
            return new NotFoundActionResponse<T>(message);
        }

        public static ActionResponse<T> NotFound(List<string> messages)
        {
            return new NotFoundActionResponse<T>(messages);
        }

        public static ActionResponse<T> NotFound(T payload, string message = null)
        {
            return new NotFoundActionResponse<T>(payload, message);
        }

        public static ActionResponse<T> NotFound(T payload, List<string> messages)
        {
            return new NotFoundActionResponse<T>(payload, messages);
        }
        #endregion


        #region Helper methods for AuthTokenExpired
        public static ActionResponse<T> AuthTokenExpired()
        {
            return new AuthTokenExpiredActionResponse<T>();
        }

        public static ActionResponse<T> AuthTokenExpired(string message)
        {
            return new AuthTokenExpiredActionResponse<T>(message);
        }

        public static ActionResponse<T> AuthTokenExpired(List<string> messages)
        {
            return new AuthTokenExpiredActionResponse<T>(messages);
        }

        public static ActionResponse<T> AuthTokenExpired(T payload, string message = null)
        {
            return new AuthTokenExpiredActionResponse<T>(payload, message);
        }

        public static ActionResponse<T> AuthTokenExpired(T payload, List<string> messages)
        {
            return new AuthTokenExpiredActionResponse<T>(payload, messages);
        }
        #endregion


    }
}
