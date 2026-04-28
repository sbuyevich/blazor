namespace MyClass.Core.Models;

public sealed record QuizTeacherState(
    string QuizTitle,
    bool HasSession,
    bool IsComplete,
    QuizTeacherQuestionState? CurrentQuestion,
    IReadOnlyList<QuizStudentAnswerStatus> Students);


