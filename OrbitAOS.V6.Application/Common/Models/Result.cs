namespace OrbitAOS.V6.Application.Common.Models
{
    /// <summary>
    /// Generic result wrapper for operation responses
    /// </summary>
    /// <typeparam name="T">Type of data returned</typeparam>
    public class Result<T>
    {
        public bool Succeeded { get; set; }
        public T? Data { get; set; }
        public string[] Errors { get; set; } = Array.Empty<string>();
        public string? Message { get; set; }

        public static Result<T> Success(T data, string? message = null)
        {
            return new Result<T>
            {
                Succeeded = true,
                Data = data,
                Message = message
            };
        }

        public static Result<T> Failure(params string[] errors)
        {
            return new Result<T>
            {
                Succeeded = false,
                Errors = errors
            };
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T>
            {
                Succeeded = false,
                Errors = new[] { error }
            };
        }
    }

    /// <summary>
    /// Result wrapper without data
    /// </summary>
    public class Result
    {
        public bool Succeeded { get; set; }
        public string[] Errors { get; set; } = Array.Empty<string>();
        public string? Message { get; set; }

        public static Result Success(string? message = null)
        {
            return new Result
            {
                Succeeded = true,
                Message = message
            };
        }

        public static Result Failure(params string[] errors)
        {
            return new Result
            {
                Succeeded = false,
                Errors = errors
            };
        }

        public static Result Failure(string error)
        {
            return new Result
            {
                Succeeded = false,
                Errors = new[] { error }
            };
        }
    }
}
