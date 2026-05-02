namespace MyClass.Core.Models;

public sealed record QuizResultPageState(
    IReadOnlyList<QuizResultStudentSummary> StudentSummaries,
    IReadOnlyList<QuizResultQuestionSummary> QuestionSummaries,
    IReadOnlyList<QuizResultExportRow> ExportRows)
{
    public bool HasResults => ExportRows.Count > 0;
}
