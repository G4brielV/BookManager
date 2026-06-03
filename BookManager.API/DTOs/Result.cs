namespace BookManager.API.DTOs;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string? Error { get; }

    // Construtor
    private Result(bool isSuccess, T? data, string? error)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
    }
    public static Result<T> Success(T data) 
        => new Result<T>(true, data, null);

    public static Result<T> Failure(string error) 
        => new Result<T>(false, default, error);
}