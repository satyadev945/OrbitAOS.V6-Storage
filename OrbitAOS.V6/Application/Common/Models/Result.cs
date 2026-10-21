// =============================================================================
// Application/Common/Models/Result.cs
// Clean Architecture: Application Layer - Result Pattern
//
// Migration Notes:
//   - Provides a consistent return type for Application layer operations
//   - Replaces exception-driven error handling in legacy controllers
//   - Enables clean error propagation from Application to Web layer
// =============================================================================

namespace OrbitAOS.V6.Application.Common.Models
{
    /// <summary>
    /// Represents the result of an Application layer operation.
    /// Encapsulates success/failure state and associated data or errors.
    /// </summary>
    public class Result
    {
        /// <summary>Indicates whether the operation succeeded.</summary>
        public bool Succeeded { get; private set; }

        /// <summary>Collection of error messages if the operation failed.</summary>
        public IEnumerable<string> Errors { get; private set; }

        /// <summary>Creates a successful result.</summary>
        public static Result Success() => new Result { Succeeded = true, Errors = Array.Empty<string>() };

        /// <summary>Creates a failed result with the specified errors.</summary>
        public static Result Failure(IEnumerable<string> errors) =>
            new Result { Succeeded = false, Errors = errors };

        /// <summary>Creates a failed result with a single error message.</summary>
        public static Result Failure(string error) =>
            new Result { Succeeded = false, Errors = new[] { error } };
    }

    /// <summary>
    /// Represents the result of an Application layer operation with a return value.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    public class Result<T>
    {
        /// <summary>Indicates whether the operation succeeded.</summary>
        public bool Succeeded { get; private set; }

        /// <summary>The result value (only valid when Succeeded is true).</summary>
        public T? Value { get; private set; }

        /// <summary>Collection of error messages if the operation failed.</summary>
        public IEnumerable<string> Errors { get; private set; } = Array.Empty<string>();

        /// <summary>Creates a successful result with the specified value.</summary>
        public static Result<T> Success(T value) =>
            new Result<T> { Succeeded = true, Value = value };

        /// <summary>Creates a failed result with the specified errors.</summary>
        public static Result<T> Failure(IEnumerable<string> errors) =>
            new Result<T> { Succeeded = false, Errors = errors };

        /// <summary>Creates a failed result with a single error message.</summary>
        public static Result<T> Failure(string error) =>
            new Result<T> { Succeeded = false, Errors = new[] { error } };
    }
}
