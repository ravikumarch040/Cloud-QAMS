namespace Qams.BuildingBlocks.Common;

public sealed record ApiResponse<T>(
    bool Success,
    T? Data,
    IReadOnlyCollection<ApiError> Errors,
    IReadOnlyCollection<string> Warnings,
    string CorrelationId,
    string ApiVersion);

public static class ApiResponse
{
    public static ApiResponse<T> Ok<T>(T data, string correlationId, string apiVersion = "v1") =>
        new(true, data, Array.Empty<ApiError>(), Array.Empty<string>(), correlationId, apiVersion);

    public static ApiResponse<T> Fail<T>(ApiError error, string correlationId, string apiVersion = "v1") =>
        new(false, default, new[] { error }, Array.Empty<string>(), correlationId, apiVersion);
}

public sealed record ApiError(string Code, string Message, string? Target = null);
