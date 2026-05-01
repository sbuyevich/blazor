namespace MyClass.Core.Services;

public interface IActiveQuizSelectionService
{
    string? GetSelectedQuizPath(int classId);

    void SetSelectedQuizPath(int classId, string? quizPath);

    void ClearSelectedQuizPath(int classId);
}
