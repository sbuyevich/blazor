using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MyClass.Core.Options;

namespace MyClass.Core.Services;

public sealed class QuizContentService(
    IOptions<QuizOptions> quizOptions,
    IHostEnvironment hostEnvironment) : IQuizContentService
{
    private const int DefaultAnswerCount = 4;
    private const int MinAnswerCount = 1;
    private const int MaxAnswerCount = 4;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private QuizContent? _cachedQuiz;
    private DateTime _cacheTime = DateTime.MinValue;


    public async Task<QuizContentResult> LoadQuizAsync(CancellationToken cancellationToken = default)
    {
        if (_cachedQuiz != null)
        {
            return QuizContentResult.Success(_cachedQuiz);
        }

        var rootFolder = ResolveRootFolder();

        if (string.IsNullOrWhiteSpace(rootFolder))
        {
            return QuizContentResult.Failure("Quiz root folder is not configured.");
        }

        if (!Directory.Exists(rootFolder))
        {
            return QuizContentResult.Failure($"Quiz root folder was not found: {rootFolder}");
        }

        var rootJsonPath = Path.Combine(rootFolder, "quiz.json");

        if (!File.Exists(rootJsonPath))
        {
            return QuizContentResult.Failure("Quiz root folder must contain quiz.json.");
        }

        var quizMetadata = await ReadJsonAsync<QuizMetadata>(rootJsonPath, cancellationToken);

        if (!quizMetadata.Succeeded || quizMetadata.Value is null)
        {
            return QuizContentResult.Failure(quizMetadata.Message);
        }

        var quizTitle = quizMetadata.Value.Title?.Trim();

        if (string.IsNullOrWhiteSpace(quizTitle))
        {
            return QuizContentResult.Failure("Root quiz.json must define a non-empty title.");
        }

        if (quizMetadata.Value.TimeLimitSeconds <= 0)
        {
            return QuizContentResult.Failure("Root quiz.json must define a positive TimeLimitSeconds value.");
        }

        var defaultAnswerCount = quizMetadata.Value.AnswerCount ?? DefaultAnswerCount;

        if (!IsSupportedAnswerCount(defaultAnswerCount))
        {
            return QuizContentResult.Failure($"Root quiz.json answerCount must be between {MinAnswerCount} and {MaxAnswerCount}.");
        }

        var questionFolders = Directory
            .EnumerateDirectories(rootFolder)
            .OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (questionFolders.Count == 0)
        {
            return QuizContentResult.Failure("Quiz root folder must contain at least one question subfolder.");
        }

        var questions = new List<QuizQuestionContent>();

        for (var index = 0; index < questionFolders.Count; index++)
        {
            var questionResult = await LoadQuestionAsync(
                questionFolders[index],
                index,
                quizMetadata.Value.TimeLimitSeconds,
                defaultAnswerCount,
                cancellationToken);

            if (!questionResult.Succeeded || questionResult.Value is null)
            {
                return QuizContentResult.Failure(questionResult.Message);
            }

            questions.Add(questionResult.Value);
        }

        _cachedQuiz = new QuizContent(quizTitle, quizMetadata.Value.TimeLimitSeconds, questions);
        _cacheTime = DateTime.UtcNow;

        return QuizContentResult.Success(_cachedQuiz);
    }

    public async Task<QuizImageResult> LoadQuestionImageAsync(
        string questionKey,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(questionKey) ||
            questionKey.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
            questionKey.Contains("..", StringComparison.Ordinal))
        {
            return QuizImageResult.Failure("Question image key is invalid.");
        }

        var rootFolder = ResolveRootFolder();
        var questionFolder = Path.Combine(rootFolder, questionKey);
        var fullRoot = Path.GetFullPath(rootFolder);
        var fullQuestionFolder = Path.GetFullPath(questionFolder);

        if (!fullQuestionFolder.StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase))
        {
            return QuizImageResult.Failure("Question image key is invalid.");
        }

        if (!Directory.Exists(fullQuestionFolder))
        {
            return QuizImageResult.Failure("Question image folder was not found.");
        }

        var imagePath = GetQuestionImagePath(fullQuestionFolder);

        if (imagePath is null)
        {
            return QuizImageResult.Failure($"Question folder '{questionKey}' must contain q.jpg.");
        }

        try
        {
            var bytes = await File.ReadAllBytesAsync(imagePath, cancellationToken);
            var mediaType = string.Equals(Path.GetExtension(imagePath), ".jpeg", StringComparison.OrdinalIgnoreCase)
                ? "image/jpeg"
                : "image/jpg";

            return QuizImageResult.Success($"data:{mediaType};base64,{Convert.ToBase64String(bytes)}");
        }
        catch (IOException exception)
        {
            return QuizImageResult.Failure($"Question image could not be read: {exception.Message}");
        }
        catch (UnauthorizedAccessException exception)
        {
            return QuizImageResult.Failure($"Question image could not be read: {exception.Message}");
        }
    }

    private string ResolveRootFolder()
    {
        var configuredRoot = quizOptions.Value.RootFolder?.Trim();

        if (string.IsNullOrWhiteSpace(configuredRoot))
        {
            return string.Empty;
        }

        return Path.IsPathRooted(configuredRoot)
            ? configuredRoot
            : Path.GetFullPath(Path.Combine(hostEnvironment.ContentRootPath, configuredRoot));
    }

    private static async Task<ValueResult<QuizQuestionContent>> LoadQuestionAsync(
        string questionFolder,
        int index,
        int defaultTimeLimitSeconds,
        int defaultAnswerCount,
        CancellationToken cancellationToken)
    {
        var questionKey = Path.GetFileName(questionFolder);

        if (string.IsNullOrWhiteSpace(questionKey))
        {
            return ValueResult<QuizQuestionContent>.Failure("Question subfolder has an invalid name.");
        }

        var metadataPath = Path.Combine(questionFolder, "q.json");

        if (!File.Exists(metadataPath))
        {
            return ValueResult<QuizQuestionContent>.Failure($"Question folder '{questionKey}' must contain q.json.");
        }

        var questionMetadata = await ReadJsonAsync<QuestionMetadata>(metadataPath, cancellationToken);

        if (!questionMetadata.Succeeded || questionMetadata.Value is null)
        {
            return ValueResult<QuizQuestionContent>.Failure(questionMetadata.Message);
        }

        var title = questionMetadata.Value.Question?.Trim();

        if (string.IsNullOrWhiteSpace(title))
        {
            title = questionKey;
        }

        var timeoutSeconds = questionMetadata.Value.TimeLimitSeconds ?? defaultTimeLimitSeconds;

        if (timeoutSeconds <= 0)
        {
            return ValueResult<QuizQuestionContent>.Failure($"Question folder '{questionKey}' must define a positive TimeLimitSeconds value.");
        }

        var answerCount = questionMetadata.Value.AnswerCount ?? defaultAnswerCount;

        if (!IsSupportedAnswerCount(answerCount))
        {
            return ValueResult<QuizQuestionContent>.Failure($"Question folder '{questionKey}' must define answerCount between {MinAnswerCount} and {MaxAnswerCount}.");
        }

        var correctAnswer = questionMetadata.Value.CorrectAnswer?.Trim();

        if (!IsAnswerInRange(correctAnswer, answerCount))
        {
            return ValueResult<QuizQuestionContent>.Failure($"Question folder '{questionKey}' must define correctAnswer between \"1\" and \"{answerCount}\".");
        }

        var imagePath = GetQuestionImagePath(questionFolder);

        if (imagePath is null)
        {
            return ValueResult<QuizQuestionContent>.Failure($"Question folder '{questionKey}' must contain q.jpg.");
        }

        return ValueResult<QuizQuestionContent>.Success(
            new QuizQuestionContent(
                questionKey,
                index,
                title,
                timeoutSeconds,
                answerCount,
                correctAnswer!,
                Uri.EscapeDataString(questionKey)));
    }

    private static bool IsSupportedAnswerCount(int answerCount)
    {
        return answerCount is >= MinAnswerCount and <= MaxAnswerCount;
    }

    private static bool IsAnswerInRange(string? answer, int answerCount)
    {
        return int.TryParse(answer, out var answerNumber) &&
            answerNumber >= 1 &&
            answerNumber <= answerCount &&
            string.Equals(answer, answerNumber.ToString(), StringComparison.Ordinal);
    }

    private static string? GetQuestionImagePath(string questionFolder)
    {
        var matches = Directory
            .EnumerateFiles(questionFolder)
            .Where(file =>
                string.Equals(Path.GetFileNameWithoutExtension(file), "q", StringComparison.OrdinalIgnoreCase) &&
                (string.Equals(Path.GetExtension(file), ".jpg", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(Path.GetExtension(file), ".jpeg", StringComparison.OrdinalIgnoreCase)))
            .ToList();

        return matches.Count == 1 ? matches[0] : null;
    }

    private static async Task<ValueResult<T>> ReadJsonAsync<T>(string path, CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = File.OpenRead(path);
            var value = await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions, cancellationToken);

            return value is null
                ? ValueResult<T>.Failure($"JSON file '{Path.GetFileName(path)}' is empty.")
                : ValueResult<T>.Success(value);
        }
        catch (JsonException exception)
        {
            return ValueResult<T>.Failure($"JSON file '{Path.GetFileName(path)}' is invalid: {exception.Message}");
        }
        catch (IOException exception)
        {
            return ValueResult<T>.Failure($"JSON file '{Path.GetFileName(path)}' could not be read: {exception.Message}");
        }
        catch (UnauthorizedAccessException exception)
        {
            return ValueResult<T>.Failure($"JSON file '{Path.GetFileName(path)}' could not be read: {exception.Message}");
        }
    }

    private sealed record QuizMetadata(string? Title, int TimeLimitSeconds, int? AnswerCount);

    private sealed record QuestionMetadata(string? Question, string? CorrectAnswer, int? TimeLimitSeconds, int? AnswerCount);

    private sealed record ValueResult<T>(bool Succeeded, string Message, T? Value)
    {
        public static ValueResult<T> Success(T value) => new(true, string.Empty, value);

        public static ValueResult<T> Failure(string message) => new(false, message, default);
    }
}


