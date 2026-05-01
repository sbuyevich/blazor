namespace MyClass.Core.Models;

public sealed record SchoolClassListResult(
    bool Succeeded,
    IReadOnlyList<SchoolClassSchoolItem> Schools,
    string Message)
{
    public static SchoolClassListResult Success(IReadOnlyList<SchoolClassSchoolItem> schools) =>
        new(true, schools, string.Empty);

    public static SchoolClassListResult Failure(string message) =>
        new(false, [], message);
}
