# Stage 10 Quiz Result Page Plan

## Summary

Create a teacher-only `/quiz-result` page that summarizes the current active quiz run stored in `QuizAnswers`. The page shows sortable student and question result grids and lets the teacher export the current answer rows to CSV.

## Key Changes

- Add a quiz result service in `MyClass.Core` using `IDbContextFactory<ApplicationDbContext>`:
  - Load answer rows for the teacher's current class.
  - Build student summary rows grouped by student.
  - Build question summary rows grouped by question.
  - Build flat CSV export rows from `QuizAnswers`.
  - Treat no-answer/timeouts as incorrect and include the full question time in aggregated time.
- Add `/quiz-result` as a teacher-only page:
  - Use the existing teacher role/login checks.
  - Keep the existing Teacher nav link.
  - Show an empty state when no active quiz result rows exist.
- Build MudBlazor result UI:
  - Student summary grid with sortable columns for student name, correct count, incorrect count, percent correct, and total answer time.
  - Question summary grid with sortable columns for question, correct count, incorrect count, percent correct, and total answer time.
  - Export button at the top of the page, disabled when there are no rows.
- Add CSV export:
  - Export the current `QuizAnswers` rows for the current class.
  - Include `StudentDisplayName`, `QuestionText`, `CorrectAnswer`, `Answer`, `AnswerTime`, and `IsCorrect`.

## Public Interfaces / Types

- Add `IQuizResultService` with result models following existing `Succeeded`, `Message`, payload conventions.
- Add lightweight result models for:
  - student summary rows
  - question summary rows
  - CSV export rows or export content
- No database schema change is required.
- Do not add long-term quiz history or recreate `QuizSessions` / `QuizSessionQuestions`.

## Test Plan

- Teacher can access `/quiz-result`.
- Student or unauthenticated user cannot use the page.
- Empty active quiz results show a clear empty state and disable export.
- Student summary counts correct and incorrect answers correctly.
- Question summary counts correct and incorrect answers correctly.
- Percent correct uses correct answers divided by all answer rows in the group.
- No-answer/timeouts count as incorrect and add full question time to aggregate time.
- Submitted answers use elapsed time from `StartedAtUtc` to `EndedAtUtc`.
- CSV export includes only current class answer rows and the required columns.
- Existing quiz start, next, restart, answer, reveal, and navigation behavior does not regress.
- `dotnet build .\MyClass.slnx` succeeds.

## Assumptions

- Stage 10 reports only the current active quiz run in `QuizAnswers`.
- `QuizAnswers` remains active-run storage, not durable reporting history.
- The current class is inferred through `QuizAnswers.StudentId -> Students.ClassId`.
- No-answer/timeouts use the question time limit from quiz content when calculating aggregate time.

## Task Breakdown

- [ ] [Task 01 - Quiz Result Service](tasks/01-quiz-result-service.md)
- [ ] [Task 02 - Teacher Result Page Access](tasks/02-teacher-result-page-access.md)
- [ ] [Task 03 - Result Grids UI](tasks/03-result-grids-ui.md)
- [ ] [Task 04 - CSV Export](tasks/04-csv-export.md)
- [ ] [Task 10 - Verification](tasks/10-verification.md)
