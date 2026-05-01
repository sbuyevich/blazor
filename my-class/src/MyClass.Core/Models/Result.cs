namespace MyClass.Core.Models;

public sealed record Result<T>(
    bool Succeeded,
    string Message,
    T? Value)
{
    public static Result<T> Success(T value, string message = "") =>
        new(true, message, value);

    public static Result<T> Failure(string message) =>
        new(false, message, default);
}
