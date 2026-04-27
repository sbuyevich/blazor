namespace MyClass.Core.Options;

public sealed class QuizOptions
{
    public string RootFolder { get; set; } = "quiz";

    public int StatusRefreshMilliseconds { get; set; } = 1500;
}


