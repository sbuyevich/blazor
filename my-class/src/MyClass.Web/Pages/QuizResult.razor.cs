using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Globalization;
using System.Text;
using MyClass.Core.Models;
using MyClass.Core.Services;

namespace  MyClass.Web.Pages;

public partial class QuizResult
{
    [CascadingParameter]
    public ClassContext CurrentClass { get; set; } = null!;

    [Inject]
    private IQuizResultService QuizResultService { get; set; } = null!;

    [Inject]
    private ILoginStateService LoginStateService { get; set; } = null!;

    [Inject]
    private ISessionStorageService SessionStorage { get; set; } = null!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = null!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = null!;

    private Result<QuizResultPageState>? _stateResult;
    private LoginState? _loginState;
    private bool _isLoading = true;
    private bool _isExporting;

    private IReadOnlyList<QuizResultStudentSummary> StudentSummaries =>
        _stateResult?.Value?.StudentSummaries ?? [];

    private IReadOnlyList<QuizResultQuestionSummary> QuestionSummaries =>
        _stateResult?.Value?.QuestionSummaries ?? [];

    private IReadOnlyList<QuizResultExportRow> ExportRows =>
        _stateResult?.Value?.ExportRows ?? [];

    private bool HasResults => _stateResult?.Succeeded == true && _stateResult.Value?.HasResults == true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        await LoadResultStateAsync();
        StateHasChanged();
    }

    private async Task LoadResultStateAsync()
    {
        _isLoading = true;
        _loginState = LoginStateService.Current ?? await SessionStorage.GetLoginStateAsync();

        if (_loginState is not null && LoginStateService.Current is null)
        {
            LoginStateService.Set(_loginState);
        }

        _stateResult = await QuizResultService.GetResultPageStateAsync(_loginState, CurrentClass);
        _isLoading = false;
    }

    private async Task ExportCsvAsync()
    {
        if (!HasResults || _isExporting)
        {
            return;
        }

        _isExporting = true;

        try
        {
            var csv = BuildCsv(ExportRows);
            await JSRuntime.InvokeVoidAsync(
                "myClassDownloadTextFile",
                "quiz-results.csv",
                "text/csv;charset=utf-8",
                csv);
        }
        catch (JSException)
        {
            Snackbar.Add("Unable to export quiz results.", Severity.Error);
        }
        finally
        {
            _isExporting = false;
        }
    }

    private static string BuildCsv(IEnumerable<QuizResultExportRow> rows)
    {
        var csv = new StringBuilder();

        csv.AppendLine("Student,Question,Pass,Student Answer,Correct Answer,Answer Time");

        foreach (var row in rows)
        {
            csv.Append(EscapeCsv(row.StudentDisplayName));
            csv.Append(',');
            csv.Append(EscapeCsv(row.QuestionText));
            csv.Append(',');
            csv.Append(EscapeCsv(row.IsCorrect.ToString(CultureInfo.InvariantCulture)));
            csv.Append(',');
            csv.Append(EscapeCsv(row.Answer));
            csv.Append(',');
            csv.Append(EscapeCsv(row.CorrectAnswer));
            csv.Append(',');
            csv.Append(EscapeCsv(FormatCsvTime(row.AnswerTime)));
            csv.AppendLine();
        }

        return csv.ToString();
    }

    private static string EscapeCsv(string value)
    {
        return value.Contains('"') || value.Contains(',') || value.Contains('\r') || value.Contains('\n')
            ? $"\"{value.Replace("\"", "\"\"", StringComparison.Ordinal)}\""
            : value;
    }

    private static string FormatPercent(double percent)
    {
        return percent.ToString("0.#", CultureInfo.CurrentCulture) + "%";
    }

    private static string FormatTime(TimeSpan time)
    {
        return time.TotalHours >= 1
            ? time.ToString(@"h\:mm\:ss", CultureInfo.CurrentCulture)
            : time.ToString(@"m\:ss", CultureInfo.CurrentCulture);
    }

    private static string FormatCsvTime(TimeSpan time)
    {
        return time.ToString(@"c", CultureInfo.InvariantCulture);
    }
}
