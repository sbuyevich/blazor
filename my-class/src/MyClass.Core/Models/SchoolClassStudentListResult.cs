namespace MyClass.Core.Models;

public sealed record SchoolClassStudentListResult(
    bool Succeeded,
    IReadOnlyList<SchoolClassStudentItem> Students,
    string Message)
{
    public static SchoolClassStudentListResult Success(IReadOnlyList<SchoolClassStudentItem> students) =>
        new(true, students, string.Empty);

    public static SchoolClassStudentListResult Failure(string message) =>
        new(false, [], message);
}
