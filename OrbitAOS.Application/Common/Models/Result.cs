namespace OrbitAOS.Application.Common.Models
{
    /// <summary>
    /// Generic result wrapper for application service operations.
    /// </summary>
    /// <typeparam name="T">The type of the result data.</typeparam>
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Data { get; private set; }
        public string? ErrorMessage { get; private set; }
        public IEnumerable<string> Errors { get; private set; } = Enumerable.Empty<string>();

        private Result() { }

        public static Result<T> Success(T data) =>
            new Result<T> { IsSuccess = true, Data = data };

        public static Result<T> Failure(string errorMessage) =>
            new Result<T> { IsSuccess = false, ErrorMessage = errorMessage };

        public static Result<T> Failure(IEnumerable<string> errors) =>
            new Result<T> { IsSuccess = false, Errors = errors, ErrorMessage = string.Join("; ", errors) };
    }

    /// <summary>
    /// Non-generic result wrapper for void operations.
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public string? ErrorMessage { get; private set; }
        public IEnumerable<string> Errors { get; private set; } = Enumerable.Empty<string>();

        private Result() { }

        public static Result Success() => new Result { IsSuccess = true };

        public static Result Failure(string errorMessage) =>
            new Result { IsSuccess = false, ErrorMessage = errorMessage };

        public static Result Failure(IEnumerable<string> errors) =>
            new Result { IsSuccess = false, Errors = errors, ErrorMessage = string.Join("; ", errors) };
    }
}
