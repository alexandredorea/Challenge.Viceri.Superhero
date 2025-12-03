namespace Challenge.Viceri.Superhero.Api.Commons;

public sealed class ApiResult<T>
{
    public bool Success { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public T? Data { get; private set; }
    public List<ErrorDetail>? Error { get; private set; }

    public static ApiResult<T> SuccessResult(T? data, string message = "Operação realizada com sucesso")
    {
        return new ApiResult<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Error = null
        };
    }

    public static ApiResult<T> FailureResult(string message, List<ErrorDetail>? errors = null)
    {
        return new ApiResult<T>
        {
            Success = false,
            Message = message,
            Data = default,
            Error = errors
        };
    }

    public static ApiResult<T> FailureResult(string message, string errorCode)
    {
        return new ApiResult<T>
        {
            Success = false,
            Message = "Ocorreu um ou mais erros",
            Data = default,
            Error =
            [
                new(errorCode, message)
            ]
        };
    }
}

public sealed record ErrorDetail(string Code, string Message);