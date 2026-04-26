using System.Text.Json;
using Microsoft.Extensions.Options;
using MyClass.Options;

namespace MyClass.Services.Quiz;

public sealed class QuizContentService(
    IOptions<QuizOptions> quizOptions,
    IHostEnvironment hostEnvironment) : IQuizContentService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<QuizContentResult> LoadQuizAsync(CancellationToken cancellationToken = default)
    {
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
            var questionResult = await LoadQuestionAsync(questionFolders[index], index, cancellationToken);

            if (!questionResult.Succeeded || questionResult.Value is null)
            {
                return QuizContentResult.Failure(questionResult.Message);
            }

            questions.Add(questionResult.Value);
        }

        return QuizContentResult.Success(new QuizContent(quizTitle, questions));
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
        CancellationToken cancellationToken)
    {
        var questionKey = Path.GetFileName(questionFolder);

        if (string.IsNullOrWhiteSpace(questionKey))
        {
            return ValueResult<QuizQuestionContent>.Failure("Question subfolder has an invalid name.");
        }

        var metadataPath = Path.Combine(questionFolder, "question.json");

        if (!File.Exists(metadataPath))
        {
            return ValueResult<QuizQuestionContent>.Failure($"Question folder '{questionKey}' must contain question.json.");
        }

        var questionMetadata = await ReadJsonAsync<QuestionMetadata>(metadataPath, cancellationToken);

        if (!questionMetadata.Succeeded || questionMetadata.Value is null)
        {
            return ValueResult<QuizQuestionContent>.Failure(questionMetadata.Message);
        }

        var title = questionMetadata.Value.Title?.Trim();

        if (string.IsNullOrWhiteSpace(title))
        {
            return ValueResult<QuizQuestionContent>.Failure($"Question folder '{questionKey}' must define a non-empty title.");
        }

        if (questionMetadata.Value.TimeoutSeconds <= 0)
        {
            return ValueResult<QuizQuestionContent>.Failure($"Question folder '{questionKey}' must define a positive timeoutSeconds value.");
        }

        if (questionMetadata.Value.CorrectAnswer is < 1 or > 4)
        {
            return ValueResult<QuizQuestionContent>.Failure($"Question folder '{questionKey}' must define correctAnswer between 1 and 4.");
        }

        var jpgFiles = Directory
            .EnumerateFiles(questionFolder)
            .Where(file =>
                string.Equals(Path.GetExtension(file), ".jpg", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(Path.GetExtension(file), ".jpeg", StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (jpgFiles.Count != 1)
        {
            return ValueResult<QuizQuestionContent>.Failure($"Question folder '{questionKey}' must contain exactly one JPG image.");
        }

        return ValueResult<QuizQuestionContent>.Success(
            new QuizQuestionContent(
                questionKey,
                index,
                title,
                questionMetadata.Value.TimeoutSeconds,
                questionMetadata.Value.CorrectAnswer,
                Uri.EscapeDataString(questionKey)));
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

    private sealed record QuizMetadata(string? Title);

    private sealed record QuestionMetadata(string? Title, int TimeoutSeconds, int CorrectAnswer);

    private sealed record ValueResult<T>(bool Succeeded, string Message, T? Value)
    {
        public static ValueResult<T> Success(T value) => new(true, string.Empty, value);

        public static ValueResult<T> Failure(string message) => new(false, message, default);
    }
}
