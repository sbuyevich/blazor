using System.Collections.Generic;

namespace MyClass.Models;

public sealed class Quiz
{
    public string Title { get; set; } = "Quiz";

    public int TimeLimitSeconds { get; set; } = 10;

    public List<QuizQuestion> Questions { get; set; } = new ();
}
