namespace MyClass.Core.Models;

public sealed record SchoolClassClassListResult(
    bool Succeeded,
    IReadOnlyList<SchoolClassClassItem> Classes,
    string Message)
{
    public static SchoolClassClassListResult Success(IReadOnlyList<SchoolClassClassItem> classes) =>
        new(true, classes, string.Empty);

    public static SchoolClassClassListResult Failure(string message) =>
        new(false, [], message);
}
