namespace PrintQueueService.Application.DTOs;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public int StatusCode { get; set; }

    public static ServiceResult<T> Ok(T data) => new()
    {
        Success = true,
        Data = data,
        StatusCode = 200
    };

    public static ServiceResult<T> Created(T data) => new()
    {
        Success = true,
        Data = data,
        StatusCode = 201
    };

    public static ServiceResult<T> NotFound(string message) => new()
    {
        Success = false,
        ErrorMessage = message,
        StatusCode = 404
    };

    public static ServiceResult<T> Conflict(string message) => new()
    {
        Success = false,
        ErrorMessage = message,
        StatusCode = 409
    };

    public static ServiceResult<T> BadRequest(string message) => new()
    {
        Success = false,
        ErrorMessage = message,
        StatusCode = 400
    };
}
