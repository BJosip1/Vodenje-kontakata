namespace Application.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public bool IsNotFound { get; }
        public List<string> ErrorItems { get; }
        public T Value { get; }

        private Result(bool isSuccess, bool isNotFound, List<string> errorList, T value)
        {
            IsSuccess = isSuccess;
            IsNotFound = isNotFound;
            ErrorItems = errorList;
            Value = value;
        }

        // SUCCESS (GET, returns value)
        public static Result<T> Success(T value) => new Result<T>(true, false, new List<string>(), value);

        // SUCCESS (POST, no value)
        public static Result<T> Success() => new Result<T>(true, false, new List<string>(), default);

        public static Result<T> Failure(List<string> errorList) => new Result<T>(false, false, errorList, default);

        public static Result<T> NotFound(string message) => new Result<T>(false, true, new List<string> { message }, default);
    }
}
