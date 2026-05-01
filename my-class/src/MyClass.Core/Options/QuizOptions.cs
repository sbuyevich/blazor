namespace MyClass.Core.Options;

public sealed class QuizOptions
{
    public string RootFolder { get; set; } = "quiz";

    public int StatusRefreshMilliseconds { get; set; } = 1500;

    public QuizRevealMessagesOptions RevealMessages { get; set; } = new();
}

public sealed class QuizRevealMessagesOptions
{
    public string Correct { get; set; } = "Correct. Good job!";

    public string Incorrect { get; set; } = "Incorrect. Not this time.";

    public string NoAnswer { get; set; } = "No answer submitted. Stay focused!";
}


