namespace MyClass.Core.Models;

public sealed record SchoolClassSchoolItem(
    int Id,
    string Name,
    int ClassCount,
    IReadOnlyList<SchoolClassClassItem> Classes);
