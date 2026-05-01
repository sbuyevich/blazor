# Task 04 - CSV Export

## Goal

Add a teacher export action that downloads the current active quiz answer rows as a CSV file.

## Work

- Add an export button at the top of the Quiz Result page.
- Disable export when no answer rows exist.
- Generate CSV from current class `QuizAnswers` rows.
- Include these columns:
  - `StudentDisplayName`
  - `QuestionText`
  - `CorrectAnswer`
  - `Answer`
  - `AnswerTime`
  - `IsCorrect`
- Escape CSV values safely.
- Use a browser download flow that fits the existing Blazor app style.
- Choose a clear filename such as `quiz-results.csv`.

## Deliverables

- Export button in the page UI.
- CSV generation through the quiz result service or page action.
- Browser download behavior.

## Acceptance Criteria

- Export is disabled when there are no active quiz result rows.
- Export includes only current class answer rows.
- CSV has the required headers.
- CSV values with commas, quotes, or newlines are escaped correctly.
- Answer time follows the same no-answer/full-time rule as the grids.
- Teacher can download the file without leaving the page.

## Notes

- Do not export historical quiz runs.
- Do not write CSV files to the server filesystem.
