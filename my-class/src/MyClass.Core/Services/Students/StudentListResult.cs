namespace MyClass.Core.Services.Students;

public sealed record StudentListResult(
    bool Succeeded,
    IReadOnlyList<StudentListItem> Students,
    string Message)
{
    public static StudentListResult Success(IReadOnlyList<StudentListItem> students) =>
        new(true, students, string.Empty);

    public static StudentListResult Failure(string message) =>
        new(false, [], message);
}


