namespace WordWise.Api.Common.Results
{
    public class ServiceResult
    {
        public bool IsSuccess { get; protected set; }
        public string? Message { get; protected set; }
        public List<string> Errors { get; protected set; } = new List<string>();

        protected ServiceResult(bool isSuccess, string? message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        protected ServiceResult(bool isSuccess, string? message, string error) : this(isSuccess, message)
        {
            if (!string.IsNullOrWhiteSpace(error))
            {
                Errors.Add(error);
            }
        }

        protected ServiceResult(bool isSuccess, string? message, IEnumerable<string> errors)
        : this(isSuccess, message)
        {
            if (errors != null)
            {
                Errors.AddRange(errors.Where(e => !string.IsNullOrWhiteSpace(e)));
            }
        }

        // Static factory methods để tạo instances
        public static ServiceResult Success(string? message = null) => new ServiceResult(true, message);

        public static ServiceResult Failure(string error) => new ServiceResult(false, null, error);

        public static ServiceResult Failure(IEnumerable<string> errors) => new ServiceResult(false, null, errors);
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; private set; }

        // Constructor được bảo vệ
        protected ServiceResult(T? data, bool isSuccess, string? message = null)
        : base(isSuccess, message)
        {
            Data = data;
        }

        protected ServiceResult(T? data, bool isSuccess, string? message, string error)
            : base(isSuccess, message, error)
        {
            Data = data;
        }

        protected ServiceResult(T? data, bool isSuccess, string? message, IEnumerable<string> errors)
            : base(isSuccess, message, errors)
        {
            Data = data;
        }

        public static ServiceResult<T> Success(T data, string? message = null) => new ServiceResult<T>(data, true, message);

        public static new ServiceResult<T> Failure(string error) => new ServiceResult<T>(default, false, null, error);

        public static new ServiceResult<T> Failure(IEnumerable<string> errors) => new ServiceResult<T>(default, false, null, errors);
    }
}
