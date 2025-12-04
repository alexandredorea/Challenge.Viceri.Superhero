namespace Challenge.Viceri.Superhero.Api.Commons;

/// <summary>
///
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class ApiResult<T>
{
    /// <summary>
    ///
    /// </summary>
    public bool Success { get; private set; }

    /// <summary>
    ///
    /// </summary>
    public string Message { get; private set; } = string.Empty;

    /// <summary>
    ///
    /// </summary>
    public T? Data { get; private set; }

    /// <summary>
    ///
    /// </summary>
    public IEnumerable<ErrorDetail>? Error { get; private set; }

    /// <summary>
    ///
    /// </summary>
    /// <param name="data"></param>
    /// <param name="message"></param>
    /// <returns></returns>
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

    /// <summary>
    ///
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="errorCode"></param>
    /// <returns></returns>
    public static ApiResult<T> FailureResult(string title, string message, string errorCode)
    {
        return new ApiResult<T>
        {
            Success = false,
            Message = title,
            Data = default,
            Error =
            [
                new(errorCode, message)
            ]
        };
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="title"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static ApiResult<T> FailureResult(string title, ErrorDetail error)
    {
        return new ApiResult<T>
        {
            Success = false,
            Message = title,
            Data = default,
            Error = [error]
        };
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="title"></param>
    /// <param name="errors"></param>
    /// <returns></returns>
    public static ApiResult<T> FailureResult(string title, IEnumerable<ErrorDetail>? errors = null)
    {
        return new ApiResult<T>
        {
            Success = false,
            Message = title,
            Data = default,
            Error = errors
        };
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="message"></param>
    /// <param name="errorCode"></param>
    /// <returns></returns>
    public static ApiResult<T> FailureResult(string message, string errorCode)
    {
        return new ApiResult<T>
        {
            Success = false,
            Message = "Ocorreu um ou mais erros.",
            Data = default,
            Error =
            [
                new(errorCode, message)
            ]
        };
    }
}

/// <summary>
///
/// </summary>
/// <param name="Code"></param>
/// <param name="Message"></param>
public sealed record ErrorDetail(string Code, string Message)
{
    public static readonly ErrorDetail None = new(string.Empty, string.Empty);
    public static readonly ErrorDetail NullValue = new("NULL_VALUE", "Foi fornecido um valor nulo.");
    public static readonly ErrorDetail NotFound = new("NOT_FOUND", "O recurso solicitado não foi encontrado.");
    public static readonly ErrorDetail Conflict = new("CONFLICT_ERROR", "Ocorreu um conflito durante a operação.");
    public static readonly ErrorDetail Validation = new("VALIDATION_ERROR", "Ocorreram um ou mais erros de validação.");
    public static readonly ErrorDetail InternalServer = new("INTERNAL_SERVER_ERROR", "Ocorreu um erro interno no servidor.");
}