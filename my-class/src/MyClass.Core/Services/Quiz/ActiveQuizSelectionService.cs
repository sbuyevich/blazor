using System.Collections.Concurrent;

namespace MyClass.Core.Services;

public sealed class ActiveQuizSelectionService : IActiveQuizSelectionService
{
    private readonly ConcurrentDictionary<int, string> _selectedQuizPaths = new();

    public string? GetSelectedQuizPath(int classId)
    {
        return _selectedQuizPaths.TryGetValue(classId, out var quizPath)
            ? quizPath
            : null;
    }

    public void SetSelectedQuizPath(int classId, string? quizPath)
    {
        if (string.IsNullOrWhiteSpace(quizPath))
        {
            ClearSelectedQuizPath(classId);
            return;
        }

        _selectedQuizPaths[classId] = quizPath;
    }

    public void ClearSelectedQuizPath(int classId)
    {
        _selectedQuizPaths.TryRemove(classId, out _);
    }
}
