using NazarMahal.Core.ActionResponses;
using NazarMahal.Core.Common;

namespace NazarMahal.Application.Common
{
    public class FileContentType : ValueObject
    {

        public string Value { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        private FileContentType() { }


        private FileContentType(string value)
        {
            Value = value;
        }


        public static ActionResponse<FileContentType> Create(string contentType)
        {
                if (string.IsNullOrEmpty(contentType))
                return new FailActionResponse<FileContentType>(new FileContentType(string.Empty), "Content type is missing");

            return new OkActionResponse<FileContentType>(new FileContentType(contentType));
        }


        public static implicit operator string(FileContentType contentType)
        {
            return contentType == null ? null : contentType.Value;
        }

        public override string ToString()
        {
            return Value;
        }

        public bool IsImage()
        {
            return string.Equals(Value, "image/jpg", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(Value, "image/jpeg", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(Value, "image/pjpeg", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(Value, "image/x-png", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(Value, "image/gif", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(Value, "image/png", StringComparison.OrdinalIgnoreCase);
        }

        public bool IsPdf()
        {
            return string.Equals(Value, "application/pdf", StringComparison.OrdinalIgnoreCase);
        }

    }
}
