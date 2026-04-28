namespace MyClass.Core.Services;

public sealed record QuizTeacherStateResult(
    bool Succeeded,
    string Message,
    QuizTeacherState? State)
{
    public static QuizTeacherStateResult Success(QuizTeacherState state) => new(true, string.Empty, state);

    public static QuizTeacherStateResult Failure(string message) => new(false, message, null);
}


